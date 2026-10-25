// C surface over the Autodesk FBX SDK. Names follow the SDK's own classes and methods,
// flattened for P/Invoke: every object is an opaque handle and strings are UTF-8.
//
// Only the parts the model exporter uses are here. The exporter logic itself lives in C#.

#ifndef FBXSDK_C_H
#define FBXSDK_C_H

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32)
  #define FBXSDK_C_API __declspec(dllexport)
#else
  #define FBXSDK_C_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/// Matrices are 16 doubles in row major order, so the translation sits in the last row at
/// indices 12, 13 and 14, matching FbxAMatrix.
typedef struct FbxManagerC FbxManagerC;
typedef struct FbxSceneC FbxSceneC;
typedef struct FbxNodeC FbxNodeC;
typedef struct FbxMeshC FbxMeshC;
typedef struct FbxSkeletonC FbxSkeletonC;
typedef struct FbxCameraC FbxCameraC;
typedef struct FbxLightC FbxLightC;
typedef struct FbxSkinC FbxSkinC;
typedef struct FbxClusterC FbxClusterC;
typedef struct FbxPoseC FbxPoseC;
typedef struct FbxSurfaceMaterialC FbxSurfaceMaterialC;
typedef struct FbxFileTextureC FbxFileTextureC;
typedef struct FbxLayerElementC FbxLayerElementC;
typedef struct FbxBlendShapeC FbxBlendShapeC;
typedef struct FbxBlendShapeChannelC FbxBlendShapeChannelC;
typedef struct FbxShapeC FbxShapeC;

/// FbxLayerElement::EMappingMode
enum FbxMappingMode
{
    FbxMappingModeNone = 0,
    FbxMappingModeByControlPoint = 1,
    FbxMappingModeByPolygonVertex = 2,
    FbxMappingModeByPolygon = 3,
    FbxMappingModeByEdge = 4,
    FbxMappingModeAllSame = 5,
};

/// FbxLayerElement::EReferenceMode
enum FbxReferenceMode
{
    FbxReferenceModeDirect = 0,
    FbxReferenceModeIndex = 1,
    FbxReferenceModeIndexToDirect = 2,
};

/// FbxLayerElement::EType, limited to what the exporter creates
enum FbxLayerElementType
{
    FbxLayerElementTypeNormal = 0,
    FbxLayerElementTypeUV = 1,
    FbxLayerElementTypeVertexColor = 2,
    FbxLayerElementTypeMaterial = 3,
};

/// FbxSkeleton::EType
enum FbxSkeletonType
{
    FbxSkeletonTypeRoot = 0,
    FbxSkeletonTypeLimb = 1,
    FbxSkeletonTypeLimbNode = 2,
    FbxSkeletonTypeEffector = 3,
};

/// FbxCluster::ELinkMode
enum FbxClusterLinkMode
{
    FbxClusterLinkModeNormalize = 0,
    FbxClusterLinkModeAdditive = 1,
    FbxClusterLinkModeTotalOne = 2,
};

/// FbxLight::EType
enum FbxLightType
{
    FbxLightTypePoint = 0,
    FbxLightTypeDirectional = 1,
    FbxLightTypeSpot = 2,
    FbxLightTypeArea = 3,
    FbxLightTypeVolume = 4,
};

// Manager and scene
FBXSDK_C_API FbxManagerC* FbxManager_Create( void );
FBXSDK_C_API void         FbxManager_Destroy( FbxManagerC* manager );
FBXSDK_C_API FbxSceneC*   FbxScene_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API FbxNodeC*    FbxScene_GetRootNode( FbxSceneC* scene );
/// Sets the axis system and unit scale of the scene's global settings.
FBXSDK_C_API void         FbxScene_SetAxisSystem( FbxSceneC* scene, int upVector, int frontVector, int coordSystem );
FBXSDK_C_API void         FbxScene_SetSystemUnit( FbxSceneC* scene, double scaleFactor );

// Nodes
FBXSDK_C_API FbxNodeC* FbxNode_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void      FbxNode_AddChild( FbxNodeC* parent, FbxNodeC* child );
FBXSDK_C_API void      FbxNode_SetLocalTransform( FbxNodeC* node, const double* translation,
                                                  const double* rotationDegrees, const double* scale );
