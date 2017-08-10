using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using AtlusGfdLib;
using System.IO;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;
using OpenTK.Input;
using AtlusGfdEditor.GUI.Controls.ModelView;

namespace AtlusGfdEditor.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private Model mModel;
        private GLShaderProgram mShaderProgram;
        private List<int> mVertexArrays = new List<int>();
        private List<int> mBuffers = new List<int>();
        private List<int> mElementCounts = new List<int>();
        private List<Matrix4> mMatrices = new List<Matrix4>();
        private List<int> mTextures = new List<int>();
        private List<Material> mMaterials = new List<Material>();
        private GLPerspectiveCamera mCamera;
        private bool mIsModelLoaded;
        private bool mIsFieldModel;
        private Archive mFieldTextures;
        private int mLastMouseX;
        private int mLastMouseY;

        public ModelViewControl() : base( 
            new GraphicsMode(32, 24, 0, 4),
            3,
            3,
            GraphicsContextFlags.Debug | GraphicsContextFlags.ForwardCompatible)
        {
            InitializeComponent();
            MakeCurrent();
            Dock = DockStyle.Fill;
        }

        public void LoadModel( Model model )
        {
            mModel = model;
            DeleteModel();
            LoadModel();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();

                if ( mShaderProgram != null )
                    mShaderProgram.Dispose();

                DeleteModel();
            }

            base.Dispose( disposing );
        }

        protected override void OnLoad( EventArgs e )
        {
            PrintGLInfo();
            InitializeGL();
            InitializeShaders();
            InitializeEvents();
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            // clear the buffers
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            if ( mIsModelLoaded )
            {
                for ( int i = 0; i < mVertexArrays.Count; i++ )
                {
                    var model = mMatrices[i];
                    var material = mMaterials[i];
                    var modelViewProj = model * mCamera.CalculateViewProjectionMatrix();

                    mShaderProgram.SetUniform( "modelViewProj", modelViewProj );

                    if ( mTextures[i] != 0 )
                    {
                        mShaderProgram.SetUniform( "hasDiffuse", true );
                        GL.BindTexture( TextureTarget.Texture2D, mTextures[i] );
                    }
                    else
                    {
                        mShaderProgram.SetUniform( "hasDiffuse", false );
                    }

                    mShaderProgram.SetUniform( "matAmbient", new Vector4( material.Ambient.X, material.Ambient.Y, material.Ambient.Z, material.Ambient.W ) );
                    mShaderProgram.SetUniform( "matDiffuse", new Vector4( material.Diffuse.X, material.Diffuse.Y, material.Diffuse.Z, material.Diffuse.W ) );
                    mShaderProgram.SetUniform( "matSpecular", new Vector4( material.Specular.X, material.Specular.Y, material.Specular.Z, material.Specular.W ) );
                    mShaderProgram.SetUniform( "matEmissive", new Vector4( material.Emissive.X, material.Emissive.Y, material.Emissive.Z, material.Emissive.W ) );

                    mShaderProgram.Use();

                    // use the vertex array
                    GL.BindVertexArray( mVertexArrays[i] );

                    // draw the polygon
                    GL.DrawElements( PrimitiveType.Triangles, mElementCounts[i], DrawElementsType.UnsignedInt, 0 );
                }
            }

            SwapBuffers();
        }

        protected override void OnResize( EventArgs e )
        {
            if ( !mIsModelLoaded )
                return;

            mCamera.AspectRatio = ( float )Width / ( float )Height;
            GL.Viewport( ClientRectangle );
        }

        private void PrintGLInfo()
        {
            Console.WriteLine( $"GL info" );
            Console.WriteLine( $"Vendor         {GL.GetString( StringName.Vendor )}" );
            Console.WriteLine( $"Renderer       {GL.GetString( StringName.Renderer )}" );
            Console.WriteLine( $"Version        {GL.GetString( StringName.Version )}" );
            Console.WriteLine( $"Extensions     {GL.GetString( StringName.Extensions )}" );
            Console.WriteLine( $"GLSL version   {GL.GetString( StringName.ShadingLanguageVersion )}" );
            Console.WriteLine();
        }

        private void InitializeGL()
        {
            GL.ClearColor( Color.LightGray );
            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.CullFace( CullFaceMode.Back );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );
        }

        private void InitializeShaders()
        {
            if ( !GLShaderProgram.TryCreate(
                Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "VertexShader.glsl" ),
                Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "FragmentShader.glsl" ),
                out mShaderProgram ) )
            {
                Console.WriteLine( "Failed to compile shaders. Trying to use basic shaders.." );

                if ( !GLShaderProgram.TryCreate(
                    Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "VertexShaderBasic.glsl" ),
                    Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "FragmentShaderBasic.glsl" ),
                    out mShaderProgram ) )
                {
                    throw new Exception( "Failed to compile basic shaders." );
                }
            }

            // register shader uniforms
            mShaderProgram.RegisterUniform<Matrix4>( "modelViewProj" );
            mShaderProgram.RegisterUniform<bool>( "hasDiffuse" );
            mShaderProgram.RegisterUniform<Vector4>( "matAmbient" );
            mShaderProgram.RegisterUniform<Vector4>( "matDiffuse" );
            mShaderProgram.RegisterUniform<Vector4>( "matSpecular" );
            mShaderProgram.RegisterUniform<Vector4>( "matEmissive" );
        }

        private void InitializeEvents()
        {
        }

        private int CreateGLTexture( Texture texture )
        {
            int textureId = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, textureId );

            // set up params
            // todo: identify and retrieve values from texture
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )TextureMagFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )TextureMinFilter.LinearMipmapLinear );

            var ddsHeader = new DDS_Header( new MemoryStream( texture.Data ) );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, ddsHeader.dwMipMapCount - 1 );

            PixelInternalFormat format;

            switch ( ddsHeader.Format )
            {
                case ImageEngineFormat.DDS_DXT1:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case ImageEngineFormat.DDS_DXT3:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case ImageEngineFormat.DDS_DXT5:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                default:
                    throw new System.Exception();
            }

            int mipWidth = ddsHeader.Width;
            int mipHeight = ddsHeader.Height;
            int blockSize = ( format == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext ) ? 8 : 16;
            int mipOffset = 0;

            unsafe
            {
                fixed ( byte* pBuffer = texture.Data )
                {
                    for ( int mipLevel = 0; mipLevel < ddsHeader.dwMipMapCount; mipLevel++ )
                    {
                        int mipSize = ( ( mipWidth * mipHeight ) / 16 ) * blockSize;
                        GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, mipSize, ( IntPtr )( pBuffer + ( 0x80 + mipOffset ) ) );

                        mipOffset += mipSize;
                        mipWidth /= 2;
                        mipHeight /= 2;
                    }
                }
            }

            return textureId;
        }

        private int CreateGLTexture( FieldTexture texture )
        {
            int textureId = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, textureId );

            // set up params
            // todo: identify and retrieve values from texture
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )TextureMagFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )TextureMinFilter.LinearMipmapLinear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, texture.MipMapCount - 1 );

            PixelInternalFormat format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
            if ( texture.Flags.HasFlag( FieldTextureFlags.DXT3 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
            }
            else if ( texture.Flags.HasFlag( FieldTextureFlags.DXT5 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }

            int mipWidth = texture.Width;
            int mipHeight = texture.Height;
            int blockSize = ( format == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext ) ? 8 : 16;
            int mipOffset = 0;

            unsafe
            {
                fixed ( byte* pBuffer = texture.Data )
                {
                    for ( int mipLevel = 0; mipLevel < texture.MipMapCount; mipLevel++ )
                    {
                        int mipSize = ( ( mipWidth * mipHeight ) / 16 ) * blockSize;

                        GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, mipSize, ( IntPtr )( pBuffer + mipOffset ) );

                        mipOffset += mipSize;
                        mipWidth /= 2;
                        mipHeight /= 2;
                    }
                }
            }

            return textureId;
        }

        private void CreateGLObjects( Node node, Geometry geometry )
        {
            var material = mModel.MaterialDictionary[geometry.MaterialName];
            mMaterials.Add( material );

            if ( material.DiffuseMap != null )
            {
                int textureId;
                var diffuseMapName = mModel.MaterialDictionary[geometry.MaterialName].DiffuseMap.Name;

                if ( mIsFieldModel && mFieldTextures.TryOpenFile( diffuseMapName, out var textureStream ) )
                {
                    using ( textureStream )
                    {
                        var texture = new FieldTexture( textureStream );
                        textureId = CreateGLTexture( texture );
                    }
                }
                else
                {
                    var texture = mModel.TextureDictionary[diffuseMapName];
                    textureId = CreateGLTexture( texture );
                }

                mTextures.Add( textureId );
            }
            else
            {
                mTextures.Add( 0 );
            }

            // vao
            int vao = GL.GenVertexArray();
            GL.BindVertexArray( vao );
            mVertexArrays.Add( vao );

            // position
            int vbo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ArrayBuffer, vbo );
            GL.BufferData( BufferTarget.ArrayBuffer, geometry.Vertices.Length * 12, geometry.Vertices, BufferUsageHint.StaticDraw );
            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 0, 0 );
            GL.EnableVertexAttribArray( 0 );
            mBuffers.Add( vbo );

            // normal
            int nbo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ArrayBuffer, nbo );
            GL.BufferData( BufferTarget.ArrayBuffer, geometry.Normals.Length * 12, geometry.Normals, BufferUsageHint.StaticDraw );
            GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, 0, 0 );
            GL.EnableVertexAttribArray( 1 );
            mBuffers.Add( nbo );

            if ( geometry.TexCoordsChannel0 != null )
            {
                // texture coord
                int tbo = GL.GenBuffer();
                GL.BindBuffer( BufferTarget.ArrayBuffer, tbo );
                GL.BufferData( BufferTarget.ArrayBuffer, geometry.TexCoordsChannel0.Length * 8, geometry.TexCoordsChannel0, BufferUsageHint.StaticDraw );
                GL.VertexAttribPointer( 2, 2, VertexAttribPointerType.Float, false, 0, 0 );
                GL.EnableVertexAttribArray( 2 );
                mBuffers.Add( tbo );
            }

            // element 
            int ebo = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, ebo );
            GL.BufferData( BufferTarget.ElementArrayBuffer, geometry.Triangles.Length * ( sizeof( int ) * 3 ), geometry.Triangles, BufferUsageHint.StaticDraw );
            mBuffers.Add( ebo );
            mElementCounts.Add( geometry.Triangles.Length * 3 );

            var oldMatrix = node.WorldTransform;
            var newMatrix = new Matrix4(
                oldMatrix.M11, oldMatrix.M12, oldMatrix.M13, oldMatrix.M14,
                oldMatrix.M21, oldMatrix.M22, oldMatrix.M23, oldMatrix.M24,
                oldMatrix.M31, oldMatrix.M32, oldMatrix.M33, oldMatrix.M34,
                oldMatrix.M41, oldMatrix.M42, oldMatrix.M43, oldMatrix.M44 );
            mMatrices.Add( newMatrix );
        }

        private void InitializeCamera()
        {
            var cameraFov = 45f;

            BoundingSphere bSphere;
            if ( !mModel.Scene.BoundingSphere.HasValue )
            {
                if ( mModel.Scene.BoundingBox.HasValue )
                {
                    bSphere = BoundingSphere.Calculate( mModel.Scene.BoundingBox.Value );
                }
                else
                {
                    bSphere = new BoundingSphere( new System.Numerics.Vector3(), 0 );
                }
            }
            else
            {
                bSphere = mModel.Scene.BoundingSphere.Value;
            }

            var cameraFovInRad = MathHelper.DegreesToRadians( cameraFov );
            var distance = ( float )( ( bSphere.Radius * 2.0f ) / Math.Tan( cameraFovInRad / 2.0f ) );
            var cameraTranslation = new Vector3( bSphere.Center.X, bSphere.Center.Y, bSphere.Center.Z ) + new Vector3( 0, -50, 300 );

            mCamera = new GLPerspectiveFreeCamera( cameraTranslation, 1f, 100000f, cameraFov, ( float )Width / ( float )Height, Quaternion.Identity );
        }

        private void LoadModel()
        {
            if ( mModel.Scene == null )
                return;

            InitializeCamera();

            foreach ( var node in mModel.Scene.Nodes )
            {
                if ( !node.HasAttachments )
                    continue;

                foreach ( var attachment in node.Attachments )
                {
                    if ( attachment.Type != NodeAttachmentType.Geometry )
                        continue;

                    var geometry = attachment.GetValue<Geometry>();

                    CreateGLObjects( node, geometry );
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

            foreach ( var vao in mVertexArrays )
            {
                GL.DeleteVertexArray( vao );
            }

            mVertexArrays.Clear();

            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

            foreach ( var buffer in mBuffers )
            {
                GL.DeleteBuffer( buffer );
            }

            mBuffers.Clear();
            mElementCounts.Clear();
            mMatrices.Clear();

            GL.BindTexture( TextureTarget.Texture2D, 0 );

            foreach ( var texture in mTextures )
            {
                if ( texture != 0 )
                    GL.DeleteTexture( texture );
            }

            mMaterials.Clear();
            mTextures.Clear();

            GL.BindVertexArray( 0 );
        }

        protected override void OnMouseMove( System.Windows.Forms.MouseEventArgs e )
        {
            if ( !mIsModelLoaded )
                return;

            if ( e.Button.HasFlag( MouseButtons.Middle ) )
            {
                float multiplier = 1.0f;
                if ( Keyboard.GetState().IsKeyDown( Key.ShiftLeft ) )
                    multiplier = 8.0f;

                int xDelta = e.X - mLastMouseX;
                int yDelta = e.Y - mLastMouseY;

                if ( Keyboard.GetState().IsKeyDown( Key.AltLeft ) )
                {
                    // Orbit around model
                    var bSphere = mModel.Scene.BoundingSphere.Value;
                    mCamera = new GLPerspectiveTargetCamera( mCamera.Translation, mCamera.ZNear, mCamera.ZFar, mCamera.FieldOfView, mCamera.AspectRatio, new Vector3( bSphere.Center.X, bSphere.Center.Y, bSphere.Center.Z ) );
                    mCamera.Translation = new Vector3(
                        mCamera.Translation.X - ( ( xDelta / 3f ) * multiplier ),
                        mCamera.Translation.Y + ( ( yDelta / 3f ) * multiplier ),
                        mCamera.Translation.Z );
                }
                else
                {
                    // Move camera
                    mCamera = new GLPerspectiveFreeCamera( mCamera.Translation, mCamera.ZNear, mCamera.ZFar, mCamera.FieldOfView, mCamera.AspectRatio, Quaternion.Identity );
                    mCamera.Translation = new Vector3(
                        mCamera.Translation.X - ( ( xDelta / 3f ) * multiplier ),
                        mCamera.Translation.Y + ( ( yDelta / 3f ) * multiplier ),
                        mCamera.Translation.Z );
                }
            }

            mLastMouseX = e.X;
            mLastMouseY = e.Y;

            Invalidate();
        }

        protected override void OnMouseWheel( System.Windows.Forms.MouseEventArgs e )
        {
            if ( !mIsModelLoaded )
                return;

            float multiplier = 1.0f;
            if ( Keyboard.GetState().IsKeyDown( Key.ShiftLeft ) )
                multiplier = 8.0f;

            mCamera.Translation = Vector3.Subtract( mCamera.Translation, ( Vector3.UnitZ * ( ( (float)e.Delta * 8 ) * multiplier ) ) );

            Invalidate();
        }
    }
}
