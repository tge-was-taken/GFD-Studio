using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Lights;
using GFDLibrary.Animations;
using GFDLibrary.Models;

namespace GFDLibrary.Conversion.FbxSdk
{
    public sealed class FbxSdkModelPackExporterConfig
    {
    }

    public class FbxSdkModelPackExporterException : Exception
    {
        public FbxSdkModelPackExporterException( string message ) : base( message ) { }
    }

    /// <summary>
    /// Writes a model pack as an FBX file through the Autodesk FBX SDK.
    /// </summary>
    /// <remarks>
    /// 3ds Max is picky about the order of layers and layer elements in a mesh, because it maps
    /// layers to map channels positionally. It expects layer 0 to hold the normals, the material,
    /// the first colour channel and the first UV channel, in that order, with the second UV
    /// channel on layer 1 and the second colour channel on layer 2. The order of the calls below
    /// is what produces that, so keep it.
    /// </remarks>
    public sealed class FbxSdkModelPackExporter : IDisposable
    {
        private readonly IntPtr mManager;
        private bool mDisposed;

        private IntPtr mScene;
        private IntPtr mBindPose;

        private ModelPack mModelPack;
        private Model mModel;
        private List<Node> mModelNodes;

        private readonly Dictionary<Node, IntPtr> mNodeToFbxNode = new();
        private readonly Dictionary<int, IntPtr> mNodeIndexToFbxNode = new();
        private readonly Dictionary<int, IntPtr> mNodeIndexToFbxCluster = new();
        private readonly Dictionary<string, IntPtr> mTexturePathToFbxFileTexture = new( StringComparer.OrdinalIgnoreCase );
        private readonly Dictionary<string, IntPtr> mMaterialNameToFbxSurfaceMaterial = new();

        private string mPath;
        private string mTextureBaseDirectoryPath;

        public FbxSdkModelPackExporter()
        {
            mManager = FbxSdk.ManagerCreate();
            if ( mManager == IntPtr.Zero )
                throw new FbxSdkModelPackExporterException( $"Failed to create the FBX manager: {FbxSdk.GetLastError()}" );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;
            if ( mManager != IntPtr.Zero )
                FbxSdk.ManagerDestroy( mManager );
        }

        public static void ExportFile( ModelPack modelPack, string path, FbxSdkModelPackExporterConfig config )
        {
            using var exporter = new FbxSdkModelPackExporter();
            exporter.Export( modelPack, path, config );
        }

        public void Export( ModelPack modelPack, string path, FbxSdkModelPackExporterConfig config )
        {
            Reset();

            mPath = path;
            var fileName = Path.GetFileNameWithoutExtension( path );
            var baseDirectoryPath = Path.GetDirectoryName( path );
            mTextureBaseDirectoryPath = Path.Combine( baseDirectoryPath ?? string.Empty, $"{fileName}_Textures" );

            ConvertToFbxScene( modelPack );
            ExportAnimations( modelPack.AnimationPack );
            ExportFbxScene();
        }

        private void Reset()
        {
            mNodeToFbxNode.Clear();
            mNodeIndexToFbxNode.Clear();
            mNodeIndexToFbxCluster.Clear();
            mTexturePathToFbxFileTexture.Clear();
            mMaterialNameToFbxSurfaceMaterial.Clear();
        }

