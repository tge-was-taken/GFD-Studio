using System;
using System.Runtime.InteropServices;

namespace GFDLibrary.Conversion.FbxSdk
{
    internal enum FbxMappingMode
    {
        None = 0,
        ByControlPoint = 1,
        ByPolygonVertex = 2,
        ByPolygon = 3,
        ByEdge = 4,
        AllSame = 5,
    }

    internal enum FbxReferenceMode
    {
        Direct = 0,
        Index = 1,
        IndexToDirect = 2,
    }

    internal enum FbxTransformChannel
    {
        Translation = 0,
        Rotation = 1,
        Scaling = 2,
    }

    internal enum FbxLayerElementType
    {
        Normal = 0,
        UV = 1,
        VertexColor = 2,
        Material = 3,
    }

    internal enum FbxSkeletonType
    {
        Root = 0,
        Limb = 1,
        LimbNode = 2,
        Effector = 3,
    }

    internal enum FbxClusterLinkMode
    {
        Normalize = 0,
        Additive = 1,
        TotalOne = 2,
    }

    internal enum FbxLightType
    {
        Point = 0,
        Directional = 1,
        Spot = 2,
        Area = 3,
        Volume = 4,
    }

    internal enum FbxAxisSystemPreset
    {
        MayaZUp = 0,
        MayaYUp = 1,
        Max = 2,
        MotionBuilder = 3,
        OpenGL = 4,
        DirectX = 5,
        Lightwave = 6,
    }

    /// <summary>
    /// The Autodesk FBX SDK, reached through the FbxSdkC shared library. Mirrors the SDK's own
    /// classes and methods; see GFDLibrary.Native.FbxSdk in the repository root for the native
    /// side and how to build it.
    /// </summary>
    internal static class FbxSdk
    {
        private const string Library = "FbxSdkC";

        /// <summary>
        /// The centimetres per unit of FbxSystemUnit::m.
        /// </summary>
        public const double SystemUnitMetre = 100.0;

        [DllImport( Library, EntryPoint = "FbxManager_Create" )]
        public static extern IntPtr ManagerCreate();

        [DllImport( Library, EntryPoint = "FbxManager_Destroy" )]
        public static extern void ManagerDestroy( IntPtr manager );

