#include "FbxSdkC.h"

#include <memory>
#include <string>
#include <vector>

#include <fbxsdk.h>

namespace
{
    thread_local std::string gLastError;

    void SetLastError( const char* message )
    {
        gLastError = message ? message : "";
    }

    /// A layer element plus the type it was created as, so the Add calls know which direct
    /// array to reach for.
    struct LayerElementHandle
    {
        int type = 0;
        FbxLayerElement* element = nullptr;
    };

    /// Owns the FbxManager plus the small handles this layer hands out for things the SDK
    /// does not expose as standalone objects. The mesh owns the elements themselves.
    struct ManagerHandle
    {
        FbxManager* manager = nullptr;
        std::vector<std::unique_ptr<LayerElementHandle>> layerElements;
    };

    ManagerHandle* Mgr( FbxManagerC* m ) { return reinterpret_cast<ManagerHandle*>( m ); }
    FbxManager* Sdk( FbxManagerC* m ) { return m ? Mgr( m )->manager : nullptr; }

    template <typename T> T* As( void* p ) { return reinterpret_cast<T*>( p ); }

    FbxAMatrix ToMatrix( const double* m )
    {
        FbxAMatrix matrix;
        for ( int row = 0; row < 4; ++row )
            for ( int col = 0; col < 4; ++col )
                matrix[row][col] = m[( row * 4 ) + col];
        return matrix;
    }

    void FromMatrix( const FbxAMatrix& matrix, double* m )
    {
        for ( int row = 0; row < 4; ++row )
            for ( int col = 0; col < 4; ++col )
                m[( row * 4 ) + col] = matrix[row][col];
    }
}

FbxManagerC* FbxManager_Create( void )
{
    FbxManager* manager = FbxManager::Create();
    if ( !manager )
    {
        SetLastError( "FbxManager::Create failed" );
        return nullptr;
    }

    FbxIOSettings* settings = FbxIOSettings::Create( manager, IOSROOT );
    manager->SetIOSettings( settings );

    auto handle = new ManagerHandle();
    handle->manager = manager;
    return reinterpret_cast<FbxManagerC*>( handle );
}

void FbxManager_Destroy( FbxManagerC* manager )
{
    if ( !manager )
        return;

    auto handle = Mgr( manager );
    if ( handle->manager )
        handle->manager->Destroy();

    delete handle;
}

FbxSceneC* FbxScene_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxSceneC*>( FbxScene::Create( Sdk( manager ), name ) );
}

FbxNodeC* FbxScene_GetRootNode( FbxSceneC* scene )
{
    return reinterpret_cast<FbxNodeC*>( As<FbxScene>( scene )->GetRootNode() );
}

void FbxScene_SetAxisSystem( FbxSceneC* scene, int upVector, int frontVector, int coordSystem )
{
    FbxAxisSystem axisSystem( static_cast<FbxAxisSystem::EUpVector>( upVector ),
                              static_cast<FbxAxisSystem::EFrontVector>( frontVector ),
                              static_cast<FbxAxisSystem::ECoordSystem>( coordSystem ) );
    As<FbxScene>( scene )->GetGlobalSettings().SetAxisSystem( axisSystem );
}

void FbxScene_SetSystemUnit( FbxSceneC* scene, double scaleFactor )
{
    As<FbxScene>( scene )->GetGlobalSettings().SetSystemUnit( FbxSystemUnit( scaleFactor ) );
}

FbxNodeC* FbxNode_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxNodeC*>( FbxNode::Create( Sdk( manager ), name ) );
}

void FbxNode_AddChild( FbxNodeC* parent, FbxNodeC* child )
{
    As<FbxNode>( parent )->AddChild( As<FbxNode>( child ) );
}

void FbxNode_SetLocalTransform( FbxNodeC* node, const double* translation,
                                const double* rotationDegrees, const double* scale )
{
    FbxNode* self = As<FbxNode>( node );
    self->LclTranslation.Set( FbxDouble3( translation[0], translation[1], translation[2] ) );
    self->LclRotation.Set( FbxDouble3( rotationDegrees[0], rotationDegrees[1], rotationDegrees[2] ) );
    self->LclScaling.Set( FbxDouble3( scale[0], scale[1], scale[2] ) );
}