        private void ConvertToFbxScene( ModelPack modelPack )
        {
            mModelPack = modelPack;
            mModel = modelPack.Model;
            mModelNodes = new List<Node>( mModel.Nodes );

            mScene = FbxSdk.SceneCreate( mManager, "" );
            if ( mScene == IntPtr.Zero )
                throw new FbxSdkModelPackExporterException( $"Failed to create the FBX scene: {FbxSdk.GetLastError()}" );

            FbxSdk.SceneSetAxisSystem( mScene, FbxAxisSystemPreset.DirectX );
            FbxSdk.SceneSetSystemUnit( mScene, FbxSdk.SystemUnitMetre );

            var textureNameToPath = ExportTextures();
            ConvertMaterials( textureNameToPath );

            // 3ds Max expects a bind pose, under this name.
            mBindPose = FbxSdk.PoseCreate( mScene, "BIND_POSES" );
            if ( mBindPose == IntPtr.Zero )
                throw new FbxSdkModelPackExporterException( "Failed to create the bind pose" );

            FbxSdk.PoseSetIsBindPose( mBindPose, 1 );
            FbxSdk.SceneAddPose( mScene, mBindPose );

            // Allocate every node up front, so references further down the tree resolve.
            for ( int i = 0; i < mModelNodes.Count; i++ )
            {
                var fbxNode = FbxSdk.NodeCreate( mScene, mModelNodes[i].Name );
                mNodeToFbxNode.Add( mModelNodes[i], fbxNode );
                mNodeIndexToFbxNode.Add( i, fbxNode );
            }

            var fbxRootNode = FbxSdk.SceneGetRootNode( mScene );

            // The root node is usually generated and empty, so it is left out of the output. Some
            // models hang a mesh or a camera straight off it, and dropping it loses that.
            if ( mModel.RootNode.HasAttachments )
            {
                ConvertNode( fbxRootNode, mModel.RootNode );
            }
            else
            {
                foreach ( var node in mModel.RootNode.Children )
                    ConvertNode( fbxRootNode, node );
            }
        }

        private Dictionary<string, string> ExportTextures()
        {
            var textureNameToPath = new Dictionary<string, string>();
            if ( mModelPack.Textures == null )
                return textureNameToPath;

            Directory.CreateDirectory( mTextureBaseDirectoryPath );

            foreach ( var texture in mModelPack.Textures )
            {
                // TODO: handle other texture types (TGA, GTF, etc.)
                var texturePath = Path.Combine( mTextureBaseDirectoryPath, texture.Key );
                if ( !Path.GetExtension( texturePath ).Equals( ".dds", StringComparison.OrdinalIgnoreCase ) )
                    texturePath += ".dds";

                File.WriteAllBytes( texturePath, texture.Value.Data );
                textureNameToPath.Add( texture.Key, texturePath );
            }

            return textureNameToPath;
        }

        private void ConvertMaterials( Dictionary<string, string> textureNameToPath )
        {
            if ( mModelPack.Materials == null )
                return;

            foreach ( var material in mModelPack.Materials.Values )
            {
                var fbxMaterial = FbxSdk.SurfacePhongCreate( mScene, material.Name );
                FbxSdk.SurfaceMaterialSetShadingModel( fbxMaterial, "Phong" );

                if ( material.DiffuseMap != null && textureNameToPath.TryGetValue( material.DiffuseMap.Name, out var texturePath ) )
                {
                    if ( !mTexturePathToFbxFileTexture.TryGetValue( texturePath, out var fbxTexture ) )
                    {
                        fbxTexture = FbxSdk.FileTextureCreate( mScene, "Bitmaptexture" );
                        FbxSdk.FileTextureSetFileName( fbxTexture, texturePath );

                        // 3ds Max sets these by default.
                        FbxSdk.FileTextureSetUVSet( fbxTexture, "UVChannel_1" );
                        FbxSdk.FileTextureSetUseMaterial( fbxTexture, 1 );

                        mTexturePathToFbxFileTexture[texturePath] = fbxTexture;
                    }

                    FbxSdk.SurfaceMaterialConnectDiffuseTexture( fbxMaterial, fbxTexture );
                }

                mMaterialNameToFbxSurfaceMaterial[material.Name] = fbxMaterial;
            }
        }

        private void ConvertNode( IntPtr fbxParentNode, Node node )
        {
            var fbxNode = mNodeToFbxNode[node];

            FbxSdk.NodeSetLocalTransform( fbxNode,
                new[] { ( double )node.Translation.X, node.Translation.Y, node.Translation.Z },
                new[] { ( double )node.Rotation.X, node.Rotation.Y, node.Rotation.Z, node.Rotation.W },
                new[] { ( double )node.Scale.X, node.Scale.Y, node.Scale.Z } );

            FbxSdk.NodeAddChild( fbxParentNode, fbxNode );

            // 3ds Max always sets the skeleton type to limb node.
            var fbxSkeleton = FbxSdk.SkeletonCreate( mScene, "" );
            FbxSdk.SkeletonSetSkeletonType( fbxSkeleton, FbxSkeletonType.LimbNode );
            FbxSdk.NodeSetSkeletonAttribute( fbxNode, fbxSkeleton );

            AddToBindPose( fbxNode );

            if ( node.HasAttachments )
                ConvertAttachments( fbxNode, node );

            if ( node.HasProperties )
            {
                // 3ds Max reads its User Defined Properties from a string property by this name.
                FbxSdk.NodeSetUserPropertyString( fbxNode, "UDP3DSMAX", BuildUdp3dsMax( node ) );
            }

            if ( node.HasChildren )
            {
                foreach ( var childNode in node.Children )
                    ConvertNode( fbxNode, childNode );
            }
        }