FBXSDK_C_API void      FbxNode_SetPreferedAngle( FbxNodeC* node, const double* anglesDegrees );
FBXSDK_C_API void      FbxNode_EvaluateGlobalTransform( FbxNodeC* node, double* matrix16 );
FBXSDK_C_API void      FbxNode_SetMeshAttribute( FbxNodeC* node, FbxMeshC* mesh );
FBXSDK_C_API void      FbxNode_SetSkeletonAttribute( FbxNodeC* node, FbxSkeletonC* skeleton );
FBXSDK_C_API void      FbxNode_SetCameraAttribute( FbxNodeC* node, FbxCameraC* camera );
FBXSDK_C_API void      FbxNode_SetLightAttribute( FbxNodeC* node, FbxLightC* light );
FBXSDK_C_API int       FbxNode_AddMaterial( FbxNodeC* node, FbxSurfaceMaterialC* material );

// Custom properties, used to round trip the fields the format has and FBX does not
FBXSDK_C_API void FbxNode_SetUserPropertyBool( FbxNodeC* node, const char* name, int value );
FBXSDK_C_API void FbxNode_SetUserPropertyInt( FbxNodeC* node, const char* name, int value );
FBXSDK_C_API void FbxNode_SetUserPropertyDouble( FbxNodeC* node, const char* name, double value );
FBXSDK_C_API void FbxNode_SetUserPropertyString( FbxNodeC* node, const char* name, const char* value );
FBXSDK_C_API void FbxNode_SetUserPropertyDouble3( FbxNodeC* node, const char* name, const double* values );
FBXSDK_C_API void FbxNode_SetUserPropertyDouble4( FbxNodeC* node, const char* name, const double* values );

// Meshes
FBXSDK_C_API FbxMeshC* FbxMesh_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void      FbxMesh_InitControlPoints( FbxMeshC* mesh, int count );
FBXSDK_C_API int       FbxMesh_GetControlPointsCount( FbxMeshC* mesh );
FBXSDK_C_API void      FbxMesh_SetControlPointAt( FbxMeshC* mesh, int index, double x, double y, double z );
FBXSDK_C_API void      FbxMesh_BeginPolygon( FbxMeshC* mesh, int material );
FBXSDK_C_API void      FbxMesh_AddPolygon( FbxMeshC* mesh, int index );
FBXSDK_C_API void      FbxMesh_EndPolygon( FbxMeshC* mesh );
FBXSDK_C_API int       FbxMesh_CreateLayer( FbxMeshC* mesh );

/// Creates a layer element on the given layer, in the order the layer already has. The exporter
/// relies on that order, because 3ds Max maps layers to map channels positionally.
FBXSDK_C_API FbxLayerElementC* FbxMesh_CreateLayerElement( FbxManagerC* manager, FbxMeshC* mesh,
                                                           int layer, int type, const char* name );
FBXSDK_C_API void FbxLayerElement_SetMappingMode( FbxLayerElementC* element, int mode );
FBXSDK_C_API void FbxLayerElement_SetReferenceMode( FbxLayerElementC* element, int mode );
FBXSDK_C_API void FbxLayerElement_AddDirectVector2( FbxLayerElementC* element, double x, double y );
FBXSDK_C_API void FbxLayerElement_AddDirectVector4( FbxLayerElementC* element, double x, double y, double z, double w );
FBXSDK_C_API void FbxLayerElement_AddDirectColor( FbxLayerElementC* element, double r, double g, double b, double a );
FBXSDK_C_API void FbxLayerElement_AddIndex( FbxLayerElementC* element, int index );

/// The exporter sizes a direct array up front and fills it by index, because some channels are
/// shorter than the control point count and the rest has to keep a default.
FBXSDK_C_API void FbxLayerElement_SetDirectCount( FbxLayerElementC* element, int count );
FBXSDK_C_API void FbxLayerElement_SetDirectVector2At( FbxLayerElementC* element, int index, double x, double y );
FBXSDK_C_API void FbxLayerElement_SetDirectVector4At( FbxLayerElementC* element, int index,
                                                      double x, double y, double z, double w );
FBXSDK_C_API void FbxLayerElement_SetDirectColorAt( FbxLayerElementC* element, int index,
                                                    double r, double g, double b, double a );

