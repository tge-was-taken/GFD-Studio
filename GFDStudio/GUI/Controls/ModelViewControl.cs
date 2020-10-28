using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Common;
using GFDLibrary.Textures;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GFDLibrary.Animations;
using GFDLibrary.Models;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.DataManagement;
using Color = System.Drawing.Color;
using Key = OpenTK.Input.Key;
using Quaternion = OpenTK.Quaternion;
using Vector3 = OpenTK.Vector3;
using Vector4 = OpenTK.Vector4;

namespace GFDStudio.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private static ModelViewControl sInstance;

        public static ModelViewControl Instance => sInstance ?? ( sInstance = new ModelViewControl() );

        private GLShaderProgram mDefaultShader;
        private GLPerspectiveCamera mCamera;
        private readonly bool mCanRender = true;
        private Point mLastMouseLocation;

        // Grid
        private GLShaderProgram mLineShader;
        private int mGridVertexArrayID;
        private GLBuffer<Vector3> mGridVertexBuffer;
        private int mGridSize = 2000;
        private int mGridSpacing = 16;
        private float mGridMinZ;

        // Primitives
        private PrimitiveMesh mCameraPrimitive;
        private PrimitiveMesh mLightPrimitive;
        private PrimitiveMesh mEplPrimitive;

        // Model
        private GLModel mModel;
        private bool mIsModelLoaded;
        private bool mIsFieldModel;
        private Archive mFieldTextures;

        // Animation
        private Stopwatch mTimeCounter;
        private double mLastTime;
        private Timer mUpdateTimer;
        private AnimationPlaybackState mAnimationPlayback = AnimationPlaybackState.Stopped;
        private double mAnimationTime;

        public Animation Animation { get; private set; }

        public bool IsAnimationLoaded => Animation != null;

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

        private ModelViewControl() : base( 
            new GraphicsMode( 32, 24, 0, 0 ),
            3,
            3,
#if GL_DEBUG
            GraphicsContextFlags.Debug | GraphicsContextFlags.ForwardCompatible )
#else
            GraphicsContextFlags.ForwardCompatible )
