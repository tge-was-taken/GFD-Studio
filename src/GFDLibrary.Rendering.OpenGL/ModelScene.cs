using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using GFDLibrary;
using GFDLibrary.Common;
using GFDLibrary.Textures;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using GFDLibrary.Animations;
using GFDLibrary.Models;
using Color = System.Drawing.Color;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;
using GFDLibrary.Materials;
using GFDLibrary.Shaders;

namespace GFDLibrary.Rendering.OpenGL
{
    /// <summary>
    /// Everything the model viewer draws, with no window of its own. A host creates a GL context,
    /// calls <see cref="Initialize"/> once, then <see cref="Resize"/> and <see cref="Render"/>.
    /// </summary>
    public sealed class ModelScene : IDisposable
    {
        private readonly Func<string, string> mResolveAppData;
        private int mWidth = 1;
        private int mHeight = 1;
        private bool mCanRender;
        private ShaderRegistry mShaderRegistry;
        private GLPerspectiveCamera mCamera;
        private Vector3 mRaypickStart;
        private Vector3 mRaypickEnd;

        // Grid
        private int mGridVertexArrayID;
        private GLBuffer<Vector3> mGridVertexBuffer;
        private int mGridSize = 96;
        private int mGridSpacing = 16;
        private float mGridMinZ;
        public Vector4 GridLineColor = new Vector4( 50.15f, 50.15f, 50.15f, 1f );
        public Color ClearColor = System.Drawing.Color.FromArgb( 60, 63, 65 );

        // Primitives
        private PrimitiveMesh mCameraPrimitive;
        private PrimitiveMesh mLightPrimitive;
        private PrimitiveMesh mEplPrimitive;

        // Model
        private GLModel mModel;
        private bool mIsModelLoaded;
        private bool mIsFieldModel;
        private Archive mFieldTextures;
        private Mesh mSelectedMesh;
        private Material mSelectedMaterial;

        // Animation
        private Stopwatch mTimeCounter;
        private double mLastTime;
        private AnimationPlaybackState mAnimationPlayback = AnimationPlaybackState.Stopped;
        private double mAnimationTime;

        public Animation Animation { get; private set; }

        public bool IsAnimationLoaded => Animation != null;

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public AnimationPlaybackState AnimationPlayback
        {
            get => mAnimationPlayback;
            set
            {
                if ( mAnimationPlayback == value || !IsAnimationLoaded )
                    return;

                mAnimationPlayback = value;

                switch ( AnimationPlayback )
                {
                    case AnimationPlaybackState.Stopped:
                        AnimationTime = 0;
                        mModel?.UnloadAnimation();
                        break;
                    case AnimationPlaybackState.Paused:
                        break;
                    case AnimationPlaybackState.Playing:
                        if ( mModel?.Animation == null && IsAnimationLoaded )
                            mModel?.LoadAnimation( Animation );
                        break;
                }

                AnimationPlaybackStateChanged?.Invoke( this, mAnimationPlayback );
            }
        }


        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public double AnimationTime
        {
            get => mAnimationTime;
            set
            {
                mAnimationTime = value;
                AnimationTimeChanged?.Invoke( this, mAnimationTime );
            }
        }

        // Events
        public event EventHandler<Animation> AnimationLoaded;
        public event EventHandler<AnimationPlaybackState> AnimationPlaybackStateChanged;
        public event EventHandler<double> AnimationTimeChanged;

        public ModelScene( Func<string, string> resolveAppData )
            => mResolveAppData = resolveAppData;

        /// <summary>
        /// Raised when something changed that the host should repaint for.
        /// </summary>
        public event EventHandler RedrawRequested;

        public bool CanRender => mCanRender;

        public bool IsModelLoaded => mIsModelLoaded;

        /// <summary>
        /// Sets the scene up against the context the host has made current.
        /// </summary>
        public void Initialize()
        {
            LogGLInfo();

            mCanRender = InitializeShaders();
            if ( !mCanRender )
                return;

            InitializeGLRenderState();
            CreateGrid();
            LoadPrimitives();

            mTimeCounter = new Stopwatch();
            mTimeCounter.Start();
        }