        [DllImport( Library, EntryPoint = "FbxScene_Create" )]
        public static extern IntPtr SceneCreate( IntPtr manager, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxScene_GetRootNode" )]
        public static extern IntPtr SceneGetRootNode( IntPtr scene );

        [DllImport( Library, EntryPoint = "FbxScene_SetAxisSystem" )]
        public static extern void SceneSetAxisSystem( IntPtr scene, FbxAxisSystemPreset preset );

        [DllImport( Library, EntryPoint = "FbxScene_SetSystemUnit" )]
        public static extern void SceneSetSystemUnit( IntPtr scene, double scaleFactor );

        [DllImport( Library, EntryPoint = "FbxScene_AddPose" )]
        public static extern void SceneAddPose( IntPtr scene, IntPtr pose );

        [DllImport( Library, EntryPoint = "FbxScene_Export" )]
        public static extern int SceneExport( IntPtr manager, IntPtr scene,
                                              [MarshalAs( UnmanagedType.LPUTF8Str )] string path,
                                              int fileFormatBinary );

        [DllImport( Library, EntryPoint = "FbxNode_Create" )]
        public static extern IntPtr NodeCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxNode_AddChild" )]
        public static extern void NodeAddChild( IntPtr parent, IntPtr child );

        [DllImport( Library, EntryPoint = "FbxNode_SetLocalTransform" )]
        public static extern void NodeSetLocalTransform( IntPtr node, double[] translation,
                                                         double[] rotationQuaternion, double[] scale );

        [DllImport( Library, EntryPoint = "FbxNode_EvaluateGlobalTransform" )]
        public static extern void NodeEvaluateGlobalTransform( IntPtr node, double[] matrix16 );

        [DllImport( Library, EntryPoint = "FbxNode_SetMeshAttribute" )]
        public static extern void NodeSetMeshAttribute( IntPtr node, IntPtr mesh );

        [DllImport( Library, EntryPoint = "FbxNode_SetSkeletonAttribute" )]
        public static extern void NodeSetSkeletonAttribute( IntPtr node, IntPtr skeleton );

        [DllImport( Library, EntryPoint = "FbxNode_SetCameraAttribute" )]
        public static extern void NodeSetCameraAttribute( IntPtr node, IntPtr camera );

        [DllImport( Library, EntryPoint = "FbxNode_SetLightAttribute" )]
        public static extern void NodeSetLightAttribute( IntPtr node, IntPtr light );

        [DllImport( Library, EntryPoint = "FbxNode_AddMaterial" )]
        public static extern int NodeAddMaterial( IntPtr node, IntPtr material );

        [DllImport( Library, EntryPoint = "FbxNode_SetUserPropertyString" )]
        public static extern void NodeSetUserPropertyString( IntPtr node,
                                                             [MarshalAs( UnmanagedType.LPUTF8Str )] string name,
                                                             [MarshalAs( UnmanagedType.LPUTF8Str )] string value );

        [DllImport( Library, EntryPoint = "FbxSkeleton_Create" )]
        public static extern IntPtr SkeletonCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxSkeleton_SetSkeletonType" )]
        public static extern void SkeletonSetSkeletonType( IntPtr skeleton, FbxSkeletonType type );

        [DllImport( Library, EntryPoint = "FbxMesh_Create" )]
        public static extern IntPtr MeshCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxMesh_InitControlPoints" )]
        public static extern void MeshInitControlPoints( IntPtr mesh, int count );

        [DllImport( Library, EntryPoint = "FbxMesh_GetControlPointsCount" )]
        public static extern int MeshGetControlPointsCount( IntPtr mesh );

        [DllImport( Library, EntryPoint = "FbxMesh_SetControlPointAt" )]
        public static extern void MeshSetControlPointAt( IntPtr mesh, int index, double x, double y, double z );

        [DllImport( Library, EntryPoint = "FbxMesh_BeginPolygon" )]
        public static extern void MeshBeginPolygon( IntPtr mesh, int material );

        [DllImport( Library, EntryPoint = "FbxMesh_AddPolygon" )]
        public static extern void MeshAddPolygon( IntPtr mesh, int index );

        [DllImport( Library, EntryPoint = "FbxMesh_EndPolygon" )]
        public static extern void MeshEndPolygon( IntPtr mesh );

        [DllImport( Library, EntryPoint = "FbxMesh_CreateElement" )]
        public static extern IntPtr MeshCreateElement( IntPtr manager, IntPtr mesh, FbxLayerElementType type,
                                                       [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxMesh_CreateLayerElement" )]
        public static extern IntPtr MeshCreateLayerElement( IntPtr manager, IntPtr mesh, int layer,
                                                            FbxLayerElementType type,
                                                            [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetMappingMode" )]
        public static extern void LayerElementSetMappingMode( IntPtr element, FbxMappingMode mode );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetReferenceMode" )]
        public static extern void LayerElementSetReferenceMode( IntPtr element, FbxReferenceMode mode );

        [DllImport( Library, EntryPoint = "FbxLayerElement_AddIndex" )]
        public static extern void LayerElementAddIndex( IntPtr element, int index );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetDirectCount" )]
        public static extern void LayerElementSetDirectCount( IntPtr element, int count );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetDirectVector2At" )]
        public static extern void LayerElementSetDirectVector2At( IntPtr element, int index, double x, double y );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetDirectVector4At" )]
        public static extern void LayerElementSetDirectVector4At( IntPtr element, int index,
                                                                  double x, double y, double z, double w );

        [DllImport( Library, EntryPoint = "FbxLayerElement_SetDirectColorAt" )]
        public static extern void LayerElementSetDirectColorAt( IntPtr element, int index,
                                                                double r, double g, double b, double a );

        [DllImport( Library, EntryPoint = "FbxSkin_Create" )]
        public static extern IntPtr SkinCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxSkin_SetSkinningTypeLinear" )]
        public static extern void SkinSetSkinningTypeLinear( IntPtr skin );

        [DllImport( Library, EntryPoint = "FbxSkin_AddCluster" )]
        public static extern void SkinAddCluster( IntPtr skin, IntPtr cluster );

        [DllImport( Library, EntryPoint = "FbxMesh_AddSkinDeformer" )]
        public static extern void MeshAddSkinDeformer( IntPtr mesh, IntPtr skin );

        [DllImport( Library, EntryPoint = "FbxCluster_Create" )]
        public static extern IntPtr ClusterCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxCluster_SetLink" )]
        public static extern void ClusterSetLink( IntPtr cluster, IntPtr node );

        [DllImport( Library, EntryPoint = "FbxCluster_SetLinkMode" )]
        public static extern void ClusterSetLinkMode( IntPtr cluster, FbxClusterLinkMode mode );

        [DllImport( Library, EntryPoint = "FbxCluster_SetTransformLinkMatrix" )]
        public static extern void ClusterSetTransformLinkMatrix( IntPtr cluster, double[] matrix16 );

        [DllImport( Library, EntryPoint = "FbxCluster_AddControlPointIndex" )]
        public static extern void ClusterAddControlPointIndex( IntPtr cluster, int index, double weight );

        [DllImport( Library, EntryPoint = "FbxPose_Create" )]
        public static extern IntPtr PoseCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxPose_SetIsBindPose" )]
        public static extern void PoseSetIsBindPose( IntPtr pose, int isBindPose );

        [DllImport( Library, EntryPoint = "FbxPose_Add" )]
        public static extern void PoseAdd( IntPtr pose, IntPtr node, double[] matrix16 );

        [DllImport( Library, EntryPoint = "FbxSurfacePhong_Create" )]
        public static extern IntPtr SurfacePhongCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxSurfaceMaterial_SetShadingModel" )]
        public static extern void SurfaceMaterialSetShadingModel( IntPtr material,
                                                                  [MarshalAs( UnmanagedType.LPUTF8Str )] string model );

        [DllImport( Library, EntryPoint = "FbxSurfaceMaterial_ConnectDiffuseTexture" )]
        public static extern void SurfaceMaterialConnectDiffuseTexture( IntPtr material, IntPtr texture );

        [DllImport( Library, EntryPoint = "FbxFileTexture_Create" )]
        public static extern IntPtr FileTextureCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxFileTexture_SetFileName" )]
        public static extern void FileTextureSetFileName( IntPtr texture,
                                                          [MarshalAs( UnmanagedType.LPUTF8Str )] string path );

        [DllImport( Library, EntryPoint = "FbxFileTexture_SetUVSet" )]
        public static extern void FileTextureSetUVSet( IntPtr texture,
                                                       [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxFileTexture_SetUseMaterial" )]
        public static extern void FileTextureSetUseMaterial( IntPtr texture, int useMaterial );

        [DllImport( Library, EntryPoint = "FbxCamera_Create" )]
        public static extern IntPtr CameraCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxCamera_SetPosition" )]
        public static extern void CameraSetPosition( IntPtr camera, double x, double y, double z );

        [DllImport( Library, EntryPoint = "FbxCamera_SetUpVector" )]
        public static extern void CameraSetUpVector( IntPtr camera, double x, double y, double z );

        [DllImport( Library, EntryPoint = "FbxCamera_SetInterestPosition" )]
        public static extern void CameraSetInterestPosition( IntPtr camera, double x, double y, double z );

        [DllImport( Library, EntryPoint = "FbxCamera_SetFieldOfView" )]
        public static extern void CameraSetFieldOfView( IntPtr camera, double degrees );

        [DllImport( Library, EntryPoint = "FbxCamera_SetNearFarPlane" )]
        public static extern void CameraSetNearFarPlane( IntPtr camera, double nearPlane, double farPlane );

        [DllImport( Library, EntryPoint = "FbxCamera_SetAspect" )]
        public static extern void CameraSetAspect( IntPtr camera, double width, double height );

        [DllImport( Library, EntryPoint = "FbxLight_Create" )]
        public static extern IntPtr LightCreate( IntPtr scene, [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

        [DllImport( Library, EntryPoint = "FbxLight_SetType" )]
        public static extern void LightSetType( IntPtr light, FbxLightType type );

        [DllImport( Library, EntryPoint = "FbxLight_SetColor" )]
        public static extern void LightSetColor( IntPtr light, double r, double g, double b );

        [DllImport( Library, EntryPoint = "FbxLight_SetFarAttenuation" )]
        public static extern void LightSetFarAttenuation( IntPtr light, int enable, double start, double end );

        [DllImport( Library, EntryPoint = "FbxLight_SetConeAngles" )]
        public static extern void LightSetConeAngles( IntPtr light, double innerDegrees, double outerDegrees );

        [DllImport( Library, EntryPoint = "FbxAnimStack_Create" )]
        public static extern IntPtr AnimStackCreate( IntPtr scene, string name );

        [DllImport( Library, EntryPoint = "FbxAnimStack_AddLayer" )]
        public static extern void AnimStackAddLayer( IntPtr stack, IntPtr layer );

        [DllImport( Library, EntryPoint = "FbxAnimStack_SetLocalTimeSpan" )]
        public static extern void AnimStackSetLocalTimeSpan( IntPtr stack, double start, double stop );

        [DllImport( Library, EntryPoint = "FbxAnimLayer_Create" )]
        public static extern IntPtr AnimLayerCreate( IntPtr scene, string name );

        [DllImport( Library, EntryPoint = "FbxNode_GetAnimCurve" )]
        public static extern IntPtr NodeGetAnimCurve( IntPtr node, IntPtr layer, FbxTransformChannel channel, int component );

        [DllImport( Library, EntryPoint = "FbxQuaternionToEulerDegrees" )]
        public static extern void QuaternionToEulerDegrees( double[] quaternion, double[] euler );

        [DllImport( Library, EntryPoint = "FbxAnimCurve_KeyModifyBegin" )]
        public static extern void AnimCurveKeyModifyBegin( IntPtr curve );

        [DllImport( Library, EntryPoint = "FbxAnimCurve_KeyModifyEnd" )]
        public static extern void AnimCurveKeyModifyEnd( IntPtr curve );

        [DllImport( Library, EntryPoint = "FbxAnimCurve_KeyAddLinear" )]
        public static extern int AnimCurveKeyAddLinear( IntPtr curve, double seconds, float value );

        [DllImport( Library, EntryPoint = "FbxGetLastError" )]
        private static extern IntPtr GetLastErrorPtr();

        public static string GetLastError()
            => Marshal.PtrToStringUTF8( GetLastErrorPtr() ) ?? string.Empty;
    }
}
