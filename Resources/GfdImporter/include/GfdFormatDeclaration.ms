
/* 	Persona 4: DAN & Persona 5 model and animation importer (*.gmd; *.gfs; *.gap) written in MaxScript
    Script by TGE. Please give credit where is due.
    Special thanks to PolarSoda, mrdudesworkshop
*/
        
-- Magic values
GfdModelMagic           = 0x47465330
GfdChunkVersion        = 0x01105070 -- not constant
GfdTextureFooterMagic  = 0x01010000

-- Chunk types
GfdChunkType_FileEnd        = 0
GfdChunkType_FileStart      = 1
GfdChunkType_Scene          = 0x00010003
GfdChunkType_MaterialList   = 0x000100FB
GfdChunkType_TextureList    = 0x000100FC
GfdChunkType_AnimationPackage = 0x000100FD

-- Material texture slot ids
GfdMaterialDiffuseTextureSlotID		= 1
GfdMaterialNormalTextureSlotID		= 2
GfdMaterialSpecularTextureSlotID	= 3
GfdMaterialReflectionTextureSlotID	= 4
GfdMaterialHighlightTextureSlotID	= 5
GfdMaterialGlowTextureSlotID 		= 6
GfdMaterialNightTextureSlotID 		= 7
GfdMaterialDetailTextureSlotID 		= 8
GfdMaterialShadowTextureSlotID 	    = 9
GfdMaterialTextureSlot10ID 			= 10
GfdMaterialTextureSlot11ID 			= 11
GfdMaterialTextureSlot12ID 			= 12
GfdMaterialTextureSlotCount			= 9

-- Material flags
GfdMaterialFlags_1							= 0x00000001 -- bit 0
GfdMaterialFlags_2							= 0x00000002 -- bit 1
GfdMaterialFlags_3							= 0x00000004 -- bit 2
GfdMaterialFlags_4							= 0x00000008 -- bit 3
GfdMaterialFlags_5							= 0x00000010 -- bit 4
GfdMaterialFlags_6							= 0x00000020 -- bit 5
GfdMaterialFlags_7							= 0x00000040 -- bit 6
GfdMaterialFlags_8							= 0x00000080 -- bit 7
GfdMaterialFlags_9							= 0x00000100 -- bit 8
GfdMaterialFlags_10                          = 0x00000200 -- bit 9
GfdMaterialFlags_11                          = 0x00000400 -- bit 10
GfdMaterialFlags_12                          = 0x00000800 -- bit 11
GfdMaterialFlags_13                          = 0x00001000 -- bit 12
GfdMaterialFlags_14                          = 0x00002000 -- bit 13
GfdMaterialFlags_15                          = 0x00004000 -- bit 14
GfdMaterialFlags_16                          = 0x00008000 -- bit 15
GfdMaterialFlags_HasAttributes               = 0x00010000 -- bit 16
GfdMaterialFlags_18                          = 0x00020000 -- bit 17
GfdMaterialFlags_19                          = 0x00040000 -- bit 18
GfdMaterialFlags_20                          = 0x00080000 -- bit 19
GfdMaterialFlags_UseDiffuseTextureSlot       = 0x00100000 -- bit 20
GfdMaterialFlags_UseNormalTextureSlot	    = 0x00200000 -- bit 21
GfdMaterialFlags_UseSpecularTextureSlot		= 0x00400000 -- bit 22
GfdMaterialFlags_UseReflectionTextureSlot	= 0x00800000 -- bit 23
GfdMaterialFlags_UseHighlightTextureSlot		= 0x01000000 -- bit 24
GfdMaterialFlags_UseGlowTextureSlot			= 0x02000000 -- bit 25
GfdMaterialFlags_UseNightTextureSlot			= 0x04000000 -- bit 26
GfdMaterialFlags_UseDetailTextureSlot	    = 0x08000000 -- bit 27
GfdMaterialFlags_UseShadowTextureSlot		= 0x10000000 -- bit 28
GfdMaterialFlags_UseTextureSlot10			= 0x20000000 -- bit 29
GfdMaterialFlags_UseTextureSlot11			= 0x40000000 -- bit 30
GfdMaterialFlags_UseTextureSlot12			= 0x80000000 -- bit 31