        private void Invalidate() => RedrawRequested?.Invoke( this, EventArgs.Empty );

        private void CreateGrid()
        {
            // thanks Skyth
            var vertices = new List<Vector3>();
            for ( int i = -mGridSize; i <= mGridSize; i += mGridSpacing )
            {
                vertices.Add( new Vector3( i, 0, -mGridSize ) );
                vertices.Add( new Vector3( i, 0, mGridSize ) );
                vertices.Add( new Vector3( -mGridSize, 0, i ) );
                vertices.Add( new Vector3( mGridSize, 0, i ) );
            }

            mGridMinZ = (int)vertices.Min( x => x.Z );
            mGridVertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray( mGridVertexArrayID );

            mGridVertexBuffer = new GLBuffer<Vector3>( BufferTarget.ArrayBuffer, vertices.ToArray() );

            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0 );
            GL.EnableVertexAttribArray( 0 );
        }

        private void LoadPrimitives()
        {
            mCameraPrimitive = new PrimitiveMesh( mResolveAppData( "primitives/camera.obj" ) );
            mLightPrimitive = new PrimitiveMesh( mResolveAppData( "primitives/light.obj" ) );
            mEplPrimitive = new PrimitiveMesh( mResolveAppData( "primitives/epl.obj" ) );
        }

        private void DrawLine( Vector3 start, Vector3 end, Vector4 color )
        {
            var lineShaderProgram = mShaderRegistry.mLineShader.Id;

            // Line vertices
            float[] vertices = {
                start.X, start.Y, start.Z,
                end.X, end.Y, end.Z
            };

            // Create and bind VAO
            int vao = GL.GenVertexArray();
            GL.BindVertexArray( vao );

            // Create, bind, and fill VBO with vertices
            int vbo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ArrayBuffer, vbo );
            GL.BufferData( BufferTarget.ArrayBuffer, vertices.Length * sizeof( float ), vertices, BufferUsageHint.StaticDraw );

            // Enable the shader program and set uniforms
            GL.UseProgram( lineShaderProgram );

            // Set line color uniform
            int lineColorLocation = GL.GetUniformLocation( lineShaderProgram, "uColor" );
            GL.Uniform4( lineColorLocation, color );

            var view = mCamera.View;
            int viewLoc = GL.GetUniformLocation( lineShaderProgram, "uView" );
            GL.UniformMatrix4( viewLoc, false, ref view );

            var projection = mCamera.Projection;
            int projLoc = GL.GetUniformLocation( lineShaderProgram, "uProjection" );
            GL.UniformMatrix4( projLoc, false, ref projection );