        private void ConvertAttachments( IntPtr fbxNode, Node node )
        {
            var typeIndices = new int[( int )NodeAttachmentType.Morph + 1];

            foreach ( var attachment in node.Attachments )
            {
                var typeIndex = typeIndices[( int )attachment.Type]++;

                switch ( attachment.Type )
                {
                    case NodeAttachmentType.Mesh:
                        ConvertMesh( fbxNode, node, attachment.GetValue<Mesh>(), typeIndex );
                        break;

                    case NodeAttachmentType.Camera:
                        ConvertCamera( fbxNode, attachment.GetValue<Camera>() );
                        break;

                    case NodeAttachmentType.Light:
                        ConvertLight( fbxNode, attachment.GetValue<Light>() );
                        break;

                    // Particles and morphs have no FBX equivalent worked out yet, so they are
                    // dropped rather than failing the whole export.
                    case NodeAttachmentType.Epl:
                    case NodeAttachmentType.EplLeaf:
                    case NodeAttachmentType.Morph:
                        break;

                    default:
                        throw new FbxSdkModelPackExporterException( $"Exporting a {attachment.Type} attachment is not implemented." );
                }
            }
        }

        private void ConvertMesh( IntPtr fbxParentNode, Node parentNode, Mesh mesh, int typeIndex )
        {
            mNodeIndexToFbxCluster.Clear();

            var isAnimated = mModel.Bones != null && mModel.Bones.Count > 0;

            var fbxMeshNode = FbxSdk.NodeCreate( mScene, ModelConversionHelpers.GetMeshExportName( parentNode.Name, typeIndex ) );

            // Parenting meshes to nodes on animated models leads to weird results.
            FbxSdk.NodeAddChild( isAnimated ? FbxSdk.SceneGetRootNode( mScene ) : fbxParentNode, fbxMeshNode );

            // 3ds Max ignores any node that is not in the bind pose, skinned or not.
            AddToBindPose( fbxMeshNode );

            var fbxMesh = FbxSdk.MeshCreate( mScene, "" );
            FbxSdk.NodeSetMeshAttribute( fbxMeshNode, fbxMesh );

            // Always transform, even without weights, so the parent node matrix is applied.
            var ( vertices, normals ) = mesh.Transform( parentNode, mModelNodes, mModel.Bones, true, true );

            FbxSdk.MeshInitControlPoints( fbxMesh, vertices.Length );
            for ( int i = 0; i < vertices.Length; i++ )
                FbxSdk.MeshSetControlPointAt( fbxMesh, i, vertices[i].X, vertices[i].Y, vertices[i].Z );

            if ( normals != null )
            {
                var fbxElementNormal = FbxSdk.MeshCreateElement( mManager, fbxMesh, FbxLayerElementType.Normal, null );
                FbxSdk.LayerElementSetMappingMode( fbxElementNormal, FbxMappingMode.ByControlPoint );
                FbxSdk.LayerElementSetReferenceMode( fbxElementNormal, FbxReferenceMode.Direct );
                FbxSdk.LayerElementSetDirectCount( fbxElementNormal, vertices.Length );

                for ( int i = 0; i < normals.Length; i++ )
                    FbxSdk.LayerElementSetDirectVector4At( fbxElementNormal, i, normals[i].X, normals[i].Y, normals[i].Z, 0 );
            }

            // Not strictly necessary, but 3ds Max writes one by default.
            var fbxElementMaterial = FbxSdk.MeshCreateElement( mManager, fbxMesh, FbxLayerElementType.Material, null );
            FbxSdk.LayerElementSetMappingMode( fbxElementMaterial, FbxMappingMode.ByPolygon );
            FbxSdk.LayerElementSetReferenceMode( fbxElementMaterial, FbxReferenceMode.IndexToDirect );

            ConvertVertexColorChannel( fbxMesh, mesh.ColorChannel0 );
            ConvertVertexColorChannel( fbxMesh, mesh.ColorChannel1 );

            ConvertTexCoordChannel( fbxMesh, mesh.TexCoordsChannel0, "UVChannel_1", 0 );
            ConvertTexCoordChannel( fbxMesh, mesh.TexCoordsChannel1, "UVChannel_2", 1 );
            ConvertTexCoordChannel( fbxMesh, mesh.TexCoordsChannel2, "UVChannel_3", 2 );

            var fbxSkin = FbxSdk.SkinCreate( mScene, "" );
            FbxSdk.SkinSetSkinningTypeLinear( fbxSkin );
            FbxSdk.MeshAddSkinDeformer( fbxMesh, fbxSkin );

            if ( mesh.VertexWeights != null )
                ConvertVertexWeights( fbxSkin, mesh );
            else if ( isAnimated )
                BindMeshToParentNode( fbxSkin, parentNode, vertices.Length );

            if ( !mMaterialNameToFbxSurfaceMaterial.TryGetValue( mesh.MaterialName ?? string.Empty, out var fbxMaterial ) )
                throw new FbxSdkModelPackExporterException( $"Mesh on node '{parentNode.Name}' references material '{mesh.MaterialName}', which the model pack does not have" );

            var materialIndex = FbxSdk.NodeAddMaterial( fbxMeshNode, fbxMaterial );
            if ( materialIndex < 0 )
                throw new FbxSdkModelPackExporterException( $"Failed to add material '{mesh.MaterialName}' to mesh node '{parentNode.Name}'" );

            for ( int i = 0; i < mesh.Triangles.Length; i++ )
            {
                FbxSdk.MeshBeginPolygon( fbxMesh, materialIndex );
                FbxSdk.MeshAddPolygon( fbxMesh, ( int )mesh.Triangles[i].A );
                FbxSdk.MeshAddPolygon( fbxMesh, ( int )mesh.Triangles[i].B );
                FbxSdk.MeshAddPolygon( fbxMesh, ( int )mesh.Triangles[i].C );
                FbxSdk.MeshEndPolygon( fbxMesh );
            }
        }

