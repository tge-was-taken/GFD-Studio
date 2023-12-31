using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Lights;
using GFDLibrary.Models.Conversion.Utilities;
using GFDLibrary.Utilities;
using Ai = Assimp;

namespace GFDLibrary.Models.Conversion
{
    public static class ModelConverter
    {
        private static readonly Matrix4x4 YToZUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );

        private static readonly Matrix4x4 ZToYUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1 );

        public static Model ConvertFromAssimpScene( string filePath, ModelConverterOptions options )
        {
            var aiScene = AssimpSceneImporter.ImportFile( filePath );
            return ConvertFromAssimpScene( aiScene, options );
        }

        public static Model ConvertFromAssimpScene( Ai.Scene aiScene, ModelConverterOptions options )
        {
            var scene = new Model( options.Version );

            // Convert assimp nodes to our nodes 
            var nodeLookup = new Dictionary<string, NodeInfo>();
            int nextNodeIndex = 0;
            scene.RootNode = ConvertAssimpNodeRecursively( aiScene, aiScene.RootNode, nodeLookup, ref nextNodeIndex, options );

            // Process the meshes attached to the assimp nodes
            var nodeToBoneIndices = new Dictionary<int, List<int>>();
            int nextBoneIndex = 0;
            var boneInverseBindMatrices = new List<Matrix4x4>();
            var transformedVertices = new List< Vector3 >();
            ProcessAssimpNodeMeshesRecursively( aiScene.RootNode, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, transformedVertices, options );

            // Don't build a bone palette if there are no skinned meshes
            if ( boneInverseBindMatrices.Count > 0 )
            {
                // Build bone palette for skinning
                scene.Bones = BuildBonePalette( boneInverseBindMatrices, nodeToBoneIndices );
            }

            // Build bounding box & sphere
            scene.BoundingBox = BoundingBox.Calculate( transformedVertices );
            scene.BoundingSphere = BoundingSphere.Calculate( scene.BoundingBox.Value, transformedVertices );

            return scene;
        }

        private static Ai.Matrix4x4 GetWorldTransform( Ai.Node aiNode )
        {
            var transform = aiNode.Transform;
            var parent = aiNode.Parent;
            while ( parent != null )
            {
                transform *= parent.Transform;
                parent = parent.Parent;
            }

            return transform;
        }

        private static bool NearlyEquals( float a, float b, float epsilon = 0.001f )
        {
            double absA = Math.Abs( a );
            double absB = Math.Abs( b );
            double diff = Math.Abs( a - b );

            if ( a == b )
            { 
                // shortcut, handles infinities
                return true;
            }
            else if ( a == 0 || b == 0 || diff < double.Epsilon )
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            { 
                // use relative error
                return diff / ( absA + absB ) < epsilon;
            }
        }

        private static bool NearlyEquals( Ai.Matrix4x4 left, Ai.Matrix4x4 right )
        {
            return NearlyEquals( left.A1, right.A1 ) && NearlyEquals( left.A2, right.A2 ) && NearlyEquals( left.A3, right.A3 ) && NearlyEquals( left.A4, right.A4 ) &&
                   NearlyEquals( left.B1, right.B1 ) && NearlyEquals( left.B2, right.B2 ) && NearlyEquals( left.B3, right.B3 ) && NearlyEquals( left.B4, right.B4 ) &&
                   NearlyEquals( left.C1, right.C1 ) && NearlyEquals( left.C2, right.C2 ) && NearlyEquals( left.C3, right.C3 ) && NearlyEquals( left.C4, right.C4 ) &&
                   NearlyEquals( left.D1, right.D1 ) && NearlyEquals( left.D2, right.D2 ) && NearlyEquals( left.D3, right.D3 ) && NearlyEquals( left.D4, right.D4 );
        }

        private static bool IsMeshAttachmentNode( Ai.Node node )
        {
            bool isMeshAttachmentNode = node.Parent != null &&                                                          // definitely not a mesh attachment if it doesnt have a parent -> RootNode
                                        node.Parent.Name != "RootNode" &&                                               // probably not a mesh attachment if its part of the scene root
                                        AssimpConverterCommon.MeshAttachmentNameRegex.IsMatch( node.Name ) &&           // match name regex
                                        NearlyEquals( node.Transform, Ai.Matrix4x4.Identity );                          // transform must be identity

            return isMeshAttachmentNode;
        }

        private static void TryAddProperty( UserPropertyDictionary dictionary, UserProperty property )
        {
            if ( !dictionary.ContainsKey( property.Name ) )
                dictionary.Add( property );
        }

        private static void TryAddFullBodyObjectProperties( UserPropertyDictionary dictionary, string name )
        {
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_envelope_tone", 100 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_envelope_edge", 100 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_envelope", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_outline", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_shadow_reciever", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_shadow_caster", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_fog_disable", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_pos_resid", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_pos_minor", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_pos_major", 0 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_pos_type", 1 ) );
            TryAddProperty( dictionary, new UserIntProperty( $"{name}.CatherineData.xrd649_ca_type", 3 ) );
        }

        private static HashSet<string> sFullBodyObjectNames = new HashSet<string>()
        {
            "bell", "bar", "heart", "clock", "drink01", "drink02", "item_block02", 
        };

        private static Node ConvertAssimpNodeRecursively( Assimp.Scene aiScene, Ai.Node aiNode, Dictionary<string, NodeInfo> nodeLookup, ref int nextIndex, ModelConverterOptions options )
        {
            aiNode.Transform.Decompose( out var scale, out var rotation, out var translation );

            // Create node
            var node = new Node( AssimpConverterCommon.UnescapeName( aiNode.Name ),
                                 new Vector3( translation.X, translation.Y, translation.Z ),
                                 new Quaternion( rotation.X, rotation.Y, rotation.Z, rotation.W ),
                                 new Vector3( scale.X, scale.Y, scale.Z ) );

            

            if ( !IsMeshAttachmentNode( aiNode ) )
            {
                // Convert properties
                ConvertAssimpMetadataToProperties( aiNode.Metadata, node );

                if ( options.SetFullBodyNodeProperties )
                {
                    if (node.Name == "See User Defined Properties" )
                    {
                        TryAddProperty( node.Properties, new UserIntProperty( "NiSortAdjustNode", 0 ) );
                    }
                    else if ( node.Name.EndsWith( "root" ) || node.Name == "Bip01" )
                    {
                        TryAddProperty( node.Properties, new UserIntProperty( "KFAccumRoot", 0 ) );
                    }
                    else if ( sFullBodyObjectNames.Contains( node.Name ) )
                    {
                        TryAddFullBodyObjectProperties( node.Properties, node.Name );
                    }
                }

                if ( options.AutoAddGFDHelperIDs ) // for P5/R
                {
                    switch( node.Name )
                    {
                        case "h_B_BD1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 501 ) );
                            break;

                        case "h_C_US3":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 3 ) );
                            break;

                        case "h_C_FS1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 21 ) );
                            break;

                        case "h_M_HR3":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 103 ) );
                            break;

                        case "h_B_KA1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 502 ) );
                            break;

                        case "h_B_CT1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 521 ) );
                            break;

                        case "h_C_US2":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 2 ) );
                            break;

                        case "h_C_BS1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 11 ) );
                            break;

                        case "h_M_HL2":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 152 ) );
                            break;

                        case "h_B_MZ1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 511 ) );
                            break;

                        case "h_M_HR2":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 102 ) );
                            break;

                        case "h_M_HL1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 151 ) );
                            break;

                        case "h_M_BC1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 301 ) );
                            break;

                        case "h_C_US1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 1 ) );
                            break;

                        case "h_M_HL3":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 153 ) );
                            break;

                        case "h_M_HR1":
                            TryAddProperty( node.Properties, new UserIntProperty( "gfdHelperID", 101 ) );
                            break;

                        default:
                            break;
                    }
                }

                if ( !nodeLookup.ContainsKey( node.Name ) )
                {
                    // Add to lookup
                    nodeLookup.Add( node.Name, new NodeInfo( aiNode, node, nextIndex++, false ) );
                }
                else
                {
                    throw new Exception( $"Duplicate node name '{node.Name}'" );
                }

                // Is this a camera?
                var index = -1;
                if ( ( index = aiScene.Cameras.FindIndex( x => x.Name == node.Name ) ) != -1 )
                {
                    var aiCamera = aiScene.Cameras[ index ];
                    var camera = new Camera( -aiCamera.Direction.ToNumerics(), aiCamera.Up.ToNumerics(), aiCamera.Position.ToNumerics(),
                        aiCamera.ClipPlaneNear, aiCamera.ClipPlaneFar, MathHelper.RadiansToDegrees( aiCamera.FieldOfview ),
                        aiCamera.AspectRatio, 0 
                    ) { Version = options.Version };

                    node.Attachments.Add( new NodeCameraAttachment( camera ) );
                }
                else if ( ( index = aiScene.Lights.FindIndex( x => x.Name == node.Name ) ) != -1 )
                {
                    var aiLight = aiScene.Lights[ index ];
                    var lightType = LightType.Point;
                    switch ( aiLight.LightType )
                    {
                        case Ai.LightSourceType.Directional:
                            lightType = LightType.Type1;
                            break;
                        case Ai.LightSourceType.Point:
                        case Ai.LightSourceType.Ambient:
                            lightType = LightType.Point;
                            break;
                        case Ai.LightSourceType.Spot:
                            lightType = LightType.Spot;
                            break;
                    }

                    var light = new Light
                    {
                        Version = options.Version,
                        AmbientColor   = aiLight.ColorAmbient.ToNumerics(),
                        DiffuseColor   = aiLight.ColorDiffuse.ToNumerics(),
                        SpecularColor  = aiLight.ColorSpecular.ToNumerics(),
                        AngleInnerCone = aiLight.AngleInnerCone,
                        AngleOuterCone = aiLight.AngleOuterCone,
                        Type           = lightType,
                        Flags          = LightFlags.Bit1 | LightFlags.Bit2
                    };
                    node.Attachments.Add( new NodeLightAttachment( light ) );
                }

                // Process children
                foreach ( var aiNodeChild in aiNode.Children )
                {
                    if ( aiNodeChild.Name == "RootNode" )
                    {
                        // For compatibility with old exports 
                        // Merge children of 'RootNode' node with actual root node
                        foreach ( var aiFakeRootNodeChild in aiNodeChild.Children )
                        {
                            var childNode = ConvertAssimpNodeRecursively( aiScene, aiFakeRootNodeChild, nodeLookup, ref nextIndex, options );
                            node.AddChildNode( childNode );
                        }
                    }
                    else
                    {
                        var childNode = ConvertAssimpNodeRecursively( aiScene, aiNodeChild, nodeLookup, ref nextIndex, options );
                        node.AddChildNode( childNode );
                    }
                }
            }
            else
            {
                nodeLookup.Add( node.Name, new NodeInfo( aiNode, node, -1, true ) );
            }

            return node;
        }

        private static void ConvertAssimpMetadataToProperties( Ai.Metadata metadata, Node node )
        {
            foreach ( var metadataEntry in metadata )
            {
                UserProperty property = null;

                // Skip some garbage entries
                if ( metadataEntry.Key == "IsNull" ||
                     metadataEntry.Key == "InheritType" ||
                     metadataEntry.Key == "DefaultAttributeIndex" ||
                     metadataEntry.Key == "UserProperties" || // dupe of UDP3DSMAX
                     metadataEntry.Key == "ScaleMax" ||
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
                            property = new UserBoolProperty( kvp.Key, true );
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
                                property = new UserVector3Property( kvp.Key, new Vector3( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2] ) );
                            }
                            else if ( arrayFloatValues.Count == 4 )
                            {
                                property = new UserVector4Property( kvp.Key, new Vector4( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2], arrayFloatValues[3] ) );
                            }
                            else
                            {
                                var arrayByteValues = arrayFloatValues.Cast<byte>();
                                property = new UserByteArrayProperty( kvp.Key, arrayByteValues.ToArray() );
                            }
                        }
                        else if ( int.TryParse( kvp.Value, out int intValue ) )
                        {
                            property = new UserIntProperty( kvp.Key, intValue );
                        }
                        else if ( float.TryParse( kvp.Value, out float floatValue ) )
                        {
                            property = new UserFloatProperty( kvp.Key, floatValue );
                        }
                        else if ( bool.TryParse( kvp.Value, out bool boolValue ) )
                        {
                            property = new UserBoolProperty( kvp.Key, boolValue );
                        }
                        else
                        {
                            property = new UserStringProperty( kvp.Key, kvp.Value );
                        }
                    }
                }
                else
                {
                    switch ( metadataEntry.Value.DataType )
                    {
                        case Ai.MetaDataType.Bool:
                            property = new UserBoolProperty( metadataEntry.Key, metadataEntry.Value.DataAs<bool>().Value );
                            break;
                        case Ai.MetaDataType.Int32:
                            property = new UserIntProperty( metadataEntry.Key, metadataEntry.Value.DataAs<int>().Value );
                            break;
                        case Ai.MetaDataType.UInt64:
                            property = new UserByteArrayProperty( metadataEntry.Key, BitConverter.GetBytes( metadataEntry.Value.DataAs<ulong>().Value ) );
                            break;
                        case Ai.MetaDataType.Float:
                            property = new UserFloatProperty( metadataEntry.Key, metadataEntry.Value.DataAs<float>().Value );
                            break;
                        case Ai.MetaDataType.String:
                            property = new UserStringProperty( metadataEntry.Key, ( string )metadataEntry.Value.Data );
                            break;
                        case Ai.MetaDataType.Vector3D:
                            var data = metadataEntry.Value.DataAs<Ai.Vector3D>().Value;
                            property = new UserVector3Property( metadataEntry.Key, new Vector3( data.X, data.Y, data.Z ) );
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

        private static void ProcessAssimpNodeMeshesRecursively( Ai.Node aiNode, Ai.Scene aiScene, Dictionary<string, NodeInfo> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, List<Vector3> transformedVertices, ModelConverterOptions options )
        {
            if ( aiNode.HasMeshes )
            {
                var nodeInfo = nodeLookup[ AssimpConverterCommon.UnescapeName( aiNode.Name ) ];
                var node = nodeInfo.Node;
                var nodeWorldTransform = node.WorldTransform;
                Matrix4x4.Invert( nodeWorldTransform, out var nodeInverseWorldTransform );

                foreach ( var aiMeshIndex in aiNode.MeshIndices )
                {
                    var aiMesh = aiScene.Meshes[aiMeshIndex];
                    var aiMaterial = aiScene.Materials[aiMesh.MaterialIndex];
                    var geometry = ConvertAssimpMeshToGeometry( aiMesh, aiMaterial, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, ref nodeWorldTransform, ref nodeInverseWorldTransform, transformedVertices, options );

                    if ( !nodeInfo.IsMeshAttachment )
                    {
                        node.Attachments.Add( new NodeMeshAttachment( geometry ) );
                    }
                    else
                    {
                        node.Parent.Attachments.Add( new NodeMeshAttachment( geometry ) );
                        node.Parent.RemoveChildNode( node );
                    }
                }
            }

            foreach ( var aiNodeChild in aiNode.Children )
            {
                ProcessAssimpNodeMeshesRecursively( aiNodeChild, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, transformedVertices, options );
            }
        }

        private static Mesh ConvertAssimpMeshToGeometry( Ai.Mesh aiMesh, Ai.Material material, Dictionary<string, NodeInfo> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, ref Matrix4x4 nodeWorldTransform, ref Matrix4x4 nodeInverseWorldTransform, List<Vector3> transformedVertices, ModelConverterOptions options )
        {
            if ( !aiMesh.HasVertices )
                throw new Exception( "Assimp mesh has no vertices" );

            var geometry = new Mesh();
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


            if (aiMesh.HasTangentBasis)
            {
                geometry.Tangents = aiMesh.Tangents
                                         .Select(x => new Vector3(x.X, x.Y, x.Z))
                                         .ToArray();
            }

            if ( aiMesh.HasTextureCoords( 0 ) )
            {
                geometry.TexCoordsChannel0 = aiMesh.TextureCoordinateChannels[0]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasTextureCoords( 1 ) && !options.MinimalVertexAttributes )
            {
                geometry.TexCoordsChannel1 = aiMesh.TextureCoordinateChannels[1]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasTextureCoords( 2) && !options.MinimalVertexAttributes )
            {
                geometry.TexCoordsChannel2 = aiMesh.TextureCoordinateChannels[2]
                                                   .Select( x => new Vector2( x.X, x.Y ) )
                                                   .ToArray();
            }

            if ( aiMesh.HasVertexColors( 0) && !options.MinimalVertexAttributes)
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

            if ( aiMesh.HasVertexColors( 1) && !options.MinimalVertexAttributes)
            {
                geometry.ColorChannel1 = aiMesh.VertexColorChannels[1]
                                               .Select( x => ( uint )( ( byte )( x.B * 255f ) | ( byte )( x.G * 255f ) << 8 | ( byte )( x.R * 255f ) << 16 | ( byte )( x.A * 255f ) << 24 ) )
                                               .ToArray();
            }

            if ( aiMesh.HasFaces )
            {
                geometry.TriangleIndexFormat = aiMesh.VertexCount <= ushort.MaxValue ? TriangleIndexFormat.UInt16 : TriangleIndexFormat.UInt32;
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
                    var boneLookupData = nodeLookup[ AssimpConverterCommon.UnescapeName( aiMeshBone.Name ) ];
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

            geometry.MaterialName = AssimpConverterCommon.UnescapeName( material.Name );
            geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
            geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );
            geometry.Flags |= GeometryFlags.Bit31;

            return geometry;
        }

        private static List<Bone> BuildBonePalette( List<Matrix4x4> boneInverseBindMatrices, Dictionary<int, List<int>> nodeToBoneIndices )
        {
            var usedBones = new List<Bone>();

            for ( int i = 0; i < boneInverseBindMatrices.Count; i++ )
            {
                // Reverse dictionary search
                var boneToNodeIndex = ( ushort ) nodeToBoneIndices
                                                 .Where( x => x.Value.Contains( i ) )
                                                 .Select( x => x.Key )
                                                 .Single();

                var inverseBindMatrix = boneInverseBindMatrices[ i ];
                usedBones.Add( new Bone( boneToNodeIndex, inverseBindMatrix ) );
            }

            return usedBones;
        }

        private struct NodeInfo
        {
            public readonly Ai.Node AssimpNode;
            public readonly Node Node;
            public readonly int Index;
            public readonly bool IsMeshAttachment;

            public NodeInfo( Ai.Node aiNode, Node node, int index, bool isMesh )
            {
                AssimpNode = aiNode;
                Node = node;
                Index = index;
                IsMeshAttachment = isMesh;
            }
        }
    }

    public class ModelConverterOptions
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

        /// <summary>
        /// Gets or sets whether to generate dummy (white) vertex colors if they're not already present. Some material shaders rely on vertex colors being present, and the lack of them will cause graphics corruption.
        /// </summary>
        public bool MinimalVertexAttributes { get; set; }

        public bool SetFullBodyNodeProperties { get; set; }

        public bool AutoAddGFDHelperIDs { get; set; }

        public ModelConverterOptions()
        {
            Version = ResourceVersion.Persona5;
            ConvertSkinToZUp = false;
            GenerateVertexColors = false;
            MinimalVertexAttributes = true;
            SetFullBodyNodeProperties = false;
        }
    }
}