            // Define vertex layout
            GL.EnableVertexAttribArray( 0 );
            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 3 * sizeof( float ), 0 );

            // Draw the line
            GL.DrawArrays( PrimitiveType.Lines, 0, 2 );

            // Clean up
            GL.DisableVertexAttribArray( 0 );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
            GL.DeleteBuffer( vbo );
            GL.BindVertexArray( 0 );
            GL.DeleteVertexArray( vao );
            GL.UseProgram( 0 );
        }

        //public readonly struct GLVertexArrayHelper : IDisposable
        //{
        //    struct BindHelper : IDisposable
        //    {
        //        public readonly void Dispose()
        //        {
        //            GL.BindVertexArray( 0 );
        //        }
        //    }

        //    public readonly int Id;

        //    public GLVertexArrayHelper()
        //    {
        //        Id = GL.GenVertexArray();
        //    }

        //    public readonly IDisposable Bind()
        //    {
        //        GL.BindVertexArray( Id );
        //        return new BindHelper();
        //    }

        //    public readonly void Dispose()
        //    {
        //        GL.DeleteVertexArray( Id );
        //    }
        //}

        //public readonly struct GLBufferHelper : IDisposable
        //{
        //    readonly struct BindHelper : IDisposable
        //    {
        //        private readonly BufferTarget _target;

        //        public BindHelper(BufferTarget target)
        //        {
        //            _target = target;
        //        }

        //        public readonly void Dispose()
        //        {
        //            GL.BindBuffer( _target, 0 );
        //        }
        //    }

        //    public readonly int Id;

        //    public GLBufferHelper()
        //    {
        //        Id = GL.GenBuffer();
        //    }

        //    public readonly IDisposable BindBuffer(BufferTarget target)
        //    {
        //        GL.BindBuffer( target, Id );
        //        return new BindHelper();
        //    }

        //    public readonly void Dispose()
        //    {
        //        GL.DeleteVertexArray( Id );
        //    }
        //}

        private void DrawSphere( Vector3 center, float radius, int latitudeSegments = 20, int longitudeSegments = 20 )
        {
            List<float> vertices = new List<float>();
            List<int> indices = new List<int>();

            // Generate vertices
            for ( int lat = 0; lat <= latitudeSegments; lat++ )
            {
                float theta = lat * MathF.PI / latitudeSegments;
                float sinTheta = MathF.Sin( theta );
                float cosTheta = MathF.Cos( theta );

                for ( int lon = 0; lon <= longitudeSegments; lon++ )
                {
                    float phi = lon * 2 * MathF.PI / longitudeSegments;
                    float sinPhi = MathF.Sin( phi );
                    float cosPhi = MathF.Cos( phi );

                    Vector3 position = new Vector3(
                        center.X + radius * cosPhi * sinTheta,
                        center.Y + radius * cosTheta,
                        center.Z + radius * sinPhi * sinTheta
                    );

                    Vector3 normal = Vector3.Normalize( position - center );

                    // Add vertex position and normal
                    vertices.Add( position.X );
                    vertices.Add( position.Y );
                    vertices.Add( position.Z );
                    vertices.Add( normal.X );
                    vertices.Add( normal.Y );
                    vertices.Add( normal.Z );
                }
            }

            // Generate indices
            for ( int lat = 0; lat < latitudeSegments; lat++ )
            {
                for ( int lon = 0; lon < longitudeSegments; lon++ )
                {
                    int first = lat * ( longitudeSegments + 1 ) + lon;
                    int second = first + longitudeSegments + 1;

                    indices.Add( first );
                    indices.Add( second );
                    indices.Add( first + 1 );

                    indices.Add( second );
                    indices.Add( second + 1 );
                    indices.Add( first + 1 );
                }
            }

            // VAO
            var vao = GL.GenVertexArray();
            GL.BindVertexArray( vao );

            // VBO
            var vbo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ArrayBuffer, vbo );
            GL.BufferData( BufferTarget.ArrayBuffer, vertices.Count * sizeof( float ), vertices.ToArray(), BufferUsageHint.StaticDraw );

            // EBO
            var ebo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, ebo );
            GL.BufferData( BufferTarget.ElementArrayBuffer, indices.Count * sizeof( int ), indices.ToArray(), BufferUsageHint.StaticDraw );

            // Specify vertex attributes
            int stride = 6 * sizeof( float ); // 3 for position, 3 for normal
            GL.EnableVertexAttribArray( 0 );
            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, stride, 0 );
            GL.EnableVertexAttribArray( 1 );
            GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof( float ) );

            mShaderRegistry.mLineShader.Use();
            mShaderRegistry.mLineShader.SetUniform( "uView", mCamera.View );
            mShaderRegistry.mLineShader.SetUniform( "uProjection", mCamera.Projection );

            //GL.DrawArrays( PrimitiveType.Lines, 0, vertices.Count / 12 );
            GL.DrawElements( PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0 );

            // Clean up
            GL.UseProgram( 0 );
            GL.BindVertexArray( 0 );
            GL.DisableVertexAttribArray( 1 );
            GL.DisableVertexAttribArray( 0 );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, ebo );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
            GL.BindVertexArray( 0 );
            GL.DeleteBuffer( ebo );
            GL.DeleteBuffer( vbo );
            GL.DeleteVertexArray( 0 );
        }

        /// <summary>
        /// Load a model for displaying in the control.
        /// </summary>
        /// <param name="modelPack"></param>
        public void LoadModel( ModelPack modelPack )
        {
            if ( !mCanRender || modelPack.Model == null )
                return;

            if ( mIsModelLoaded )
            {
                // Unload previously loaded model to free memory
                UnloadModel();
            }

            // Load model into optimized format
            mModel = new GLModel( modelPack, ( material, textureName ) =>
            {
                if (string.IsNullOrWhiteSpace(textureName))
                    return null;
                if ( mIsFieldModel && mFieldTextures.TryOpenFile( textureName, out var textureStream ) )
                {
                    using ( textureStream )
                    {
                        var texture = new FieldTexturePS3( textureStream );
                        return new GLTexture( texture );
                    }
                }
                else if ( modelPack.Textures.TryGetTexture( textureName, out var texture ) )
                {
                    return new GLTexture( texture );
                }
                else
                {
                    Trace.TraceWarning( $"tTexture '{textureName}' used by material '{material.Name}' is missing" );
                }

                return null;
            } );

            foreach ( var node in modelPack.Model.Nodes.Where( x => x.HasAttachments ) )
            {
                var glNode = mModel.Nodes.Find( x => x.Node == node );

                foreach ( var attachment in node.Attachments )
                {
                    switch ( attachment.Type )
                    {
                        case NodeAttachmentType.Camera:
                            glNode.Meshes.Add( mCameraPrimitive.Instantiate( true, false, PrimitiveMesh.DefaultColor ) );
                            break;

                        case NodeAttachmentType.Light:
                            glNode.Meshes.Add( mLightPrimitive.Instantiate( true, false, PrimitiveMesh.DefaultColor ) );
                            break;

                        case NodeAttachmentType.Epl:
                            glNode.Meshes.Add( mEplPrimitive.Instantiate( true, true, PrimitiveMesh.DefaultColor ) );
                            break;
                    }
                }
            }

            mIsModelLoaded = true;

            // Initialize camera
            InitializeCamera();

            if ( Animation != null )
            {
                // Apply previously loaded animation to new model
                LoadAnimation( Animation, AnimationPlayback != AnimationPlaybackState.Playing );
            }

            Invalidate();
        }

        public void LoadAnimation( Animation animation, bool reset = true )
        {
            Animation = animation;
            mModel?.LoadAnimation( Animation );

            AnimationLoaded?.Invoke( this, animation );

            if ( reset )
            {
                AnimationTime = 0;
                AnimationPlayback = AnimationPlaybackState.Playing;
            }
        }

        public void Dispose()
        {
            mShaderRegistry?.mDefaultShader?.Dispose();

            if ( mIsModelLoaded )
                UnloadModel();
        }

        private void ExecuteTimedCallback( Action action )
        {
            // Update timings
            var curTime = mTimeCounter.Elapsed.TotalSeconds;
            var deltaTime = curTime - mLastTime;

            if ( AnimationPlayback == AnimationPlaybackState.Playing )
            {
                var nextAnimationTime = AnimationTime + ( deltaTime * Animation.Speed.GetValueOrDefault( 1f ) );
                AnimationTime = nextAnimationTime >= Animation.Duration ? 0 : nextAnimationTime;
            }

            action();

            // Remember current time
            mLastTime = curTime;
        }

        /// <summary>
        /// Draws a frame into whatever framebuffer the host has bound.
        /// </summary>
        public void Render()
        {
            if ( !mCanRender || mCamera == null )
                return;

            ExecuteTimedCallback( () =>
            {
                // clear the buffers
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

                DrawGrid( mCamera.View, mCamera.Projection );

                if ( mIsModelLoaded )
                {
                    // Draw model
                    mModel.Draw( new DrawContext()
                    {
                        ShaderRegistry = mShaderRegistry,
                        Camera = mCamera,
                        AnimationTime = AnimationTime,
                        SelectedMaterial = mSelectedMaterial,
                        SelectedMesh = mSelectedMesh
                    } );
                }

                //foreach ( var node in mModel.Nodes )
                //{
                //    DrawSphere( node.WorldTransform.Translation.ToOpenTK(), 1, 8, 8 );
                //    foreach ( var mesh in node.Meshes )
                //    {
                //        if (mesh.Mesh.BoundingSphere.HasValue)
                //        {
                //            var worldCenter = System.Numerics.Vector3.Transform( mesh.Mesh.BoundingSphere.Value.Center, node.WorldTransform );
                //            DrawSphere( worldCenter.ToOpenTK(), mesh.Mesh.BoundingSphere.Value.Radius, 8, 8 );
                //        }
                //    }
                //}

                //DrawLine( mRaypickStart, mRaypickEnd, new Vector4( 1, 0, 0, 1 ) );
            } );
        }

        /// <summary>
        /// Moves the animation on without drawing, so a host can decide whether a frame is due.
        /// </summary>
        public bool Advance()
        {
            ExecuteTimedCallback( () => { } );
            return AnimationPlayback == AnimationPlaybackState.Playing;
        }

        private void DrawGrid( Matrix4 view, Matrix4 projection )
        {
            mShaderRegistry.mLineShader.Use();
            mShaderRegistry.mLineShader.SetUniform( "uView", view );
            mShaderRegistry.mLineShader.SetUniform( "uProjection", projection );
            mShaderRegistry.mLineShader.SetUniform( "uColor", GridLineColor );
            mShaderRegistry.mLineShader.SetUniform( "uMinZ", mGridMinZ );

            GL.BindVertexArray( mGridVertexArrayID );
            GL.DrawArrays( PrimitiveType.Lines, 0, mGridVertexBuffer.Count );
        }

        public void Resize( int width, int height )
        {
            mWidth = Math.Max( width, 1 );
            mHeight = Math.Max( height, 1 );

            if ( !mCanRender )
                return;

            GL.Viewport( 0, 0, mWidth, mHeight );

            if ( mCamera != null )
                mCamera.AspectRatio = (float)mWidth / mHeight;
        }

        /// <summary>
        /// Log GL info for diagnostics.
        /// </summary>
        private void LogGLInfo()
        {
            // todo: log to file? would help with debugging crashes on clients
            Trace.TraceInformation( "GL Info:" );
            Trace.TraceInformation( $"     Vendor         {GL.GetString( StringName.Vendor )}" );
            Trace.TraceInformation( $"     Renderer       {GL.GetString( StringName.Renderer )}" );
            Trace.TraceInformation( $"     Version        {GL.GetString( StringName.Version )}" );
            Trace.TraceInformation( $"     Extensions     {GL.GetString( StringName.Extensions )}" );
            Trace.TraceInformation( $"     GLSL version   {GL.GetString( StringName.ShadingLanguageVersion )}" );
            Trace.TraceInformation( "" );
        }

        /// <summary>
        /// Initializes GL state before rendering starts.
        /// </summary>
        private void InitializeGLRenderState()
        {
            GL.ClearColor( ClearColor );
            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.CullFace( CullFaceMode.Back );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );

#if GL_DEBUG
            GL.Enable( EnableCap.DebugOutputSynchronous );
            GL.DebugMessageCallback( GLDebugMessageCallback, IntPtr.Zero );
#endif

            GL.Enable( EnableCap.Multisample );
        }

        [Conditional( "DEBUG" )]
        private void GLDebugMessageCallback( DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam )
        {
            // notication for buffer using VIDEO memory
            if ( id == 0x00020071 )
                return;

            var msg = Marshal.PtrToStringAnsi( message, length );
            Trace.TraceInformation( $"GL Debug: {severity} {type} {msg}" );
        }

        /// <summary>
        /// Initializes shaders and links the shader program.
        /// </summary>
        private bool InitializeShaders()
        {
            mShaderRegistry = new ShaderRegistry();
            return mShaderRegistry.InitializeShaders( mResolveAppData );
        }

        private void UnloadModel()
        {
            if ( !mIsModelLoaded )
                return;

            mIsModelLoaded = false;
            mModel.Dispose();
        }

        private void InitializeCamera()
        {
            var cameraFov = 45f;

            BoundingSphere bSphere;
            if ( !mModel.ModelPack.Model.BoundingSphere.HasValue )
            {
                if ( mModel.ModelPack.Model.BoundingBox.HasValue )
                {
                    bSphere = BoundingSphere.Calculate( mModel.ModelPack.Model.BoundingBox.Value );
                }
                else
                {
                    bSphere = new BoundingSphere( new System.Numerics.Vector3(), 0 );
                }
            }
            else
            {
                bSphere = mModel.ModelPack.Model.BoundingSphere.Value;
            }

            mCamera = new GLPerspectiveCamera( 1f, 100000f, cameraFov,
                                (float)mWidth / mHeight, bSphere, Vector3.Zero, Vector3.Zero );
        }

        //
        // Input events
        //

        /// <summary>
        /// Turns the model, in pixels of pointer travel.
        /// </summary>
        public void Orbit( float deltaX, float deltaY, float multiplier = 0.5f )
        {
            if ( !mIsModelLoaded )
                return;

            mCamera.ModelRotation = new Vector3(
                mCamera.ModelRotation.X + deltaY * 0.01f * multiplier,
                mCamera.ModelRotation.Y + deltaX * 0.01f * multiplier,
                mCamera.ModelRotation.Z );

            Invalidate();
        }

        /// <summary>
        /// Slides the model, in pixels of pointer travel.
        /// </summary>
        public void Pan( float deltaX, float deltaY, float multiplier = 0.5f )
        {
            if ( !mIsModelLoaded )
                return;

            mCamera.ModelTranslation = new Vector3(
                mCamera.ModelTranslation.X + ( deltaX / 3f ) * multiplier,
                mCamera.ModelTranslation.Y - ( deltaY / 3f ) * multiplier,
                mCamera.ModelTranslation.Z );

            Invalidate();
        }

        public void Zoom( float delta, float multiplier = 0.25f )
        {
            if ( !mIsModelLoaded )
                return;

            var translation = mCamera.ModelTranslation;
            translation.Z += delta * multiplier;
            mCamera.ModelTranslation = translation;

            Invalidate();
        }

        public void ResetView()
        {
            if ( !mIsModelLoaded )
                return;

            mCamera.ModelTranslation = Vector3.Zero;
            mCamera.ModelRotation = Vector3.Zero;
            Invalidate();
        }

        private bool RayIntersectsSphere( Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, out float distance )
        {
            // Vector from the ray's origin to the center of the sphere
            Vector3 m = rayOrigin - sphereCenter;

            float b = Vector3.Dot( m, rayDirection );
            float c = Vector3.Dot( m, m ) - sphereRadius * sphereRadius;

            // If ray starts outside sphere (c > 0) and points away from sphere (b > 0), no intersection
            if ( c > 0.0f && b > 0.0f )
            {
                distance = 0.0f;
                return false;
            }

            // Calculate discriminant
            float discriminant = b * b - c;

            // If discriminant is negative, no intersection
            if ( discriminant < 0.0f )
            {
                distance = 0.0f;
                return false;
            }

            // Calculate distance to the closest intersection point
            distance = -b - MathF.Sqrt( discriminant );

            // If distance is negative, the ray started inside the sphere so clamp to zero
            if ( distance < 0.0f )
                distance = 0.0f;

            return true;
        }

        public bool RayIntersectsBox( Vector3 rayOrigin, Vector3 rayDir, Vector3 boxMin, Vector3 boxMax, out float distance )
        {
            float tMin = ( boxMin.X - rayOrigin.X ) / rayDir.X;
            float tMax = ( boxMax.X - rayOrigin.X ) / rayDir.X;

            if ( tMin > tMax ) (tMin, tMax) = (tMax, tMin);

            float tyMin = ( boxMin.Y - rayOrigin.Y ) / rayDir.Y;
            float tyMax = ( boxMax.Y - rayOrigin.Y ) / rayDir.Y;

            if ( tyMin > tyMax ) (tyMin, tyMax) = (tyMax, tyMin);

            if ( ( tMin > tyMax ) || ( tyMin > tMax ) )
            {
                distance = 0;
                return false;
            }

            if ( tyMin > tMin ) tMin = tyMin;
            if ( tyMax < tMax ) tMax = tyMax;

            float tzMin = ( boxMin.Z - rayOrigin.Z ) / rayDir.Z;
            float tzMax = ( boxMax.Z - rayOrigin.Z ) / rayDir.Z;

            if ( tzMin > tzMax ) (tzMin, tzMax) = (tzMax, tzMin);

            if ( ( tMin > tzMax ) || ( tzMin > tMax ) )
            {
                distance = 0;
                return false;
            }

            if ( tzMin > tMin ) tMin = tzMin;
            if ( tzMax < tMax ) tMax = tzMax;

            distance = tMin;
            return tMin >= 0;
        }

        public bool Raypick( int mouseX, int mouseY )
        {
            // Nothing to pick against until a model and its camera exist, which is also the
            // state the viewer sits in when the shaders did not compile.
            if ( !mCanRender || mCamera == null || mModel == null )
                return false;

            // Step 1: Convert mouse position to NDC
            var x = ( 2.0f * mouseX ) / mWidth - 1.0f;
            var y = 1.0f - ( 2.0f * mouseY ) / mHeight;
            var rayNDC = new Vector4( x, y, -1.0f, 1.0f ); // near plane

            // Step 2: Convert NDC to world coordinates
            var invProjectionMatrix = Matrix4.Invert( mCamera.Projection );
            var invViewMatrix = Matrix4.Invert( mCamera.View );

            var rayCamera = invProjectionMatrix * rayNDC;
            rayCamera.Z = -1.0f;
            rayCamera.W = 0.0f;

            var rayWorld4 = invViewMatrix * rayCamera;
            var rayWorld = new Vector3( rayWorld4.X, rayWorld4.Y, rayWorld4.Z );
            rayWorld.Normalize();

            var rayOrigin = new Vector3( invViewMatrix.M41, invViewMatrix.M42, invViewMatrix.M43 );
            mRaypickStart = rayOrigin;
            mRaypickEnd = rayOrigin + rayWorld * 10000f;

            var anySelected = false;

            Debug.WriteLine( rayOrigin );

            // TODO sorting

            float closestDistance = float.MaxValue;
            GLNode closestNode = null;
            GLMesh closestMesh = null;

            foreach ( var glNode in mModel.Nodes )
            {
                foreach ( var glMesh in glNode.Meshes )
                {
                    if ( glMesh.IsVisible && (glMesh.Mesh?.BoundingSphere.HasValue ?? false) )
                    {
                        //var sphere = glMesh.Mesh.BoundingSphere.Value;
                        //// Transform the sphere center to world space
                        //var sphereCenterWorld = System.Numerics.Vector3.Transform( sphere.Center, glNode.WorldTransform ).ToOpenTK();

                        //// Check for intersection
                        //if ( RayIntersectsSphere( rayOrigin, rayWorld, sphereCenterWorld, sphere.Radius, out float distance ) )
                        //{
                        //    // Check if this sphere is the closest
                        //    if ( distance < closestDistance )
                        //    {
                        //        closestDistance = distance;
                        //        closestMesh = glMesh;
                        //        closestNode = glNode;
                        //    }
                        //}
                        var boundingBox = glMesh.Mesh.BoundingBox.Value;

                        // Transform the bounding box to world space
                        var boxMinWorld = System.Numerics.Vector3.Transform( boundingBox.Min, glNode.WorldTransform ).ToOpenTK();
                        var boxMaxWorld = System.Numerics.Vector3.Transform( boundingBox.Max, glNode.WorldTransform ).ToOpenTK();

                        // Check for intersection with the bounding box
                        if ( RayIntersectsBox( rayOrigin, rayWorld, boxMinWorld, boxMaxWorld, out float distance ) )
                        {
                            // Check if this bounding box is the closest intersection
                            if ( distance < closestDistance )
                            {
                                closestDistance = distance;
                                closestMesh = glMesh;
                                closestNode = glNode;
                            }
                        }
                    }
                }
            }

            // Select the closest mesh if found
            //if ( closestMesh != null )
            //{
            //    SetSelection( closestMesh.Mesh );
            //    Debug.WriteLine( $"Selected {closestNode.Node.Name} mesh" );
            //    return true;
            //}

            return false;
        }

        public void ClearSelection()
        {
            mSelectedMesh = null;
            mSelectedMaterial = null;
            Invalidate();
        }

        public void SetSelection( Mesh data )
        {
            ClearSelection();
            mSelectedMesh = data;
        }

        public void SetSelection( Material data )
        {
            ClearSelection();
            mSelectedMaterial = data;
        }
    }
}