void FbxNode_SetPreferedAngle( FbxNodeC* node, const double* anglesDegrees )
{
    As<FbxNode>( node )->SetPreferedAngle( FbxVector4( anglesDegrees[0], anglesDegrees[1], anglesDegrees[2] ) );
}

void FbxNode_EvaluateGlobalTransform( FbxNodeC* node, double* matrix16 )
{
    FromMatrix( As<FbxNode>( node )->EvaluateGlobalTransform(), matrix16 );
}

void FbxNode_SetMeshAttribute( FbxNodeC* node, FbxMeshC* mesh )
{
    As<FbxNode>( node )->SetNodeAttribute( As<FbxMesh>( mesh ) );
}

void FbxNode_SetSkeletonAttribute( FbxNodeC* node, FbxSkeletonC* skeleton )
{
    As<FbxNode>( node )->SetNodeAttribute( As<FbxSkeleton>( skeleton ) );
}

void FbxNode_SetCameraAttribute( FbxNodeC* node, FbxCameraC* camera )
{
    As<FbxNode>( node )->SetNodeAttribute( As<FbxCamera>( camera ) );
}

void FbxNode_SetLightAttribute( FbxNodeC* node, FbxLightC* light )
{
    As<FbxNode>( node )->SetNodeAttribute( As<FbxLight>( light ) );
}

int FbxNode_AddMaterial( FbxNodeC* node, FbxSurfaceMaterialC* material )
{
    return As<FbxNode>( node )->AddMaterial( As<FbxSurfaceMaterial>( material ) );
}

namespace
{
    template <typename TObject, typename TData, typename TType>
    void SetUserProperty( TObject* object, const char* name, const TType& dataType, const TData& value )
    {
        FbxProperty property = FbxProperty::Create( object, dataType, name );
        property.ModifyFlag( FbxPropertyFlags::eUserDefined, true );
        property.Set( value );
    }
}

void FbxNode_SetUserPropertyBool( FbxNodeC* node, const char* name, int value )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxBoolDT, value != 0 );
}

void FbxNode_SetUserPropertyInt( FbxNodeC* node, const char* name, int value )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxIntDT, value );
}

void FbxNode_SetUserPropertyDouble( FbxNodeC* node, const char* name, double value )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxDoubleDT, value );
}

void FbxNode_SetUserPropertyString( FbxNodeC* node, const char* name, const char* value )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxStringDT, FbxString( value ? value : "" ) );
}

void FbxNode_SetUserPropertyDouble3( FbxNodeC* node, const char* name, const double* values )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxDouble3DT, FbxDouble3( values[0], values[1], values[2] ) );
}

void FbxNode_SetUserPropertyDouble4( FbxNodeC* node, const char* name, const double* values )
{
    SetUserProperty( As<FbxNode>( node ), name, FbxDouble4DT,
                     FbxDouble4( values[0], values[1], values[2], values[3] ) );
}

FbxMeshC* FbxMesh_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxMeshC*>( FbxMesh::Create( Sdk( manager ), name ) );
}

void FbxMesh_InitControlPoints( FbxMeshC* mesh, int count )
{
    As<FbxMesh>( mesh )->InitControlPoints( count );
}

int FbxMesh_GetControlPointsCount( FbxMeshC* mesh )
{
    return As<FbxMesh>( mesh )->GetControlPointsCount();
}

void FbxMesh_SetControlPointAt( FbxMeshC* mesh, int index, double x, double y, double z )
{
    As<FbxMesh>( mesh )->SetControlPointAt( FbxVector4( x, y, z ), index );
}

void FbxMesh_BeginPolygon( FbxMeshC* mesh, int material )
{
    As<FbxMesh>( mesh )->BeginPolygon( material );
}

void FbxMesh_AddPolygon( FbxMeshC* mesh, int index )
{
    As<FbxMesh>( mesh )->AddPolygon( index );
}

void FbxMesh_EndPolygon( FbxMeshC* mesh )
{
    As<FbxMesh>( mesh )->EndPolygon();
}

