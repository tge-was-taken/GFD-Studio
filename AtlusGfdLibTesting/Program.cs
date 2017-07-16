using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AtlusGfdLib;
using AtlusGfdLib.IO;
using AtlusLibSharp.FileSystems.PAKToolArchive;
using AtlusLibSharp.Graphics.RenderWare;
using AtlusLibSharp.Utilities;
using CSharpImageLibrary;
using CSharpImageLibrary.DDS;

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
            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_158_00.GMD" );
            Resource.Save( model, @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_158_00_new.GMD" );


            //ExportDAE( Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\field_tex\f013_014_0.GFS" ) );

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

        static void RMDToGMD( RmdScene rmdScene )
        {
            var rmdClump = rmdScene.Clumps[0];

            var model = Resource.Load<Model>( @"D:\Modding\Persona 5 EU\Main game\Extracted\data\model\character\0001\c0001_158_00.GMD" );

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
                        material.Properties = null;
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
}