        private void ConvertVertexWeights( IntPtr fbxSkin, Mesh mesh )
        {
            for ( int i = 0; i < mesh.VertexWeights.Length; i++ )
            {
                var vertexWeights = mesh.VertexWeights[i];

                for ( int j = 0; j < vertexWeights.Weights.Length; j++ )
                {
                    var weight = vertexWeights.Weights[j];
                    if ( weight < 0.001f )
                        continue;

                    var nodeIndex = mModel.Bones[vertexWeights.Indices[j]].NodeIndex;
                    var fbxCluster = GetOrCreateCluster( fbxSkin, nodeIndex, mModelNodes[nodeIndex].WorldTransform );
                    FbxSdk.ClusterAddControlPointIndex( fbxCluster, i, weight );
                }
            }
        }

        /// <summary>
        /// An unweighted mesh on an animated model is parented to the scene root, so it needs a
        /// skin binding every vertex to its real parent to keep following that node.
        /// </summary>
        private void BindMeshToParentNode( IntPtr fbxSkin, Node parentNode, int vertexCount )
        {
            var parentNodeIndex = mModelNodes.IndexOf( parentNode );
            var fbxCluster = GetOrCreateCluster( fbxSkin, parentNodeIndex, parentNode.WorldTransform );

            for ( int i = 0; i < vertexCount; i++ )
                FbxSdk.ClusterAddControlPointIndex( fbxCluster, i, 1 );
        }

