# GFD official type and member names

Recovered from a Persona 5 BETA PS3 executable that still carries symbols and
Hex-Rays type information for the whole `gfd` runtime.

- IDB: `SW.ELF` (`E:\Projects\p5_2014\SW.ELF`), IDA MCP instance on port 13339.
- All addresses below are from that image.

## What the evidence is

Two independent sources, used together:

1. **The `gfd*StreamRead` / `gfd*StreamWrite` serialisers.** One pair per
  structure, named after the structure. They read the fields in file order, so
  the Nth read corresponds to the Nth read in our `ReadCore`.
2. **The IDB's local type library.** It contains the real C struct definitions
  with member names (`gfdMaterial_tag_0`, `gfdKeyList_tag_0`). The
  decompiler propagates those names into the serialiser output, so most fields
  can be read directly off a symbol rather than inferred.

A name taken from a struct member or a symbol is **high**. A name taken from an
assert string, a file path, or an adjacent string literal is **medium**. A name
derived only from position or from what the value is used for is **low**, and is
marked as such.

Note: most functions exist twice in the image (e.g. `gfdMaterialStreamRead` and
`gfdMaterialStreamRead_0`). The `_0` copy is the one with decompilable bodies;
addresses below point at it.

## Enumerations recovered from dispatch tables

These are read straight out of jump/function tables, so they are high
confidence.

### `gfdObjectID` → C# `NodeAttachmentType`

From `gfdObjectFunctionTable` at `0x14715E4` (56 bytes per entry, first dword is
a name string).

