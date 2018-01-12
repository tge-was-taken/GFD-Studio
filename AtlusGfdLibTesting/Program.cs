using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using AtlusGfdLib;
using AtlusGfdLib.Assimp;
using AtlusLibSharp.Graphics.RenderWare;
using AtlusLibSharp.Utilities;
using CSharpImageLibrary;

namespace AtlusGfdLibTesting
{
    // Notes:
    // Geometry complexity limit is around 3k tris/1.5k verts, if exceeded the game crashes

    class Program
    {
        static void Main( string[] args )
        {
            var animationPackage = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\emt0001.GAP" ).AnimationPackage;
            return;


            /*
            var model = Resource.Load< Model >( @"D:\Modding\Persona 4 Dancing CPK RIP\data\dance\player\pc001_01.GMD" );
            var material = model.MaterialDictionary["_pc001_01_01"];
            return;
            */

            /*
            var options = new ModelConverterOptions
            {
                MaterialPreset = MaterialPreset.CharacterClothP4D,
                Version = 0x01105030,
                ConvertSkinToZUp = false,
                GenerateVertexColors = true,
            };

            var model = ModelConverter.ConvertFromAssimpScene( @"D:\Modding\Persona 4 Dancing CPK RIP\Game mods\DanteOverYu\Dante.FBX", options );

            Resource.Save( model, @"D:\Modding\Persona 4 Dancing CPK RIP\Game mods\DanteOverYu\mod\dance\player\pc001_01.GMD" );
            return;
            */

            /*
            var model = ModelFactory.ConvertFromAssimpScene( @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\CityEscape.FBX", MaterialPreset.FieldTerrainDiffuseCastShadow );
            model.TextureDictionary = TextureDictionary.ConvertToFieldTextureArchive( model.TextureDictionary, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\mod\model\field_tex\textures\tex000_100_00_00.bin" );
            Resource.Save( model, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\mod\model\field_tex\f000_100_0.GFS" );
            */

            /*
            var model = ModelFactory.ConvertFromAssimpScene( @"D:\Modding\Persona 4 Dancing CPK RIP\Game mods\DanteOverYu\Dante.FBX",
                                                            MaterialPreset.CharacterClothP4D, false, false, 0x01105030 );

            Resource.Save( model, @"D:\Modding\Persona 4 Dancing CPK RIP\Game mods\DanteOverYu\mod\dance\player\pc001_01.GMD" );
            */

            /*
            var model = Resource.Load< Model >( @"D:\Modding\Persona 4 Dancing CPK RIP\data\dance\player\pc001_f1.GMD" );
            model.Scene.Flags |= ~SceneFlags.HasMorphs;
            model.Scene.Flags |= ~SceneFlags.HasSkinning;
            model.Scene.MatrixPalette = null;
            model.TextureDictionary.Clear();
            model.MaterialDictionary.Clear();

            foreach ( var sceneNode in model.Scene.Nodes )
                sceneNode.Attachments.Clear();
            Resource.Save( model, @"D:\Modding\Persona 4 Dancing CPK RIP\Game mods\DanteOverYu\mod\dance\player\pc001_f1.GMD" );
            */

            /*
            var model = ModelFactory.ConvertFromAssimpScene( @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\SA2Man.FBX", MaterialPreset.CharacterSkinP5 );
            var originalModel = Resource.Load< Model >( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_002_00.GMD" );
            foreach ( var entry in originalModel.MaterialDictionary )
                model.MaterialDictionary[ entry.Key ] = entry.Value;

            foreach ( var entry in originalModel.TextureDictionary )
                model.TextureDictionary[entry.Key] = entry.Value;

            Resource.Save( model, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\mod\model\character\0001\c0001_002_00.GMD" );
            */

            Process.Start( @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\make_cpk_rpcs3.bat" );

            /*
            var rmdPacFile = new PakToolArchiveFile( @"D:\Modding\Persona 3 & 4\Persona3\CVM_BTL\MODEL\PACK\BC001_WP0.PAC" );
            var rmdPacEntry = rmdPacFile.Entries.Single( x => x.Name.EndsWith( "rmd", System.StringComparison.InvariantCultureIgnoreCase ) );
            var rmdScene = new RmdScene( rmdPacEntry.Data );

            RMDToGMD( rmdScene );
            Process.Start( @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\make_cpk_rpcs3.bat" );
            */

            /*
            var rmdPacFile = new PakToolArchiveFile( @"D:\Modding\Persona 3 & 4\Persona3\CVM_DATA\FIELD\PACK\F004_002.FPC" );
            var rmdPacEntry = rmdPacFile.Entries.Single( x => x.Name.EndsWith( "rws", System.StringComparison.InvariantCultureIgnoreCase ) );
            var rmdScene = new RmdScene( rmdPacEntry.Data );

            RMDToGMDField( rmdScene );

            //BonePaletteTests();
            */

            //ModelViewerTest( Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" ) );

            //var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\field_tex\f051_010_0.GFS" );
            //ExportDAE( Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" ) );

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

        static Matrix4x4 InverseBindMatrixToWorldTransformMatrix( Matrix4x4 inverseBindMatrix )
        {
            Matrix4x4.Invert( inverseBindMatrix, out var bindMatrix );

            var zToYUpMatrix = new Matrix4x4( 1, 0, 0, 0,
                                              0, 0, -1, 0,
                                              0, 1, 0, 0,
                                              0, 0, 0, 1 );

            var bindMatrixYUp = bindMatrix * zToYUpMatrix;

            return bindMatrixYUp;
        }

        static Matrix4x4 WorldTransformMatrixToInverseBindMatrix( Matrix4x4 worldTransformMatrix )
        {
            var yToZUpMatrix = new Matrix4x4( 1, 0, 0, 0,
                                              0, 0, 1, 0,
                                              0, -1, 0, 0,
                                              0, 0, 0, 1 );

            var bindMatrixZUp = worldTransformMatrix * yToZUpMatrix;

            Matrix4x4.Invert( bindMatrixZUp, out var inverseBindMatrix );

            return inverseBindMatrix;
        }

        static void PrintMatrix( Matrix4x4 matrix )
        {
            for ( int i = 0; i < 4; i++ )
            {
                for ( int j = 0; j < 4; j++ )
                {
                    PrintFloat( matrix.GetElementAt( i, j ) );
                    Console.Write(" ");
                }
            }

            Console.Write("\n");
        }

        static void PrintFloat( float @float )
        {
            if ( @float >= -0f )
                Console.Write(" ");

            Console.Write( @float.ToString("N6") );
        }

        static void RMDToGMD( RmdScene rmdScene )
        {
            var rmdClump = rmdScene.Clumps[0];

            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_002_00.GMD" );

            //var model = new Model( 0x01105070 );

            model.TextureDictionary = new TextureDictionary( 0x01105070 );
            foreach ( var rwTexture in rmdScene.TextureDictionary.Textures )
            {
                var bitmap = rwTexture.GetBitmap();

                var bitmapStream = new MemoryStream();
                bitmap.Save( bitmapStream, ImageFormat.Bmp );

                var image = new ImageEngineImage( bitmapStream );
                var ddsData = image.Save( new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.DDS_DXT5 ), MipHandling.Default );

                var texture = new Texture( rwTexture.Name + ".dds", TextureFormat.DDS, ddsData );
                model.TextureDictionary.Add( texture );
            }

            model.MaterialDictionary = new MaterialDictionary( 0x01105070 );
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
                        material.DrawOrder = 0;
                        material.Field49 = 1;
                        material.Field4A = 0;
                        material.Field4B = 1;
                        material.Field4C = 0;
                        material.Field50 = 0;
                        material.Field5C = 0;
                        material.Field6C = 0xfffffff8;
                        material.Field70 = 0xfffffff8;
                        material.Field90 = 0;
                        material.Field92 = 4;
                        material.Field94 = 1;
                        material.Field96 = 0;
                        material.Field98 = 0xffffffff;
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

            model.Scene.MatrixPalette = null;

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

                    uint y = 0;
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
                    geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );

                    model.Scene.RootNode.Attachments.Add( new NodeGeometryAttachment( geometry ) );
                }
            }


