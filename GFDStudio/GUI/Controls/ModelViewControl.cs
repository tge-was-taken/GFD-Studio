using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GFDLibrary;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;
using GFDLibrary.Common;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using GFDStudio.DataManagement;
using GFDStudio.GUI.Controls.ModelView;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Linq;

namespace GFDStudio.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private ModelPack mModelPack;
        private GLShaderProgram mShaderProgram;
        private readonly List<GLMesh> mMeshes = new List<GLMesh>();
        private GLPerspectiveCamera mCamera;
        private bool mCanRender = true;
        private bool mIsModelLoaded;
        private bool mIsFieldModel;
        private Archive mFieldTextures;
        private Point mLastMouseLocation;

        public ModelViewControl() : base( 
            new GraphicsMode( 32, 24, 0, 4 ),
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

            if ( !InitializeGLShaders() )
            {
                Visible = false;
                mCanRender = false;
            }
            else
            {
                InitializeGLRenderState();
            }
        }

        /// <summary>
        /// Load a model for displaying in the control.
        /// </summary>
        /// <param name="modelPack"></param>
        public void LoadModel( ModelPack modelPack )
        {
            if ( !mCanRender )
                return;

            mModelPack = modelPack;
            DeleteModel();
            LoadModel();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                    components.Dispose();

                if ( mShaderProgram != null )
                    mShaderProgram.Dispose();

                if ( mIsModelLoaded )
                    DeleteModel();
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Executed during the initial load of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad( EventArgs e )
        {

        }

        /// <summary>
        /// Executed when a frame is rendered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint( PaintEventArgs e )
        {
            if ( !mCanRender )
                return;

            // clear the buffers
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            if ( mIsModelLoaded )
            {
                foreach ( var geometry in mMeshes )
                {
                    if ( !geometry.IsVisible )
                        continue;

                    mShaderProgram.Use();

                    // set up model view projection matrix uniforms
                    mShaderProgram.SetUniform( "model", geometry.ModelMatrix );
                    mShaderProgram.SetUniform( "view", mCamera.CalculateViewMatrix() );
                    mShaderProgram.SetUniform( "projection", mCamera.CalculateProjectionMatrix() );


                    // set material uniforms
                    mShaderProgram.SetUniform( "matHasDiffuse", geometry.Material.HasDiffuse );

                    if ( geometry.Material.HasDiffuse )
                        GL.BindTexture( TextureTarget.Texture2D, geometry.Material.DiffuseTextureId );

                    mShaderProgram.SetUniform( "matAmbient", geometry.Material.Ambient );
                    mShaderProgram.SetUniform( "matDiffuse", geometry.Material.Diffuse );
                    mShaderProgram.SetUniform( "matSpecular", geometry.Material.Specular );
                    mShaderProgram.SetUniform( "matEmissive", geometry.Material.Emissive );
                    mShaderProgram.SetUniform( "matHasAlphaTransparency", geometry.Material.HasAlphaTransparency );

                    // checks if all uniforms were assigned
                    mShaderProgram.Check();

                    // use the vertex array
                    GL.BindVertexArray( geometry.VertexArrayId );

                    // draw the polygon
                    GL.DrawElements( PrimitiveType.Triangles, geometry.ElementIndexCount, DrawElementsType.UnsignedInt, 0 );
                }
            }

            SwapBuffers();
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
        /// Initializes shaders and links the shader program. Assumes only 1 shader program will be used.
        /// </summary>
        private bool InitializeGLShaders()
        {
            if ( !GLShaderProgram.TryCreate( DataStore.GetPath( "shaders/default.glsl.vs" ),
                                             DataStore.GetPath( "shaders/default.glsl.fs" ),
                out mShaderProgram ) )
            {
                Trace.TraceWarning( "Failed to compile shaders. Trying to use basic shaders.." );

                if ( !GLShaderProgram.TryCreate( DataStore.GetPath( "shaders/basic.glsl.vs" ),
                                                 DataStore.GetPath( "shaders/basic.glsl.fs" ),
                    out mShaderProgram ) )
                {
                    Trace.TraceError( "Failed to compile basic shaders. Disabling GL rendering." );
                    return false;
                }
            }

            // register shader uniforms
            mShaderProgram.RegisterUniform<Matrix4>( "model" );
            mShaderProgram.RegisterUniform<Matrix4>( "view" );
            mShaderProgram.RegisterUniform<Matrix4>( "projection" );
            mShaderProgram.RegisterUniform<bool>( "matHasDiffuse" );
            mShaderProgram.RegisterUniform<Vector4>( "matAmbient" );
            mShaderProgram.RegisterUniform<Vector4>( "matDiffuse" );
            mShaderProgram.RegisterUniform<Vector4>( "matSpecular" );
            mShaderProgram.RegisterUniform<Vector4>( "matEmissive" );
            mShaderProgram.RegisterUniform<bool>( "matHasAlphaTransparency" );
            return true;
        }

        //
        // Loading / saving model
        //

        private void LoadModel()
        {
            if ( !mCanRender || mModelPack.Model == null )
                return;

            InitializeCamera();

            var nodes = mModelPack.Model.Nodes.ToList();

            foreach ( var node in mModelPack.Model.Nodes )
            {
                if ( !node.HasAttachments )
                    continue;

                foreach ( var attachment in node.Attachments )
                {
                    if ( attachment.Type != NodeAttachmentType.Mesh )
                        continue;

                    var glMesh = CreateGLMesh( attachment.GetValue<Mesh>(), node, nodes, mModelPack.Model.BonePalette );
                    var transform = node.WorldTransform;
                    glMesh.ModelMatrix = ToMatrix4( ref transform );

                    mMeshes.Add( glMesh );
                }
            }

            mIsModelLoaded = true;

            Invalidate();
        }

        private void DeleteModel()
        {
            if ( !mIsModelLoaded )
                return;

            mIsModelLoaded = false;

            GL.BindVertexArray( 0 );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
            GL.BindTexture( TextureTarget.Texture2D, 0 );

            foreach ( var geometry in mMeshes )
            {
                GL.DeleteVertexArray( geometry.VertexArrayId );
                GL.DeleteBuffer( geometry.PositionBufferId );
                GL.DeleteBuffer( geometry.NormalBufferId );
                GL.DeleteBuffer( geometry.TextureCoordinateChannel0BufferId );
                GL.DeleteBuffer( geometry.ElementBufferId );

                if ( geometry.Material.HasDiffuse )
                {
                    GL.DeleteTexture( geometry.Material.DiffuseTextureId );
                }
            }

            mMeshes.Clear();
        }

        private void InitializeCamera()
        {
            var cameraFov = 45f;

            BoundingSphere bSphere;
            if ( !mModelPack.Model.BoundingSphere.HasValue )
            {
                if ( mModelPack.Model.BoundingBox.HasValue )
                {
                    bSphere = BoundingSphere.Calculate( mModelPack.Model.BoundingBox.Value );
                }
                else
                {
                    bSphere = new BoundingSphere( new System.Numerics.Vector3(), 0 );
                }
            }
            else
            {
                bSphere = mModelPack.Model.BoundingSphere.Value;
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

        private static Matrix4 ToMatrix4( ref System.Numerics.Matrix4x4 matrix )
        {
            unsafe
            {
                fixed ( System.Numerics.Matrix4x4* pMatrix = &matrix )
                    return *( Matrix4* )pMatrix;
            }
        }

        private static Matrix4 ToMatrix4( System.Numerics.Matrix4x4 matrix )
        {
            unsafe
            {
                return *( Matrix4* )&matrix;
            }
        }

        //
        // Texture stuff
        //

        private static int CreateGLTexture( Texture texture )
        {
            int textureId = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, textureId );

            var ddsHeader = new DDS_Header( new MemoryStream( texture.Data ) );

            // todo: identify and retrieve values from texture
            // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
            int mipMapCount = ddsHeader.dwMipMapCount;
            if ( mipMapCount > 0 )
                --mipMapCount;
            else
                mipMapCount = 1;

            SetGLTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, mipMapCount );

            var format = GetGLTexturePixelInternalFormat( ddsHeader.Format );

            SetGLTextureDDSImageData( ddsHeader.Width, ddsHeader.Height, format, mipMapCount, texture.Data, 0x80 );

            return textureId;
        }

        private static int CreateGLTexture( FieldTexturePS3 texture )
        {
            int textureId = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, textureId );

            // todo: identify and retrieve values from texture
            // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
            SetGLTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, texture.MipMapCount - 1 );

            var format = GetGLTexturePixelInternalFormat( texture.Flags );

            SetGLTextureDDSImageData( texture.Width, texture.Height, format, texture.MipMapCount, texture.Data );

            return textureId;
        }

        private static PixelInternalFormat GetGLTexturePixelInternalFormat( ImageEngineFormat format )
        {
            switch ( format )
            {
                case ImageEngineFormat.DDS_DXT1:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;

                case ImageEngineFormat.DDS_DXT3:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;

                case ImageEngineFormat.DDS_DXT5:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;

                case ImageEngineFormat.DDS_ARGB_8:
                    return PixelInternalFormat.Rgba8;

                default:
                    throw new NotImplementedException(format.ToString());
            }
        }

        private static PixelInternalFormat GetGLTexturePixelInternalFormat( FieldTextureFlags flags )
        {
            PixelInternalFormat format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
            if ( flags.HasFlag( FieldTextureFlags.DXT3 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
            }
            else if ( flags.HasFlag( FieldTextureFlags.DXT5 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }

            return format;
        }

        private static void SetGLTextureParameters( TextureWrapMode wrapS, TextureWrapMode wrapT, TextureMagFilter magFilter, TextureMinFilter minFilter, int maxMipLevel )
        {
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )wrapS );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )wrapT );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )magFilter );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )minFilter );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, maxMipLevel );
        }

        private static void SetGLTextureDDSImageData( int width, int height, PixelInternalFormat format, int mipMapCount, byte[] data, int dataOffset = 0 )
        {
            var dataHandle = GCHandle.Alloc( data, GCHandleType.Pinned );

            SetGLTextureDDSImageData( width, height, format, mipMapCount, ( dataHandle.AddrOfPinnedObject() + dataOffset ) );

            dataHandle.Free();
        }

        private static void SetGLTextureDDSImageData( int width, int height, PixelInternalFormat format, int mipMapCount, IntPtr data )
        {
            int mipWidth = width;
            int mipHeight = height;
            int blockSize = ( format == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext ) ? 8 : 16;
            int mipOffset = 0;

            for ( int mipLevel = 0; mipLevel < mipMapCount; mipLevel++ )
            {
                int mipSize = ( ( mipWidth * mipHeight ) / 16 ) * blockSize;

                if ( mipSize > blockSize )
                    GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, mipSize, data + mipOffset );

                mipOffset += mipSize;
                mipWidth /= 2;
                mipHeight /= 2;
            }
        }

        //
        // Model stuff
        //

        private GLMesh CreateGLMesh( Mesh mesh, Node parentNode, List<Node> nodes, BonePalette bonePalette )
        {
            var glMesh = new GLMesh();

            var vertices = mesh.Vertices;
            var normals = mesh.Normals;

            if ( mesh.VertexWeights != null )
                ( vertices, normals ) = mesh.Transform( parentNode, nodes, bonePalette );

            // vertex array
            glMesh.VertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray( glMesh.VertexArrayId );

            // positions
            glMesh.PositionBufferId = CreateGLVertexAttributeBuffer( vertices.Length * Vector3.SizeInBytes, vertices, 0, 3 );

            // normals
            if ( normals != null )
                glMesh.NormalBufferId = CreateGLVertexAttributeBuffer( normals.Length * Vector3.SizeInBytes, normals, 1, 3 );

            if ( mesh.TexCoordsChannel0 != null )
            {
                // texture coordinate channel 0
                glMesh.TextureCoordinateChannel0BufferId = CreateGLVertexAttributeBuffer( mesh.TexCoordsChannel0.Length * Vector2.SizeInBytes, mesh.TexCoordsChannel0, 2, 2 );
            }

            // element index buffer
            glMesh.ElementBufferId = CreateGLBuffer( BufferTarget.ElementArrayBuffer, mesh.Triangles.Length * Triangle.SizeInBytes, mesh.Triangles );
            glMesh.ElementIndexCount = mesh.Triangles.Length * 3;

            // material
            if ( mesh.MaterialName != null && mModelPack.Materials != null )
            {
                if ( mModelPack.Materials.TryGetMaterial( mesh.MaterialName, out var material ) )
                {
                    glMesh.Material = CreateGLMaterial( material );
                }
                else
                {
                    Trace.TraceError( $"Mesh referenced material \"{mesh.MaterialName}\" which does not exist in the model" );
                }
            }

            glMesh.IsVisible = true;

            return glMesh;
        }

        private static int CreateGLBuffer<T>( BufferTarget target, int size, T[] data ) where T : struct
        {
            // generate buffer id
            int buffer = GL.GenBuffer();

            // mark buffer as active
            GL.BindBuffer( target, buffer );

            // upload data to buffer store

#if GL_DEBUG
            try
            {
#endif
            GL.BufferData( target, size, data, BufferUsageHint.StaticDraw );
#if GL_DEBUG
            }
            catch ( Exception )
            {
            }
#endif

            return buffer;
        }

        private static int CreateGLVertexAttributeBuffer<T>( int size, T[] vertexData, int attributeIndex, int attributeSize, VertexAttribPointerType type = VertexAttribPointerType.Float ) where T : struct
        {
            // create buffer for vertex data store
            int buffer = CreateGLBuffer( BufferTarget.ArrayBuffer, size, vertexData );

            // configure vertex attribute
            GL.VertexAttribPointer( attributeIndex, attributeSize, type, false, 0, 0 );

            // enable vertex attribute
            GL.EnableVertexAttribArray( attributeIndex );

            return buffer;
        }

        private GLMaterial CreateGLMaterial( Material material )
        {
            var glMaterial = new GLMaterial();

            // color parameters
            glMaterial.Ambient = new Vector4( material.Ambient.X, material.Ambient.Y, material.Ambient.Z, material.Ambient.W );
            glMaterial.Diffuse = new Vector4( material.Diffuse.X, material.Diffuse.Y, material.Diffuse.Z, material.Diffuse.W );
            glMaterial.Specular = new Vector4( material.Specular.X, material.Specular.Y, material.Specular.Z, material.Specular.W );
            glMaterial.Emissive = new Vector4( material.Emissive.X, material.Emissive.Y, material.Emissive.Z, material.Emissive.W );

            // texture
            if ( material.DiffuseMap != null )
            {
                glMaterial.DiffuseTextureId = CreateGLTextureMap( material, material.DiffuseMap.Name );
                glMaterial.HasAlphaTransparency = material.DrawOrder != MaterialDrawOrder.Front; // && material.Field4D == 1 && material.Field90 == 0x0080;
            }

            return glMaterial;
        }

        private int CreateGLTextureMap( Material material, string name )
        {
            if ( mIsFieldModel && mFieldTextures.TryOpenFile( name, out var textureStream ) )
            {
                using ( textureStream )
                {
                    var texture = new FieldTexturePS3( textureStream );
                    return CreateGLTexture( texture );
                }
            }
            else if ( mModelPack.Textures.TryGetTexture( name, out var texture ) )
            {
                return CreateGLTexture( texture );
            }
            else
            {
                Trace.TraceWarning( $"tTexture '{ name }' used by material '{ material.Name }' is missing" );
            }

            return 0;
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

                    if ( mModelPack.Model.BoundingSphere.HasValue )
                    {
                        var bSphere = mModelPack.Model.BoundingSphere.Value;
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
                        translation = CalculateCameraTranslation( mCamera.FieldOfView, mModelPack.Model.BoundingSphere.Value );

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

        private struct GLMesh
        {
            public int VertexArrayId;
            public int PositionBufferId;
            public int NormalBufferId;
            public int TextureCoordinateChannel0BufferId;
            public int ElementBufferId;
            public int ElementIndexCount;
            public GLMaterial Material;
            public Matrix4 ModelMatrix;
            public bool IsVisible;
        }

        private struct GLMaterial
        {
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public Vector4 Emissive;
            public int DiffuseTextureId;
            public bool HasAlphaTransparency;

            public bool HasDiffuse => DiffuseTextureId != 0;
        }
    }
}
