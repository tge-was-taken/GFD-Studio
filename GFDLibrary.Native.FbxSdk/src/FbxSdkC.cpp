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

    FbxScene* Scn( FbxSceneC* s ) { return reinterpret_cast<FbxScene*>( s ); }

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

    FbxLayerElement::EType ToElementType( int type, bool* ok )
    {
        *ok = true;
        switch ( type )
        {
            case FbxLayerElementTypeNormal:      return FbxLayerElement::eNormal;
            case FbxLayerElementTypeUV:          return FbxLayerElement::eUV;
            case FbxLayerElementTypeVertexColor: return FbxLayerElement::eVertexColor;
            case FbxLayerElementTypeMaterial:    return FbxLayerElement::eMaterial;
            default:
                *ok = false;
                return FbxLayerElement::eUnknown;
        }
    }

    FbxLayerElementC* TrackElement( FbxManagerC* manager, int type, FbxLayerElement* element )
    {
        if ( !element )
        {
            SetLastError( "creating the layer element failed" );
            return nullptr;
        }

        auto handle = std::make_unique<LayerElementHandle>();
        handle->type = type;
        handle->element = element;

        auto raw = handle.get();
        Mgr( manager )->layerElements.push_back( std::move( handle ) );

        return reinterpret_cast<FbxLayerElementC*>( raw );
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
    settings->SetBoolProp( EXP_FBX_MATERIAL, true );
    settings->SetBoolProp( EXP_FBX_TEXTURE, true );
    settings->SetBoolProp( EXP_FBX_EMBEDDED, false );
    settings->SetBoolProp( EXP_FBX_SHAPE, true );
    settings->SetBoolProp( EXP_FBX_GOBO, true );
    settings->SetBoolProp( EXP_FBX_ANIMATION, true );
    settings->SetBoolProp( EXP_FBX_GLOBAL_SETTINGS, true );
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
    return reinterpret_cast<FbxNodeC*>( Scn( scene )->GetRootNode() );
}

void FbxScene_SetAxisSystem( FbxSceneC* scene, int preset )
{
    FbxAxisSystem axisSystem = FbxAxisSystem::DirectX;
    switch ( preset )
    {
        case FbxAxisSystemPresetMayaZUp:        axisSystem = FbxAxisSystem::MayaZUp; break;
        case FbxAxisSystemPresetMayaYUp:        axisSystem = FbxAxisSystem::MayaYUp; break;
        case FbxAxisSystemPresetMax:            axisSystem = FbxAxisSystem::Max; break;
        case FbxAxisSystemPresetMotionBuilder:  axisSystem = FbxAxisSystem::Motionbuilder; break;
        case FbxAxisSystemPresetOpenGL:         axisSystem = FbxAxisSystem::OpenGL; break;
        case FbxAxisSystemPresetDirectX:        axisSystem = FbxAxisSystem::DirectX; break;
        case FbxAxisSystemPresetLightwave:      axisSystem = FbxAxisSystem::Lightwave; break;
        default:
            SetLastError( "unknown axis system preset" );
            return;
    }

    Scn( scene )->GetGlobalSettings().SetAxisSystem( axisSystem );
}

void FbxScene_SetSystemUnit( FbxSceneC* scene, double scaleFactor )
{
    Scn( scene )->GetGlobalSettings().SetSystemUnit( FbxSystemUnit( scaleFactor ) );
}

FbxNodeC* FbxNode_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxNodeC*>( FbxNode::Create( Scn( scene ), name ) );
}

void FbxNode_AddChild( FbxNodeC* parent, FbxNodeC* child )
{
    As<FbxNode>( parent )->AddChild( As<FbxNode>( child ) );
}

void FbxNode_SetLocalTransform( FbxNodeC* node, const double* translation,
                                const double* rotationQuaternion, const double* scale )
{
    FbxNode* self = As<FbxNode>( node );

    FbxAMatrix rotation;
    rotation.SetQ( FbxQuaternion( rotationQuaternion[0], rotationQuaternion[1],
                                  rotationQuaternion[2], rotationQuaternion[3] ) );

    self->LclRotation.Set( rotation.GetR() );
    self->LclScaling.Set( FbxDouble3( scale[0], scale[1], scale[2] ) );
    self->LclTranslation.Set( FbxDouble3( translation[0], translation[1], translation[2] ) );
    self->SetPreferedAngle( self->LclRotation.Get() );
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

FbxSkeletonC* FbxSkeleton_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxSkeletonC*>( FbxSkeleton::Create( Scn( scene ), name ) );
}