int FbxMesh_CreateLayer( FbxMeshC* mesh )
{
    return As<FbxMesh>( mesh )->CreateLayer();
}

FbxLayerElementC* FbxMesh_CreateLayerElement( FbxManagerC* manager, FbxMeshC* mesh, int layerIndex,
                                             int type, const char* name )
{
    FbxMesh* self = As<FbxMesh>( mesh );

    while ( !self->GetLayer( layerIndex ) )
        self->CreateLayer();

    FbxLayer* layer = self->GetLayer( layerIndex );

    FbxLayerElement::EType elementType;
    switch ( type )
    {
        case FbxLayerElementTypeNormal:      elementType = FbxLayerElement::eNormal; break;
        case FbxLayerElementTypeUV:          elementType = FbxLayerElement::eUV; break;
        case FbxLayerElementTypeVertexColor: elementType = FbxLayerElement::eVertexColor; break;
        case FbxLayerElementTypeMaterial:    elementType = FbxLayerElement::eMaterial; break;
        default:
            SetLastError( "unknown layer element type" );
            return nullptr;
    }

    FbxLayerElement* element = layer->CreateLayerElementOfType( elementType );
    if ( !element )
    {
        SetLastError( "CreateLayerElementOfType failed" );
        return nullptr;
    }

    if ( name )
        element->SetName( name );

    auto handle = std::make_unique<LayerElementHandle>();
    handle->type = type;
    handle->element = element;

    auto raw = handle.get();
    Mgr( manager )->layerElements.push_back( std::move( handle ) );

    return reinterpret_cast<FbxLayerElementC*>( raw );
}

void FbxLayerElement_SetMappingMode( FbxLayerElementC* element, int mode )
{
    As<LayerElementHandle>( element )->element->SetMappingMode( static_cast<FbxLayerElement::EMappingMode>( mode ) );
}

void FbxLayerElement_SetReferenceMode( FbxLayerElementC* element, int mode )
{
    As<LayerElementHandle>( element )->element->SetReferenceMode( static_cast<FbxLayerElement::EReferenceMode>( mode ) );
}

void FbxLayerElement_AddDirectVector2( FbxLayerElementC* element, double x, double y )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementUV*>( handle->element )->GetDirectArray().Add( FbxVector2( x, y ) );
}

void FbxLayerElement_AddDirectVector4( FbxLayerElementC* element, double x, double y, double z, double w )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementNormal*>( handle->element )->GetDirectArray().Add( FbxVector4( x, y, z, w ) );
}

void FbxLayerElement_AddDirectColor( FbxLayerElementC* element, double r, double g, double b, double a )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementVertexColor*>( handle->element )->GetDirectArray().Add( FbxColor( r, g, b, a ) );
}

void FbxLayerElement_AddIndex( FbxLayerElementC* element, int index )
{
    auto handle = As<LayerElementHandle>( element );
    switch ( handle->type )
    {
        case FbxLayerElementTypeMaterial:
            static_cast<FbxLayerElementMaterial*>( handle->element )->GetIndexArray().Add( index );
            break;
        case FbxLayerElementTypeUV:
            static_cast<FbxLayerElementUV*>( handle->element )->GetIndexArray().Add( index );
            break;
        case FbxLayerElementTypeNormal:
            static_cast<FbxLayerElementNormal*>( handle->element )->GetIndexArray().Add( index );
            break;
        case FbxLayerElementTypeVertexColor:
            static_cast<FbxLayerElementVertexColor*>( handle->element )->GetIndexArray().Add( index );
            break;
    }
}

FbxSkinC* FbxSkin_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxSkinC*>( FbxSkin::Create( Sdk( manager ), name ) );
}

void FbxSkin_SetSkinningTypeLinear( FbxSkinC* skin )
{
    As<FbxSkin>( skin )->SetSkinningType( FbxSkin::eLinear );
}

void FbxSkin_AddCluster( FbxSkinC* skin, FbxClusterC* cluster )
{
    As<FbxSkin>( skin )->AddCluster( As<FbxCluster>( cluster ) );
}