#endif
        {
            InitializeComponent();

            // make the control fill up the space of the parent cotnrol
            Dock = DockStyle.Fill;

            // required to use GL in the context of this control
            MakeCurrent();
            LogGLInfo();

            if ( !InitializeShaders() )
            {
                Visible = false;
                mCanRender = false;
            }
            else
            {
                InitializeGLRenderState();
            }

            CreateGrid();
            LoadPrimitives();
        }

        private void CreateGrid()
        {
            // thanks Skyth
            var vertices = new List<Vector3>();
            for ( int i = -mGridSize; i <= mGridSize; i += mGridSpacing )
            {
                vertices.Add( new Vector3( i,   0, -mGridSize ) );
                vertices.Add( new Vector3( i,   0, mGridSize ) );
                vertices.Add( new Vector3( -mGridSize, 0, i ) );
                vertices.Add( new Vector3( mGridSize,  0, i ) );
            }

            mGridMinZ = ( int ) vertices.Min( x => x.Z );
            mGridVertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray( mGridVertexArrayID );

            mGridVertexBuffer = new GLBuffer<Vector3>( BufferTarget.ArrayBuffer, vertices.ToArray() );

            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0 );
            GL.EnableVertexAttribArray( 0 );
        }

        private void LoadPrimitives()
        {
            mCameraPrimitive = new PrimitiveMesh( "primitives/camera.obj" );
            mLightPrimitive = new PrimitiveMesh( "primitives/light.obj" );
            mEplPrimitive = new PrimitiveMesh( "primitives/epl.obj" );
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
                    Trace.TraceWarning( $"tTexture '{ textureName }' used by material '{ material.Name }' is missing" );
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

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                components?.Dispose();

                mDefaultShader?.Dispose();

                if ( mIsModelLoaded )
                    UnloadModel();
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Executed during the initial load of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad( EventArgs e )
        {
            mTimeCounter = new Stopwatch();
            mTimeCounter.Start();

            mUpdateTimer = new Timer();
            mUpdateTimer.Interval = ( int ) ( ( 1f / 60f ) * 1000 );
            mUpdateTimer.Tick += ( o, s ) =>
            {
                if ( !mCanRender )
                    return;

                ExecuteTimedCallback( () =>
                {
                    if ( AnimationPlayback == AnimationPlaybackState.Playing )
                        Invalidate();
                });
            };
            mUpdateTimer.Start();
        }

        private void ExecuteTimedCallback( Action action )
        {
            // Update timings
            var curTime   = mTimeCounter.Elapsed.TotalSeconds;
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
        /// Executed when a frame is rendered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint( PaintEventArgs e )
        {
            if ( !mCanRender || mCamera == null)
                return;

            ExecuteTimedCallback( () =>
            {
                // clear the buffers
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

                DrawGrid( mCamera.View, mCamera.Projection );

                if ( mIsModelLoaded )
                {
                    // Draw model
                    mModel.Draw( mDefaultShader, mCamera, AnimationTime );
                }

                SwapBuffers();
            });
        }


        private void DrawGrid( Matrix4 view, Matrix4 projection )
        {
            mLineShader.Use();
            mLineShader.SetUniform( "uView", view );
            mLineShader.SetUniform( "uProjection", projection );
            mLineShader.SetUniform( "uColor", new Vector4( 0.15f, 0.15f, 0.15f, 1f ) );
            mLineShader.SetUniform( "uMinZ", mGridMinZ );

            GL.BindVertexArray( mGridVertexArrayID );
            GL.DrawArrays( PrimitiveType.Lines, 0, mGridVertexBuffer.Count );
        }

        /// <summary>
        /// Executed when control is resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize( EventArgs e )
        {
            if ( !mCanRender || !mIsModelLoaded )
                return;

            mCamera.AspectRatio = ( float )Width / Height;
            GL.Viewport( ClientRectangle );
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
            GL.ClearColor( Color.LightGray );
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

        [Conditional("DEBUG")]
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
            if ( !GLShaderProgram.TryCreate( DataStore.GetPath( "shaders/default.glsl.vs" ),
                                             DataStore.GetPath( "shaders/default.glsl.fs" ),
                                             out mDefaultShader ) )
            {
                if ( !GLShaderProgram.TryCreate( DataStore.GetPath( "shaders/basic.glsl.vs" ),
                                                 DataStore.GetPath( "shaders/basic.glsl.fs" ),
                                                 out mDefaultShader ) )
                {
                    return false;
                }
            }

            if ( !GLShaderProgram.TryCreate( DataStore.GetPath( "shaders/line.glsl.vs" ), DataStore.GetPath( "shaders/line.glsl.fs" ), out mLineShader ) )
                return false;

            return true;
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

            mCamera = new GLPerspectiveFreeCamera( CalculateCameraTranslation( cameraFov, bSphere), 1f, 100000f, cameraFov, ( float )Width / ( float )Height, Quaternion.Identity );
        }

        private static Vector3 CalculateCameraTranslation( float cameraFov, BoundingSphere bSphere )
        {
            var cameraFovInRad = MathHelper.DegreesToRadians( cameraFov );
            var distance = ( float )( ( bSphere.Radius * 2.0f ) / Math.Tan( cameraFovInRad / 2.0f ) );
            var cameraTranslation = new Vector3( bSphere.Center.X, bSphere.Center.Y, bSphere.Center.Z ) + new Vector3( 0, -50, 300 );

            return cameraTranslation;
        }

        //
        // Mouse events
        //

        private Point GetMouseLocationDelta( Point location )
        {
            location.X -= mLastMouseLocation.X;
            location.Y -= mLastMouseLocation.Y;

            return location;
        }

        protected override void OnMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            if ( !mIsModelLoaded )
                return;

            if ( e.Button.HasFlag( MouseButtons.Middle ) )
            {
                float multiplier = 0.5f;
                var keyboardState = Keyboard.GetState();

                if ( keyboardState.IsKeyDown( Key.ShiftLeft ) )
                {
                    multiplier *= 10f;
                }
                else if ( keyboardState.IsKeyDown( Key.ControlLeft) )
                {
                    multiplier /= 2f;
                }

                var locationDelta = GetMouseLocationDelta( e.Location );

                if ( keyboardState.IsKeyDown( Key.AltLeft ) )
                {
                    // Orbit around model

                    if ( mModel.ModelPack.Model.BoundingSphere.HasValue )
                    {
                        var bSphere = mModel.ModelPack.Model.BoundingSphere.Value;
                        var camera = new GLPerspectiveTargetCamera( mCamera.Translation, mCamera.ZNear, mCamera.ZFar, mCamera.FieldOfView, mCamera.AspectRatio, new Vector3( bSphere.Center.X, bSphere.Center.Y, bSphere.Center.Z ) );
                        camera.Rotate( -locationDelta.Y / 100f, -locationDelta.X / 100f );
                        mCamera = camera;
                    }
                }
                else
                {
                    // Move camera
                    var translation = mCamera.Translation;
                    if ( !( mCamera is GLPerspectiveFreeCamera ) )
                        translation = CalculateCameraTranslation( mCamera.FieldOfView, mModel.ModelPack.Model.BoundingSphere.Value );

                    mCamera = new GLPerspectiveFreeCamera( translation, mCamera.ZNear, mCamera.ZFar, mCamera.FieldOfView, mCamera.AspectRatio, Quaternion.Identity );
                    mCamera.Translation = new Vector3(
                        mCamera.Translation.X - ( ( locationDelta.X / 3f ) * multiplier ),
                        mCamera.Translation.Y + ( ( locationDelta.Y / 3f ) * multiplier ),
                        mCamera.Translation.Z );
                }

                Invalidate();
            }

            mLastMouseLocation = e.Location;
        }

        protected override void OnMouseWheel( System.Windows.Forms.MouseEventArgs e )
        {
            if ( !mIsModelLoaded )
                return;

            float multiplier = 0.25f;
            var keyboardState = Keyboard.GetState();

            if ( keyboardState.IsKeyDown( Key.ShiftLeft ) )
            {
                multiplier *= 10f;
            }
            else if ( keyboardState.IsKeyDown( Key.ControlLeft ) )
            {
                multiplier /= 2f;
            }

            var translation = mCamera.Translation;
            translation.Z += -( ( float )e.Delta * multiplier );
            mCamera.Translation = translation;

            Invalidate();
        }
    }
}