GfdMaterialTextureSlotNames = #
(
    "DiffuseTextureSlot",
    "NormalTextureSlot",
    "SpecularTextureSlot",
    "ReflectionTextureSlot",
    "HighlightTextureSlot",
    "GlowTextureSlot",
    "NightTextureSlot",
    "DetailTextureSlot",
    "ShadowTextureSlot",
    "Tex10",
    "Tex11",
    "Tex12",
    "Tex13"
)

-- Scene flags
GfdSceneFlags_HasBBox    = 0x00000001
GfdSceneFlags_HasBSphere = 0x00000002
GfdSceneFlags_Skinned    = 0x00000004
GfdSceneFlags_Morphed    = 0x00000008

-- Node user property types
GfdUserPropertyType_None	    = 0
GfdUserPropertyType_Int		= 1
GfdUserPropertyType_Float	= 2
GfdUserPropertyType_Bool	    = 3
GfdUserPropertyType_String	= 4
GfdUserPropertyType_ByteVector3 = 5
GfdUserPropertyType_ByteVector4 = 6
GfdUserPropertyType_Vector3 = 7
GfdUserPropertyType_Vector4 = 8
GfdUserPropertyType_ByteArray = 9

-- Mesh index type
GfdGeometryIndexType_None    = 0
GfdGeometryIndexType_UInt16  = 1
GfdGeometryIndexType_UInt32  = 2

-- Mesh flags
GfdGeometryFlags_HasVertexWeights    = 0x00000001
GfdGeometryFlags_HasMaterial         = 0x00000002
GfdGeometryFlags_HasTriangles        = 0x00000004
GfdGeometryFlags_HasBoundingBox      = 0x00000008
GfdGeometryFlags_HasBoundingSphere 	= 0x00000008
GfdGeometryFlags_HasMorphTargets		= 0x00000040
GfdGeometryFlags_1000                = 0x00001000

-- Mesh FVF flags
GfdGeometryVertexFlags_Position 	    = 0x00000002
GfdGeometryVertexFlags_Normal        = 0x00000010	-- Bit 1
GfdGeometryVertexFlags_Unknown4 	    = 0x00000004	-- Bit 2
GfdGeometryVertexFlags_Unknown5 	    = 0x00000008	-- Bit 3
GfdGeometryVertexFlags_Unknown7 	    = 0x00000020	-- Bit 5
GfdGeometryVertexFlags_Color         = 0x00000040	-- Bit 6
GfdGeometryVertexFlags_Unknown9	    = 0x00000080	-- Bit 7
GfdGeometryVertexFlags_Tex1 		    = 0x00000100	-- Bit 8
GfdGeometryVertexFlags_Tex2 		    = 0x00000200	-- Bit 9
GfdGeometryVertexFlags_Tex3 		    = 0x00000400	-- Bit 10
GfdGeometryVertexFlags_Tex4 		    = 0x00000800	-- Bit 11
GfdGeometryVertexFlags_Tex5 		    = 0x00001000	-- Bit 12
GfdGeometryVertexFlags_Tex6 		    = 0x00002000	-- Bit 13
GfdGeometryVertexFlags_Tex7 		    = 0x00004000	-- Bit 14
GfdGeometryVertexFlags_Tex8 		    = 0x00008000	-- Bit 15
GfdGeometryVertexFlags_Tangent 	    = 0x10000000 -- Bit 28 (guess)
GfdGeometryVertexFlags_Binormal      = 0x20000000 -- Bit 29
GfdGeometryVertexFlags_Color2        = 0x40000000 -- Bit 30
GfdGeometryVertexFlags_Flag80000000  = 0x80000000 -- Bit 31