        private IntPtr GetOrCreateCluster( IntPtr fbxSkin, int nodeIndex, Matrix4x4 worldTransform )
        {
            if ( mNodeIndexToFbxCluster.TryGetValue( nodeIndex, out var fbxCluster ) )
                return fbxCluster;

            fbxCluster = FbxSdk.ClusterCreate( mScene, "" );
            FbxSdk.ClusterSetLink( fbxCluster, mNodeIndexToFbxNode[nodeIndex] );
            FbxSdk.ClusterSetLinkMode( fbxCluster, FbxClusterLinkMode.Normalize );

            // The node's own world transform, not EvaluateGlobalTransform, which is known to
            // return the wrong matrix now and then.
            FbxSdk.ClusterSetTransformLinkMatrix( fbxCluster, ToRowMajor( worldTransform ) );

            FbxSdk.SkinAddCluster( fbxSkin, fbxCluster );
            mNodeIndexToFbxCluster[nodeIndex] = fbxCluster;
            return fbxCluster;
        }

        private void ConvertVertexColorChannel( IntPtr fbxMesh, Graphics.Color[] colors )
        {
            if ( colors == null )
                return;

            var controlPointCount = FbxSdk.MeshGetControlPointsCount( fbxMesh );
            var fbxElement = FbxSdk.MeshCreateElement( mManager, fbxMesh, FbxLayerElementType.VertexColor, null );
            FbxSdk.LayerElementSetMappingMode( fbxElement, FbxMappingMode.ByControlPoint );
            FbxSdk.LayerElementSetReferenceMode( fbxElement, FbxReferenceMode.Direct );
            FbxSdk.LayerElementSetDirectCount( fbxElement, controlPointCount );

            // The channel can be shorter than the control point count, and the remainder has to
            // stay white rather than black.
            for ( int i = 0; i < controlPointCount; i++ )
                FbxSdk.LayerElementSetDirectColorAt( fbxElement, i, 1, 1, 1, 1 );

            for ( int i = 0; i < colors.Length; i++ )
                FbxSdk.LayerElementSetDirectColorAt( fbxElement, i,
                    colors[i].R / 255.0, colors[i].G / 255.0, colors[i].B / 255.0, colors[i].A / 255.0 );
        }

        private void ConvertTexCoordChannel( IntPtr fbxMesh, Vector2[] texCoords, string name, int layer )
        {
            if ( texCoords == null )
                return;

            // 3ds Max requires every UV channel to be on its own layer, as these map to map channels.
            var fbxElement = FbxSdk.MeshCreateLayerElement( mManager, fbxMesh, layer, FbxLayerElementType.UV, name );
            FbxSdk.LayerElementSetMappingMode( fbxElement, FbxMappingMode.ByControlPoint );
            FbxSdk.LayerElementSetReferenceMode( fbxElement, FbxReferenceMode.Direct );
            FbxSdk.LayerElementSetDirectCount( fbxElement, FbxSdk.MeshGetControlPointsCount( fbxMesh ) );

            for ( int i = 0; i < texCoords.Length; i++ )
                FbxSdk.LayerElementSetDirectVector2At( fbxElement, i, texCoords[i].X, 1.0f - texCoords[i].Y );
        }

        // TODO test & implement properly
        private void ConvertCamera( IntPtr fbxParentNode, Camera camera )
        {
            var fbxCamera = FbxSdk.CameraCreate( mScene, "" );
            FbxSdk.NodeSetCameraAttribute( fbxParentNode, fbxCamera );

            FbxSdk.CameraSetPosition( fbxCamera, camera.Position.X, camera.Position.Y, camera.Position.Z );
            FbxSdk.CameraSetUpVector( fbxCamera, camera.Up.X, camera.Up.Y, camera.Up.Z );

            var target = camera.Position + camera.Direction;
            FbxSdk.CameraSetInterestPosition( fbxCamera, target.X, target.Y, target.Z );

            FbxSdk.CameraSetFieldOfView( fbxCamera, camera.FieldOfView );
            FbxSdk.CameraSetNearFarPlane( fbxCamera, camera.ClipPlaneNear, camera.ClipPlaneFar );
            FbxSdk.CameraSetAspect( fbxCamera, camera.AspectRatio, 1.0 );
        }