// Skinning
FBXSDK_C_API FbxSkinC*    FbxSkin_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void         FbxSkin_SetSkinningTypeLinear( FbxSkinC* skin );
FBXSDK_C_API void         FbxSkin_AddCluster( FbxSkinC* skin, FbxClusterC* cluster );
FBXSDK_C_API void         FbxMesh_AddSkinDeformer( FbxMeshC* mesh, FbxSkinC* skin );
FBXSDK_C_API FbxClusterC* FbxCluster_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void         FbxCluster_SetLink( FbxClusterC* cluster, FbxNodeC* node );
FBXSDK_C_API void         FbxCluster_SetLinkMode( FbxClusterC* cluster, int mode );
FBXSDK_C_API void         FbxCluster_SetTransformLinkMatrix( FbxClusterC* cluster, const double* matrix16 );
FBXSDK_C_API void         FbxCluster_AddControlPointIndex( FbxClusterC* cluster, int index, double weight );

// Bind pose
FBXSDK_C_API FbxPoseC* FbxPose_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void      FbxPose_SetIsBindPose( FbxPoseC* pose, int isBindPose );
FBXSDK_C_API void      FbxPose_Add( FbxPoseC* pose, FbxNodeC* node, const double* matrix16 );
FBXSDK_C_API void      FbxScene_AddPose( FbxSceneC* scene, FbxPoseC* pose );

// Blend shapes
FBXSDK_C_API FbxBlendShapeC*        FbxBlendShape_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API FbxBlendShapeChannelC* FbxBlendShapeChannel_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API FbxShapeC*             FbxShape_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void FbxMesh_AddBlendShapeDeformer( FbxMeshC* mesh, FbxBlendShapeC* blendShape );
FBXSDK_C_API void FbxBlendShape_AddChannel( FbxBlendShapeC* blendShape, FbxBlendShapeChannelC* channel );
FBXSDK_C_API void FbxBlendShapeChannel_AddTargetShape( FbxBlendShapeChannelC* channel, FbxShapeC* shape, double fullWeight );
FBXSDK_C_API void FbxShape_InitFrom( FbxShapeC* shape, FbxMeshC* mesh );
FBXSDK_C_API void FbxShape_SetControlPointAt( FbxShapeC* shape, int index, double x, double y, double z );
FBXSDK_C_API void FbxShape_SetNormalAt( FbxShapeC* shape, int index, double x, double y, double z );

// Materials and textures
FBXSDK_C_API FbxSurfaceMaterialC* FbxSurfacePhong_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API FbxFileTextureC*     FbxFileTexture_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void FbxFileTexture_SetFileName( FbxFileTextureC* texture, const char* path );
FBXSDK_C_API void FbxSurfaceMaterial_ConnectDiffuseTexture( FbxSurfaceMaterialC* material, FbxFileTextureC* texture );
FBXSDK_C_API void FbxSurfaceMaterial_SetUserPropertyString( FbxSurfaceMaterialC* material, const char* name, const char* value );
FBXSDK_C_API void FbxSurfaceMaterial_SetUserPropertyInt( FbxSurfaceMaterialC* material, const char* name, int value );
FBXSDK_C_API void FbxSurfaceMaterial_SetUserPropertyBool( FbxSurfaceMaterialC* material, const char* name, int value );
FBXSDK_C_API void FbxSurfaceMaterial_SetUserPropertyDouble( FbxSurfaceMaterialC* material, const char* name, double value );

// Cameras and lights
FBXSDK_C_API FbxCameraC* FbxCamera_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void        FbxCamera_SetAspect( FbxCameraC* camera, double width, double height );
FBXSDK_C_API void        FbxCamera_SetFieldOfView( FbxCameraC* camera, double degrees );
FBXSDK_C_API void        FbxCamera_SetNearFarPlane( FbxCameraC* camera, double nearPlane, double farPlane );
FBXSDK_C_API FbxLightC*  FbxLight_Create( FbxManagerC* manager, const char* name );
FBXSDK_C_API void        FbxLight_SetType( FbxLightC* light, int type );
FBXSDK_C_API void        FbxLight_SetColor( FbxLightC* light, double r, double g, double b );

/// Writes the scene. Returns 0 on success; the message is available from FbxGetLastError.
FBXSDK_C_API int FbxScene_Export( FbxManagerC* manager, FbxSceneC* scene, const char* path, int fileFormatBinary );

/// The message from the most recent failure on this thread.
FBXSDK_C_API const char* FbxGetLastError( void );

#ifdef __cplusplus
}
#endif

#endif