void FbxSkeleton_SetSkeletonType( FbxSkeletonC* skeleton, int type )
{
    As<FbxSkeleton>( skeleton )->SetSkeletonType( static_cast<FbxSkeleton::EType>( type ) );
}

FbxMeshC* FbxMesh_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxMeshC*>( FbxMesh::Create( Scn( scene ), name ) );
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
    As<FbxMesh>( mesh )->BeginPolygon( material, -1, -1, false );
}

void FbxMesh_AddPolygon( FbxMeshC* mesh, int index )
{
    As<FbxMesh>( mesh )->AddPolygon( index );
}

void FbxMesh_EndPolygon( FbxMeshC* mesh )
{
    As<FbxMesh>( mesh )->EndPolygon();
}

FbxLayerElementC* FbxMesh_CreateElement( FbxManagerC* manager, FbxMeshC* mesh, int type, const char* name )
{
    FbxMesh* self = As<FbxMesh>( mesh );

    FbxLayerElement* element = nullptr;
    switch ( type )
    {
        case FbxLayerElementTypeNormal:      element = self->CreateElementNormal(); break;
        case FbxLayerElementTypeUV:          element = self->CreateElementUV( name ); break;
        case FbxLayerElementTypeVertexColor: element = self->CreateElementVertexColor(); break;
        case FbxLayerElementTypeMaterial:    element = self->CreateElementMaterial(); break;
        default:
            SetLastError( "unknown layer element type" );
            return nullptr;
    }

    if ( element && name )
        element->SetName( name );

    return TrackElement( manager, type, element );
}

FbxLayerElementC* FbxMesh_CreateLayerElement( FbxManagerC* manager, FbxMeshC* mesh, int layerIndex,
                                              int type, const char* name )
{
    FbxMesh* self = As<FbxMesh>( mesh );

    while ( !self->GetLayer( layerIndex ) )
        self->CreateLayer();

    bool ok = false;
    FbxLayerElement::EType elementType = ToElementType( type, &ok );
    if ( !ok )
    {
        SetLastError( "unknown layer element type" );
        return nullptr;
    }

    FbxLayerElement* element = self->GetLayer( layerIndex )->CreateLayerElementOfType( elementType );
    if ( element && name )
        element->SetName( name );

    return TrackElement( manager, type, element );
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

void FbxLayerElement_SetDirectCount( FbxLayerElementC* element, int count )
{
    auto handle = As<LayerElementHandle>( element );
    switch ( handle->type )
    {
        case FbxLayerElementTypeUV:
            static_cast<FbxLayerElementUV*>( handle->element )->GetDirectArray().SetCount( count );
            break;
        case FbxLayerElementTypeNormal:
            static_cast<FbxLayerElementNormal*>( handle->element )->GetDirectArray().SetCount( count );
            break;
        case FbxLayerElementTypeVertexColor:
            static_cast<FbxLayerElementVertexColor*>( handle->element )->GetDirectArray().SetCount( count );
            break;
    }
}

void FbxLayerElement_SetDirectVector2At( FbxLayerElementC* element, int index, double x, double y )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementUV*>( handle->element )->GetDirectArray().SetAt( index, FbxVector2( x, y ) );
}

void FbxLayerElement_SetDirectVector4At( FbxLayerElementC* element, int index,
                                         double x, double y, double z, double w )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementNormal*>( handle->element )->GetDirectArray().SetAt( index, FbxVector4( x, y, z, w ) );
}

void FbxLayerElement_SetDirectColorAt( FbxLayerElementC* element, int index,
                                       double r, double g, double b, double a )
{
    auto handle = As<LayerElementHandle>( element );
    static_cast<FbxLayerElementVertexColor*>( handle->element )->GetDirectArray().SetAt( index, FbxColor( r, g, b, a ) );
}

FbxSkinC* FbxSkin_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxSkinC*>( FbxSkin::Create( Scn( scene ), name ) );
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

FbxClusterC* FbxCluster_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxClusterC*>( FbxCluster::Create( Scn( scene ), name ) );
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

FbxPoseC* FbxPose_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxPoseC*>( FbxPose::Create( Scn( scene ), name ) );
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
    Scn( scene )->AddPose( As<FbxPose>( pose ) );
}

FbxBlendShapeC* FbxBlendShape_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxBlendShapeC*>( FbxBlendShape::Create( Scn( scene ), name ) );
}

FbxBlendShapeChannelC* FbxBlendShapeChannel_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxBlendShapeChannelC*>( FbxBlendShapeChannel::Create( Scn( scene ), name ) );
}

FbxShapeC* FbxShape_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxShapeC*>( FbxShape::Create( Scn( scene ), name ) );
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