GfdAnimationPackageFlags_Flag4 = 4 -- 1 << 2
GfdAnimationPackageFlags_Flag10 = 10 -- 1 << 10

GfdAnimationFlags_HasProperties = 0x800000 -- 1 << 23
GfdAnimationFlags_Flag1000000  = 0x1000000 -- 1 << 24
GfdAnimationFlags_Flag2000000  = 0x2000000 -- 1 << 25
GfdAnimationFlags_Flag4000000  = 0x4000000 -- 1 << 26
GfdAnimationFlags_Flag10000000 = 0x10000000 -- 1 << 28
GfdAnimationFlags_Flag20000000 = 0x20000000 -- 1 << 29
GfdAnimationFlags_HasBoundingBox = 0x40000000 -- 1 << 30
GfdAnimationFlags_Flag80000000 = 0x80000000 -- 1 << 31

GfdAnimationKeyframeType_Type00 = 0
GfdAnimationKeyframeType_PRSingle = 1
GfdAnimationKeyframeType_PRSSingle = 2
GfdAnimationKeyframeType_Type03 = 3
GfdAnimationKeyframeType_Type04 = 4
GfdAnimationKeyframeType_Morph = 5
GfdAnimationKeyframeType_Type06 = 6
GfdAnimationKeyframeType_Type07 = 7
GfdAnimationKeyframeType_Type08 = 8
GfdAnimationKeyframeType_Type09 = 9
GfdAnimationKeyframeType_Type10 = 10
GfdAnimationKeyframeType_Type11 = 11
GfdAnimationKeyframeType_Type12 = 12
GfdAnimationKeyframeType_Type13 = 13
GfdAnimationKeyframeType_Type14 = 14
GfdAnimationKeyframeType_Type15 = 15
GfdAnimationKeyframeType_Type16 = 16
GfdAnimationKeyframeType_Type17 = 17
GfdAnimationKeyframeType_Type18 = 18
GfdAnimationKeyframeType_Type19 = 19
GfdAnimationKeyframeType_Type20 = 20
GfdAnimationKeyframeType_Type21 = 21
GfdAnimationKeyframeType_Type22 = 22
GfdAnimationKeyframeType_Type23 = 23
GfdAnimationKeyframeType_Type24 = 24
GfdAnimationKeyframeType_Type25 = 25
GfdAnimationKeyframeType_Type26 = 26
GfdAnimationKeyframeType_PRSHalf = 27
GfdAnimationKeyframeType_PRHalf = 28
GfdAnimationKeyframeType_Type29 = 29
GfdAnimationKeyframeType_Type30 = 30
GfdAnimationKeyframeType_PHalf = 31 -- P3/P5D
GfdAnimationKeyframeType_RHalf = 32 -- P3/P5D
GfdAnimationKeyframeType_SHalf = 33 -- P3/P5D

GfdAnimationControllerType_Invalid = 0
GfdAnimationControllerType_PRS = 1
GfdAnimationControllerType_Material = 2
GfdAnimationControllerType_Camera = 3
GfdAnimationControllerType_Morph = 4

struct GfdBoundingBox
(
    MaxExtents,
    MinExtents
)

-- Struct declarations
struct GfdChunkDescriptor
(
    StartOffset,
    Version,
    Type,
    Size
)

struct GfdAnimationPackage
(
    Flags,
    AnimationCount,
    Animations = #(),
    BlendAnimationCount,
    BlendAnimations = #(),
    ExtraData
)

struct GfdAnimationExtraData
(
    Field00,
    Field10,
    Field04,
    Field14,
    Field08,
    Field18,
    Field0C,
    Field1C
)

struct GfdAnimation
(
    Flags,
    Duration,
    ControllerCount,
    Controllers = #(),
    Field10,
    Field14,
    BoundingBox,
    Field1C,
    Properties,
    Field24
)

struct GfdAnimationFlag80000000Data
(
    Field00,
    Field04,
    Field20
)

struct GfdAnimationFlag10000000DataEntry
(
    Field00,
    Field04
)

