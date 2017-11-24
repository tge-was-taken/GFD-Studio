using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AtlusGfdLib.Assimp;
using Ai = Assimp;

namespace AtlusGfdLib
{
    public static class SceneConverter
    {
        private static readonly Matrix4x4 YToZUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );

        private static readonly Matrix4x4 ZToYUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1 );

        public static Scene ConvertFromAssimpScene( string filePath, SceneConverterOptions options )
        {
            var aiScene = AssimpImporter.ImportFile( filePath );
            return ConvertFromAssimpScene( aiScene, options );
        }

        public static Scene ConvertFromAssimpScene( Ai.Scene aiScene, SceneConverterOptions options )
        {
            var scene = new Scene( options.Version );

            // Convert assimp nodes to our nodes 
            var nodeLookup = new Dictionary<string, NodeInfo>();
            int nextNodeIndex = 0;
            scene.RootNode = ConvertAssimpNodeRecursively( aiScene.RootNode, nodeLookup, ref nextNodeIndex );

            // Process the meshes attached to the assimp nodes
            var nodeToBoneIndices = new Dictionary<int, List<int>>();
            int nextBoneIndex = 0;
            var boneInverseBindMatrices = new List<Matrix4x4>();
            var transformedVertices = new List< Vector3 >();
            ProcessAssimpNodeMeshesRecursively( aiScene.RootNode, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, transformedVertices, options );

            // Don't build a matrix palette if there are no skinned meshes
            if ( boneInverseBindMatrices.Count > 0 )
            {
                // Build matrix palette for skinning
                scene.MatrixPalette = BuildMatrixPalette( boneInverseBindMatrices, nodeToBoneIndices );
            }

            // Build bounding box & sphere
            scene.BoundingBox = BoundingBox.Calculate( transformedVertices );
            scene.BoundingSphere = BoundingSphere.Calculate( scene.BoundingBox.Value, transformedVertices );

            return scene;
        }

        private static string UnescapeName( string name )
        {
            return name.Replace( "___", " " );
        }

        private static Node ConvertAssimpNodeRecursively( Ai.Node aiNode, Dictionary<string, NodeInfo> nodeLookup, ref int nextIndex )
        {
            aiNode.Transform.Decompose( out var scale, out var rotation, out var translation );

            // Create node
            var node = new Node( UnescapeName( aiNode.Name ),
                                 new Vector3( translation.X, translation.Y, translation.Z ),
                                 new Quaternion( rotation.X, rotation.Y, rotation.Z, rotation.W ),
                                 new Vector3( scale.X, scale.Y, scale.Z ) );

            // Convert properties
            ConvertAssimpMetadataToProperties( aiNode.Metadata, node );

            // Add to lookup
            nodeLookup.Add( node.Name, new NodeInfo( aiNode, node, nextIndex++ ) );

            // Process children
            foreach ( var aiNodeChild in aiNode.Children )
            {
                if ( aiNodeChild.Name == "RootNode" )
                {
                    // For compatibility with old exports 
                    foreach ( var aiFakeRootNodeChild in aiNodeChild.Children )
                    {
                        var childNode = ConvertAssimpNodeRecursively( aiFakeRootNodeChild, nodeLookup, ref nextIndex );
                        node.AddChildNode( childNode );
                    }
                }
                else
                {
                    var childNode = ConvertAssimpNodeRecursively( aiNodeChild, nodeLookup, ref nextIndex );
                    node.AddChildNode( childNode );
                }
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
                     metadataEntry.Key == "UserProperties" || // dupe of UDP3DSMAX
                     metadataEntry.Key == "MaxHandle" )
                {
                    continue;
                }

                if ( metadataEntry.Key == "UDP3DSMAX" )
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

        private static void ProcessAssimpNodeMeshesRecursively( Ai.Node aiNode, Ai.Scene aiScene, Dictionary<string, NodeInfo> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, List<Vector3> transformedVertices, SceneConverterOptions options )
        {
            if ( aiNode.HasMeshes )
            {
                var nodeLookupData = nodeLookup[ UnescapeName( aiNode.Name ) ];
                var node = nodeLookupData.Node;
                var nodeWorldTransform = node.WorldTransform;
                Matrix4x4.Invert( nodeWorldTransform, out var nodeInverseWorldTransform );

                foreach ( var aiMeshIndex in aiNode.MeshIndices )
                {
                    var aiMesh = aiScene.Meshes[aiMeshIndex];
                    var aiMaterial = aiScene.Materials[aiMesh.MaterialIndex];
                    var geometry = ConvertAssimpMeshToGeometry( aiMesh, aiMaterial, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, ref nodeWorldTransform, ref nodeInverseWorldTransform, transformedVertices, options );
                    node.Attachments.Add( new NodeGeometryAttachment( geometry ) );
                }
            }

            foreach ( var aiNodeChild in aiNode.Children )
            {
                ProcessAssimpNodeMeshesRecursively( aiNodeChild, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, transformedVertices, options );
            }
        }

        private static Geometry ConvertAssimpMeshToGeometry( Ai.Mesh aiMesh, Ai.Material material, Dictionary<string, NodeInfo> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, ref Matrix4x4 nodeWorldTransform, ref Matrix4x4 nodeInverseWorldTransform, List<Vector3> transformedVertices, SceneConverterOptions options )
        {
            if ( !aiMesh.HasVertices )
                throw new Exception( "Assimp mesh has no vertices" );

            var geometry = new Geometry();
            var geometryTransformedVertices = new Vector3[ aiMesh.VertexCount ];

            geometry.Vertices = aiMesh.Vertices
                                      .Select( x => new Vector3( x.X, x.Y, x.Z ) )
                                      .ToArray();

            for ( int i = 0; i < geometry.Vertices.Length; i++ )
                geometryTransformedVertices[ i ] = Vector3.Transform( geometry.Vertices[ i ], nodeWorldTransform );

            transformedVertices.AddRange( geometryTransformedVertices );

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

            if ( aiMesh.HasTextureCoords( 1 ) )
            {
                geometry.TexCoordsChannel1 = aiMesh.TextureCoordinateChannels[1]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasTextureCoords( 2 ) )
            {
                geometry.TexCoordsChannel2 = aiMesh.TextureCoordinateChannels[2]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasVertexColors( 0 ) )
            {
                geometry.ColorChannel0 = aiMesh.VertexColorChannels[ 0 ]
                                               .Select( x => ( uint ) ( ( byte ) ( x.B * 255f ) | ( byte ) ( x.G * 255f ) << 8 | ( byte ) ( x.R * 255f ) << 16 | ( byte ) ( x.A * 255f ) << 24 ) )
                                               .ToArray();
            }
            else if ( options.GenerateVertexColors )
            {
                geometry.ColorChannel0 = new uint[geometry.VertexCount];
                for ( int i = 0; i < geometry.ColorChannel0.Length; i++ )
                    geometry.ColorChannel0[i] = 0xFFFFFFFF;
            }

            if ( aiMesh.HasVertexColors( 1 ) )
            {
                geometry.ColorChannel1 = aiMesh.VertexColorChannels[1]
                                               .Select( x => ( uint )( ( byte )( x.B * 255f ) | ( byte )( x.G * 255f ) << 8 | ( byte )( x.R * 255f ) << 16 | ( byte )( x.A * 255f ) << 24 ) )
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
                    var boneLookupData = nodeLookup[ UnescapeName( aiMeshBone.Name ) ];
                    int nodeIndex = boneLookupData.Index;

                    // Calculate inverse bind matrix
                    var boneNode = boneLookupData.Node;
                    var bindMatrix = boneNode.WorldTransform * nodeInverseWorldTransform;
                    
                    if ( options.ConvertSkinToZUp )
                        bindMatrix *= YToZUpMatrix;

                    Matrix4x4.Invert( bindMatrix, out var inverseBindMatrix );

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

            geometry.MaterialName = UnescapeName( material.Name );
            geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
            geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );
            geometry.Flags |= GeometryFlags.Flag80000000;

            return geometry;
        }

        private static MatrixPalette BuildMatrixPalette( List<Matrix4x4> boneInverseBindMatrices, Dictionary<int, List<int>> nodeToBoneIndices )
        {
            var matrixPalette = new MatrixPalette( boneInverseBindMatrices.Count );

            for ( int i = 0; i < matrixPalette.BoneToNodeIndices.Length; i++ )
            {
                // Reverse dictionary search
                matrixPalette.BoneToNodeIndices[i] = ( ushort )nodeToBoneIndices
                    .Where( x => x.Value.Contains( i ) )
                    .Select( x => x.Key )
                    .Single();
            }

            // Inverse bind matrices are already ordered correctly
            matrixPalette.InverseBindMatrices = boneInverseBindMatrices.ToArray();

            return matrixPalette;
        }

        private struct NodeInfo
        {
            public readonly Ai.Node AssimpNode;
            public readonly Node Node;
            public readonly int Index;

            public NodeInfo( Ai.Node aiNode, Node node, int index )
            {
                AssimpNode = aiNode;
                Node = node;
                Index = index;
            }
        }
    }

    public class SceneConverterOptions
    {
        /// <summary>
        /// Gets or sets the version to use for the converted resources.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets whether to convert the up axis of the inverse bind pose matrices to Z-up. This is used by Persona 5's battle models for example.
        /// </summary>
        public bool ConvertSkinToZUp { get; set; }

        /// <summary>
        /// Gets or sets whether to generate dummy (white) vertex colors if they're not already present. Some material shaders rely on vertex colors being present, and the lack of them will cause graphics corruption.
        /// </summary>
        public bool GenerateVertexColors { get; set; }

        public SceneConverterOptions()
        {
            Version = Resource.PERSONA5_RESOURCE_VERSION;
            ConvertSkinToZUp = false;
            GenerateVertexColors = false;
        }
    }
}