| Value | Official name | C# `NodeAttachmentType` | Confidence |
|---|---|---|---|
| 1 | `scene` | *(none; C# calls 1 `Model`)* | high |
| 2 | `mesh` | `Model` *(C# `Mesh` value 2 is a misnomer)* | high |
| 3 | `node` | `Node` | high |
| 4 | `geometry` | `Mesh` | high |
| 5 | `camera` | `Camera` | high |
| 6 | `light` | `Light` | high |
| 7 | `epl` | `Epl` | high |
| 8 | `eplleaf` | `EplLeaf` | high |
| 9 | `morph` | `Morph` | high |

Important naming collision to keep in mind while reading the rest of this
document: **GFD's `gfdMesh` is our `Model`, and GFD's `gfdGeometry` is our
`Mesh`.**

### `gfdExtensionID` → C# `MaterialAttributeType`

From `gfdExtensionFuncTable` at `0x1082774`, cross-checked against the `id`
constant each reader stores (`0x10000` through `0x10004`; low 16 bits are the type).

| Value | Official struct | C# class | Confidence |
|---|---|---|---|
| 0 | `gfdExtensionShaderToon` | `MaterialAttributeType0` | high |
| 1 | `gfdExtensionShaderEdge` | `MaterialAttributeType1` | high |
| 2 | `gfdExtensionShaderOutline` | `MaterialAttributeType2` | high |
| 3 | `gfdExtensionShaderWater` | `MaterialAttributeType3` | high |
| 4 | `gfdExtensionShaderShadowEdge` | `MaterialAttributeType4` | high |

Types 5-8 in our `MaterialAttributeType` do not exist in this build.

### `gfdKeyType` → C# `KeyType`

From `keyTypeTable` at `0x1070538`, 28 bytes per entry:
`{blend, decompBlend, decomp, streamRead, streamWrite, diff, elemSize}`. The
table index is the on-disk key type value. The table gives the *key payload
struct*, not a per-type semantic name, so the semantic names in our `KeyType`
enum (`CameraFieldOfView`, `MaterialSingle_4`) are not confirmed by it.

| Value | Payload struct | Size | C# `KeyType` | Confidence |
|---|---|---|---|---|
| 1 | `gfdKeyTR` | 0x1C | `NodePR` | high (payload), medium (name) |
| 2 | `gfdKeyTRS` | 0x28 | `NodePRS` | high / medium |
| 3 | `gfdKeyRGB` | 0x0C | `Vector3` | high / medium |
| 4 | `gfdKeyRGBA` | 0x10 | `Quaternion` | high (payload is RGBA, i.e. 4 floats) / low (our name) |
| 5 | `gfdKeyFLOAT` | 0x04 | `Single` | high / medium |
| 6 | `gfdKeyRGB` | 0x0C | `Vector3_2` | high / low |
| 7 | `gfdKeyRGB` | 0x0C | `Vector3_3` | high / low |
| 8 | `gfdKeyRGB` | 0x0C | `Vector3_4` | high / low |
| 9 | `gfdKeyFLOAT` | 0x04 | `Single_2` | high / low |
| 10 | `gfdKeyRGBA` | 0x10 | `Quaternion_2` | high / low |
| 11 | `gfdKeyFLOAT` | 0x04 | `Single_3` | high / low |
| 12 | `gfdKeyFLOAT` | 0x04 | `MaterialSingle_4` | high / low |
| 13 | `gfdKeyUV` | 0x14 | `Single5` | high / low |
| 14 | `gfdKeyRGB` | 0x0C | `MaterialVector3_5` | high / low |
| 15 | `gfdKeyFLOAT` | 0x04 | `Single_5` | high / low |
| 16 | `gfdKeyFLOAT` | 0x04 | `Single_6` | high / low |
| 17 | `gfdKeyTRANSFORM` | 0x2C | `NodePRSByte` | high / low |
| 18 | `gfdKeyCOLOR` | 0x10 | `Single4Byte` | high / low |
| 19 | `gfdKeyALPHA` | 0x08 | `SingleByte` | high / low |
| 20 | `gfdKeyUV` | 0x14 | `Single5_2` | high / low |
| 21 | `gfdKeyUVStep` | 0x14 | `Single5Alt` | high / low |
| 22 | `gfdKeyMOTION` | 0x30 | `Type22` | high / low |
| 23 | `gfdKeyFLOAT` | 0x04 | `CameraFieldOfView` | high / low |
| 24 | `gfdKeyFLOAT` | 0x04 | `Single_8` | high / low |
| 25 | `gfdKeyFLOATStep` | 0x04 | `SingleAlt_2` | high / low |
| 26 | `gfdKeyCompressTR` | 0x0E | `NodePRHalf` | high / medium |
| 27 | `gfdKeyCompressTRS` | 0x14 | `NodePRSHalf` | high / medium |
| 28 | `gfdKeyCompressTR` | 0x0E | `NodePRHalf_2` | high / medium |
| 29 | `gfdKeyFLOAT` | 0x04 | `MaterialSingle_9` | high / low |
| 30 | `gfdKeyFLOATStep` | 0x04 | `SingleAlt_3` | high / low |

Types 31+ in our enum do not exist in this build (the table ends at 30).

Only types 26, 27 and 28 have a non-null `decompBlend`, which is exactly the
condition `gfdKeyListStreamRead` uses to read the trailing 6 floats. This
confirms our `AnimationLayer.UsesScaleVectors` set for pre-Metaphor versions
should be {26, 27, 28} and nothing else.

### `gfdPropertyType` → C# `UserPropertyValueType`

From the switch in `gfdPropertyStreamRead_0` (`0x876480`), with each arm's size
assert. Values 1-9 match our enum exactly, including the `size` semantics
(string stores `size` = length + 1 and reads `size - 1` bytes).

### Chunk IDs

From the dispatch loop in `gfdMeshStreamReadInside_0` (`0x8DCDA8`).

| ID | Reader called | C# `ResourceChunkType` |
|---|---|---|
| `0x00010003` | inline (model/scene body) | `Model` |
| `0x000100F8` | `gfdPropertyStreamRead` | `ChunkType000100F8` |
| `0x000100F9` | `gfdPhysicsSectorStreamRead` | `ChunkType000100F9` |
| `0x000100FB` | `gfdMaterialStreamRead` | `MaterialDictionary` |
| `0x000100FC` | `gfdTextureStreamRead` | `TextureDictionary` |
| `0x000100FD` | `gfdAnimControllerStreamRead` | `AnimationPack` |
| `0x000100FE` | `gfdTexturePathStreamRead` | *(not modelled in C#)* |

`ChunkType000100F8` is a `gfdProperty` (user property dictionary) and
`ChunkType000100F9` is a `gfdPhysicsSector`; both deserve renaming.

---

## `gfdMaterial` → `GFDLibrary.Materials.Material`

Struct `gfdMaterial_tag_0`. Reader `gfdMaterialStreamRead_0` at **`0x8CD69C`**,
writer `gfdMaterialStreamWrite_0` at `0x8CCA44`.

| # | Official field | Type / width | C# member | Conf. |
|---|---|---|---|---|
| 0 | `name` | `gfdName` (u16 len + bytes + u32 hash) | `Name` | high |
| 1 | `flags` | `gfdMaterialFlag` = u32 | `Flags` | high |
| 2 | `ambient` | `gfdRGBAFloat` = 4 × f32 | `LegacyParameters.AmbientColor` | high |
| 3 | `diffuse` | `gfdRGBAFloat` = 4 × f32 | `LegacyParameters.DiffuseColor` | high |
| 4 | `emissive` | `gfdRGBAFloat` = 4 × f32 | `LegacyParameters.EmissiveColor` | high |
| 5 | `specular` | `gfdSpecular` = 4 × f32 | `LegacyParameters.SpecularColor` | high |
| 6 | `reflectivity` | f32 | `LegacyParameters.Reflectivity` | high |
| 7 | `diffusivity` | f32 | `LegacyParameters.Diffusivity` | high |
| 8 | `blend.type` | u8 (u16 pre-`0x1103040`) | `DrawMethod` | high |
| 9 | `blend.srcColor` | u8 (u16 pre-`0x1103040`) | `BlendSourceColor` | high |
| 10 | `blend.dstColor` | u8 (u16 pre-`0x1103040`) | `BlendDestinationColor` | high |
| 11 | `blend.srcAlpha` | u8 (u16 pre-`0x1103040`) | `SourceAlpha` | high |
| 12 | `blend.dstAlpha` | u8 (u16 pre-`0x1103040`) | `DestinationAlpha` | high |
| 13 | `blend.multiple` | u8 (u16 pre-`0x1103040`, absent ≤ `0x108011B`) | `HighlightMapBlendMode` | high |
| 14 | `alpha_test.ref` | u16 | `AlphaClip` | high |
| 15 | `alpha_test.func` | `gfdAlphaFunc` = u16 | `AlphaClipMode` | high |
| 16 | `flags2` | `gfdMaterialFlag2` = u16 (u32 `sort_priority` only, ≤ `0x1104800`) | `Flags2` / `Field94` | high |
| 17 | `sort_priority` | u16 | `SortPriority` | high |
| 18 | `shader.id.type` | u16 | `ShaderId` | high |
| 19 | `shader.id.texcoord.in` | u32 | `TexCoordFlags0` | high |
| 20 | `shader.id.texcoord.out` | u32 | `TexCoordFlags1` | high |
| 21 | `culling` | `gfdCulling` = u16 | `DisableBackfaceCulling` | high |
| 22 | `constant` | u32 (only > `0x1103040`) | `Field98` | high |
| 23 | `textures[0..9]` | 10 × `gfdMaterialTexture`, present per flag bit `0x100000 << i` | `DiffuseMap` .. `TextureMap10` | high |
| 24 | `extension` | `gfdExtension`, present if `flags & 0x10000` | `Attributes` | high |

Notes worth acting on:

- `Field98` is `constant`, the material constant colour (there is a
 `gfdMaterialSetConstantColor` setter). High confidence.
- `HighlightMapBlendMode` is officially `blend.multiple`, the *multi-texture*
 blend mode, matching `gfdMaterialSetMultipleBlendType`. High confidence.
- `DisableBackfaceCulling` is officially just `culling` (a `gfdCulling` mode,
 not a boolean); `gfdMaterialSetCulling` exists. High confidence.
- `AlphaClip` / `AlphaClipMode` are officially `alpha_test.ref` /
 `alpha_test.func`, matching `gfdMaterialSetAlphaTestRef` /
 `gfdMaterialSetAlphaTestFunc`. High confidence.
- `MaterialFlags.HasAttributes` (bit 16) gates `extension`; the ten texture bits
 are 20-29 (`0x100000 << i`), in the same order as our `TextureMaps` list.
 High confidence.
- `Field6C_2` is Metaphor-only (> `0x2110160`) and has no counterpart here.

### `gfdMaterialTexture` → `GFDLibrary.Models.TextureMap`

Struct `gfdMaterialTexture_tag_0`. Read inline inside
`gfdMaterialStreamRead_0` at **`0x8CD69C`**.

| # | Official field | Type / width | Struct offset | C# member | Conf. |
|---|---|---|---|---|---|
| 0 | *(texture name)* | `gfdName` | → resolves `texture` at 0x40 | `Name` | high |
| 1 | `flags` | `gfdMaterialTextureFlag` = u32 | 0x44 | `Flags` | high |
| 2 | `min` | `gfdTextureFilter` = u8 | 0x48 | `MinificationFilter` | high |
| 3 | `mag` | `gfdTextureFilter` = u8 | 0x49 | `MagnificationFilter` | high |
| 4 | `wraps` | `gfdTextureWrap` = u8 | 0x4A | `WrapModeU` | high |
| 5 | `wrapt` | `gfdTextureWrap` = u8 | 0x4B | `WrapModeV` | high |
| 6-21 | `tm` | `VmathMatrix4` = 16 × f32, row-major in 4 groups of 4 | 0x00 | `Field4C` .. `Field88` | high |

The sixteen unnamed floats are a single 4×4 **texture transform matrix** (`tm`).
Our C# offset comments (0x44, 0x48, 0x49, 0x4A, 0x4B, then 0x4C to 0x88) line up
exactly with the official struct offsets for everything but `tm`, which the
runtime keeps at offset 0 while writing it last. The decompiler shows four
`stvx` stores at +0, +16, +32, +48 of four consecutive float reads each.

### `gfdExtensionShaderToon` → `MaterialAttributeType0`

Reader at **`0x9A4598`**, writer at `0x9A43D8`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `lightColor` | `gfdRGBAFloat` = 4 × f32 | `Color` | high |
| 1 | `lightThreshold` | f32 | `LightTreshold` | high |
| 2 | `lightFactor` | f32 | `LightFactor` | high |
| 3 | `shadowBrightness` | f32 | `LightBrightness` | high |
| 4 | `shadowThreshold` | f32 | `ShadowTreshold` | high |
| 5 | `shadowFactor` | f32 | `ShadowFactor` | high |
| 6 | `flags` | u32 (or 3-4 × u8 bools on older versions) | `Type0Flags` | high |

Our `LightBrightness` is officially `shadowBrightness`. On versions
≤ `0x1104220` the field is skipped entirely and defaulted to 0.25.

### `gfdExtensionShaderEdge` → `MaterialAttributeType1`

Reader at **`0x9A4170`**, writer at `0x9A3F98`.

| # | Official field | Type | Conf. |
|---|---|---|---|
| 0 | `lightColor` | 4 × f32 | high |
| 1 | `lightThreshold` | f32 | high |
| 2 | `lightFactor` | f32 | high |
| 3 | `shadowColor` | 4 × f32 | high |
| 4 | `shadowThreshold` | f32 | high |
| 5 | `shadowFactor` | f32 | high |
| 6 | `flags` | u32 (or up to 4 × u8 bools on older versions) | high |

### `gfdExtensionShaderOutline` → `MaterialAttributeType2`

Reader at **`0x9A3ED8`**, writer at `0x9A3D88`.

| # | Official field | Type | Conf. |
|---|---|---|---|
| 0 | `flags` | u32 | high |
| 1 | `palette` | u32 | high |

### `gfdExtensionShaderWater` → `MaterialAttributeType3`

Reader at **`0x9A3C18`**, writer at `0x9A39E0`.

| # | Official field | Type | Conf. |
|---|---|---|---|
| 0 | `waveLength` | `gfdCommonVec3` = 3 × f32 | high |
| 1 | `amplitude` | 3 × f32 | high |
| 2 | `speed` | 3 × f32 | high |
| 3 | `reflectAlpha` | f32 | high |
| 4 | `refractAlpha` | f32 | high |
| 5 | `refractRatio` | f32 | high |
| 6 | `flags` | u32 | high |

### `gfdExtensionShaderShadowEdge` → `MaterialAttributeType4`

Reader at **`0x9A3838`**, writer at `0x9A35B0`.

| # | Official field | Type | Conf. |
|---|---|---|---|
| 0 | `lightColor` | 4 × f32 | high |
| 1 | `lightThreshold` | f32 | high |
| 2 | `lightFactor` | f32 | high |
| 3 | `shadowColor` | 4 × f32 | high |
| 4 | `shadowThreshold` | f32 | high |
| 5 | `shadowFactor` | f32 | high |
| 6 | `darkMapSpeed` | f32 | high |
| 7 | `darkMapPower` | f32 | high |
| 8 | `darkMapScale` | f32 | high |
| 9 | `darkMapHeight` | f32 | high |
| 10 | `darkMapAlpha` | f32 | high |
| 11 | `darkMapDirection` | u8 | high |
| 12 | `darkGradHeight` | f32 | high |
| 13 | `darkGradAlpha` | f32 | high |
| 14 | `flags` | u32 | high |

(`alpha`, the last struct member, is initialised to 1.0 and not streamed.)

---

## `gfdMesh` → `GFDLibrary.Models.Model` (and `ModelPack` chunk loop)

Struct `gfdMesh_tag_0`. Reader `gfdMeshStreamReadInside_0` at **`0x8DCDA8`**
(chunk dispatcher, i.e. our `ModelPack`), writer `gfdMeshStreamWrite_0` at
`0x8DC590`. The `0x00010003` arm is the body that maps to our `Model`.

| # | Official field | Type / width | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdMeshFlag` = u32 | `Model.Flags` | high |
| 1 | *(bone count)* | u32, if `flags & 4` | `Bones.Count` | high |
| 2 | `skinBoneMatrixArray` | boneCount × `VmathMatrix4` (16 × f32) | `Bone.InverseBindMatrix` | high |
| 3 | `skinBoneObject` indices | boneCount × u16 (node index) | `Bone.NodeIndex` | high |
| 4 | `boundingBox` | `gfdBoundingBox` = `max[3]`, `min[3]` f32, if `flags & 1` | `BoundingBox` | high |
| 5 | `boundingSphere` | `gfdBoundingSphere` = `center[3]`, `radius` f32, if `flags & 2` | `BoundingSphere` | high |
| 6 | `hierarchy` | `gfdNode` tree | `RootNode` | high |

`gfdMeshFlag` bits used by the reader: `0x1` bounding box, `0x2` bounding
sphere, `0x4` skinning (bone palette), `0x8` morph object collection. These map
one-for-one onto our `ModelFlags.HasBoundingBox / HasBoundingSphere /
HasSkinning / HasMorphs`. High confidence. Bits `0x3C0` are preserved verbatim
by the reader but drive nothing in it.

`Field100_10` (our byte after the bone palette, version ≥ `0x2040001`) does not
exist in this build.

Note the bounding box field order: the official struct is `max` first, then
`min`, and the reader reads them in that order.

---

## `gfdNode` → `GFDLibrary.Models.Node`

Struct `gfdNode_tag_0`. Reader `gfdNodeStreamRead_0` at **`0x8E7998`**; the
recursive wrapper is `gfdNodeStreamReadHierarchy_0` at `0x8E7CE8` (manually
unrolled eight levels deep before recursing, our `ReadRecursive` is equivalent).
Writer `gfdNodeStreamWrite_0` at `0x8E28BC`.

| # | Official field | Type / width | C# member | Conf. |
|---|---|---|---|---|
| 0 | `name` | `gfdName` | `Name` | high |
| 1 | `transform.translate` | `VmathVector3` = 3 × f32 | `Translation` | high |
| 2 | `transform.rotate` | `VmathQuat` = 4 × f32 | `Rotation` | high |
| 3 | `transform.scale` | `VmathVector3` = 3 × f32 | `Scale` | high |
| 4 | *(unnamed pad)* | u8, only ≤ `0x1090000` | discarded byte | high (presence), low (meaning) |
| 5 | *(object count)* | i32 | `Attachments.Count` | high |
| 6 | `objectHead`/`objectTail` list | N × `gfdObject` (`gfdObjectStreamRead`) | `Attachments` | high |
| 7 | *(has-property flag)* | u8, only > `0x1060000` | `hasProperties` | high |
| 8 | `property` | `gfdProperty` | `Properties` | high |
| 9 | `visibility` | f32, only > `0x1104230` | `FieldE0` | high |

`FieldE0` is officially `visibility` (the node's visibility scalar; the
constructor default of 1.0 in both the runtime and our C# agrees). High
confidence, the struct member is named and lives at node offset 0xE0, which our
own comment already guessed.

The hierarchy wire format is `node`, `childCount` (i32), then that many
hierarchies, recursively.

### `gfdObject` dispatch

`gfdObjectStreamRead_0` at **`0x8F43E0`** reads a u32 `gfdObjectID` and calls
`gfdObjectFunctionTable[id].streamRead`. See the `gfdObjectID` table above.

---

## `gfdGeometry` → `GFDLibrary.Models.Mesh`

Struct `gfdGeometry_tag_1`. Reader `gfdGeometryStreamRead_0` at **`0x8EC754`**,
writer `gfdGeometryStreamWrite_0` at `0x8EA950`.

| # | Official field | Type / width | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdGeometryFlag` = u32 | `Flags` | high |
| 1 | `fvf` | `gfdVertexFormat` = u32 | `VertexAttributeFlags` | high |
| 2 | `numTriangles` | u32, if `flags & 4` | `TriangleCount` | high |
| 3 | `idxf` | `gfdIndexFormat` = u16, if `flags & 4` | `TriangleIndexFormat` | high |
| 4 | `numVertices` | u32 | `VertexCount` | high |
| 5 | `type` | `gfdGeometryType` = u32, only > `0x1103020` | `Field14` | high |
| 6 | vertex buffer | `numVertices × stride` bytes, stride derived from `fvf` | `Vertices`, `Normals` | high |
| 7 | `morph` | `gfdMorphTarget`, if `flags & 0x40` | `MorphTargets` | high |
| 8 | index buffer | `numTriangles × 3` × (u16 if `idxf == 1` else u32), if `flags & 4` | `Triangles` | high |
| 9 | `material` name | `gfdName`, if `flags & 2` | `MaterialName` | high |
| 10 | `boundingBox` | `max[3]`, `min[3]` f32, if `flags & 8` | `BoundingBox` | high |
| 11 | `boundingSphere` | `center[3]`, `radius` f32, if `flags & 0x10` | `BoundingSphere` | high |
| 12 | `lod` start | f32, if `flags & 0x1000` | `LodStart` | high |
| 13 | `lod` end | f32, if `flags & 0x1000` | `LodEnd` | high |

`Field14` is officially `type` (`gfdGeometryType`). High confidence, the
decompiler assigns straight into `pGeometry->type`.

`gfdGeometryFlag` bits exercised by the reader: `0x1` skinning (`skin`), `0x2`
material name, `0x4` triangles, `0x8` bounding box, `0x10` bounding sphere,
`0x20` (passthrough), `0x40` morph, `0x80` (passthrough), `0x1000` LOD range,
`0x4000` (passthrough), `0x40000000` extra vertex usage bit. These match our
`GeometryFlags`. High confidence for the ones with reads attached.

### `gfdVertexFormat` (`fvf`) bits

Stride arithmetic in the same function, plus the two asserts in
`gfdMorphTargetStreamRead` (`"fvf & GFDFVF_XYZ"`,
`"(fvf & (~(GFDFVF_XYZ|GFDFVF_NORMAL))) == 0"` with mask `0xFFFFFFED`).

| Bit | Size added | Official name | C# `VertexAttributeFlags` | Conf. |
|---|---|---|---|---|
| `0x00000002` | 12 | `GFDFVF_XYZ` | `Position` | high (named in assert) |
| `0x00000004` | 16 | *(position4 / XYZW)* |  | medium |
| `0x00000010` | 12 | `GFDFVF_NORMAL` | `Normal` | high (named in assert) |
| `0x00000040` | 4 | *(diffuse colour)* | `Color0` | medium |
| `0x00000100` | 8 | *(texcoord 0)* | `TexCoord0` | medium |
| `0x00000200` | 8 | *(texcoord 1)* | `TexCoord1` | medium |
| `0x00000400` | 8 | *(texcoord 2)* | `TexCoord2` | medium |
| `0x10000000` | 12 | *(tangent)* | `Tangent` | medium |
| `0x20000000` | 12 | *(binormal)* | `Binormal` | medium |
| `0x40000000` | 4 | *(second colour)* | `Color2` | medium |
| `0x80000000` | 20 | *(unknown 20-byte attribute)* |  | low |

Skinned geometry (`flags & 1`) adds 20 bytes per vertex on top of the stride:
4 × f32 weights plus a packed u32 of four byte indices. This matches our
`ReadLegacyVertexWeights`. High confidence.

---

## `gfdMorphTarget` → `GFDLibrary.Models.MorphTargetList`

Struct `gfdMorphTarget_tag_0`. Reader `gfdMorphTargetStreamRead_0` at
**`0x97D844`**, writer at `0x97D394`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `fvf` | `gfdVertexFormat` = u32 | `MorphTargetList.Flags` | high |
| 1 | `numTargets` | i32 | `MorphTargetList.Count` | high |
| 2 | `targets[i]` | `gfdShape` | each `MorphTarget` | high |

### `gfdShape` → `GFDLibrary.Models.MorphTarget`

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `fvf` | u32 | `MorphTarget.Flags` | high |
| 1 | `numVertices` | i32 | `VertexCount` | high |
| 2 | `vertices` | `numVertices × stride` f32, stride 12 (24 if `fvf & NORMAL`) | `Vertices` | high |

Both `Flags` fields are officially `fvf` (vertex format). High confidence; the
default value 2 our C# uses is `GFDFVF_XYZ`.

## `gfdMorphController` → `GFDLibrary.Models.Morph`

Struct `gfdMorphController_tag`. Reader `gfdMorphControllerStreamRead_0` at
**`0x97D05C`**, writer at `0x97CE8C`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `numTargets` | i32 | `TargetCount` | high |
| 1 | `weights` | `numTargets × f32` | `TargetInts` | high |
| 2 | `name` | `gfdName` | `NodeName` | high |

`TargetInts` is officially `weights` and the values are **floats**, not ints 
the reader calls `gfdStreamReadFloatArray` into `pMorphController->weights`.
High confidence; worth fixing in our model.

---

## `gfdTexture` → `GFDLibrary.Textures.Texture`

Struct `gfdTexture_tag_0`. Reader `gfdTextureStreamRead_0` at **`0x8F2CF8`**,
writer `gfdTextureStreamWrite_0` at `0x8F259C`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `name` | u16 length + that many bytes (no hash) | `Name` | high |
| 1 | *(format)* | u16, passed to `gfdTextureCreateFromMemory` | `Format` | medium |
| 2 | *(data size)* | u32 | `Data.Length` | high |
| 3 | *(image data)* | that many bytes | `Data` | high |
| 4 | `min` | `gfdTextureFilter` = u8, only > `0x1050000` | `Field1C` | high |
| 5 | `mag` | `gfdTextureFilter` = u8 | `Field1D` | high |
| 6 | `wraps` | `gfdTextureWrap` = u8 | `Field1E` | high |
| 7 | `wrapt` | `gfdTextureWrap` = u8 | `Field1F` | high |

`Field1C`-`Field1F` are the default sampler state: `min`, `mag`, `wraps`,
`wrapt`. High confidence, the decompiler writes straight into those named
members of `gfdTexture_tag_0`. Our constructor defaults (1, 1, 0, 0) match
`TextureMap`'s defaults for the same four fields.

When a texture of the same name is already in the texture list, the reader
*skips* the record instead of re-reading it; that is also where the "skip 2,
read size, skip size, skip 4" shape confirms the field widths.

---

## `gfdAnimation` → `GFDLibrary.Animations.Animation`

Struct `gfdAnimation_tag_1`. Reader `gfdAnimationStreamRead_0` at **`0x89C390`**,
writer `gfdAnimationStreamWrite_0` at `0x89AE98`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdAnimationFlag` = u32, only > `0x1104110` | `Flags` | high |
| 1 | `duration` | f32 | `Duration` | high |
| 2 | `numJoints` | u32 | `Controllers.Count` | high |
| 3 | `joints` | N × `gfdAnimationJoint` | `Controllers` | high |
| 4 | `effect` | `gfdAnimationEffect`, if `flags & 0x10000000` | `Field10` | high |
| 5 | `neck` | `gfdAnimationNeck`, if `flags & 0x20000000` | `Field14` | high |
| 6 | `translation` | `gfdAnimationTranslation`, if `flags & 0x80000000` | `Field1C` | high |
| 7 | `boundingBox` | `max[3]`, `min[3]` f32, if `flags & 0x40000000` | `BoundingBox` | high |
| 8 | *(property marker)* | no read; `flags & 0x4000000` only copied through | `Properties` | medium |
| 9 | `frequency` | f32, if `flags & 0x2000000` | `Speed` | high |

`Controllers` is officially `joints`; `Speed` is officially `frequency`;
`Field10` is `effect`; `Field14` is `neck`; `Field1C` is `translation`. All
high confidence (named struct members assigned directly).

Flag bit correspondence, all high: `Bit28` = `0x10000000` effect, `Bit29` =
`0x20000000` neck, `Bit31` = `0x80000000` translation, `HasBoundingBox` =
`0x40000000`, `HasSpeed` (`Bit25`) = `0x2000000` frequency, `Bit26` =
`0x4000000`. Note `HasProperties` in our enum is bit 23, but this build reads no
property block at all and instead has a no-op `0x4000000` (bit 26) arm, so our
bit-23 property handling is unverified here.

Also note the ordering: this build reads boundingBox **before** frequency, which
is what our C# does.

### `gfdAnimationJoint` → `GFDLibrary.Animations.AnimationController`

Read inline in `gfdAnimationStreamRead_0` at **`0x89C390`** (32-byte entries).

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `type` | `gfdAnimJointType` = u16 | `TargetKind` | high |
| 1 | `id` | u32 | `TargetId` | high |
| 2 | `name` | `gfdName` | `TargetName` | high |
| 3 | `numKeyLists` | u32 | `Layers.Count` | high |
| 4 | `keyLists` | N × `gfdKeyList` | `Layers` | high |

Our class name `AnimationController` is a misnomer: officially this is a
*joint*, and `gfdAnimController` is a different thing entirely (our
`AnimationPack`). `TargetKind` is `type` (`gfdAnimJointType`).

The reader also ORs `1 << (type - 1)` into `animation->flags`, which is where
the low `AnimationFlags.Bit0`/`Bit1` bits come from.

### `gfdKeyList` → `GFDLibrary.Animations.AnimationLayer`

Struct `gfdKeyList_tag_0`. Reader `gfdKeyListStreamRead_0` at **`0x896638`**,
writer `gfdKeyListStreamWrite_0` at `0x896440`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `type` | `gfdKeyType` = u32 (asserted ≤ 30) | `KeyType` | high |
| 1 | `numKeys` | u32 | `Keys.Count` | high |
| 2 | `times` | `numKeys × f32` | key timings | high |
| 3 | `keys` | `numKeys ×` the type's `streamRead` | `Keys` | high |
| 4 | `diff.t` | 3 × f32, only when the type has a `decompBlend` | `PositionScale` | high |
| 5 | `diff.s` | 3 × f32, same condition | `ScaleScale` | high |

`PositionScale`/`ScaleScale` are officially `diff.t`/`diff.s` of a
`gfdKeyMaxDiff` ("max difference" used to decompress the quantised TR/TRS keys).
High confidence.

### Key payload layouts

| Official struct | Reader | Members | Conf. |
|---|---|---|---|
| `gfdKeyTR` | `0x89420C` | `t[3]`, `q[4]` f32 | high |
| `gfdKeyTRS` | `0x89413C` | `t[3]`, `q[4]`, `s[3]` f32 | high |
| `gfdKeyCompressTR` | `0x8939C0` | 7 × u16 (3 translate, 4 quaternion) | high |
| `gfdKeyCompressTRS` | `0x8938F0` | 10 × u16 (3 translate, 4 quaternion, 3 scale) | high |
| `gfdKeyTRANSFORM` | `0x894544` | `gfdKeyTRS data` then `interp` u8 | high |
| `gfdKeyUV` | `0x893F84` | `offset[2]`, `tile[2]`, `angle` f32 | high |
| `gfdKeyRGB` | `0x8940DC` | `rgb[3]` f32 | high |
| `gfdKeyRGBA` | `0x89406C` | `rgba[4]` f32 | high |
| `gfdKeyFLOAT` | `0x894038` | `f` f32 | high |
| `gfdKeyALPHA` | `0x895794` | `data` f32 then `interp` u8 | high |
| `gfdKeyCOLOR` | `0x896C10` | `data` 3 × f32 then `interp` u8 | high |
| `gfdKeyMOTION` | `0x894448` | `type` u32, `data` (u32, u16, 3 × f32), `next` (u32, u16, 3 × f32), `interp` u8 | high |

Note `gfdKeyTRANSFORM`, `gfdKeyALPHA` and `gfdKeyCOLOR` declare `interp` *first*
in the struct but stream it *last*.

### `gfdAnimController` → `GFDLibrary.Animations.AnimationPack`

Struct `gfdAnimController_tag_0`. Reader `gfdAnimControllerStreamRead_0` at
**`0x891F34`**, writer at `0x88EC04`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdAnimControllerFlag` = u32, only > `0x1104950` | `Flags` | high |
| 1 | *(base count)* | u32 | `Animations.Count` | high |
| 2 | `baseArray` | N × `gfdAnimation` | `Animations` | high |
| 3 | *(additional count)* | u32 | `BlendAnimations.Count` | high |
| 4 | `additionalArray` | N × `gfdAnimation` | `BlendAnimations` | high |
| 5 | `neck` | `gfdAnimationNeck`, if `flags & 4` | `Bit29Data` | high |

`AnimationPackFlags.Bit2` (`0x4`) is the `neck` flag; `BlendAnimations` is
officially `additionalArray`. High confidence.

### `gfdAnimationNeck` → `AnimationBit29Data`

Struct `gfdAnimationNeck_tag_0`. Read inline at **`0x89C390`** and **`0x891F34`**.
Interleaved on the wire: animation, angle, animation, angle

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `animationList[0]` | `gfdAnimation` | `Field00` | high |
| 1 | `angleList[0]` | f32 | `Field10` | high |
| 2 | `animationList[1]` | `gfdAnimation` | `Field04` | high |
| 3 | `angleList[1]` | f32 | `Field14` | high |
| 4 | `animationList[2]` | `gfdAnimation` | `Field08` | high |
| 5 | `angleList[2]` | f32 | `Field18` | high |
| 6 | `animationList[3]` | `gfdAnimation` | `Field0C` | high |
| 7 | `angleList[3]` | f32 | `Field1C` | high |

### `gfdAnimationTranslation` → `AnimationBit31Data`

Struct `gfdAnimationTranslation_tag_0`. Read inline at **`0x89C390`**.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `id` | u32 | `Field00` | high |
| 1 | `name` | `gfdName` | `Field04` | high |
| 2 | `keyList` | `gfdKeyList` (initialised to type 1 = `gfdKeyTR`) | `Field20` | high |

### `gfdAnimationEffect` → `List<AnimationBit28DataEntry>`

Struct `gfdAnimationEffect_tag` (a `gfdItemArray` of 20-byte entries). Read
inline at **`0x89C390`**.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | *(count)* | u32 | list count | high |
| 1 | entry `+0x00` | `gfdEPL *` | `Field00` | high |
| 2 | entry `+0x04` | `gfdName` | `Field04` | high |

---

## `gfdProperty` / `gfdPropertyChunk` → `UserPropertyDictionary` / `UserProperty`

Structs `gfdProperty_tag_0`, `gfdPropertyChunk_tag_0`. Reader
`gfdPropertyStreamRead_0` at **`0x876480`**, writer at `0x875D60`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | *(chunk count)* | u32 | `Count` | high |
| 1 | `type` | `gfdPropertyType` = u32 | `UserProperty.ValueType` | high |
| 2 | `name` | `gfdName` | `UserProperty.Name` | high |
| 3 | `size` | i32 | `size` | high |
| 4 | `data` | `gfdPropertyData` union, per `type` | `UserProperty` value | high |

Union members (`gfdPropertyData_tag`) and the asserted sizes:

| `type` | Union member | Size assert | C# `UserPropertyValueType` | Conf. |
|---|---|---|---|---|
| 1 | `i` (i32) | 4 | `Int` | high |
| 2 | `f` (f32) | 4 | `Float` | high |
| 3 | `b` (u8) | 1 | `Bool` | high |
| 4 | `s` (char*) | reads `size - 1` bytes + NUL | `String` | high |
| 5 | `c3` (`gfdRGB`, 3 × u8) | 3 | `ByteVector3` | high |
| 6 | `c4` (`gfdRGBA`, 4 × u8) | 4 | `ByteVector4` | high |
| 7 | `v3` (`gfdCommonVec3`, 3 × f32) | 12 | `Vector3` | high |
| 8 | `v4` (`gfdCommonVec4`, 4 × f32) | 16 | `Vector4` | high |
| 9 | `p` (void*) | reads `size` bytes | `ByteArray` | high |

`UserPropertyValueType.ByteVector3`/`ByteVector4` are officially *colours*
(`gfdRGB` / `gfdRGBA`), not generic byte vectors. High confidence.

---

## `gfdName` → `ResourceReader.ReadStringWithHash`

Struct `gfdName_tag`: `flags`, `string`, `length`, `hash`. Reader
`gfdNameStreamRead_0` at **`0x8792D0`**, writer at `0x879068`.

| # | Official field | Type | Conf. |
|---|---|---|---|
| 0 | `length` | u16 | high |
| 1 | `string` | that many bytes, NUL-terminated in memory | high |
| 2 | `hash` | u32, read from the stream only above `0x1080000`-ish; below that recomputed with `gfdHash32CRCCalc`, and in one middle band read-and-discarded then recomputed | high |

The hash is a CRC32 over the string bytes (`gfdHash32CRCCalc_0` at the call
site). Our `StringHasher` should match that.

---

## `gfdPhysicsSector` → `GFDLibrary.Misc.ChunkType000100F9`

Struct `gfdPhysicsSector_tag_0`. Reader `gfdPhysicsSectorStreamRead_0` at
**`0x94CD64`**, writer at `0x949118`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `numIteration` | i32 | `Iterations` | high |
| 1 | `deceleration` | f32 | `Deceleration` | high |
| 2 | `maxSteps` | f32 | `MaxSteps` | high |
| 3 | `tolTransfers` | f32 | `TransferTolerance` | high |
| 4 | `tolVelocity` | f32 | `VelocityTolerance` | high |
| 5 | *(particle count)* | i32 | `entry1Count` | high |
| 6 | *(collision count)* | i32 | `entry2Count` | high |
| 7 | *(particle-constraint count)* | i32 | `entry3Count` | high |
| 8 | `particleArray` | N particles | `ChunkType000100F9Entry1` list | high |
| 9 | `collisionArray` | N collisions | `ChunkType000100F9Entry2` list | high |
| 10 | `particleConstraintArray` | N constraints | `ChunkType000100F9Entry3` list | high |

Our class names should become `PhysicsSector`, `PhysicsParticle`,
`PhysicsCollision` and `PhysicsParticleConstraint`. The asserts name the arrays
and their limits directly, e.g.
`"gfdItemArrayGetCount(pSector->particleArray) < GFDPHYSICSSECTOR_PARTICLE_MAX"`.

Particle fields (Entry1), in file order:

| # | Official offset | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `+0x34` | f32 | `SphereRadius` | high (position), low (official name) |
| 1 | `+0x38` | f32 | `Mass` | high / low |
| 2 | `+0x3C` | f32, only > `0x1104120` | `RestoringForce` | high / low |
| 3 | `+0x40` | f32, only > `0x1104120` | `WindRate` | high / low |
| 4 |  | u8 "has node name" |  | high |
| 5 | `name` → `+0x44` node ptr | `gfdName` | `NodeName` | high |
| 5' | `+0x10,+0x14,+0x18` | 3 × f32 (when no node name) | `Field10`, `Field08`, `Field04` | high (order), low (names) |

Note that when there is no node name, the three floats stream in the order our
C# already uses (`Field10`, `Field08`, `Field04`), but they are consecutive
fields of one `VmathVector3` at `+0x10` in the runtime, so our three names
should become one vector. Medium confidence.

Collision fields (Entry2): u16 shape kind (`0` sphere → one f32 radius;
`1` capsule → two f32), then a 4×4 f32 transform, then a u8 "has node name" and
an optional `gfdName`. High confidence for the shapes; the runtime calls
`gfdPhysicsSectorAllocSphereCollisionInside` / `...AllocCapsuleCollisionInside`.

Constraint fields (Entry3): f32, f32, f32 (only > `0x1104120`; otherwise derived
from the two particles' radii), u16 particle index A, u16 particle index B. High
confidence for the two u16 particle indices, the reader uses them to index
`particleArray`.

---

## `gfdCamera` → `GFDLibrary.Cameras.Camera`

Reader `gfdCameraStreamRead_0` at **`0x8D0AF8`**, writer at `0x8D0028`.
`gfdCamera_tag` is opaque in the type library, so names here come from the
setters (`gfdCameraSetViewTransform`, `gfdCameraSetNearClipPlane`,
`gfdCameraSetFarClipPlane`, `gfdCameraSetFovy`, `gfdCameraSetAspect`,
`gfdCameraSetRoll`).

| # | Official field | Type | Offset | C# member | Conf. |
|---|---|---|---|---|---|
| 0 | view transform | `VmathMatrix4` = 16 × f32 | +0x10 | `ViewMatrix` | medium |
| 1 | near clip plane | f32 (default 1.0) | +0x180 | `ClipPlaneNear` | medium |
| 2 | far clip plane | f32 (default 10000.0) | +0x184 | `ClipPlaneFar` | medium |
| 3 | `fovy` | f32 (default 55.0) | +0x188 | `FieldOfView` | medium |
| 4 | aspect | f32 (default 1.7777778) | +0x18C | `AspectRatio` | medium |
| 5 | roll | f32, only > `0x1104060` | +0x190 | `Field190` | medium |

`Field190` is the camera **roll** (the only remaining camera scalar with a
setter, and the only field read under that version gate). Medium confidence 
inferred from the setter list and the offset, not from a named struct member.
`Field198`/`Field19C`/`Field1A0` are later-version additions absent here.

---

## `gfdLight` → `GFDLibrary.Lights.Light`

Reader `gfdLightStreamRead_0` at **`0x8C3E24`**, writer at `0x8C39C0`.
`gfdLight_tag` is opaque; names below come from the setters
(`gfdLightSetAmbient`, `SetDiffuse`, `SetSpecular`, `SetDirection`,
`SetPosition`, `SetCone`, `SetAttenuation`, `SetAttenuationDistance`,
`SetScale`, `SetAlpha`).

| # | Official field | Type | Offset | C# member | Conf. |
|---|---|---|---|---|---|
| 0 | `flags` | u32, only > `0x1104190` | +0x7C | `Flags` | high |
| 1 | `type` | `gfdLightType` = u32 |  | `Type` | high |
| 2 | ambient | 4 × f32 | +0x30 | `AmbientColor` | medium |
| 3 | diffuse | 4 × f32 | +0x40 | `DiffuseColor` | medium |
| 4 | specular | 4 × f32 | +0x50 | `SpecularColor` | medium |
| 5a | *(type 1 and 3)* 3 × f32 | f32 ×3 | +0x20,+0x24,+0x28 | `Field20`, `Field04`, `Field08` | high (order), low (names) |
| 5b | *(type 3 only)* cone inner/outer | 2 × f32 | +0x74,+0x78 | `AngleInnerCone`, `AngleOuterCone` | medium |
| 5c | *(type 2 and 3)* 3 × f32 | f32 ×3 | +0x10,+0x14,+0x18 | `Field10`, `Field04`, `Field08` | high (order), low (names) |
| 6a | attenuation start/end, if `flags & 2` | 2 × f32 | +0x6C,+0x70 | `AttenuationStart`, `AttenuationEnd` | medium |
| 6b | attenuation coefficients otherwise | 3 × f32 | +0x60,+0x64,+0x68 | `Field60`, `Field64`, `Field68` | medium |

`gfdLightType`: 1 = directional, 2 = point, 3 = spot (the `type != 2 / != 3 /
== 1` cascade plus the setter semantics). Medium confidence; our
`LightType.Type1`/`Point`/`Spot` already encode that.

Branch 6b is almost certainly the classic constant/linear/quadratic attenuation
triple used by `gfdLightSetAttenuation`, versus the distance pair used by
`gfdLightSetAttenuationDistance` under flag `0x2`. Medium confidence.

`Field98`/`Field9C` in our C# are later-version additions absent here.

---

## Serialiser address index

Every `gfd*StreamRead` / `gfd*StreamWrite` pair relevant to the C# model, so a
reader can go straight to the evidence. All are the `_0` copies in `SW.ELF`.

| Structure | `StreamRead` | `StreamWrite` |
|---|---|---|
| `gfdMesh` (our `Model`/`ModelPack`) | `0x8E0C88` (inside: `0x8DCDA8`) | `0x8DC590` |
| `gfdNode` | `0x8E7998` (hierarchy: `0x8E7CE8`) | `0x8E28BC` |
| `gfdObject` | `0x8F43E0` | `0x8F4318` |
| `gfdGeometry` (our `Mesh`) | `0x8EC754` | `0x8EA950` |
| `gfdMaterial` | `0x8CD69C` | `0x8CCA44` |
| `gfdExtension` | `0x94DD94` | `0x94DC68` |
| `gfdExtensionShaderToon` | `0x9A4598` | `0x9A43D8` |
| `gfdExtensionShaderEdge` | `0x9A4170` | `0x9A3F98` |
| `gfdExtensionShaderOutline` | `0x9A3ED8` | `0x9A3D88` |
| `gfdExtensionShaderWater` | `0x9A3C18` | `0x9A39E0` |
| `gfdExtensionShaderShadowEdge` | `0x9A3838` | `0x9A35B0` |
| `gfdTexture` | `0x8F2CF8` | `0x8F259C` |
| `gfdTexturePath` | `0x8F36D4` |  |
| `gfdMorphTarget` | `0x97D844` | `0x97D394` |
| `gfdMorphController` | `0x97D05C` | `0x97CE8C` |
| `gfdShape` | `0x97DA3C` | `0x97CDF4` |
| `gfdAnimation` | `0x89C390` | `0x89AE98` |
| `gfdAnimController` (our `AnimationPack`) | `0x891F34` | `0x88EC04` |
| `gfdKeyList` (our `AnimationLayer`) | `0x896638` | `0x896440` |
| `gfdProperty` | `0x876480` | `0x875D60` |
| `gfdName` | `0x8792D0` | `0x879068` |
| `gfdPhysicsSector` | `0x94CD64` | `0x949118` |
| `gfdCamera` | `0x8D0AF8` | `0x8D0028` |
| `gfdLight` | `0x8C3E24` | `0x8C39C0` |
| `gfdEPL` | `0x8A7310` | `0x8A3FE8` |
| `gfdEPLLeaf` (dispatch) | see `gfdEPL*StreamRead` family |  |
| `gfdScene` | `0x8C043C` | `0x8BDED4` |
| `gfdEnvironment` | `0x8AE908` | `0x8ACA98` |
| `gfdTimeLine` | `0x88ABAC` | `0x88A214` |
| `gfdNavigation` | `0x99E88C` | `0x99E0AC` |
| `gfdVertexTerrain` | `0x97760C` | `0x976F80` |
| `gfdAsset` | `0x88384C` | `0x883214` |
| `gfdSceneFieldMeshParams` | `0x8BEEEC` | `0x8BC3A8` |

Not covered in detail above but present with symbols: the whole `gfdEPL*` leaf
family (roughly 60 reader/writer pairs, one per effect leaf type), which maps
onto `GFDLibrary/Effects/`. Those are worth a follow-up pass using the same
method; the leaf type names are all in the symbol table
(`gfdEPLPolygonGlitterCylinder`, `gfdEPLPostEffectRadialBlur`, and so on).

## Low-confidence items, collected

Everything below rests on position or on an inferred role, not a symbol:

- Every "semantic" `KeyType` name (`CameraFieldOfView`, `MaterialSingle_4`,
 `Single5Alt`). Only the payload struct per value is certain.
- `gfdVertexFormat` bit names other than `GFDFVF_XYZ` (`0x2`) and
 `GFDFVF_NORMAL` (`0x10`); and the meaning of the 20-byte `0x80000000`
 attribute entirely.
- The light field triples at `+0x10`/`+0x20` (our `Field04`, `Field08`,
 `Field10`, `Field20`).
- Physics particle `SphereRadius`, `Mass`, `RestoringForce`, `WindRate`, the
 file order and widths are certain, the names are our existing guesses.
- `gfdMaterialFlag`, `gfdMaterialFlag2`, `gfdGeometryFlag`, `gfdMeshFlag` and
 `gfdAnimationFlag` bit *names*. The bits that drive reads are confirmed by
 behaviour; the rest of our enum names are not corroborated by this image (no
 `GFD_*_FLAG_*` strings survive in it).
- The `gfdNode` pad byte read on versions ≤ `0x1090000`.