struct GfdAnimationController
(
    Type,
    TargetId,
    TargetName,
    TrackCount,
    Tracks = #()
)

struct GfdAnimationKeyframeTrack
(
    KeyframeType,
    KeyframeCount,
    Keyframes = #(),
    KeyframeTimings = #(),
    BasePosition = [0,0,0],
    BaseScale = [0,0,0]
)

-- Type 01
struct GfdAnimationKeyframePRSingle
(
    Position,
    Rotation
)

-- Type 02
struct GfdAnimationKeyframePRSSingle
(
    Position,
    Rotation,
    Scale
)

-- Type 05
struct GfdAnimationKeyframeMorph
(
    ChannelValue
)

-- Type 12
struct GfdAnimationMaterialKeyframeType12
(
    Field00
)

-- Type 14
struct GfdAnimationMaterialKeyframeType14
(
    Field00,
    Field04,
    Field08
)

-- Type 27
struct GfdAnimationKeyframePRSHalf
(
    Position,
    Rotation,
    Scale
)

-- Type 28
struct GfdAnimationKeyframePRHalf
(
    Position,
    Rotation
)

-- Type 29
struct GfdAnimationMaterialKeyframeType29
(
    Field00
)

-- Type 31
struct GfdAnimationKeyframePHalf
(
    Position
)

-- Type 32
struct GfdAnimationKeyframeRHalf
(
    Rotation
)

-- Type 33
struct GfdAnimationKeyframeSHalf
(
    Scale
)

struct GfdTexture
(
    Name,
    PixelFormat,
    DataLength
)

struct GfdMaterial
(
    Name,
    Hash,
    Flags,
    Ambient,
    Diffuse,
    Specular,
    Emissive,
    Field40,
    Field44,
    Field48,
    Field49,
    Field4A,
    Field4B,
    Field4C,
    Field4D,
    Field90,
    Field92,
    Field94,
    Field96,
    Field5C,
    Field6C,
    Field70,
    Field50,
    Field98,
    TextureSlots = #()
)

struct GfdMaterialTextureSlot
(
    TextureName,
    Hash
)

struct GfdScene
(
    Field0,
    Flags,
    UsedBoneCount,
    InvBindMatrices = #(),
    UsedBoneMap = #(),
    Nodes = #()
)

struct GfdNode
(
    Parent,
    Name,
    Hash,
    Position,
    Rotation,
    Scale,
    AttachmentCount,
    Attachments = #(),
    ChildCount,
    Children = #(),
    MaxBoneNode,
    UserProperties = #()
)

struct GfdNodeAttachment
(
    Type,
    Object
)

struct GfdUserProperty
(
    Type,
    Key,
    Hash,
    Size,
    Value
)

struct GfdGeometry
(
    Flags,
    VertexFormat,
    FaceCount,
    IndexType,
    VertexCount,
    Field7,
    VertexPositions = #(),
    VertexNormals = #(),
    VertexTangents = #(),
    VertexColors = #(),
    VertexWeights = #(),
    VertexWeightIndices = #(),
    VertexTexCoordChannels = #(#(), #(), #(), #(), #(), #(), #(), #()),
    MorphTargetList,
    FaceIndices = #(),
    MaterialName,
    MaterialHash,
    MaxMeshNode
)

struct GfdMorphTargetList
(
    Flags,
    Targets = #()
)

struct GfdMorphTarget
(
    Flags,
    VertexPositionOffsets = #()
)

struct GfdCamera
(
    Transform,
    Field180,
    Field184,
    Field188,
    Field18C,
    Field190
)

struct GfdLight
(
    Flags,
    Type,
    Field30,
    Field40,
    Field50,
    Field20,
    Field02,
    Field08,
    Field10,
    Field6C,
    Field70,
    Field60,
    Field64,
    Field68,
    Field74,
    Field78,
    Field04
)

struct GfdModel
(
    Textures,
    Materials,
    Scene,
    AnimationPackage
)
