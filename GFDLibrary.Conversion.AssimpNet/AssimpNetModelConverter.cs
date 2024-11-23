﻿using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Conversion.AssimpNet.Utilities;
using GFDLibrary.Graphics;
using GFDLibrary.Lights;
using GFDLibrary.Models;
using GFDLibrary.Utilities;
using Ai = Assimp;

namespace GFDLibrary.Conversion.AssimpNet
{
    public static class AssimpNetModelConverter
    {
        private static readonly Matrix4x4 YToZUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );

        private static readonly Matrix4x4 ZToYUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1 );

        public static Model ConvertFromAssimpScene( string filePath, ModelConverterOptions options )
        {
            var aiScene = AssimpHelper.ImportScene( filePath );
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
            var transformedVertices = new List<Vector3>();
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

        private static bool IsLegacyMeshAttachmentNode( Ai.Node node )
        {
            bool isMeshAttachmentNode = node.Parent != null &&                                                          // definitely not a mesh attachment if it doesnt have a parent -> RootNode
                                        node.Parent.Name != "RootNode" &&                                               // probably not a mesh attachment if its part of the scene root
                                        AssimpConverterCommon.LegacyMeshAttachmentNameRegex.IsMatch( node.Name ) &&     // match name regex
                                        NearlyEquals( node.Transform, Ai.Matrix4x4.Identity );                          // transform must be identity
            return isMeshAttachmentNode;
        }

        private static bool IsMeshAttachmentNode( Ai.Node node )
        {
            if ( IsLegacyMeshAttachmentNode( node ) )
                return true;

            bool isMeshAttachmentNode = node.Parent != null &&                                                          // mesh attachments are parented to the root of the scene to prevent animation issues
                                        node.Parent.Name == "RootNode" &&
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

        private static void AddUserProperties_P5R( Node node )
        {
            switch ( node.Name )
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

        private static void AddUserProperties_Metaphor(Node node)
        {

            void AddEyeAngleLimitProperties(string side, Node node)
            {
                TryAddProperty( node.Properties, new UserFloatProperty( $"{side}eyeAngleUpLimit", 2f ) );
                TryAddProperty( node.Properties, new UserFloatProperty( $"{side}eyeAngleDownLimit", 3f ) );
                TryAddProperty( node.Properties, new UserFloatProperty( $"{side}eyeAngleInLimit", 5f ) );
                TryAddProperty( node.Properties, new UserFloatProperty( $"{side}eyeAngleOutLimit", 7f ) );
            }

            switch (node.Name)
            {
                case "root":
                    TryAddProperty( node.Properties, new UserBoolProperty( "no_anim_export", false ) );
                    AddEyeAngleLimitProperties( "L", node );
                    AddEyeAngleLimitProperties( "R", node );
                    TryAddProperty( node.Properties, new UserFloatProperty( "HeadAngleUpLimit", 20f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "HeadAngleDownLimit", 4f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "HeadAngleLeftLimit", 50f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "HeadAngleRightLimit", 50f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "UbodyAngleUpLimit", 20f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "UbodyAngleDownLimit", 20f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "UbodyAngleLeftLimit", 10f ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "UbodyAngleRightLimit", 10f ) );
                    break;
                case "mesh_cha":
                    TryAddProperty( node.Properties, new UserVector3Property( "gfdGradationColor", new Vector3(0, 0, 0) ) );
                    TryAddProperty( node.Properties, new UserFloatProperty( "gfdGradationScale", 2f ));
                    TryAddProperty( node.Properties, new UserFloatProperty( "gfdGradationFade", 0.2f ));
                    TryAddProperty( node.Properties, new UserFloatProperty( "gfdGradiationAlpha", 0.5f ));
                    TryAddProperty( node.Properties, new UserStringProperty( "gfdGradationHipNode", "c_pelvis" ));
                    TryAddProperty( node.Properties, new UserStringProperty( "gfdGradationRightHeelNode", "r_foot" ));
                    TryAddProperty( node.Properties, new UserStringProperty( "gfdGradationLeftHeelNode", "l_foot" ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdShadowCaster", false ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdShadowReceiver", false ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdWaterCaster", true ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdHide", false ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdSplitPerVertexNormals", false ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdGeomType", true ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdBillboard", true ));
                    TryAddProperty( node.Properties, new UserBoolProperty( "gfdSortByName", false ));
                    break;
            }
        }

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

                if ( options.AutoAddGFDHelperIDs )
                {
                    if (options.Version >= 0x2000000)
                        AddUserProperties_Metaphor( node );
                    else 
                        AddUserProperties_P5R( node );
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
                    var aiCamera = aiScene.Cameras[index];
                    var camera = new Camera( -aiCamera.Direction.ToNumerics(), aiCamera.Up.ToNumerics(), aiCamera.Position.ToNumerics(),
                        aiCamera.ClipPlaneNear, aiCamera.ClipPlaneFar, MathHelper.RadiansToDegrees( aiCamera.FieldOfview ),
                        aiCamera.AspectRatio, 0
                    )
                    { Version = options.Version };

                    node.Attachments.Add( new NodeCameraAttachment( camera ) );
                }
                else if ( ( index = aiScene.Lights.FindIndex( x => x.Name == node.Name ) ) != -1 )
                {
                    var aiLight = aiScene.Lights[index];
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
                        AmbientColor = aiLight.ColorAmbient.ToNumerics(),
                        DiffuseColor = aiLight.ColorDiffuse.ToNumerics(),
                        SpecularColor = aiLight.ColorSpecular.ToNumerics(),
                        AngleInnerCone = aiLight.AngleInnerCone,
                        AngleOuterCone = aiLight.AngleOuterCone,
                        Type = lightType,
                        Flags = LightFlags.Bit1 | LightFlags.Bit2
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
                     metadataEntry.Key == "ScalingMax" ||
                     metadataEntry.Key == "MaxHandle" )
                {
                    continue;
                }

                if ( metadataEntry.Key == "UDP3DSMAX" )
                {
                    var properties = ( (string)metadataEntry.Value.Data )
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
                        property = UserPropertyParser.ParseProperty( kvp.Key, kvp.Value );
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
                            property = new UserStringProperty( metadataEntry.Key, (string)metadataEntry.Value.Data );
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
                var nodeInfo = nodeLookup[AssimpConverterCommon.UnescapeName( aiNode.Name )];
                Node targetNode = nodeInfo.Node;
                var sourceToTargetModelMatrix = Matrix4x4.Identity;
                if ( nodeInfo.IsMeshAttachment && nodeInfo.Node.Parent?.Name == "RootNode")
                {
                    var originalParentNodeName = nodeInfo.Node.Name[..nodeInfo.Node.Name.IndexOf( ModelConversionHelpers.MeshAttachmentNameSuffix )];
                    if ( nodeLookup.TryGetValue( originalParentNodeName, out var originalParentNodeInfo ) )
                    {
                        targetNode = originalParentNodeInfo.Node;
                        // Need to transform the vertices from the local space of the original node to the local space of the new node
                        sourceToTargetModelMatrix = targetNode.WorldTransform.Inverted() * nodeInfo.Node.WorldTransform;
                    }
                }

                var sourceNode = nodeInfo.Node;
                var targetNodeWorldTransform = targetNode.WorldTransform;
                Matrix4x4.Invert( targetNodeWorldTransform, out var targetNodeInverseWorldTransform );

                foreach ( var aiMeshIndex in aiNode.MeshIndices )
                {
                    var aiMesh = aiScene.Meshes[aiMeshIndex];
                    var aiMaterial = aiScene.Materials[aiMesh.MaterialIndex];
                    var geometry = ConvertAssimpMeshToGeometry( aiMesh, aiMaterial, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, sourceToTargetModelMatrix, ref targetNodeWorldTransform, ref targetNodeInverseWorldTransform, transformedVertices, options );

                    if ( !nodeInfo.IsMeshAttachment )
                    {
                        targetNode.Attachments.Add( new NodeMeshAttachment( geometry ) );
                    }
                    else
                    {
                        targetNode.Attachments.Add( new NodeMeshAttachment( geometry ) );
                        sourceNode.Parent.RemoveChildNode( sourceNode );
                    }
                }
            }

            foreach ( var aiNodeChild in aiNode.Children )
            {
                ProcessAssimpNodeMeshesRecursively( aiNodeChild, aiScene, nodeLookup, ref nextBoneIndex, nodeToBoneIndices, boneInverseBindMatrices, transformedVertices, options );
            }
        }

        private static Mesh ConvertAssimpMeshToGeometry( Ai.Mesh aiMesh, Ai.Material material, Dictionary<string, NodeInfo> nodeLookup, ref int nextBoneIndex, Dictionary<int, List<int>> nodeToBoneIndices, List<Matrix4x4> boneInverseBindMatrices, Matrix4x4 modelMatrix, ref Matrix4x4 nodeWorldTransform, ref Matrix4x4 nodeInverseWorldTransform, List<Vector3> transformedVertices, ModelConverterOptions options )
        {
            if ( !aiMesh.HasVertices )
                throw new Exception( "Assimp mesh has no vertices" );
            
            var meshName = AssimpConverterCommon.UnescapeName( aiMesh.Name );
            var materialName = AssimpConverterCommon.UnescapeName( material.Name );
            var geometryOptions = options.DefaultMesh;
            if ( options.Meshes.TryGetValue( meshName, out var geometryOptionsOverride ) )
                geometryOptions = geometryOptionsOverride;

            var geometry = new Mesh( options.Version );
            var geometryTransformedVertices = new Vector3[ aiMesh.VertexCount ];

            geometry.Vertices = aiMesh.Vertices
                                      .Select( x => Vector3.Transform( new Vector3( x.X, x.Y, x.Z ), modelMatrix ) )
                                      .ToArray();

            for ( int i = 0; i < geometry.Vertices.Length; i++ )
                geometryTransformedVertices[i] = Vector3.Transform( geometry.Vertices[i], nodeWorldTransform );

            transformedVertices.AddRange( geometryTransformedVertices );

            var useNormals = geometryOptions.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal );
            if ( useNormals )
            {
                if ( aiMesh.HasNormals )
                {
                    geometry.Normals = aiMesh.Normals
                           .Select( x => Vector3.TransformNormal( new Vector3( x.X, x.Y, x.Z ), modelMatrix ) )
                           .ToArray();
                }
            }
            else
            {
                // TODO 
            }

            var useTangent = geometryOptions.VertexAttributeFlags.HasFlag(VertexAttributeFlags.Tangent);
            if ( useTangent )
            {
                if ( aiMesh.HasTangentBasis )
                {
                    geometry.Tangents = aiMesh.Tangents
                       .Select( x => new Vector3( x.X, x.Y, x.Z ) )
                       .ToArray();
                }
                else
                {
                    // TODO
                }
            }

            var useBinormal = geometryOptions.VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal );
            if ( useBinormal )
            {
                if ( aiMesh.HasTangentBasis )
                {
                    geometry.Binormals = aiMesh.BiTangents
                        .Select( x => new Vector3( x.X, x.Y, x.Z ) )
                        .ToArray();
                }
                else
                {
                    // TODO
                }
            }

            var texCoordFlags = new[] { VertexAttributeFlags.TexCoord0, VertexAttributeFlags.TexCoord1, VertexAttributeFlags.TexCoord2 };
            for ( int channel = 0; channel < 3; channel++ )
            {
                var useTexCoord = geometryOptions.VertexAttributeFlags.HasFlag( texCoordFlags[channel] );
                if ( !useTexCoord ) continue;

                var channelOptions = geometryOptions.TexCoordChannelMap[channel];
                Vector2[] texCoords;
                if ( aiMesh.HasTextureCoords( channelOptions.SourceChannel ) )
                {
                    texCoords = aiMesh.TextureCoordinateChannels[channelOptions.SourceChannel]
                                          .Select( x => new Vector2( x.X, x.Y ) )
                                          .ToArray();
                }
                else
                {
                    texCoords = new Vector2[aiMesh.VertexCount];
                }

                switch ( channel )
                {
                    case 0:
                        geometry.TexCoordsChannel0 = texCoords;
                        break;
                    case 1:
                        geometry.TexCoordsChannel1 = texCoords;
                        break;
                    default:
                        geometry.TexCoordsChannel2 = texCoords;
                        break;
                }
            }

            var colorFlags = new[]
            {
                VertexAttributeFlags.Color0,
                VertexAttributeFlags.Color1,
                VertexAttributeFlags.Color2
            };

            // If only one color channel is active, we might use the first available source channel if the required source channel is not present
            var activeColorChannels = colorFlags.Where( flag => geometryOptions.VertexAttributeFlags.HasFlag( flag ) );
            var useFirstAvailableSourceChannelAsFallback = activeColorChannels.Count() == 1;

            for ( int i = 0; i < 3; i++ )
            {
                if ( geometryOptions.VertexAttributeFlags.HasFlag( colorFlags[i] ) )
                {
                    var channelOptions = geometryOptions.ColorChannelMap[i];
                    Graphics.Color[] colorChannel = null;

                    if ( aiMesh.HasVertexColors( channelOptions.SourceChannel ) )
                    {
                        colorChannel = aiMesh.VertexColorChannels[channelOptions.SourceChannel]
                                             .Select( x => SwizzleColor( x, channelOptions.Swizzle ) )
                                             .ToArray();
                    }
                    else if ( useFirstAvailableSourceChannelAsFallback )
                    {
                        // Try to find the first available source channel
                        var firstAvailableChannel = Enumerable.Range( 0, aiMesh.VertexColorChannelCount )
                                                              .FirstOrDefault( ch => aiMesh.HasVertexColors( ch ) );
                        if ( firstAvailableChannel != -1 )
                        {
                            colorChannel = aiMesh.VertexColorChannels[firstAvailableChannel]
                                                 .Select( x => SwizzleColor( x, channelOptions.Swizzle ) )
                                                 .ToArray();
                        }
                    }

                    // If still no color channel, use default color if specified
                    if ( colorChannel == null && channelOptions.UseDefaultColor )
                    {
                        colorChannel = new Graphics.Color[geometry.VertexCount];
                        for ( int j = 0; j < colorChannel.Length; j++ )
                            colorChannel[j] = channelOptions.DefaultColor;
                    }

                    switch ( i )
                    {
                        case 0:
                            geometry.ColorChannel0 = colorChannel;
                            break;
                        case 1:
                            geometry.ColorChannel1 = colorChannel;
                            break;
                        case 2:
                            geometry.ColorChannel2 = colorChannel;
                            break;
                    }
                }
            }

            if ( aiMesh.HasFaces )
            {
                geometry.TriangleIndexFormat = aiMesh.VertexCount <= ushort.MaxValue ? TriangleIndexFormat.UInt16 : TriangleIndexFormat.UInt32;
                geometry.Triangles = aiMesh.Faces
                                           .Select( x => new Triangle( (uint)x.Indices[0], (uint)x.Indices[1], (uint)x.Indices[2] ) )
                                           .ToArray();
            }

            if ( aiMesh.HasBones )
            {
                geometry.VertexWeights = new VertexWeight[geometry.VertexCount];
                var weightCount = ResourceVersion.IsV2(options.Version) ? 8 : 4;
                for ( int i = 0; i < geometry.VertexWeights.Length; i++ )
                {
                    geometry.VertexWeights[i].Indices = new ushort[weightCount];
                    geometry.VertexWeights[i].Weights = new float[weightCount];
                }

                var vertexWeightCounts = new int[geometry.VertexCount];

                for ( var i = 0; i < aiMesh.Bones.Count; i++ )
                {
                    var aiMeshBone = aiMesh.Bones[i];

                    // Find node index for the bone
                    var boneLookupData = nodeLookup[AssimpConverterCommon.UnescapeName( aiMeshBone.Name )];
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

                        geometry.VertexWeights[aiVertexWeight.VertexID].Indices[vertexWeightCount] = ( ushort )boneIndex;
                        geometry.VertexWeights[aiVertexWeight.VertexID].Weights[vertexWeightCount] = aiVertexWeight.Weight;
                    }
                }
            }

            geometry.MaterialName = AssimpConverterCommon.UnescapeName( material.Name );
            geometry.BoundingBox = BoundingBox.Calculate( geometry.Vertices );
            geometry.BoundingSphere = BoundingSphere.Calculate( geometry.BoundingBox.Value, geometry.Vertices );

            const GeometryFlags nonOverridableGeometryFlags =
                GeometryFlags.HasBoundingBox | GeometryFlags.HasBoundingSphere |
                GeometryFlags.HasMaterial | GeometryFlags.HasMorphTargets |
                GeometryFlags.HasTriangles | GeometryFlags.HasVertexWeights;
            if ( geometryOptionsOverride is not null )
            {
                geometry.Flags |= geometryOptionsOverride.GeometryFlags & ~nonOverridableGeometryFlags;
            }
            else
            {
                geometry.Flags |= GeometryFlags.Bit7;
            }

            return geometry;
        }

        private static GFDLibrary.Graphics.Color SwizzleColor( Ai.Color4D sourceColor, ColorSwizzle swizzle )
        {
            float[] channels = new float[4] { sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A };
            return new GFDLibrary.Graphics.Color(
                channels[(int)swizzle.Red],
                channels[(int)swizzle.Green],
                channels[(int)swizzle.Blue],
                channels[(int)swizzle.Alpha]
            );
        }

        private static List<Bone> BuildBonePalette( List<Matrix4x4> boneInverseBindMatrices, Dictionary<int, List<int>> nodeToBoneIndices )
        {
            var usedBones = new List<Bone>();

            for ( int i = 0; i < boneInverseBindMatrices.Count; i++ )
            {
                // Reverse dictionary search
                var boneToNodeIndex = (ushort)nodeToBoneIndices
                                                 .Where( x => x.Value.Contains( i ) )
                                                 .Select( x => x.Key )
                                                 .Single();

                var inverseBindMatrix = boneInverseBindMatrices[i];
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

    public static class UserPropertyParser
    {
        private static readonly Dictionary<string, Func<string, string, UserProperty>> PropertyParsers = new Dictionary<string, Func<string, string, UserProperty>>( StringComparer.OrdinalIgnoreCase )
        {
            {"int", ParseIntProperty},
            {"float", ParseFloatProperty},
            {"bool", ParseBoolProperty},
            {"string", ParseStringProperty},
            {"bytevector3", ParseByteVector3Property},
            {"bytevector4", ParseByteVector4Property},
            {"vector3", ParseVector3Property},
            {"vector4", ParseVector4Property},
            {"bytearray", ParseByteArrayProperty}
        };

        public static UserProperty ParseProperty( string key, string value )
        {
            if ( value == null )
            {
                return new UserBoolProperty( key, true ); // Assume flag bool
            }

            var match = Regex.Match( value, @"^(\w+)\((.*)\)$" );
            if ( match.Success )
            {
                var type = match.Groups[1].Value;
                var content = match.Groups[2].Value;

                if ( PropertyParsers.TryGetValue( type, out var parser ) )
                {
                    return parser( key, content );
                }
            }

            if ( value.StartsWith( "[" ) && value.EndsWith( "]" ) )
            {
                return ParseArrayProperty( key, value );
            }

            // Try parsing as simple types
            if ( int.TryParse( value, out int intValue ) )
            {
                return new UserIntProperty( key, intValue );
            }
            if ( float.TryParse( value, out float floatValue ) )
            {
                return new UserFloatProperty( key, floatValue );
            }
            if ( bool.TryParse( value, out bool boolValue ) )
            {
                return new UserBoolProperty( key, boolValue );
            }

            // Default to string if no other type matches
            return new UserStringProperty( key, value );
        }

        private static UserProperty ParseIntProperty( string key, string content )
        {
            if ( int.TryParse( content, out var intValue ) )
            {
                return new UserIntProperty( key, intValue );
            }
            throw new FormatException( $"Invalid int format: {content}" );
        }

        private static UserProperty ParseFloatProperty( string key, string content )
        {
            if ( float.TryParse( content, out var floatValue ) )
            {
                return new UserFloatProperty( key, floatValue );
            }
            throw new FormatException( $"Invalid float format: {content}" );
        }

        private static UserProperty ParseBoolProperty( string key, string content )
        {
            if ( bool.TryParse( content, out var boolValue ) )
            {
                return new UserBoolProperty( key, boolValue );
            }
            throw new FormatException( $"Invalid bool format: {content}" );
        }

        private static UserProperty ParseStringProperty( string key, string content )
        {
            return new UserStringProperty( key, content );
        }

        private static UserProperty ParseByteVector3Property( string key, string content )
        {
            var values = content.Split( ',' );
            if ( values.Length == 3 && byte.TryParse( values[0], out byte x ) && byte.TryParse( values[1], out byte y ) && byte.TryParse( values[2], out byte z ) )
            {
                return new UserByteVector3Property( key, new ByteVector3( x, y, z ) );
            }
            throw new FormatException( $"Invalid ByteVector3 format: {content}" );
        }

        private static UserProperty ParseByteVector4Property( string key, string content )
        {
            var values = content.Split( ',' );
            if ( values.Length == 4 && byte.TryParse( values[0], out byte x ) && byte.TryParse( values[1], out byte y ) && byte.TryParse( values[2], out byte z ) && byte.TryParse( values[3], out byte w ) )
            {
                return new UserByteVector4Property( key, new ByteVector4( x, y, z, w ) );
            }
            throw new FormatException( $"Invalid ByteVector4 format: {content}" );
        }

        private static UserProperty ParseVector3Property( string key, string content )
        {
            var values = content.Split( ',' );
            if ( values.Length == 3 && float.TryParse( values[0], out float x ) && float.TryParse( values[1], out float y ) && float.TryParse( values[2], out float z ) )
            {
                return new UserVector3Property( key, new Vector3( x, y, z ) );
            }
            throw new FormatException( $"Invalid Vector3 format: {content}" );
        }

        private static UserProperty ParseVector4Property( string key, string content )
        {
            var values = content.Split( ',' );
            if ( values.Length == 4 && float.TryParse( values[0], out float x ) && float.TryParse( values[1], out float y ) && float.TryParse( values[2], out float z ) && float.TryParse( values[3], out float w ) )
            {
                return new UserVector4Property( key, new Vector4( x, y, z, w ) );
            }
            throw new FormatException( $"Invalid Vector4 format: {content}" );
        }

        private static UserProperty ParseByteArrayProperty( string key, string content )
        {
            try
            {
                byte[] byteArray = Convert.FromBase64String( content );
                return new UserByteArrayProperty( key, byteArray );
            }
            catch ( FormatException )
            {
                throw new FormatException( $"Invalid Base64 string: {content}" );
            }
        }

        private static UserProperty ParseArrayProperty( string key, string value )
        {
            var arrayContents = value.Substring( 1, value.Length - 2 );
            var arrayValues = arrayContents.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

            var arrayFloatValues = arrayValues.Select( v => float.TryParse( v, out float f ) ? f : throw new FormatException( $"Failed to parse array value as float: {v}" ) ).ToList();

            if ( arrayFloatValues.Count == 3 )
            {
                return new UserVector3Property( key, new Vector3( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2] ) );
            }
            else if ( arrayFloatValues.Count == 4 )
            {
                return new UserVector4Property( key, new Vector4( arrayFloatValues[0], arrayFloatValues[1], arrayFloatValues[2], arrayFloatValues[3] ) );
            }
            else
            {
                var arrayByteValues = arrayFloatValues.Select( f => (byte)f ).ToArray();
                return new UserByteArrayProperty( key, arrayByteValues );
            }
        }
    }
}