            Resource.Save( model, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\mod\model\character\0001\c0001_002_00.GMD" );

        }

        static TextureDictionary ConvertTextureDictionary( RwTextureDictionaryNode rwTextureDictionary )
        {
            TextureDictionary textureDictionary = new TextureDictionary( 0x01105070 );

            foreach ( var rwTexture in rwTextureDictionary.Textures )
            {
                var texture = ConvertTexture( rwTexture );
                textureDictionary.Add( texture );
            }

            return textureDictionary;
        }

        static Texture ConvertTexture( RwTextureNativeNode texture )
        {
            var ddsData = RwTextureToDDS( texture );
            return new Texture( texture.Name + ".dds", TextureFormat.DDS, ddsData );
        }

        static byte[] RwTextureToDDS( RwTextureNativeNode texture )
        {
            var bitmap = texture.GetBitmap();

            var bitmapStream = new MemoryStream();
            bitmap.Save( bitmapStream, ImageFormat.Bmp );

            var image = new ImageEngineImage( bitmapStream );
            var ddsData = image.Save( new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.DDS_DXT1 ), MipHandling.KeepTopOnly );

            return ddsData;
        }

        static MaterialDictionary ConvertMaterials( IEnumerable<RwMaterialListNode> rwMaterials )
        {
            var materials = new MaterialDictionary( 0x01105070 );

            foreach ( var rwMatList in rwMaterials )
            {
                foreach ( var rwMat in rwMatList )
                {
                    string materialName = rwMat.TextureReferenceNode.ReferencedTextureName + "_mat";
                    if ( materials.ContainsMaterial( materialName ) )
                        continue;

                    Material material;

                    if ( rwMat.IsTextured )
                    {
                        material = new Material( materialName );
                        material.Ambient = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                        material.Diffuse = new Vector4( 0.3921569f, 0.3921569f, 0.3921569f, 1f );
                        material.DiffuseMap = new TextureMap( rwMat.TextureReferenceNode.ReferencedTextureName + ".dds" );

                        /*
                        material.DiffuseMap.Field44 = 1;
                        material.DiffuseMap.Field48 = 1;
                        material.DiffuseMap.Field49 = 1;
                        material.DiffuseMap.Field4A = 0;
                        material.DiffuseMap.Field4B = 0;
                        material.DiffuseMap.Field4C = 0.707106769f;
                        material.DiffuseMap.Field50 = -0.707106769f;
                        material.DiffuseMap.Field54 = 0;
                        material.DiffuseMap.Field58 = 0;
                        material.DiffuseMap.Field5C = 0.707106769f;
                        material.DiffuseMap.Field60 = 0.707106769f;
                        material.DiffuseMap.Field64 = 0;
                        material.DiffuseMap.Field68 = 0;
                        material.DiffuseMap.Field6C = 0;
                        material.DiffuseMap.Field70 = 0;
                        material.DiffuseMap.Field74 = 0;
                        material.DiffuseMap.Field78 = 0;
                        material.DiffuseMap.Field7C = -0.207106769f;
                        material.DiffuseMap.Field80 = 0.5f;
                        material.DiffuseMap.Field84 = 0;
                        material.DiffuseMap.Field88 = 0;
                        */

                        material.Emissive = new Vector4( 0, 0, 0, 0 );
                        material.Field40 = 1;
                        material.Field44 = 0.1f;
                        material.DrawOrder = 0;
                        material.Field49 = 1;
                        material.Field4A = 0;
                        material.Field4B = 1;
                        material.Field4C = 0;
                        material.Field50 = 0;
                        material.Field5C = 0;
                        material.Field6C = 0xfffffff8;
                        material.Field70 = 0xfffffff8;
                        material.Field90 = 0;
                        material.Field92 = 4;
                        material.Field94 = 1;
                        material.Field96 = 0;
                        material.Field98 = 0xffffffff;
                        material.Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight | MaterialFlags.EnableLight2 | MaterialFlags.ReceiveShadow | MaterialFlags.HasDiffuseMap;
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

                    materials.Add( material );
                }
            }

            return materials;
        }

        static Node RwFrameToNode( RwFrame frame, Dictionary<string, Node> nodeLookup )
        {
            Matrix4x4.Decompose( frame.Transform, out var scale, out var rotation, out var translation );

            Node node = new Node(
                GetNameForRwFrame( frame ),
                translation,
                rotation,
                scale );

            nodeLookup.Add( node.Name, node );

            if ( frame.Parent != null )
                node.Parent = nodeLookup[GetNameForRwFrame( frame )];

            foreach ( var child in frame.Children )
            {
                node.Children.Add( RwFrameToNode( child, nodeLookup) );
            }

            return node;
        }

        static string GetNameForRwFrame( RwFrame frame )
        {
            return frame.HasHAnimExtension ? frame.HAnimFrameExtensionNode.NameId.ToString() : "RootNode";
        }

        static List<Geometry> RwAtomicToGeometryList( RwAtomicNode rwAtomic, RwGeometryListNode geometryList, RwFrameListNode frameList )
        {
            var rwGeometry = geometryList[rwAtomic.GeometryIndex];
            var rwFrame = frameList[rwAtomic.FrameIndex];
            var rwMeshList = ( RwMeshListNode )rwGeometry.ExtensionNodes.Find( x => x.Id == RwNodeId.RwMeshListNode );
            var geometries = new List<Geometry>();

            foreach ( var rwMesh in rwMeshList.MaterialMeshes )
            {
                var rwMaterial = rwGeometry.Materials[rwMesh.MaterialIndex];
                var rwIndices = MeshUtilities.ToTriangleList( rwMesh.Indices, false );

                var geometry = new Geometry();
                geometry.TriangleIndexType = TriangleIndexType.UInt16;
                geometry.Triangles = new Triangle[rwIndices.Length / 3];

                uint y = 0;
                for ( int i = 0; i < geometry.Triangles.Length; i++ )
                {
                    geometry.Triangles[i] = new Triangle( y++, y++, y++ );
                }

                geometry.MaterialName = rwMaterial.TextureReferenceNode.ReferencedTextureName + "_mat";
                geometry.Vertices = new Vector3[rwIndices.Length];
                geometry.Normals = new Vector3[rwIndices.Length];
                geometry.TexCoordsChannel0 = new Vector2[rwIndices.Length];

                for ( int i = 0; i < rwIndices.Length; i++ )
                {
                    geometry.Vertices[i] = Vector3.Transform( rwGeometry.Vertices[rwIndices[i]], rwFrame.WorldTransform );
                    geometry.Normals[i] = Vector3.TransformNormal( rwGeometry.Normals[rwIndices[i]], rwFrame.WorldTransform );
                    geometry.TexCoordsChannel0[i] = rwGeometry.TextureCoordinateChannels[0][rwIndices[i]];
                }

                geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
                geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );

                geometries.Add( geometry );
            }

            return geometries;
        }

        static void RMDToGMDField( RmdScene rmdScene )
        {
            var rmdCollisionGeometry = rmdScene.Clumps[0];
            var rmdLevelGeometry = rmdScene.Clumps[1];

            var model = new Model( 0x01105070 );
            model.TextureDictionary = ConvertTextureDictionary( rmdScene.TextureDictionary );
            
            model.Scene = new Scene( 0x01105070 );

            var nodeLookup = new Dictionary<string, Node>();
            model.Scene.RootNode = RwFrameToNode( rmdLevelGeometry.FrameList[0], nodeLookup );

            //var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\field_tex\f000_100_0.GFS" );
            model.MaterialDictionary = ConvertMaterials( rmdLevelGeometry.GeometryList.Select( x => x.Materials ) );
            var allVertices = new List<Vector3>();

            foreach ( var rwAtomic in rmdLevelGeometry.Atomics )
            {
                //model.Scene.RootNode.Attachments.AddRange( RwAtomicToGeometryList( rwAtomic, rmdLevelGeometry.GeometryList, rmdLevelGeometry.FrameList ) );
            }

            model.Scene.BoundingBox = BoundingBox.Calculate( allVertices );
            model.Scene.BoundingSphere = BoundingSphere.Calculate( model.Scene.BoundingBox.Value, allVertices );

            // generate field texture pack

            var archiveBuilder = new ArchiveBuilder();
            var bgTexArcDataStream = new MemoryStream();

            using ( var streamWriter = new StreamWriter( bgTexArcDataStream, System.Text.Encoding.Default, 4096, true ) )
            {
                streamWriter.WriteLine( "1," );
                streamWriter.WriteLine( $"{rmdScene.TextureDictionary.Textures.Count}," );           
            }

            archiveBuilder.AddFile( "bgTexArcData00.txt", bgTexArcDataStream );
            //var archive = new Archive( @"D:\Modding\Persona 5 EU\Main game\Extracted\ps3\model\field_tex\textures\tex000_100_00_00.bin" );
            //var dummyFieldTexture = archive.OpenFile( "dummy_field.dds" );

            foreach ( var rwTexture in rmdScene.TextureDictionary.Textures )
            {
                var ddsData = RwTextureToDDS( rwTexture );
                var ddsRawData = new byte[ddsData.Length - 0x80];
                Array.Copy( ddsData, 0x80, ddsRawData, 0, ddsRawData.Length );

                var texture = new FieldTexture( TexturePixelFormat.DXT1, 1, ( short )rwTexture.Width, ( short )rwTexture.Height, ddsRawData );
                var textureStream = new MemoryStream();
                texture.Save( textureStream );

                archiveBuilder.AddFile( rwTexture.Name + ".dds", textureStream );
            }

            archiveBuilder.BuildFile( @"D:\Modding\Persona 5 EU\Game mods\TestLevel\mod\model\field_tex\textures\tex000_100_00_00.bin" );

            Resource.Save( model, @"D:\Modding\Persona 5 EU\Game mods\TestLevel\mod\model\field_tex\f000_100_0.GFS" );
        }

        static void ExportDAE( Model model )
        {
            AssimpExporter.ExportFile( model, "model.dae" );
        }
    }
}