void FbxMesh_AddSkinDeformer( FbxMeshC* mesh, FbxSkinC* skin )
{
    As<FbxMesh>( mesh )->AddDeformer( As<FbxSkin>( skin ) );
}

FbxClusterC* FbxCluster_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxClusterC*>( FbxCluster::Create( Sdk( manager ), name ) );
}

void FbxCluster_SetLink( FbxClusterC* cluster, FbxNodeC* node )
{
    As<FbxCluster>( cluster )->SetLink( As<FbxNode>( node ) );
}

void FbxCluster_SetLinkMode( FbxClusterC* cluster, int mode )
{
    As<FbxCluster>( cluster )->SetLinkMode( static_cast<FbxCluster::ELinkMode>( mode ) );
}

void FbxCluster_SetTransformLinkMatrix( FbxClusterC* cluster, const double* matrix16 )
{
    As<FbxCluster>( cluster )->SetTransformLinkMatrix( ToMatrix( matrix16 ) );
}

void FbxCluster_AddControlPointIndex( FbxClusterC* cluster, int index, double weight )
{
    As<FbxCluster>( cluster )->AddControlPointIndex( index, weight );
}

FbxPoseC* FbxPose_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxPoseC*>( FbxPose::Create( Sdk( manager ), name ) );
}

void FbxPose_SetIsBindPose( FbxPoseC* pose, int isBindPose )
{
    As<FbxPose>( pose )->SetIsBindPose( isBindPose != 0 );
}

void FbxPose_Add( FbxPoseC* pose, FbxNodeC* node, const double* matrix16 )
{
    As<FbxPose>( pose )->Add( As<FbxNode>( node ), FbxMatrix( ToMatrix( matrix16 ) ) );
}

void FbxScene_AddPose( FbxSceneC* scene, FbxPoseC* pose )
{
    As<FbxScene>( scene )->AddPose( As<FbxPose>( pose ) );
}

FbxBlendShapeC* FbxBlendShape_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxBlendShapeC*>( FbxBlendShape::Create( Sdk( manager ), name ) );
}

FbxBlendShapeChannelC* FbxBlendShapeChannel_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxBlendShapeChannelC*>( FbxBlendShapeChannel::Create( Sdk( manager ), name ) );
}

FbxShapeC* FbxShape_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxShapeC*>( FbxShape::Create( Sdk( manager ), name ) );
}

void FbxMesh_AddBlendShapeDeformer( FbxMeshC* mesh, FbxBlendShapeC* blendShape )
{
    As<FbxMesh>( mesh )->AddDeformer( As<FbxBlendShape>( blendShape ) );
}

void FbxBlendShape_AddChannel( FbxBlendShapeC* blendShape, FbxBlendShapeChannelC* channel )
{
    As<FbxBlendShape>( blendShape )->AddBlendShapeChannel( As<FbxBlendShapeChannel>( channel ) );
}

void FbxBlendShapeChannel_AddTargetShape( FbxBlendShapeChannelC* channel, FbxShapeC* shape, double fullWeight )
{
    As<FbxBlendShapeChannel>( channel )->AddTargetShape( As<FbxShape>( shape ), fullWeight );
}

void FbxShape_InitFrom( FbxShapeC* shape, FbxMeshC* mesh )
{
    FbxShape* self = As<FbxShape>( shape );
    const int count = As<FbxMesh>( mesh )->GetControlPointsCount();
    self->InitControlPoints( count );
    self->InitNormals( count );
}

void FbxShape_SetControlPointAt( FbxShapeC* shape, int index, double x, double y, double z )
{
    As<FbxShape>( shape )->GetControlPoints()[index] = FbxVector4( x, y, z );
}

void FbxShape_SetNormalAt( FbxShapeC* shape, int index, double x, double y, double z )
{
    As<FbxShape>( shape )->GetLayer( 0 )->GetNormals()->GetDirectArray().SetAt( index, FbxVector4( x, y, z ) );
}

FbxSurfaceMaterialC* FbxSurfacePhong_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxSurfaceMaterialC*>( FbxSurfacePhong::Create( Sdk( manager ), name ) );
}