FbxSurfaceMaterialC* FbxSurfacePhong_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxSurfaceMaterialC*>( FbxSurfacePhong::Create( Scn( scene ), name ) );
}

void FbxSurfaceMaterial_SetShadingModel( FbxSurfaceMaterialC* material, const char* model )
{
    As<FbxSurfaceMaterial>( material )->ShadingModel.Set( FbxString( model ? model : "" ) );
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

FbxFileTextureC* FbxFileTexture_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxFileTextureC*>( FbxFileTexture::Create( Scn( scene ), name ) );
}

void FbxFileTexture_SetFileName( FbxFileTextureC* texture, const char* path )
{
    As<FbxFileTexture>( texture )->SetFileName( path );
}

void FbxFileTexture_SetUVSet( FbxFileTextureC* texture, const char* name )
{
    As<FbxFileTexture>( texture )->UVSet.Set( FbxString( name ? name : "" ) );
}

void FbxFileTexture_SetUseMaterial( FbxFileTextureC* texture, int useMaterial )
{
    As<FbxFileTexture>( texture )->UseMaterial.Set( useMaterial != 0 );
}

FbxCameraC* FbxCamera_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxCameraC*>( FbxCamera::Create( Scn( scene ), name ) );
}

void FbxCamera_SetPosition( FbxCameraC* camera, double x, double y, double z )
{
    As<FbxCamera>( camera )->Position.Set( FbxDouble3( x, y, z ) );
}

void FbxCamera_SetUpVector( FbxCameraC* camera, double x, double y, double z )
{
    As<FbxCamera>( camera )->UpVector.Set( FbxDouble3( x, y, z ) );
}

void FbxCamera_SetInterestPosition( FbxCameraC* camera, double x, double y, double z )
{
    As<FbxCamera>( camera )->InterestPosition.Set( FbxDouble3( x, y, z ) );
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

void FbxCamera_SetAspect( FbxCameraC* camera, double width, double height )
{
    As<FbxCamera>( camera )->SetAspect( FbxCamera::eFixedResolution, width, height );
}

FbxLightC* FbxLight_Create( FbxSceneC* scene, const char* name )
{
    return reinterpret_cast<FbxLightC*>( FbxLight::Create( Scn( scene ), name ) );
}

void FbxLight_SetType( FbxLightC* light, int type )
{
    As<FbxLight>( light )->LightType.Set( static_cast<FbxLight::EType>( type ) );
}

void FbxLight_SetColor( FbxLightC* light, double r, double g, double b )
{
    As<FbxLight>( light )->Color.Set( FbxDouble3( r, g, b ) );
}

void FbxLight_SetFarAttenuation( FbxLightC* light, int enable, double start, double end )
{
    FbxLight* self = As<FbxLight>( light );
    self->EnableFarAttenuation.Set( enable != 0 );
    self->FarAttenuationStart.Set( start );
    self->FarAttenuationEnd.Set( end );
}

void FbxLight_SetConeAngles( FbxLightC* light, double innerDegrees, double outerDegrees )
{
    FbxLight* self = As<FbxLight>( light );
    self->InnerAngle.Set( innerDegrees );
    self->OuterAngle.Set( outerDegrees );
}

namespace
{
    /// The ascii writer's description carries the version, so it is matched on substring
    /// rather than by an exact name.
    int FindAsciiWriterFormat( FbxManager* sdk )
    {
        FbxIOPluginRegistry* registry = sdk->GetIOPluginRegistry();
        for ( int i = 0, count = registry->GetWriterFormatCount(); i < count; ++i )
        {
            if ( !registry->WriterIsFBX( i ) )
                continue;

            if ( FbxString( registry->GetWriterFormatDescription( i ) ).Find( "ascii" ) >= 0 )
                return i;
        }

        return -1;
    }
}

int FbxScene_Export( FbxManagerC* manager, FbxSceneC* scene, const char* path, int fileFormatBinary )
{
    FbxManager* sdk = Sdk( manager );

    int format = sdk->GetIOPluginRegistry()->GetNativeWriterFormat();
    if ( !fileFormatBinary )
    {
        format = FindAsciiWriterFormat( sdk );
        if ( format < 0 )
        {
            SetLastError( "no FBX ascii writer format available" );
            return -1;
        }
    }

    FbxExporter* exporter = FbxExporter::Create( sdk, "" );

    if ( !exporter->Initialize( path, format, sdk->GetIOSettings() ) ||
         !exporter->SetFileExportVersion( FBX_2014_00_COMPATIBLE ) ||
         !exporter->Export( Scn( scene ) ) )
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