        // TODO test & implement properly
        private void ConvertLight( IntPtr fbxParentNode, Light light )
        {
            var fbxLight = FbxSdk.LightCreate( mScene, "" );
            FbxSdk.NodeSetLightAttribute( fbxParentNode, fbxLight );

            FbxSdk.LightSetType( fbxLight, light.Type switch
            {
                LightType.Spot => FbxLightType.Spot,
                LightType.Type1 => FbxLightType.Directional,
                _ => FbxLightType.Point,
            } );

            FbxSdk.LightSetColor( fbxLight, light.DiffuseColor.X, light.DiffuseColor.Y, light.DiffuseColor.Z );
            FbxSdk.LightSetFarAttenuation( fbxLight, 1, light.AttenuationStart, light.AttenuationEnd );

            if ( light.Type == LightType.Spot )
                FbxSdk.LightSetConeAngles( fbxLight, light.AngleInnerCone, light.AngleOuterCone );
        }

        private void AddToBindPose( IntPtr fbxNode )
        {
            var matrix = new double[16];
            FbxSdk.NodeEvaluateGlobalTransform( fbxNode, matrix );
            FbxSdk.PoseAdd( mBindPose, fbxNode, matrix );
        }

        /// <summary>
        /// Animations go in as a take each, keyed against the nodes already in the scene.
        /// </summary>
        private void ExportAnimations( AnimationPack pack )
        {
            if ( pack == null || pack.Animations == null || pack.Animations.Count == 0 )
                return;

            var byName = new Dictionary<string, IntPtr>();

            foreach ( var pair in mNodeToFbxNode )
            {
                if ( pair.Key.Name != null )
                    byName[pair.Key.Name] = pair.Value;
            }

            new FbxSdkAnimationExporter( mScene, byName ).Export( pack );
        }

        private void ExportFbxScene()
        {
            var binary = !mPath.EndsWith( ".ascii.fbx", StringComparison.OrdinalIgnoreCase );
            if ( FbxSdk.SceneExport( mManager, mScene, mPath, binary ? 1 : 0 ) != 0 )
                throw new FbxSdkModelPackExporterException( $"Failed to export the FBX scene: {FbxSdk.GetLastError()}" );
        }

        /// <summary>
        /// Formats the node's user properties the way 3ds Max parses them back out of UDP3DSMAX.
        /// </summary>
        private static string BuildUdp3dsMax( Node node )
        {
            // Invariant culture, because a locale that writes 1,5 for 1.5 produces a line 3ds Max
            // cannot parse.
            var builder = new StringBuilder();

            foreach ( var property in node.Properties )
            {
                var value = property.Value switch
                {
                    UserIntProperty p => $"int({p.Value.ToString( CultureInfo.InvariantCulture )})",
                    UserFloatProperty p => $"float({p.Value.ToString( CultureInfo.InvariantCulture )})",
                    UserBoolProperty p => $"bool({( p.Value ? "true" : "false" )})",
                    UserStringProperty p => $"string({p.Value})",
                    UserByteVector3Property p => $"bytevector3({p.Value.X},{p.Value.Y},{p.Value.Z})",
                    UserByteVector4Property p => $"bytevector4({p.Value.X},{p.Value.Y},{p.Value.Z},{p.Value.W})",
                    UserVector3Property p => $"vector3({Format( p.Value.X )},{Format( p.Value.Y )},{Format( p.Value.Z )})",
                    UserVector4Property p => $"vector4({Format( p.Value.X )},{Format( p.Value.Y )},{Format( p.Value.Z )},{Format( p.Value.W )})",
                    UserByteArrayProperty p => $"bytearray({Convert.ToBase64String( p.Value )})",
                    _ => throw new NotSupportedException( $"Unsupported user property type: {property.Value.ValueType}" ),
                };

                builder.Append( property.Key ).Append( '=' ).Append( value ).Append( "\r\n" );
            }

            return builder.ToString();
        }

        private static string Format( float value ) => value.ToString( CultureInfo.InvariantCulture );

        /// <summary>
        /// Flattens to the 16 row major doubles the native layer expects, so the translation ends
        /// up at indices 12, 13 and 14.
        /// </summary>
        private static double[] ToRowMajor( Matrix4x4 m ) => new double[]
        {
            m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44,
        };
    }
}
