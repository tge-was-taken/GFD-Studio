using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using AtlusGfdLib;
using AtlusGfdLib.IO;
using AtlusLibSharp.Graphics.RenderWare;
using AtlusLibSharp.Utilities;
using CSharpImageLibrary;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace AtlusGfdLibTesting
{
    class Program
    {
        static void Main( string[] args )
        {
            /*
            var rmdPacFile = new PakToolArchiveFile( @"D:\Modding\Persona 3 & 4\Persona3\CVM_BTL\MODEL\PACK\BC001_WP0.PAC" );
            var rmdPacEntry = rmdPacFile.Entries.Single( x => x.Name.EndsWith( "rmd", System.StringComparison.InvariantCultureIgnoreCase ) );
            var rmdScene = new RmdScene( rmdPacEntry.Data );

            RMDToGMD( rmdScene );
            */

            //ModelViewerTest( Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" ) );

            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\field_tex\f051_010_0.GFS" );
            //ExportDAE( model );

            /*
            var archive = new Archive( @"D:\Modding\Persona 5\Dump\model\field_tex\textures\tex051_010_00_00.bin" );
            var archiveBuilder = new ArchiveBuilder();
            foreach ( var file in archive )
            {
                archiveBuilder.AddFile( file, archive.OpenFile( file ) );
            }
            var newArchive = archiveBuilder.Build();

            newArchive.SaveToFile( @"D:\Modding\Persona 5\Dump\model\field_tex\textures\tex051_010_00_00_new.bin" );
            */

            /*
            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" );
            var textures = model.TextureDictionary;
            var materials = model.MaterialDictionary;
            var scene = model.Scene;
            var animations = model.AnimationPackage;

            ResourceExporter.ExportToAssimpScene( model );
            */

            /*
            foreach ( var material in model.MaterialDictionary.Materials )
            {
                if ( material.DiffuseMap == null )
                    continue;

                var newMat = new Material( material.Name );
                newMat.Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.Flag80 | MaterialFlags.Flag800 | MaterialFlags.Flag4000 | MaterialFlags.HasDiffuseMap;
                newMat.Ambient = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                newMat.Diffuse = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                newMat.DiffuseMap = material.DiffuseMap;
                newMat.Emissive = new Vector4( 0, 0, 0, 0 );
                newMat.Field40 = 1;
                newMat.Field44 = 0.1f;
                newMat.Field48 = 0;
                newMat.Field49 = 1;
                newMat.Field4A = 0;
                newMat.Field4B = 1;
                newMat.Field4C = 0;
                newMat.Field50 = 0;
                newMat.Field5C = 0;
                newMat.Field6C = unchecked(( int )0xfffffff8);
                newMat.Field70 = unchecked(( int )0xfffffff8);
                newMat.Field90 = 0;
                newMat.Field92 = 4;
                newMat.Field94 = 1;
                newMat.Field96 = 0;
                newMat.Field98 = -1;
                newMat.GlowMap = null;
                newMat.HighlightMap = null;
                newMat.NightMap = null;
                newMat.Properties = null;
                newMat.ReflectionMap = null;
                newMat.ShadowMap = null;
                newMat.Specular = new Vector4( 0, 0, 0, 0 );
                newMat.SpecularMap = null;

                model.MaterialDictionary[material.Name] = newMat;
            }
            */

            /*
            var mapOriginal = model.Scene.MatrixMap;
            var skinnedNodeIndices = new List<int>();
            var worldMatrices = new List<Matrix4x4>();
            var originalSkinnedNodeIndices = mapOriginal.BoneToNodeIndices.Distinct().ToList();
            var duplicateKeys = mapOriginal.BoneToNodeIndices.GroupBy( x => x )
                        .Where( group => group.Count() > 1 )
                        .Select( group => group.Key )
                        .ToList();

            for ( int i = 0; i < scene.Nodes.Count; i++ )
            {
                var node = scene.Nodes[i];

                if ( !node.HasAttachments )
                    continue;

            }
            */

            /*
            foreach ( var geometry in node.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).Select( x => x.GetValue<Geometry>() ) )
            {
                if ( ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) ) )
                {
                    Matrix4x4.Invert( node.WorldTransform, out var nodeInverseWorldTransform );

                    foreach ( var vertexWeight in geometry.VertexWeights )
                    {
                        for ( int j = 0; j < 4; j++ )
                        {
                            if ( vertexWeight.Weights[j] == 0.0f )
                                continue;

                            int index = mapOriginal.BoneToNodeIndices[vertexWeight.Indices[j]];

                            if ( !skinnedNodeIndices.Contains( index ) )
                            {
                                vertexWeight.Indices[j] = (byte)skinnedNodeIndices.Count;
                                skinnedNodeIndices.Add( index );
                                worldMatrices.Add( scene.Nodes[index].WorldTransform * nodeInverseWorldTransform );                             
                            }
                            else
                            {
                                vertexWeight.Indices[j] = (byte)skinnedNodeIndices.IndexOf( index );
                            }
                        }
                    }
                }
            }
        }
        */

            /*
            for ( int i = 0; i < scene.Nodes.Count; i++ )
            {
                var node = scene.Nodes[i];

                if ( !node.HasAttachments )
                    continue;

                foreach ( var geometry in node.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).Select( x => x.GetValue<Geometry>() ) )
                {
                    if ( ( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) ) )
                    {
                        foreach ( var vertexWeight in geometry.VertexWeights )
                        {
                            for ( int j = 0; j < 4; j++ )
                            {
                                if ( vertexWeight.Weights[j] == 0.0f )
                                    continue;

                                int index = mapOriginal.BoneToNodeIndices[vertexWeight.Indices[j]];

                                if ( duplicateKeys.Contains( ( ushort )index ) )
                                {
                                    //vertexWeight.Indices[j] = 0;
                                    //vertexWeight.Indices[j] = (byte)Array.IndexOf( mapOriginal.BoneToNodeIndices, index );
                                }
                            }
                        }
                    }
                }
            }
            */

            /*
            for ( int i = 0; i < mapOriginal.BoneInverseBindMatrices.Length; i++ )
            {

            }
            */

            /*
            for ( int i = 0; i < mapOriginal.RemapIndices.Length; i++ )
            {
                var node = model.Scene.Nodes[mapOriginal.RemapIndices[i]];
                Matrix4x4.Invert( node.WorldTransform, out var matrix );
                
                Debug.WriteLine( node.Name );
                Debug.WriteLine( node.LocalTransform );
                //Debug.WriteLine( matrix );
                Debug.WriteLine( mapOriginal.Matrices[i] );
            }
            */

            //Debug.WriteLine( $"Max {mapOriginal.RemapIndices.Max()}" );
            //Debug.WriteLine( $"Min {mapOriginal.RemapIndices.Min()}" );

            // remap indices -> breadth first indices into nodes list
            // first index = first weighted node
            // i suppose it contains every node that would affect the transform of any mesh


            //var mapNew = MatrixMap.Create( model.Scene.Nodes );

            //scene.MatrixMap = map;
        }

        static void ModelViewerTest( Model model )
        {
            using ( var window = new RenderWindow() )
            {
                RenderModel renderModel = new RenderModel( model );
 
                window.Model = renderModel;
                window.Run();
            }
        }

        static void RMDToGMD( RmdScene rmdScene )
        {
            var rmdClump = rmdScene.Clumps[0];

            //var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_158_00.GMD" );

            var model = new Model( 0x01105070 );

            model.TextureDictionary.Clear();
            foreach ( var rwTexture in rmdScene.TextureDictionary.Textures )
            {
                var bitmap = rwTexture.GetBitmap();
                bitmap.Save( rwTexture.Name + ".png" );

                var pngImage = new ImageEngineImage( rwTexture.Name + ".png" );
                var ddsData = pngImage.Save( new ImageFormats.ImageEngineFormatDetails(ImageEngineFormat.DDS_DXT5), MipHandling.Default );

                var texture = new Texture( rwTexture.Name + ".dds", TextureFormat.DDS, ddsData );
                model.TextureDictionary.Add( texture );
            }

            model.MaterialDictionary.Clear();
            foreach ( var rwGeometry in rmdClump.GeometryList )
            {
                foreach ( var rwMaterial in rwGeometry.Materials )
                {
                    string materialName = rwMaterial.TextureReferenceNode.ReferencedTextureName + "_material";
                    if ( model.MaterialDictionary.ContainsMaterial( materialName ) )
                        continue;

                    Material material;

                    if ( rwMaterial.IsTextured )
                    {
                        // flag 1 = no apparent effect
                        // flag 2 = no apparent effect
                        // flag 4 = no apparent effect
                        // flag 8 = no apparent effect
                        // flag 10 = crash
                        // flag 20 = no apparent effect 
                        // flag 40 = no apparent effect 
                        // flag 80 = Affected by light
                        // flag 100 = no apparent effect 
                        // flag 200 = no apparent effect 
                        // flag 400 = no apparent effect
                        // flag 800 = Affected by light
                        // flag 1000 = turns mesh purple with a wireframe effect
                        // flag 2000 = no apparent effect
                        // flag 4000 = Affected by shadows
                        // flag 8000 = cast shadow
                        // flag 20000 = crash
                        // flag 40000 = crash
                        // flag 80000 = disable bloom
                        // flag 20000000 = crash
                        // flag 40000000 = no apparent effect
                        // flag 80000000 = no apparent effect
                        // enabling cast shadow + receive shadow = crash

                        material = new Material( materialName );
                        material.Flags = MaterialFlags.EnableLight | MaterialFlags.EnableLight2 | MaterialFlags.CastShadow | MaterialFlags.DisableBloom | MaterialFlags.HasDiffuseMap;
                        material.Ambient = new Vector4( 1, 1, 1, 1f );
                        material.Diffuse = new Vector4( 1, 1, 1, 1f );
                        material.DiffuseMap = new TextureMap( rwMaterial.TextureReferenceNode.ReferencedTextureName + ".dds" );
                        material.Emissive = new Vector4( 0, 0, 0, 0 );
                        material.Field40 = 1;
                        material.Field44 = 0.1f;
                        material.Field48 = 0;
                        material.Field49 = 1;
                        material.Field4A = 0;
                        material.Field4B = 1;
                        material.Field4C = 0;
                        material.Field50 = 0;
                        material.Field5C = 0;
                        material.Field6C = unchecked(( int )0xfffffff8);
                        material.Field70 = unchecked(( int )0xfffffff8);
                        material.Field90 = 0;
                        material.Field92 = 4;
                        material.Field94 = 1;
                        material.Field96 = 0;
                        material.Field98 = -1;
                        material.GlowMap = null;
                        material.HighlightMap = null;
                        material.NightMap = null;
                        material.Attributes = null;
                        material.ReflectionMap = null;
                        material.ShadowMap = null;
                        material.Specular = new Vector4( 0, 0, 0, 0 );
                        material.SpecularMap = null;
                    }
                    else
                    {
                        material = new Material( materialName );
                    }

                    model.MaterialDictionary.Add( material );
                }
            }

            model.Scene.MatrixMap = null;

            foreach ( var node in model.Scene.Nodes )
            {
                if ( node.HasAttachments )
                {
                    node.Attachments.Clear();
                }
            }

            foreach ( var rwAtomic in rmdClump.Atomics )
            {
                var rwGeometry = rmdClump.GeometryList[rwAtomic.GeometryIndex];
                var rwFrame = rmdClump.FrameList[rwAtomic.FrameIndex];
                var rwMeshList = (RwMeshListNode)rwGeometry.ExtensionNodes.Find( x => x.Id == RwNodeId.RwMeshListNode );

                foreach ( var rwMesh in rwMeshList.MaterialMeshes )
                {
                    var rwIndices = MeshUtilities.ToTriangleList( rwMesh.Indices , false);
                    var geometry = new Geometry();
                    geometry.TriangleIndexType = TriangleIndexType.UInt16;
                    geometry.Triangles = new Triangle[rwIndices.Length / 3];

                    int y = 0;
                    for ( int i = 0; i < geometry.Triangles.Length; i++ )
                    {
                        geometry.Triangles[i] = new Triangle( y++, y++, y++ );
                    }

                    geometry.MaterialName = rwGeometry.Materials[rwMesh.MaterialIndex].TextureReferenceNode.ReferencedTextureName + "_material";

                    geometry.Vertices = new Vector3[rwIndices.Length];
                    geometry.Normals = new Vector3[rwIndices.Length];
                    geometry.TexCoordsChannel0 = new Vector2[rwIndices.Length];

                    for ( int i = 0; i < rwIndices.Length; i++ )
                    {
                        geometry.Vertices[i] = Vector3.Transform(rwGeometry.Vertices[rwIndices[i]], rwFrame.WorldTransform);
                        geometry.Normals[i] = Vector3.TransformNormal(rwGeometry.Normals[rwIndices[i]], rwFrame.WorldTransform);
                        geometry.TexCoordsChannel0[i] = rwGeometry.TextureCoordinateChannels[0][rwIndices[i]];
                    }

                    geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
                    geometry.BoundingSphere = BoundingSphere.Calculate( geometry.Vertices );

                    model.Scene.RootNode.Attachments.Add( new NodeGeometryAttachment( geometry ) );
                }
            }


            Resource.Save( model, @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_158_00_new.GMD" );

        }

        static void ExportDAE( Model model )
        {
            AssimpExporter.ExportToFile( model, "model.dae" );
        }
    }

    public class RenderWindow : OpenTK.GameWindow
    {
        public RenderModel Model { get; set; }

        public RenderWindow() : base( 1920, 1080 )
        {
        }

        protected override void OnLoad( EventArgs e )
        {
            GL.ClearColor( Color.LightGray );
            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.CullFace( CullFaceMode.Back );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );
        }

        protected override void OnResize( EventArgs e )
        {
            GL.Viewport( 0, 0, Width, Height );
        }

        protected override void OnKeyDown( KeyboardKeyEventArgs e )
        {
            switch ( e.Key )
            {
                case Key.S:

                    break;
            }

        }

        protected override void OnUpdateFrame( OpenTK.FrameEventArgs e )
        {

        }

        protected override void OnRenderFrame( OpenTK.FrameEventArgs e )
        {
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            if ( Model != null )
                Model.Render();

            SwapBuffers();
        }
    }

    public class RenderTexture
    {
        public Texture Texture { get; }

        public int TextureBufferHandle { get; private set; }

        public RenderTexture( Texture texture )
        {
            Texture = texture;
            Initialize();
        }

        public void Initialize()
        {
            // create texture and bind it
            TextureBufferHandle = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, TextureBufferHandle );

            // set up params
            // todo: identify and retrieve values from texture
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )TextureWrapMode.Repeat );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )TextureMagFilter.Nearest );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )TextureMinFilter.Nearest );

            var ddsHeader = new CSharpImageLibrary.Headers.DDS_Header( new MemoryStream( Texture.Data ) );

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

            int width = ddsHeader.Width;
            int height = ddsHeader.Height;
            int blockSize = ( format == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext ) ? 8 : 16;
            int mipOffset = 0;

            unsafe
            {
                fixed ( byte* pBuffer = Texture.Data )
                {
                    ddsHeader.dwMipMapCount = 1;
                    for ( int mipLevel = 0; mipLevel < ddsHeader.dwMipMapCount; mipLevel++ )
                    {
                        int mipSize = ( ( ddsHeader.Width + 3 ) / 4 ) * ( ( ddsHeader.Height + 3 ) / 4 ) * blockSize;
                        GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, format, ddsHeader.Width, ddsHeader.Height, 0, mipSize, ( IntPtr )( pBuffer + ( 128 + mipOffset ) ) );

                        mipOffset += mipSize;
                        width /= 2;
                        height /= 2;
                    }
                }
            }

            // unbind
            GL.BindTexture( TextureTarget.Texture2D, 0 );
        }
    }

    public class RenderModel
    {
        public Model Model { get; }

        public Dictionary<string, RenderTexture> TextureLookup { get; private set; }

        public RenderScene RenderScene { get; private set; }

        public RenderModel( Model model )
        {
            Model = model;
            Initialize();
        }

        public void Initialize()
        {
            if ( Model.TextureDictionary != null )
            {
                TextureLookup = new Dictionary<string, RenderTexture>();
                foreach ( var texture in Model.TextureDictionary.Textures )
                {
                    TextureLookup[texture.Name] = new RenderTexture( texture );
                }
            }

            if ( Model.Scene != null )
            {
                RenderScene = new RenderScene( Model.Scene, this );
            }
        }

        public void Render()
        {
            if ( RenderScene != null )
                RenderScene.Render();
        }
    }

    public class RenderScene
    {
        public RenderModel RenderModel { get;  }

        public Scene Scene { get; set; }

        public Dictionary<Geometry, RenderGeometry> GeometryLookup { get; set; }

        public RenderScene( Scene scene, RenderModel renderModel )
        {
            RenderModel = renderModel;
            Scene = scene;
            GeometryLookup = new Dictionary<Geometry, RenderGeometry>();

            foreach ( var node in scene.Nodes.Where(x => x.HasAttachments) )
            {
                foreach ( var geometry in node.Attachments.Where(x => x.Type == NodeAttachmentType.Geometry).Select(x => x.GetValue<Geometry>()) )
                {
                    var diffuseMap = RenderModel.Model.MaterialDictionary[geometry.MaterialName].DiffuseMap;
                    RenderTexture renderTexture = null;

                    if ( diffuseMap != null )
                    {
                        renderTexture = RenderModel.TextureLookup[diffuseMap.Name];
                    }

                    GeometryLookup[geometry] = new RenderGeometry( geometry, renderTexture );
                }
            }
        }

        public void Render()
        {
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(45f * 0.0174532925f, 16f / 9f, 0.1f, 100.0f );
            var view = Matrix4x4.CreateLookAt( new Vector3( 4, 3, 3 ), new Vector3(), new Vector3( 0, 1, 0 ) );
            var model = Matrix4x4.Identity;
            var modelViewProjection = projection * view * model;

            void RenderNodes(Node node)
            {
                model *= node.LocalTransform;

                if ( node.HasAttachments )
                {
                    foreach ( var attachment in node.Attachments )
                    {
                        switch ( attachment.Type )
                        {
                            case NodeAttachmentType.Invalid:
                                break;
                            case NodeAttachmentType.Scene:
                                break;
                            case NodeAttachmentType.Mesh:
                                break;
                            case NodeAttachmentType.Node:
                                break;

                            case NodeAttachmentType.Geometry:
                                var geometry = GeometryLookup[attachment.GetValue<Geometry>()];
                                geometry.Render( );
                                break;

                            case NodeAttachmentType.Camera:
                                break;
                            case NodeAttachmentType.Light:
                                break;
                            case NodeAttachmentType.Epl:
                                break;
                            case NodeAttachmentType.EplLeaf:
                                break;
                            case NodeAttachmentType.Morph:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public class RenderGeometry
    {
        public Geometry Geometry { get; }

        public RenderTexture RenderTexture { get; }

        public int IndexBufferHandle { get; private set; }

        public int VertexBufferHandle { get; private set; }

        public int NormalBufferHandle { get; private set; }

        public int TexCoord0BufferHandle { get; private set; }

        public int TexCoord1BufferHandle { get; private set; }

        public int TexCoord2BufferHandle { get; private set; }

        public int ProgramHandle { get; private set; }

        public int DiffuseHandle { get; private set; }

        public int ModelViewProjectionHandle { get; private set; }

        public RenderGeometry( Geometry geometry, RenderTexture texture )
        {
            Geometry = geometry;
            RenderTexture = texture;

            Initialize();
        }

        public void Initialize()
        {
            if ( Geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                CreateIndexBuffer();
            }

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                CreateVertexBuffer();
            }

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ))
            {
                CreateTexCoordBuffer();
            }

            CreateShaders();
        }

        private void CreateTexCoordBuffer()
        {
            TexCoord0BufferHandle = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ArrayBuffer, TexCoord0BufferHandle );
            GL.BufferData( BufferTarget.ArrayBuffer, ( sizeof( float ) * 2 ) * Geometry.VertexCount, Geometry.TexCoordsChannel0, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
        }

        private void CreateIndexBuffer()
        {
            // generate buffer and bind it to the current context
            IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, IndexBufferHandle );

            // flatten the triangles into an array of ints
            int[] indices = new int[Geometry.TriangleCount * 3];
            int j = 0;
            for ( int i = 0; i < Geometry.Triangles.Length; i++, j += 3 )
            {
                Array.Copy( Geometry.Triangles[i].Indices, 0, indices, j, 3 );
            }

            // bind the index data to the bound buffer
            GL.BufferData( BufferTarget.ElementArrayBuffer, ( sizeof( int ) * 3 ) * Geometry.TriangleCount, indices, BufferUsageHint.StaticDraw );

            // unbind the buffer
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
        }

        private void CreateVertexBuffer()
        {
            // create vertex buffer 
            VertexBufferHandle = GL.GenBuffer();

            // bind it to the current context
            GL.BindBuffer( BufferTarget.ArrayBuffer, VertexBufferHandle );

            // bind the vertex data to the buffer
            GL.BufferData( BufferTarget.ArrayBuffer, ( sizeof( float ) * 3 ) * Geometry.VertexCount, Geometry.Vertices, BufferUsageHint.StaticDraw );

            // unbind
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
        }

        private void CreateShaders()
        {
            int vertexShaderHandle = GL.CreateShader( ShaderType.VertexShader );
            GL.ShaderSource( vertexShaderHandle,
                "#version 330 core\n" +
                "" +
                "layout(location = 0) in vec3 InPos;\n" +
                "layout(location = 2) in vec2 InTex0;\n" +
                "" +
                "out vec2 OutTex0;\n" +
                "" +
                "uniform mat4 ModelViewProjection;\n" +
                "" +
                "void main()\n" +
                "{\n" +
                "   gl_Position = vec4(InPos, 1);\n" +
                "   OutTex0 = InTex0;\n" +
                "}\n" );
            GL.CompileShader( vertexShaderHandle );

            Console.WriteLine( GL.GetShaderInfoLog( vertexShaderHandle ) );
            

            int fragmentShaderHandle = GL.CreateShader( ShaderType.FragmentShader );
            GL.ShaderSource( fragmentShaderHandle,
                "#version 330 core\n" +
                "" +
                "in vec2 InTex0;\n" +
                "" +
                "out vec3 OutColor;\n" +
                "" +
                "uniform sampler2D SamplerDiffuse;\n" +
                "" +
                "void main()\n" +
                "{\n" +
                "   OutColor = texture( SamplerDiffuse, InTex0 ).rgb;\n" +
                "}\n" );
            GL.CompileShader( fragmentShaderHandle );

            Console.WriteLine( GL.GetShaderInfoLog( fragmentShaderHandle ) );

            ProgramHandle = GL.CreateProgram();
            GL.AttachShader( ProgramHandle, vertexShaderHandle );
            GL.AttachShader( ProgramHandle, fragmentShaderHandle );
            GL.LinkProgram( ProgramHandle );

            Console.WriteLine( GL.GetProgramInfoLog( ProgramHandle ) );

            GL.DetachShader( ProgramHandle, vertexShaderHandle );
            GL.DetachShader( ProgramHandle, fragmentShaderHandle );

            GL.DeleteShader( vertexShaderHandle );
            GL.DeleteShader( fragmentShaderHandle );

            DiffuseHandle = GL.GetUniformLocation( ProgramHandle, "SamplerDiffuse" );
            ModelViewProjectionHandle = GL.GetUniformLocation( ProgramHandle, "ModelViewProjection" );
        }

        public void Render()
        {
            // Bind everything

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                GL.EnableVertexAttribArray( 0 );
                GL.BindBuffer( BufferTarget.ArrayBuffer, VertexBufferHandle );
                GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 0, 0 );
            }

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                GL.EnableVertexAttribArray( 2 );
                GL.BindBuffer( BufferTarget.ArrayBuffer, TexCoord0BufferHandle );
                GL.VertexAttribPointer( 2, 2, VertexAttribPointerType.Float, false, 0, 0 );
            }

            GL.UseProgram( ProgramHandle );

            if ( RenderTexture != null )
            {
                GL.BindTexture( TextureTarget.Texture2D, RenderTexture.TextureBufferHandle );
                GL.Uniform1( DiffuseHandle, 0 );
            }

            //GL.UniformMatrix4( ModelViewProjectionHandle, 16, false, mvp );

            if ( Geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                GL.BindBuffer( BufferTarget.ElementArrayBuffer, IndexBufferHandle );
                GL.DrawElements( BeginMode.Triangles, Geometry.TriangleCount * 3, DrawElementsType.UnsignedInt, 0 );             
            }

            // Unbind everything
            if ( RenderTexture != null )
            {
                GL.BindTexture( TextureTarget.Texture2D, 0 );
            }

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                GL.DisableVertexAttribArray( 0 );
            }

            if ( Geometry.VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                GL.DisableVertexAttribArray( 2 );
            }

            if ( Geometry.Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
            }
        }
    }
}
