using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using AtlusGfdLib;
using AtlusGfdLib.Assimp;
using AtlusGfdLib.IO;
using AtlusLibSharp.FileSystems.PAKToolArchive;
using AtlusLibSharp.Graphics.RenderWare;
using AtlusLibSharp.Utilities;
using CSharpImageLibrary;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Ai = Assimp;

namespace AtlusGfdLibTesting
{
    // Notes:
    // Geometry complexity limit is around 3k tris/1.5k verts, if exceeded the game crashes

    class Program
    {
        static void Main( string[] args )
        {
            /*
            var model = Resource.Load< Model >( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_002_00.GMD" );
            ReplaceModel( model, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\Demifiend.FBX" );
            */

            var model = ModelFactory.CreateFromAssimpScene(
                @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\Demifiend.FBX",
                0x01105070,
                MaterialFactory.CreatePresetMaterial( MaterialPreset.CharacterSkin ) );

            AssimpExporter.ExportFile( model, "test.dae" );

            Resource.Save( model, @"D:\Modding\Persona 5 EU\Game mods\TeapotKun\mod\model\character\0001\c0001_002_00.GMD" );

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

        private static void ReplaceModel( Model referenceModel, string path )
        {
            // Set up Assimp context
            var aiContext = new Ai.AssimpContext();
            aiContext.SetConfig( new Ai.Configs.MeshVertexLimitConfig( 1500 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.MeshTriangleLimitConfig( 3000 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.VertexCacheSizeConfig( 63 ) ); // PS3/RSX vertex cache size

            // Apply ALL the optimizations
            var aiScene = aiContext.ImportFile( path,
                                                Ai.PostProcessSteps.ImproveCacheLocality | Ai.PostProcessSteps.FindInvalidData | Ai.PostProcessSteps.FlipUVs | Ai.PostProcessSteps.JoinIdenticalVertices |
                                                Ai.PostProcessSteps.LimitBoneWeights | Ai.PostProcessSteps.Triangulate | Ai.PostProcessSteps.GenerateSmoothNormals | Ai.PostProcessSteps.OptimizeMeshes );

            // Build textures & Materials
            var baseMaterial = MaterialFactory.CreateCharacterSkinMaterial( "", "", "" );

            referenceModel.TextureDictionary.Clear();                  
            referenceModel.MaterialDictionary.Clear();

            foreach ( var aiSceneMaterial in aiScene.Materials )
            {
                var material = baseMaterial.ShallowCopy();
                material.Name = aiSceneMaterial.Name;
                //var material = new Material( aiSceneMaterial.Name );

                if ( aiSceneMaterial.HasTextureDiffuse )
                {
                    var relativeFilePath = aiSceneMaterial.TextureDiffuse.FilePath;
                    var fullFilePath = Path.GetFullPath( Path.Combine( Path.GetDirectoryName( path ), relativeFilePath ) );
                    Texture texture;

                    if ( relativeFilePath.EndsWith( ".dds" ) )
                    {
                        var textureName = Path.GetFileName( relativeFilePath );
                        texture = new Texture( textureName, TextureFormat.DDS, File.ReadAllBytes( fullFilePath ) );
                    }
                    else
                    {                    
                        var bitmap = new Bitmap( fullFilePath );
                        var textureName = Path.GetFileNameWithoutExtension( relativeFilePath ) + ".dds";
                        texture = TextureEncoder.Encode( textureName, TextureFormat.DDS, bitmap );
                    }
                    
                    referenceModel.TextureDictionary.Add( texture );

                    /*
                    material.Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2 | MaterialFlags.Flag20 | MaterialFlags.Flag40 | MaterialFlags.EnableLight | MaterialFlags.EnableLight2 | MaterialFlags.CastShadow;
                    material.Ambient = new Vector4( 1f, 1f, 1f, 1f );
                    material.Diffuse = new Vector4( 1f, 1f, 1f, 1f );
                    material.Field40 = 1;
                    material.Field44 = 0.1f;
                    material.Field48 = 0;
                    material.Field49 = 1;
                    material.Field4A = 0;
                    material.Field4B = 1;
                    material.Field4C = 0;
                    material.Field50 = 0;
                    material.Field5C = 0;
                    material.Field6C = 0xfffffff8; // any changes to these 2 values will either make the texture not appear
                    material.Field70 = 0xfffffff8; // or cause a crash
                    material.Field90 = 0;
                    material.Field92 = 4; // doesn't seem to change anything
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
                    */
                    material.DiffuseMap = new TextureMap( texture.Name );
                    material.ShadowMap = new TextureMap( texture.Name );
                    //material.ShadowMap = new TextureMap( Path.GetFileNameWithoutExtension( texture.Name ) + "_shadow" + ".dds" );
                }

                referenceModel.MaterialDictionary.Add( material );
            }

            var nodeLookup = new Dictionary< string, (Ai.Node aiNode, Node node, int index) >();
            int nextNodeIndex = 0;
            referenceModel.Scene.RootNode = ProcessAssimpNodeRecursively( aiScene.RootNode, nodeLookup, ref nextNodeIndex );

            var nodeToBoneIndices = new Dictionary<int, List<int>>();
            int nextBoneIndex = 0;
            var boneInverseBindMatrices = new List<Matrix4x4>();
            ProcessAssimpNodeMeshesRecursively( aiScene.RootNode, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices );

            // Build matrix palette
            referenceModel.Scene.MatrixPalette = new MatrixPalette( boneInverseBindMatrices.Count );

            for ( int i = 0; i < referenceModel.Scene.MatrixPalette.BoneToNodeIndices.Length; i++ )
            {
                // Reverse dictionary search
                referenceModel.Scene.MatrixPalette.BoneToNodeIndices[ i ] = (ushort)nodeToBoneIndices
                    .Where( x => x.Value.Contains( i ) )
                    .Select( x => x.Key )
                    .Single();
            }

            // Inverse bind matrices are already ordered correctly
            referenceModel.Scene.MatrixPalette.InverseBindMatrices = boneInverseBindMatrices.ToArray();
        }

        private static Node ProcessAssimpNodeRecursively( Ai.Node aiNode, Dictionary<string, (Ai.Node aiNode, Node node, int index)> nodeLookup, ref int nextIndex )
        {
            aiNode.Transform.Decompose( out var scale, out var rotation, out var translation );

            // Create node
            var node = new Node( aiNode.Name,
                                 new Vector3( translation.X, translation.Y, translation.Z ),
                                 new Quaternion( rotation.X, rotation.Y, rotation.Z, rotation.W ),
                                 new Vector3( scale.X, scale.Y, scale.Z ) );

            // Convert properties
            ConvertAssimpMetadataToProperties( aiNode.Metadata, node );

            // Add to lookup
            nodeLookup.Add( node.Name, (aiNode, node, nextIndex++) );

            // Process children
            foreach ( var aiNodeChild in aiNode.Children )
            {
                var childNode = ProcessAssimpNodeRecursively( aiNodeChild, nodeLookup, ref nextIndex );
                node.AddChildNode( childNode );
            }

            return node;
        }

        private static void ConvertAssimpMetadataToProperties( Ai.Metadata metadata, Node node )
        {
            foreach ( var metadataEntry in metadata )
            {
                NodeProperty property = null;

                // Skip some garbage entries
                if ( metadataEntry.Key == "IsNull" ||
                     metadataEntry.Key == "InheritType" ||
                     metadataEntry.Key == "DefaultAttributeIndex" ||
                     metadataEntry.Key == "UDP3DSMAX" || // dupe of UserProperties
                     metadataEntry.Key == "MaxHandle" )
                {
                    continue;
                }

                if ( metadataEntry.Key == "UserProperties" )
                {
                    var properties = ( ( string )metadataEntry.Value.Data )
                        .Split( new[] { "&cr;&lf;", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries );

                    if ( properties.Length == 0 )
                        continue;

                    foreach ( var propertyString in properties )
                    {
                        // Parse property string
                        KeyValuePair<string, string> kvp;
                        if ( propertyString.Contains( '=' ) )
                        {
                            var split = propertyString.Split( '=' );
                            kvp = new KeyValuePair<string, string>( split[0].TrimEnd(), split[1].TrimStart() );
                        }
                        else
                        {
                            var split = propertyString.Split( ' ' );
                            kvp = new KeyValuePair<string, string>( split[0], split.Length > 1 ? split[1] : null );
                        }

                        // Parse value
                        if ( kvp.Value == null )
                        {
                            // Assume flag bool
                            property = new NodeBoolProperty( kvp.Key, true );
                        }
                        else if ( kvp.Value.StartsWith( "[" ) && kvp.Value.EndsWith( "]" ) )
                        {
                            // Array/Vector
                            var arrayContents = kvp.Value.Substring( 1, kvp.Value.Length - 2 );
                            var arrayValues = arrayContents.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

                            var arrayFloatValues = new List<float>();
                            foreach ( var arrayValue in arrayValues )
                            {
                                if ( !float.TryParse( arrayValue, out var arrayFloatValue ) )
                                {
                                    throw new Exception( $"Failed to parse array user property value as float: {arrayValue}" );
                                }

                                arrayFloatValues.Add( arrayFloatValue );
                            }

                            if ( arrayFloatValues.Count == 3 )
                            {
                                property = new NodeVector3Property( kvp.Key, new Vector3( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2] ) );
                            }
                            else if ( arrayFloatValues.Count == 4 )
                            {
                                property = new NodeVector4Property( kvp.Key, new Vector4( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2], arrayFloatValues[3] ) );
                            }
                            else
                            {
                                var arrayByteValues = arrayFloatValues.Cast<byte>();
                                property = new NodeByteArrayProperty( kvp.Key, arrayByteValues.ToArray() );
                            }
                        }
                        else if ( int.TryParse( kvp.Value, out int intValue ) )
                        {
                            property = new NodeIntProperty( kvp.Key, intValue );
                        }
                        else if ( float.TryParse( kvp.Value, out float floatValue ) )
                        {
                            property = new NodeFloatProperty( kvp.Key, floatValue );
                        }
                        else if ( bool.TryParse( kvp.Value, out bool boolValue ) )
                        {
                            property = new NodeBoolProperty( kvp.Key, boolValue );
                        }
                        else
                        {
                            property = new NodeStringProperty( kvp.Key, kvp.Value );
                        }
                    }
                }
                else
                {
                    switch ( metadataEntry.Value.DataType )
                    {
                        case Ai.MetaDataType.Bool:
                            property = new NodeBoolProperty( metadataEntry.Key, metadataEntry.Value.DataAs<bool>().Value );
                            break;
                        case Ai.MetaDataType.Int:
                            property = new NodeIntProperty( metadataEntry.Key, metadataEntry.Value.DataAs<int>().Value );
                            break;
                        case Ai.MetaDataType.UInt64:
                            property = new NodeByteArrayProperty( metadataEntry.Key, BitConverter.GetBytes( metadataEntry.Value.DataAs<ulong>().Value ) );
                            break;
                        case Ai.MetaDataType.Float:
                            property = new NodeFloatProperty( metadataEntry.Key, metadataEntry.Value.DataAs<float>().Value );
                            break;
                        case Ai.MetaDataType.String:
                            property = new NodeStringProperty( metadataEntry.Key, ( string )metadataEntry.Value.Data );
                            break;
                        case Ai.MetaDataType.Vector3D:
                            var data = metadataEntry.Value.DataAs<Ai.Vector3D>().Value;
                            property = new NodeVector3Property( metadataEntry.Key, new Vector3( data.X, data.Y, data.Z ) );
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if ( property == null )
                {
                    throw new Exception( "Property shouldn't be null" );
                }

                node.Properties.Add( property.Name, property );
            }
        }

        private static void ProcessAssimpNodeMeshesRecursively( Ai.Node aiNode, Ai.Scene aiScene, Dictionary<string, (Ai.Node aiNode, Node node, int index)> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices )
        {
            if ( aiNode.HasMeshes )
            {
                var nodeLookupData = nodeLookup[aiNode.Name];
                var node = nodeLookupData.node;

                Matrix4x4.Invert( node.WorldTransform, out var invertedNodeWorldTransform );

                foreach ( var aiMeshIndex in aiNode.MeshIndices )
                {
                    var aiMesh = aiScene.Meshes[aiMeshIndex];
                    var aiMaterial = aiScene.Materials[ aiMesh.MaterialIndex ];
                    var geometry = ConvertAssimpMeshToGeometry( aiMesh, aiMaterial, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, ref invertedNodeWorldTransform );
                    node.Attachments.Add( new NodeGeometryAttachment( geometry ) );
                }
            }

            foreach ( var aiNodeChild in aiNode.Children )
            {
                ProcessAssimpNodeMeshesRecursively( aiNodeChild, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices );
            }
        }

        private static Geometry ConvertAssimpMeshToGeometry( Ai.Mesh aiMesh, Ai.Material material, Dictionary<string, (Ai.Node aiNode, Node node, int index)> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, ref Matrix4x4 invertedNodeWorldTransform )
        {
            var geometry = new Geometry();

            if ( aiMesh.HasVertices )
            {
                geometry.Vertices = aiMesh.Vertices
                                          .Select( x => new Vector3( x.X, x.Y, x.Z ) )
                                          .ToArray();
            }

            if ( aiMesh.HasNormals )
            {
                geometry.Normals = aiMesh.Normals
                                         .Select( x => new Vector3( x.X, x.Y, x.Z ) )
                                         .ToArray();
            }

            if ( aiMesh.HasTextureCoords( 0 ) )
            {
                geometry.TexCoordsChannel0 = aiMesh.TextureCoordinateChannels[0]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasFaces )
            {
                geometry.TriangleIndexType = TriangleIndexType.UInt16;
                geometry.Triangles = aiMesh.Faces
                                           .Select( x => new Triangle( ( uint )x.Indices[0], ( uint )x.Indices[1], ( uint )x.Indices[2] ) )
                                           .ToArray();
            }

            if ( aiMesh.HasBones )
            {
                geometry.VertexWeights = new VertexWeight[geometry.VertexCount];
                for ( int i = 0; i < geometry.VertexWeights.Length; i++ )
                {
                    geometry.VertexWeights[i].Indices = new byte[4];
                    geometry.VertexWeights[i].Weights = new float[4];
                }

                var vertexWeightCounts = new int[geometry.VertexCount];

                for ( var i = 0; i < aiMesh.Bones.Count; i++ )
                {
                    var aiMeshBone = aiMesh.Bones[i];

                    // Find node index for the bone
                    var boneLookupData = nodeLookup[aiMeshBone.Name];
                    int nodeIndex = boneLookupData.index;

                    // Calculate inverse bind matrix
                    var boneNode = boneLookupData.node;
                    Matrix4x4.Invert( boneNode.WorldTransform * invertedNodeWorldTransform, out var inverseBindMatrix );

                    // Get bone index
                    int boneIndex;
                    if ( !nodeToBoneIndices.TryGetValue( nodeIndex, out var boneIndices ) )
                    {
                        // No entry for the node was found, so we add a new one
                        boneIndex = nextBoneIndex++;
                        nodeToBoneIndices.Add( nodeIndex, new List<int>() { boneIndex } );
                        boneInverseBindMatrices.Add( inverseBindMatrix );
                    }
                    else
                    {
                        // Entry for the node was found
                        // Try to find the bone index based on whether the inverse bind matrix matches
                        boneIndex = -1;
                        foreach ( int index in boneIndices )
                        {
                            if ( boneInverseBindMatrices[index].Equals( inverseBindMatrix ) )
                                boneIndex = index;
                        }

                        if ( boneIndex == -1 )
                        {
                            // None matching inverse bind matrix was found, so we add a new entry
                            boneIndex = nextBoneIndex++;
                            nodeToBoneIndices[nodeIndex].Add( boneIndex );
                            boneInverseBindMatrices.Add( inverseBindMatrix );
                        }
                    }

                    foreach ( var aiVertexWeight in aiMeshBone.VertexWeights )
                    {
                        int vertexWeightCount = vertexWeightCounts[aiVertexWeight.VertexID]++;

                        geometry.VertexWeights[aiVertexWeight.VertexID].Indices[vertexWeightCount] = ( byte )boneIndex;
                        geometry.VertexWeights[aiVertexWeight.VertexID].Weights[vertexWeightCount] = aiVertexWeight.Weight;
                    }
                }
            }

            geometry.MaterialName = material.Name;
            geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
            geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );

            return geometry;
        }

        static void BonePaletteTests()
        {
            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_051_00.GMD" );
            //var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\9000\c9000_000_00.GMD" );
            var scene = model.Scene;
            var matrixPalette = scene.MatrixPalette;
            var inverseBindMatrices = matrixPalette.InverseBindMatrices;
            var boneToNodeIndices = matrixPalette.BoneToNodeIndices;

            Console.WriteLine( "BonePaletteTests" );
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine( $"[bonePaletteCount: {matrixPalette.MatrixCount}]" );
            Console.WriteLine();

            // find and print duplicate bone to node indices
            {
                var duplicateBoneToNodeIndices = boneToNodeIndices
                    .GroupBy( x => x ) // group same entries
                    .Where( x => x.Count() > 1 ) // filter out non-duplicates
                    .ToList();

                foreach ( var duplicateGroup in duplicateBoneToNodeIndices )
                {
                    var duplicateList = duplicateGroup.ToList();
                    var duplicateNodeIndex = duplicateList[0];

                    // print number of duplicates and which node has duplicates
                    var node = scene.Nodes[duplicateNodeIndex];
                    Console.WriteLine( $"[duplicates: {duplicateList.Count:D2}] [nodeIndex: {duplicateNodeIndex:D3}] [name: {node.Name.PadRight(20)}] [matrix: {node.WorldTransform}]");

                    // print which bones use the duplicate node index
                    var boneIndex = -1;
                    var accumulatedMatrix = Matrix4x4.Identity;
                    for ( int i = 0; i < duplicateList.Count; i++ )
                    {
                        boneIndex = Array.IndexOf( boneToNodeIndices, duplicateNodeIndex, boneIndex + 1 );

                        // also print the matrices used by each of the bones which use the duplicate node index
                        // and the difference between them
                        var inverseBindMatrix = inverseBindMatrices[boneIndex];

                        //accumulatedMatrix += InverseBindMatrixToWorldTransformMatrix( inverseBindMatrix );

                        Console.WriteLine( $"[duplicate: {i:D2}]  [boneIndex: {boneIndex:D3}]                              [matrix: {InverseBindMatrixToWorldTransformMatrix(inverseBindMatrix)}]" );
                    }

                    // it appears that the sum of these duplicated matrices is equal to the inverse of the world transform of the node
                    Console.WriteLine( $"                                                                        {accumulatedMatrix}" );

                    Console.WriteLine();
                }
            }

            {
                for ( int i = 0; i < boneToNodeIndices.Length; i++ )
                {
                    var inverseBindMatrix = inverseBindMatrices[i];
                    var node = scene.Nodes[boneToNodeIndices[i]];

                    Console.WriteLine( node.Name );

                    PrintMatrix( InverseBindMatrixToWorldTransformMatrix( inverseBindMatrix ) );
                    PrintMatrix( node.WorldTransform );
                    Console.WriteLine();
                }
            }

            {
                List<int> skinnedNodeIndices = new List<int>();
                List<Matrix4x4> skinnedNodeWorldTransformMatrices = new List<Matrix4x4>();

                Console.WriteLine( "Nodes in matrix palette:" );
                foreach ( var item in boneToNodeIndices )
                {
                    Console.WriteLine( scene.Nodes[item].Name );
                }
                Console.WriteLine();

                // collect skinned nodes
                foreach ( var node in scene.Nodes )
                {
                    if ( !node.HasAttachments )
                        continue;

                    foreach ( var attachment in node.Attachments )
                    {
                        if ( attachment.Type != NodeAttachmentType.Geometry )
                            continue;

                        var geometry = attachment.GetValue<Geometry>();

                        if ( !geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                        {
                            // these nodes dont actually show up in the matrix palette?
                            //Console.WriteLine( node.Name );
                            //skinnedNodeIndices.Add( 0 );
                            //skinnedNodeWorldTransformMatrices.Add( Matrix4x4.Identity );
                            continue;
                        }

                        foreach ( var vertexWeight in geometry.VertexWeights )
                        {
                            for ( int i = 0; i < vertexWeight.Indices.Length; i++ )
                            {
                                if ( vertexWeight.Weights[i] == 0.0f )
                                    continue;

                                int nodeIndex = matrixPalette.BoneToNodeIndices[vertexWeight.Indices[i]];
                                if ( !skinnedNodeIndices.Contains( nodeIndex ) )
                                {
                                    skinnedNodeIndices.Add( nodeIndex );
                                    skinnedNodeWorldTransformMatrices.Add( scene.Nodes[nodeIndex].WorldTransform );
                                }
                            }
                        }
                    }
                }

                /*
                Console.WriteLine( "Unique skinned nodes" );
                foreach ( var item in skinnedNodeIndices )
                {
                    Console.WriteLine( scene.Nodes[item].Name );
                }
                Console.WriteLine();
                */

                Console.WriteLine( "Unique skinned nodes (reverse)" );
                for ( int i = 0; i < skinnedNodeIndices.Count; i++ )
                {
                    Console.WriteLine( scene.Nodes[skinnedNodeIndices[skinnedNodeIndices.Count - ( i + 1 )] ].Name );
                }
                Console.WriteLine();

                // create new matrix palette
                var newMatrixPalette = MatrixPalette.Create( skinnedNodeIndices, skinnedNodeWorldTransformMatrices, out var nodeToBoneMap );

                // remap bone indices
                foreach ( var geometry in scene.EnumerateGeometries() )
                {
                    if ( !geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                        continue;

                    foreach ( var vertexWeight in geometry.VertexWeights )
                    {
                        for ( int i = 0; i < vertexWeight.Indices.Length; i++ )
                        {
                            if ( vertexWeight.Weights[i] == 0.0f )
                                continue;

                            int nodeIndex = matrixPalette.BoneToNodeIndices[vertexWeight.Indices[i]];
                            vertexWeight.Indices[i] = (byte)nodeToBoneMap[nodeIndex];
                        }
                    }
                }
            }
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
                        material.Field48 = 0;
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
                        material.Field48 = 0;
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

                var texture = new FieldTexture( false, 1, ( short )rwTexture.Width, ( short )rwTexture.Height, ddsRawData );
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