FbxFileTextureC* FbxFileTexture_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxFileTextureC*>( FbxFileTexture::Create( Sdk( manager ), name ) );
}

void FbxFileTexture_SetFileName( FbxFileTextureC* texture, const char* path )
{
    As<FbxFileTexture>( texture )->SetFileName( path );
}

void FbxSurfaceMaterial_ConnectDiffuseTexture( FbxSurfaceMaterialC* material, FbxFileTextureC* texture )
{
    As<FbxSurfacePhong>( material )->Diffuse.ConnectSrcObject( As<FbxFileTexture>( texture ) );
}

void FbxSurfaceMaterial_SetUserPropertyString( FbxSurfaceMaterialC* material, const char* name, const char* value )
{
    SetUserProperty( As<FbxSurfaceMaterial>( material ), name, FbxStringDT, FbxString( value ? value : "" ) );
}

void FbxSurfaceMaterial_SetUserPropertyInt( FbxSurfaceMaterialC* material, const char* name, int value )
{
    SetUserProperty( As<FbxSurfaceMaterial>( material ), name, FbxIntDT, value );
}

void FbxSurfaceMaterial_SetUserPropertyBool( FbxSurfaceMaterialC* material, const char* name, int value )
{
    SetUserProperty( As<FbxSurfaceMaterial>( material ), name, FbxBoolDT, value != 0 );
}

void FbxSurfaceMaterial_SetUserPropertyDouble( FbxSurfaceMaterialC* material, const char* name, double value )
{
    SetUserProperty( As<FbxSurfaceMaterial>( material ), name, FbxDoubleDT, value );
}

FbxCameraC* FbxCamera_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxCameraC*>( FbxCamera::Create( Sdk( manager ), name ) );
}

void FbxCamera_SetAspect( FbxCameraC* camera, double width, double height )
{
    As<FbxCamera>( camera )->SetAspect( FbxCamera::eFixedRatio, width, height );
}

void FbxCamera_SetFieldOfView( FbxCameraC* camera, double degrees )
{
    As<FbxCamera>( camera )->FieldOfView.Set( degrees );
}

void FbxCamera_SetNearFarPlane( FbxCameraC* camera, double nearPlane, double farPlane )
{
    FbxCamera* self = As<FbxCamera>( camera );
    self->NearPlane.Set( nearPlane );
    self->FarPlane.Set( farPlane );
}

FbxLightC* FbxLight_Create( FbxManagerC* manager, const char* name )
{
    return reinterpret_cast<FbxLightC*>( FbxLight::Create( Sdk( manager ), name ) );
}

void FbxLight_SetType( FbxLightC* light, int type )
{
    As<FbxLight>( light )->LightType.Set( static_cast<FbxLight::EType>( type ) );
}

void FbxLight_SetColor( FbxLightC* light, double r, double g, double b )
{
    As<FbxLight>( light )->Color.Set( FbxDouble3( r, g, b ) );
}

int FbxScene_Export( FbxManagerC* manager, FbxSceneC* scene, const char* path, int fileFormatBinary )
{
    FbxManager* sdk = Sdk( manager );
    FbxExporter* exporter = FbxExporter::Create( sdk, "" );

    int format = fileFormatBinary
        ? sdk->GetIOPluginRegistry()->GetNativeWriterFormat()
        : sdk->GetIOPluginRegistry()->FindWriterIDByDescription( "FBX ascii (*.fbx)" );

    if ( !exporter->Initialize( path, format, sdk->GetIOSettings() ) )
    {
        SetLastError( exporter->GetStatus().GetErrorString() );
        exporter->Destroy();
        return -1;
    }

    exporter->SetFileExportVersion( FBX_2014_00_COMPATIBLE );

    if ( !exporter->Export( As<FbxScene>( scene ) ) )
    {
        SetLastError( exporter->GetStatus().GetErrorString() );
        exporter->Destroy();
        return -1;
    }

    exporter->Destroy();
    return 0;
}

const char* FbxGetLastError( void )
{
    return gLastError.c_str();
}
