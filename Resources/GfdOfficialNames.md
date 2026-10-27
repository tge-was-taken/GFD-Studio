# GFD official type and member names

Recovered from a Persona 5 BETA PS3 executable that still carries symbols and
Hex-Rays type information for the whole `gfd` runtime.

- IDB: `SW.ELF` (`E:\Projects\p5_2014\SW.ELF`), IDA MCP instance on port 13339.
- All addresses below are from that image.

## What has been applied

The renames carried into the code so far are the ten texture slots and their flags, the
material `constant`, `culling` and multi blend mode, and the toon attribute thresholds. Each
one keeps its old name as a `[FormerName]` alias, so YAML written before the rename still
loads. Everything else below is still named `Field*` or `Bit*` in the code.

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
| 13 | `blend.multiple` | u8 (u16 pre-`0x1103040`, absent ≤ `0x108011B`) | `MultiBlendMode` | high |
| 14 | `alpha_test.ref` | u16 | `AlphaClip` | high |
| 15 | `alpha_test.func` | `gfdAlphaFunc` = u16 | `AlphaClipMode` | high |
| 16 | `flags2` | `gfdMaterialFlag2` = u16 (u32 `sort_priority` only, ≤ `0x1104800`) | `Flags2` / `Field94` | high |
| 17 | `sort_priority` | u16 | `SortPriority` | high |
| 18 | `shader.id.type` | u16 | `ShaderId` | high |
| 19 | `shader.id.texcoord.in` | u32 | `TexCoordFlags0` | high |
| 20 | `shader.id.texcoord.out` | u32 | `TexCoordFlags1` | high |
| 21 | `culling` | `gfdCulling` = u16 | `Culling` | high |
| 22 | `constant` | u32 (only > `0x1103040`) | `ConstantColor` | high |
| 23 | `textures[0..9]` | 10 × `gfdMaterialTexture`, present per flag bit `0x100000 << i` | `DiffuseMap` .. `ReserveMap` | high |
| 24 | `extension` | `gfdExtension`, present if `flags & 0x10000` | `Attributes` | high |

Notes worth acting on:

- `ConstantColor` (was `Field98`) is `constant`, the material constant colour (there is a
 `gfdMaterialSetConstantColor` setter). High confidence.
- `MultiBlendMode` (was `HighlightMapBlendMode`) is officially `blend.multiple`, the *multi-texture*
 blend mode, matching `gfdMaterialSetMultipleBlendType`. High confidence.
- `Culling` (was `DisableBackfaceCulling`) is officially just `culling` (a `gfdCulling` mode,
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
| 1 | `lightThreshold` | f32 | `LightThreshold` | high |
| 2 | `lightFactor` | f32 | `LightFactor` | high |
| 3 | `shadowBrightness` | f32 | `ShadowBrightness` | high |
| 4 | `shadowThreshold` | f32 | `ShadowThreshold` | high |
| 5 | `shadowFactor` | f32 | `ShadowFactor` | high |
| 6 | `flags` | u32 (or 3-4 × u8 bools on older versions) | `Type0Flags` | high |

Our `ShadowBrightness` (was `LightBrightness`) is officially `shadowBrightness`. On versions
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

**Corrected and upgraded.** The first pass derived most of these from stride
arithmetic and marked them medium. They are now named outright by the game's
own debug printer, `gfdThJobGeometryCreateJobData_0` at **`0x91D8D0`**, which
prints an fvf as text with one explicit bit test per name:

```
if ( (fvf & 0x00000002) != 0 )  gfdPrintf_0("XYZ");
if ( (fvf & 0x00000010) != 0 )  gfdPrintf_0(" | NORMAL");
if ( (fvf & 0x00000040) != 0 )  gfdPrintf_0(" | DIFFUSE");
if ( (fvf & 0x20000000) != 0 )  gfdPrintf_0(" | TANGENT");
if ( (fvf & 0x10000000) != 0 )  gfdPrintf_0(" | BINORMAL");
if ( (fvf & 0x00000100) != 0 )  gfdPrintf_0(" | TEXCOORD0");
if ( (fvf & 0x00000200) != 0 )  gfdPrintf_0(" | TEXCOORD1");
if ( (fvf & 0x00000400) != 0 )  gfdPrintf_0(" | TEXCOORD2");
```

That block appears twice in the function and both copies agree. A second,
independent printer in `gfdEditorGeometry.c` carries the same set as the string
run at `0xF23E78`: `|NORMAL`, `|DIFFUSE`, `|TANGENT`, `|BINORMAL`, `|TEX0`,
`|TEX1`, `|TEX2`.

**The first pass had tangent and binormal the wrong way round.** `0x20000000`
is TANGENT and `0x10000000` is BINORMAL, not the reverse.

| Bit | Size added | Official name | C# `VertexAttributeFlags` | Conf. |
|---|---|---|---|---|
| `0x00000002` | 12 | `XYZ` (`GFDFVF_XYZ`) | `Position` | high |
| `0x00000004` | 16 | *(position4 / XYZW)* |  | medium |
| `0x00000010` | 12 | `NORMAL` (`GFDFVF_NORMAL`) | `Normal` | high |
| `0x00000040` | 4 | `DIFFUSE` | `Color0` | high |
| `0x00000100` | 8 | `TEXCOORD0` | `TexCoord0` | high |
| `0x00000200` | 8 | `TEXCOORD1` | `TexCoord1` | high |
| `0x00000400` | 8 | `TEXCOORD2` | `TexCoord2` | high |
| `0x10000000` | 12 | `BINORMAL` | `Binormal` | high |
| `0x20000000` | 12 | `TANGENT` | `Tangent` | high |
| `0x40000000` | 4 | *(second colour)* | `Color2` | medium |
| `0x80000000` | 20 | *(unknown 20-byte attribute)* |  | low |

`0x2` and `0x10` keep their `GFDFVF_` spellings from the two asserts in
`gfdMorphTargetStreamRead` (`"fvf & GFDFVF_XYZ"`,
`"(fvf & (~(GFDFVF_XYZ|GFDFVF_NORMAL))) == 0"`, mask `0xFFFFFFED`); the printer
agrees on the bits.

#### Layout order is not print order

The printer names the tangent before the binormal, but in the vertex the
binormal comes first. The evidence for that is not the stride loop in
`gfdGeometryStreamRead_0`, which only accumulates a total and so proves nothing
about order. It is `gfdDeviceRenderTransferImDataToVertexBuffer_0` at
**`0x833228`**, which builds the vertex element descriptors by testing the bits
in sequence and assigning each a consecutive slot:

```
fvf & 0x2 -> fvf & 0x10 -> fvf & 0x10000000 -> fvf & 0x20000000
  -> fvf & 0x40 -> fvf & 0x100 -> fvf & 0x200 -> fvf & 0x400
  -> fvf & 0x40000000 -> fvf & 0x80000000
```

So the in-vertex order is position, normal, **binormal, tangent**, diffuse,
texcoord0-2. High confidence: this is the function that defines the layout,
not a side effect of how a size was summed.

Note also that round-tripping a file byte for byte does **not** validate this
ordering on its own. If the reader and the writer agree, any consistent order
round trips. The transfer function is what settles it.


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
- *(resolved in the third pass: every `gfdVertexFormat` bit except
 `0x4`, `0x40000000` and `0x80000000` is now named by the debug printer at
 `0x91D8D0`. The meaning of the 20-byte `0x80000000` attribute is still
 unknown.)*
- The light field triples at `+0x10`/`+0x20` (our `Field04`, `Field08`,
 `Field10`, `Field20`).
- Physics particle `SphereRadius`, `Mass`, `RestoringForce`, `WindRate`, the
 file order and widths are certain, the names are our existing guesses.
- `gfdMaterialFlag`, `gfdMaterialFlag2`, `gfdGeometryFlag`, `gfdMeshFlag` and
 `gfdAnimationFlag` bit *names*. The bits that drive reads are confirmed by
 behaviour; the rest of our enum names are not corroborated by this image (no
 `GFD_*_FLAG_*` strings survive in it).
- The `gfdNode` pad byte read on versions ≤ `0x1090000`.

---
---

# Second pass: the Epl family, flag enumerations, and leftovers

Same IDB (`SW.ELF`, port 13339), same confidence rules.

## The key that unlocks the Epl family

Our `GFDLibrary/Effects/` classes name their placeholders `FieldNNN` where
`NNN` is a hex **struct offset**, and those offsets are the offsets of the
official `gfdEPL*Params` structs in the IDB's type library, member for member.
So the mapping is mechanical rather than inferred: look up the offset, read the
member name off the struct.

Worked example, `gfdEPLPolygonGlitterStreamRead_0` at `0x95E450`. The reader
stores into `+0`, `+4`, `+8`, `+32`, `+336`, `+12`, `+20`, `+236`, `+28`,
`+36`, `+136`, `+320`, `+324`, `+328`, `+332`, in that order. The struct
`gfdEPLPolygonGlitterBasicParams` (size `0x154`) declares `flags` at `0x000`,
`life` at `0x004`, `number` at `0x008`, `count` at `0x00C`, `fade` at `0x014`,
`random` at `0x01C`, `blend` at `0x020`, `rgbaIn` at `0x024`, `rgbaOut` at
`0x088`, `scale` at `0x0EC`, `repeatTexV` at `0x140`, `moveTexV` at `0x144`,
`adjust` at `0x148`, `seed` at `0x150`. Our `EplGlitterPolygon` reads
`Field00`, `Field04`, `Field08`, `Field20`, `Field150`, `Field08_`, `Field14`,
`FieldEC`, `Field1C`, `Field24`, `Field88`, `Field140`, `Field144`, `Field148`,
`Field14C` in exactly that order. Every offset lines up.

Because of this, the Epl tables below are **high** confidence for any row where
the C# offset matches a declared struct member. Rows where our C# offset does
not land on a member (a later-version field, or a mis-numbered placeholder) are
marked explicitly.

### Three shared building blocks

These three recur everywhere and are worth naming once.

| Official | Size | C# | Confidence |
|---|---|---|---|
| `gfdCurve` | `0x54` | `EplLeafCommonData` | high |
| `gfdCurve4` | `0x64` | `EplLeafCommonData2` | high |
| `gfdRange` / `gfdFade` / `gfdEPLAdjustment` | `0x8` each | read as `Vector2` | high |

`gfdCurve` (`gfdCurve_tag`): `type` (`gfdCurveType`, u16) at `0x00`, `start`
(`gfdCurveTarget`) at `0x04`, `end` at `0x0C`, `point[2]` (`gfdCurvePoint`,
4 × f32) at `0x14`, `table[23]` (u16, 46 bytes) at `0x24`.

`gfdCurve4` (`gfdCurve4_tag`): `type` at `0x00`, `target[4]` at `0x04`, `0x0C`,
`0x14`, `0x1C`, `point[2]` at `0x24`, `table[23]` at `0x34`.

`gfdCurveTarget` is a union: `i` (i32) / `f` (f32) / `c` (u32) / `r`
(`gfdRange`, two floats). That is exactly our `EplLeafCommonData` switch on
`Type`: cases 0/1/2 read one scalar per target, case 3 reads a `gfdRange` per
target. High confidence.

`gfdRange` is `{datums, range}`; `gfdFade` is `{in, out}`;
`gfdEPLAdjustment` is `{scale, speed}`. All high.

Mapping for `EplLeafCommonData` → `gfdCurve`:

| # | Official field | C# member | Conf. |
|---|---|---|---|
| 0 | `type` (u16) | `Type` | high |
| 1 | `start.i/f/c` (or `start.r.datums`) | `Field04` | high |
| 2 | `start.r.range` (type 3 only) | `Field08` | high |
| 3 | `end.i/f/c` (or `end.r.datums`) | `Field0C` | high |
| 4 | `end.r.range` (type 3 only) | `Field10` | high |
| 5 | `point[2]` (4 × f32) | `Field14` | high |
| 6 | `table[23]` (46 bytes) | `Field24` | high |

Mapping for `EplLeafCommonData2` → `gfdCurve4`: `Field04`/`Field0C`/`Field14`/
`Field1C` are `target[0..3]` (with `Field08`/`Field10`/`Field18`/`Field20` the
second half of each when `type == 3`), `Field14_` is `point[2]`, `Field24` is
`table[23]`. All high. Note our `Field24` name is wrong by `0x10`: the table
lives at `0x34` in `gfdCurve4`, not `0x24`. `Field62` is a later-version
addition past the end of `gfdCurve4` and has no counterpart here.

## Epl top level

### `gfdEPL` → `GFDLibrary.Effects.Epl`

Struct `gfdEPL_tag_0`. Reader `gfdEPLStreamReadInside_0` at **`0x8A6F20`**
(outer entry `gfdEPLStreamRead_0` at `0x8A7310`), writer `gfdEPLStreamWrite_0`
at `0x8A3FE8`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdEPLFlag` = u32 (reader ORs in `4`) | `Flags` | high |
| 1 | `hierarchy` | `gfdNode` tree | `RootNode` | high |
| 2 | `timeLine` | `gfdTimeLine` | `Animation` | high |
| 3 | n/a | u16, only > `0x1105060` | `Field40` | low (absent here) |
| 4 | n/a | 2 × `Vector4`, only > `0x2110207` | `Field60`, `Field70` | low (absent here) |

Our `Epl.Animation` is officially the `timeLine`, read by
`gfdTimeLineStreamRead_0` (`0x88ABAC`). `leafArray` is not streamed: the reader
rebuilds it by collecting `gfdEPLLeaf` objects (object id 8) out of the node
hierarchy. High confidence.

### `gfdEPLLeaf` → `EplLeaf`

Struct `gfdEPLLeaf_tag`. Reader `gfdEPLLeafStreamRead_0` at **`0x8A4A70`**,
writer at `0x8A39F8`.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | n/a | 2 × `Vector2`, only > `0x2110060` | `Field34`, `Field3C` | low (absent here) |
| 1 | `flags` | `gfdEPLLeafFlag` = u32 | `Flags` | high |
| 2 | `name` | `gfdName` | `Name` | high |
| 3 | `parts` | `gfdEPLParts` | `Data` | high |

### `gfdEPLParts` → `EplLeafDataHeader`

Struct `gfdEPLParts_tag`. Reader `gfdEPLPartsStreamRead_0` at **`0x8A36DC`**.

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `category` | `gfdEPLCategory` = u32, asserted `<= 0x11` | `Type` | high |
| 1 | `type` | `gfdEPLType` = u32 | `Field04` | high |
| 2 | `handle` | the category's `streamRead` result | the leaf body | high |

Our `EplLeafDataHeader.Type` is officially `category` (which leaf kind) and
`Field04` is `type` (which variant within it). Note that each category's own
reader then reads the variant **again** as its first u32, which is why our
`EplXxx.Type` immediately follows the header. High confidence.

### `gfdEPLCategory` → `EplLeafType`

From `gfdEPLPartsCategoryFuncTable` at `0x1470F34` (84 bytes per entry; the
first dword is a name string). Our enum matches one for one.

| Value | Official name string | Official category struct | C# `EplLeafType` | Conf. |
|---|---|---|---|---|
| 0 | `nop` (no handlers) | n/a | `Dummy` | high |
| 1 |  | `gfdEPLParticle` | `Particle` | high |
| 2 |  | `gfdEPLPolygonFlash` | `FlashPolygon` | high |
| 3 |  | `gfdEPLPolygonCircle` | `CirclePolygon` | high |
| 4 |  | `gfdEPLPolygonThunder` | `LightningPolygon` | high |
| 5 |  | `gfdEPLPolygonTrack` | `TrajectoryPolygon` | high |
| 6 |  | `gfdEPLPolygonWind` | `WindPolygon` | high |
| 7 |  | `gfdEPLMesh` | `Model` | high |
| 8 |  | `gfdEPLPolygonTrack` (alias of 5) | `TrajectoryPolygon2` | high |
| 9 |  | `gfdEPLPolygonBoard` | `BoardPolygon` | high |
| 10 |  | `gfdEPLObjectParticle` | `ObjectPartciles` | high |
| 11 |  | `gfdEPLPolygonGlitter` | `GlitterPolygon` | high |
| 12 |  | `gfdEPLPolygonGlitter` (alias of 11) | `GlitterPolygon2` | high |
| 13 |  | `gfdEPLDirectionalParticle` | `DirectionalParticles` | high |
| 14 |  | `gfdEPLCamera` | `Camera` | high |
| 15 |  | `gfdEPLLight` | `Light` | high |
| 16 |  | `gfdEPLPostEffect` | `PostEffect` | high |
| 17 |  | `gfdEPLHelper` | `Helper` | high |

Officially these are "polygon" categories, not "polygons": our
`LightningPolygon` is `gfdEPLPolygonThunder` and our `TrajectoryPolygon` is
`gfdEPLPolygonTrack`.

### `gfdEPLTexture` and `gfdEPLDisplacement`

Readers `gfdEPLTextureStreamRead_0` at **`0x8A1BF4`** and
`gfdEPLDisplacementStreamRead_0` at **`0x8A121C`**. Not currently modelled
separately in C#.

`gfdEPLTextureHeader`: `flags` `0x00`, `intervalW` `0x04`, `intervalH` `0x08`,
`fps` `0x0C`. `gfdEPLTexture` then adds `asset`, `repeatW`, `repeatH`,
`pitchW`, `pitchH`, `length`. All high.

`gfdEPLDisplacementHeader`: `flags` `0x00`, `move` (`gfdCurve`) `0x004`,
`scale` `0x058`, `threshold` `0x0AC`, `life` `0x100`, `fps` `0x104`. High.

## Epl leaf: Particle (`gfdEPLParticle` / `gfdEPLBehavior`)

Reader `gfdEPLParticleStreamRead_0` at **`0x9754D4`**, writer at `0x9740BC`.
The behaviour block readers are `gfdEPLBehaviorParamsStreamRead_0` at
**`0x96E7AC`** plus one per variant.

Our `EplParticleEmitter` is really `gfdEPLBehaviorBasicParams` (size `0x170`).

| # | Official field | Type | C# member | Conf. |
|---|---|---|---|---|
| 0 | `flags` | `gfdEPLBehaviorFlag` u32 @ `0x15C` | `Field15C` | high |
| 1 | `life` | f32 @ `0x160` | `Field160` | high |
| 2 | `seed` | u32 @ `0x164` | `Field164` | high |
| 3 | `emitter.life` | f32 @ `0x040` | `Field040` | high |
| 4 | `emitter.count` | `gfdRange` @ `0x044` | `Field44` | high |
| 5 | n/a | later-version `Vector2` pair | `Field4C`, `Field54` | low (absent here) |
| 6 | `color.alpha` | f32 @ `0x0B4` | `FieldB4` | high |
| 7 | `color.blend` | `gfdBlendType` @ `0x0C4` | `FieldC4` | high |
| 8 | `color.fade` | `gfdFade` @ `0x0B8` | `FieldB8` | high |
| 9 | `color.curve4` | `gfdCurve4` @ `0x050` | `Field50` | high |
| 10 | `color.random` | f32 @ `0x0C0` | `FieldC0` | high |
| 11 | `scale.curve4` | `gfdCurve4` @ `0x0C8` | `FieldC8` | high |
| 12 | `scale.rate` | `gfdCommonVec2` @ `0x12C` | `Field12C`, `Field130` | high |
| 13 | `rotate.start` | `gfdRange` @ `0x134` | `Field134` | high |
| 14 | `rotate.speed` | `gfdRange` @ `0x13C` | `Field13C` | high |
| 15 | `rotate.accele` | f32 @ `0x144` | `Field144` | high |
| 16 | `rotate.type` | `gfdEPLRotateType` @ `0x148` | `Field148` | high |
| 17 | `blur.length` | u32 @ `0x14C` | `Field14C` | high |
| 18 | `blur.interval` | f32 @ `0x150` | `Field150` | high |
| 19 | `adjust.scale` | f32 @ `0x154` | `Field154` | high |
| 20 | `adjust.speed` | f32 @ `0x158` | `Field158` | high |

`Field140` and `Field134_Old` are read only on older versions and fall inside
`rotate`; treat them as legacy layout, **low**.

The speculative comments currently in `EplLeafParticle.cs` are mostly wrong and
should be replaced:

| C# member | Current comment | Official | Conf. |
|---|---|---|---|
| `Field15C` | `randomSpawnDelay` | `flags` | high |
| `Field160` | `particleLifetime` | `life` | high |
| `Field164` | `angleSeed` | `seed` | high |
| `Field040` | `despawnTimer` | `emitter.life` | high |
| `Field44` | `SpawnChoker` | `emitter.count` | high |
| `FieldB4` | `colorOverLifeOffset` | `color.alpha` | high |
| `FieldB8` | `OopacityOverLife` | `color.fade` | high |
| `FieldC4` | `drawQueueIndex` | `color.blend` | high |
| `Field134` | `spawner angles` | `rotate.start` | high |
| `Field13C` | `cycle rate from birth` | `rotate.speed` | high |
| `Field14C` | `particleMultiplier` | `blur.length` | high |
| `Field154` | `particleScale` | `adjust.scale` | high |
| `Field158` | `particleSpeed` | `adjust.speed` | high |

### Behaviour variants (`gfdEPLBehaviorType`)

From `gfdEPLBehaviorTypeTable` at `0x1082C68` (32 bytes per entry, first entry
at `+0x10`). The same six variants are shared by Particle, PolygonTrack and
ObjectParticle.

| Value | Official | Our class (`EplLeafParticle.cs`) | Params reader | Conf. |
|---|---|---|---|---|
| 1 | `Smoke` | `EplSmokeEffectParams` | `0x96E6DC` | high |
| 2 | `Spark` | `EplExplosionEffectParams` | `0x96E5CC` | high |
| 3 | `Spiral` | `EplSpiralEffectParams` | `0x96E4D8` | high |
| 4 | `Sphere` | `EplBallEffectParams` | `0x96E408` | high |
| 5 | `Ring` | `EplCircleEffectParams` | `0x96E2FC` | high |
| 6 | `Line` | `EplStraightLineEffectParams` | `0x96E258` | high |

Per-variant fields, all at offsets inside `gfdEPLBehavior<Variant>Params`
(the variant block begins at `0x170`), all **high**:

| Official | Offset | C# member |
|---|---|---|
| **Smoke** `radius` | `0x170` | `Field170` |
| `speed` | `0x174` | `Field174` |
| `gravity` | `0x17C` | `Field17C` |
| `ampliBegin` | `0x184` | `Field184` |
| `ampliEnd` | `0x18C` | `Field18C` |
| `ampliSpeed` | `0x194` | `Field194` |
| **Spark** `radius` | `0x170` | `Field170` |
| `spreadXZ` | `0x178` | `Field178` |
| `spreadY` | `0x180` | `Field180` |
| `speed` | `0x188` | `Field188` |
| `gravity` | `0x190` | `Field190` |
| `accele` | `0x198` | `Field198` |
| **Spiral** `height` | `0x170` | `Field170` |
| `radiusBegin` | `0x174` | `Field174` |
| `radiusEnd` | `0x17C` | `Field17C` |
| `rotateSpeed` | `0x184` | `Field184` |
| `rotateAccele` | `0x18C` | `Field18C` |
| `riseSpeed` | `0x190` | `Field190` |
| `gravity` | `0x198` | `Field198` |
| **Sphere** `radiusBegin` | `0x170` | `Field170` |
| `radiusEnd` | `0x178` | `Field178` |
| `rotateSpeed` | `0x180` | `Field180` |
| `rotateAccele` | `0x188` | `Field188` |
| `gravity` | `0x18C` | `Field18C` |
| **Ring** `radius` | `0x170` | `Field170` |
| `height` | `0x178` | `Field178` |
| `spreadSpeed` | `0x180` | `Field180` |
| `spreadAccele` | `0x188` | `Field188` |
| `rotateSpeed` | `0x18C` | `Field18C` |
| `rotateAccele` | `0x194` | `Field194` |
| `gravity` | `0x198` | `Field198` |
| **Line** `length` | `0x170` | `Field170` |
| `speed` | `0x174` | `Field174` |
| `gravity` | `0x17C` | `Field17C` |
| `ampliBegin` | `0x184` | `Field184` |
| `ampliEnd` | `0x18C` | `Field18C` |
| `ampliSpeed` | `0x194` | `Field194` |

Our variant class names should be renamed: Explosion → Spark, Ball → Sphere,
Circle → Ring, StraightLine → Line.

There is also a runtime-only `gfdEPLBehavior<Variant>Physics` struct per
variant (the per-particle integration state: `genesis`, `ascentDir`/`riseDir`/
`axis`, `gravity`, `amplitude`, `ampliInterval`, `ampliSpeed`, `ampliRadian`,
`speed`, `radius`, `rotate`, `rotateSpeed`). It is not streamed.

### `gfdEPLParticleBasicParams` / `gfdEPLObjectParticleBasicParams`

`gfdEPLParticleBasicParams` is just `flags` at `0x00` plus `reserve[3]`; our
`EplParticle.Field10` and friends sit in that block, **low**.

`gfdEPLObjectParticleBasicParams`: `flags` `0x00`, `number` `0x04`, `axis[3]`
`0x08`. Our `EplObjectParticles.Field00` = `flags`, `Field04` = `number`,
`Field08`/`Field0C`/`Field10` = `axis[0..2]`. High. Reader
`gfdEPLObjectParticleStreamRead_0` at **`0x956424`**.

## Epl leaf: PostEffect (`gfdEPLPostEffect`)

Reader `gfdEPLPostEffectStreamRead_0` at **`0x950AA4`**, writer at `0x94FFDC`.
Variants from `gfdEPLPostEffectTypeTable` at `0x10827B0` (40 bytes per entry,
first at `+0x18`); the assert reads `type < GFDEPLPOSTEFFECT_TYPE_MAX` with
`type > 8` rejected.

`gfdEPLPostEffectBasicParams`: `flags` `0x00`, `life` `0x04`. Our
`EplPostEffect.Field00` = `flags`, `Field04` = `life`. High.

| Value | Official | Our class | Reader | Conf. |
|---|---|---|---|---|
| 1 | `RadialBlur` | `EplPostEffectRadiationBlurData` | `0x9517C8` | high |
| 2 | `StraightBlur` | `EplPostEffectStraightBlurData` | `0x951750` | high |
| 3 | `NoiseBlur` | `EplPostEffectNoiseBlurData` | `0x951690` | high |
| 4 | `DistortBlur` | `EplPostEffectDistortionBlurData` | `0x9515D4` | high |
| 5 | `Fill` | `EplPostEffectFillData` | `0x951544` | high |
| 6 | `LensFlare` | `EplPostEffectLensFlareData` | `0x9514C0` | high |
| 7 | `ColorCorrect` | `EplPostEffectColorCorrectionData` | `0x950DB0` | high |
| 8 | `Monotone` | `EplPostEffectMonotoneData` | `0x950D5C` | high |

Types 9, 10 and 11 in our switch (`ChromaticAberration`, `LensFlareMake`,
`MotionBlur`, `ColorCorrectionExcludeToon`, `AfterimageBlur`) do not exist in
this build. No evidence either way for them here.

All of the following are **high** (offset lands on a named member):

| Official | Offset | C# member |
|---|---|---|
| **RadialBlur** `rgba` (`gfdCurve4`) | `0x010` | `Field10` |
| `blend` | `0x074` | `Field74` |
| `power` (`gfdCurve4`) | `0x078` | `Field78` |
| `falloff` | `0x0DC` | `FieldDC` |
| `centerX` | `0x0E0` | `FieldE0` |
| `centerY` | `0x0E4` | `FieldE4` |
| `ssaoMask` (`gfdBool`, 1 byte) | `0x0E8` | `FieldE8` |
| **StraightBlur** `rgba` | `0x010` | `Field10` |
| `blend` | `0x074` | `Field74` |
| `power` | `0x078` | `Field78` |
| `direction` | `0x0DC` | `FieldDC` |
| **NoiseBlur** `rgba` | `0x010` | `Field10` |
| `blend` | `0x074` | `Field74` |
| `power` | `0x078` | `Field78` |
| `scale` (`gfdCurve4`) | `0x0DC` | `FieldDC` |
| `ssaoMask` | `0x140` | `Field140` |
| **DistortBlur** `rgba` | `0x010` | `Field10` |
| `blend` | `0x074` | `Field74` |
| `power[0]` | `0x078` | `Field78` |
| `power[1]` | `0x0DC` | `FieldDC` |
| `uv[0]` | `0x140` | `Field140`, `Field144` |
| `uv[1]` | `0x148` | `Field148`, `Field14C` |
| **Fill** `rgba[0..3]` | `0x010`, `0x074`, `0x0D8`, `0x13C` | `Field10`, `Field74`, `FieldD8`, `Field13C` |
| `blend` | `0x1A0` | `FieldlA0` |
| **LensFlare** `flags` | `0x010` | `Field10` |
| `templ` (`gfdLensFlareTemplate`) | `0x014` | `Field14` |
| `filter` (`gfdLensFlareFilterType`) | `0x018` | `Field18` |
| `rgba` | `0x01C` | `Field1C` |
| `brightness` | `0x080` | `Field80` |
| **ColorCorrect** `cyan` | `0x010` | `Field10` |
| `magenta` | `0x014` | `Field14` |
| `yellow` | `0x018` | `Field18` |
| `dodge` | `0x01C` | `Field1C` |
| `burn` | `0x020` | `Field20` |
| `alpha` | `0x024` | `Field24` |
| `fade` | `0x028` | `Field28` |
| **Monotone** `alpha` | `0x010` | `Field10` |
| `fade` | `0x014` | `Field14` |

`FieldlA0` in `EplPostEffectFillData` is spelt with a lowercase L; it is
`blend` at `0x1A0`.

## Epl leaf: GlitterPolygon (`gfdEPLPolygonGlitter`)

Reader `gfdEPLPolygonGlitterStreamRead_0` at **`0x95E450`**, writer at
`0x95CDB4`. Variants from `gfdEPLPolygonGlitterTypeTable` at `0x10829E4`
(24 bytes per entry).

`gfdEPLPolygonGlitterBasicParams` → `EplGlitterPolygon`, all **high**:

| # | Official | Offset | C# member |
|---|---|---|---|
| 0 | `flags` | `0x000` | `Field00` |
| 1 | `life` | `0x004` | `Field04` |
| 2 | `number` | `0x008` | `Field08` |
| 3 | `blend` | `0x020` | `Field20` |
| 4 | `seed` | `0x150` | `Field150` |
| 5 | `count` (`gfdRange`) | `0x00C` | `Field08_` |
| 6 | `fade` (`gfdFade`) | `0x014` | `Field14` |
| 7 | `scale` (`gfdCurve`) | `0x0EC` | `FieldEC` |
| 8 | `random` | `0x01C` | `Field1C` |
| 9 | `rgbaIn` (`gfdCurve4`) | `0x024` | `Field24` |
| 10 | `rgbaOut` (`gfdCurve4`) | `0x088` | `Field88` |
| 11 | `repeatTexV` | `0x140` | `Field140` |
| 12 | `moveTexV` | `0x144` | `Field144` |
| 13 | `adjust.scale` | `0x148` | `Field148` |
| 14 | `adjust.speed` | `0x14C` | `Field14C` |

`Field08_` is mis-numbered: `count` is at `0x0C`, not `0x08`.

After the variant block the reader reads a `gfdBool` and, if set, a `gfdAsset`
that becomes `gfdEPLMaterial`. Our `Field70` is that boolean and
`EmbeddedFile` is the asset. High. The same pattern ends every leaf.

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Spark` | `EplGlitterPolygonExplosion` | high |
| 2 | `Scatter` | `EplGlitterPolygonSplash` | high |
| 3 | `Cylinder` | `EplGlitterPolygonCylinder` | high |
| 4 | `Wall` | `EplGlitterPolygonWall` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Spark** `radius` | `0x154` | `Field154` | high |
| `width` | `0x15C` | `Field15C` | high |
| `length` | `0x164` | `Field164` | high |
| `speed` | `0x16C` | `Field16C` | high |
| `accele` | `0x174` | `Field174` | high |
| **Scatter** `radius` (`gfdCurve`) | `0x154` | `Field154` | high |
| `spread` (`gfdCurve`) | `0x1A8` | `Field1A8` | high |
| `width` | `0x1FC` | `Field1FC` | high |
| `length` | `0x204` | `Field204` | high |
| `speed` | `0x20C` | `Field20C` | high |
| `accele` | `0x214` | `Field214` | high |
| **Cylinder** `radius` (`gfdCurve`) | `0x154` | `Field154` | high |
| `width` | `0x1A8` | `Field1A8` | high |
| `length` | `0x1B0` | `Field1B0` | high |
| `speed` | `0x1B8` | `Field1B8` | high |
| `accele` | `0x1C0` | `Field1C0` | high |
| `rotateSpeed` | `0x1C4` | `Field1C4` | high |
| `rotateAccele` | `0x1CC` | `Field1CC` | high |
| `rotateType` | `0x1D0` | `Field1D0` | high |
| **Wall** `distance` (`gfdCurve`) | `0x154` | `Field154` | high |
| `width` | `0x1A8` | `Field1A8` | high |
| `length` | `0x1B0` | `Field1B0` | high |
| `speed` | `0x1B8` | `Field1B8` | high |
| `accele` | `0x1C0` | `Field1C0` | high |

## Epl leaf: WindPolygon (`gfdEPLPolygonWind`)

Reader `gfdEPLPolygonWindStreamRead_0` at **`0x962918`**, writer at `0x96173C`.
Variants from `gfdEPLPolygonWindTypeTable` at `0x1082AA4`.

`gfdEPLPolygonWindBasicParams` → `EplWindPolygon`, all **high**:

| Official | Offset | C# member |
|---|---|---|
| `flags` | `0x000` | `Field00` |
| `life` | `0x004` | `Field04` |
| `number` | `0x008` | `Field08` |
| `count` (`gfdRange`) | `0x00C` | `Field0C` |
| `rgb` (`gfdCurve4`) | `0x014` | `Field14` |
| `alpha` | `0x078` | `Field78` |
| `fade` (`gfdFade`) | `0x07C` | `Field7C` |
| `random` | `0x084` | `Field84` |
| `blend` | `0x088` | `Field88` |
| `split` | `0x08C` | `Field8C` |
| `repeatTexV` (`gfdRange`) | `0x090` | `Field90` |
| `moveTexV` (`gfdRange`) | `0x098` | `Field98` |
| `adjust.scale` | `0x0A0` | `FieldA0` |
| `adjust.speed` | `0x0A4` | `FieldA4` |
| `seed` | `0x0A8` | `FieldA8` |

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Spiral` | `EplWindPolygonSpiral` | high |
| 2 | `Spark` | `EplWindPolygonExplosion` | high |
| 3 | `Sphere` | `EplWindPolygonBall` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Spiral** `radius` (`gfdCurve`) | `0x0AC` | `FieldAC` | high |
| `height` (`gfdCurve`) | `0x100` | `Field100` | high |
| `width` (`gfdCurve`) | `0x154` | `Field154` | high |
| `slant` | `0x1A8` | `Field1A8` | high |
| `round` | `0x1B0` | `Field1B0` | high |
| `rotateSpeed` | `0x1B8` | `Field1B8` | high |
| `rotateAccele` | `0x1C0` | `Field1C0` | high |
| `rotateType` | `0x1C4` | `Field1C4` | high |
| `bothEnds` (`gfdFade`) | `0x1C8` | `Field1C8` | high |
| **Spark** `radius` (`gfdCurve`) | `0x0AC` | `FieldAC` | high |
| `widthRatio` | `0x100` | `Field100` | high |
| `rotateSpeed` | `0x108` | `Field108` | high |
| `rotateAccele` | `0x110` | `Field110` | high |
| `rotateType` | `0x114` | `Field114` | high |
| `moveTexV` | `0x118` | `Field118` | high |
| **Sphere** `radius` (`gfdCurve`) | `0x0AC` | `FieldAC` | high |
| `width` (`gfdCurve`) | `0x100` | `Field100` | high |
| `round` | `0x154` | `Field154` | high |
| `rotateSpeed` | `0x15C` | `Field15C` | high |
| `rotateAccele` | `0x164` | `Field164` | high |
| `rotateType` | `0x168` | `Field168` | high |
| `bothEnds` (`gfdFade`) | `0x16C` | `Field16C` | high |

## Epl leaf: FlashPolygon (`gfdEPLPolygonFlash`)

Reader `gfdEPLPolygonFlashStreamRead_0` at **`0x96C000`**, writer at `0x96A98C`.
Variants from `gfdEPLPolygonFlashTypeTable` at `0x1082BD8`.

`gfdEPLPolygonFlashBasicParams` → `EplFlashPolygon`, all **high**:

| Official | Offset | C# member |
|---|---|---|
| `flags` | `0x000` | `Field00` |
| `life` | `0x004` | `Field04` |
| `number` | `0x008` | `Field08` |
| `count` (`gfdRange`) | `0x00C` | `Field08_` |
| `alpha` | `0x014` | `Field14` |
| `fade` (`gfdFade`) | `0x018` | `Field18` |
| `random` | `0x020` | `Field20` |
| `blend` | `0x024` | `Field24` |
| `scale` (`gfdCurve`) | `0x028` | `Field28` |
| `adjust.scale` | `0x07C` | `Field7C` |
| `adjust.speed` | `0x080` | `Field80` |
| `seed` | `0x084` | `Field84` |

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Radiate` | `EplFlashPolygonRadiation` | high |
| 2 | `Spark` | `EplFlashPolygonExplosion` | high |
| 3 | `Ring` | `EplFlashPolygonRing` | high |
| 4 | `Scatter` | `EplFlashPolygonSplash` | high |
| 5 | `Cylinder` | `EplFlashPolygonCylinder` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Radiate** `rgbaIn` | `0x088` | `Field88` | high |
| `rgbaOut` | `0x08C` | `Field8C` | high |
| `widthIn` | `0x090` | `Field90` | high |
| `widthOut` | `0x098` | `Field98` | high |
| `length` | `0x0A0` | `FieldA0` | high |
| `rotateSpeed` | `0x0A8` | `FieldA8` | high |
| `rotateAccele` | `0x0B0` | `FieldB0` | high |
| `rotateType` | `0x0B4` | `FieldB4` | high |
| **Spark** `radius` | `0x088` | `Field88` | high |
| `rgbaIn` | `0x090` | `Field90` | high |
| `rgbaOut` | `0x094` | `Feild94` | high |
| `width` | `0x098` | `Field94` (mis-numbered) | high |
| `length` | `0x0A0` | `FieldA0` | high |
| `speed` | `0x0A8` | `FieldA8` | high |
| `accele` | `0x0B0` | `FieldB0` | high |
| **Ring** `radius` (`gfdCurve`) | `0x088` | `Field88` | high |
| `spread` (`gfdCurve`) | `0x0DC` | `FieldDC` | high |
| `rgbaIn` | `0x130` | `Field130` | high |
| `rgbaOut` | `0x134` | `Field134` | high |
| `widthIn` | `0x138` | `Field138` | high |
| `widthOut` | `0x140` | `Field140` | high |
| `length` | `0x148` | `Field148` | high |
| `rotateSpeed` | `0x150` | `Field150` | high |
| `rotateAccele` | `0x158` | `Field158` | high |
| `rotateType` | `0x15C` | `Field15C` | high |
| **Scatter** same through `length`, then `speed` | `0x150` | `Field150` | high |
| `accele` | `0x158` | `Field158` | high |
| **Cylinder** `radius` (`gfdCurve`) | `0x088` | `Field88` | high |
| `rgb` (`gfdCurve4`) | `0x0DC` | `FieldDC` | high |
| `width` | `0x140` | `Field140` | high |
| `length` | `0x148` | `Field148` | high |
| `speed` | `0x150` | `Field150` | high |
| `accele` | `0x158` | `Field158` | high |
| `rotateSpeed` | `0x15C` | `Field15C` | high |
| `rotateAccele` | `0x164` | `Field164` | high |
| `rotateType` | `0x168` | `Field168` | high |

Two naming problems in `EplLeafFlashPolygon.cs`: the Spark variant has both
`Feild94` (a typo, and correctly `rgbaOut` at `0x94`) and `Field94` (which is
actually `width` at `0x98`).

## Epl leaf: CirclePolygon (`gfdEPLPolygonCircle`)

Reader `gfdEPLPolygonCircleStreamRead_0` at **`0x968F6C`**, writer at
`0x9681FC`. Variants from `gfdEPLPolygonCircleTypeTable` at `0x1082B4C`
(28 bytes per entry).

`gfdEPLPolygonCircleBasicParams` → `EplCirclePolygon`, all **high**:
`flags` `0x00` → `Field00`, `life` `0x04` → `Field04`, `fade` `0x08` →
`Field08`, `blend` `0x10` → `Field10`, `adjust.scale` `0x14` → `Field14`,
`adjust.speed` `0x18` → `Field18`, `seed` `0x1C` → `Field1C`, `split` `0x20` →
`Field20`.

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Ring` | `EplCirclePolygonRing` | high |
| 2 | `Track` | `EplCirclePolygonTrajectory` | high |
| 3 | `Fill` | `EplCirclePolygonFill` | high |
| 4 | `Hoop` | `EplCirclePolygonHoop` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Ring** `occurrence` | `0x024` | `Field24` | high |
| `radius` (`gfdCurve`) | `0x028` | `Field28` | high |
| `width` (`gfdCurve`) | `0x07C` | `Field7C` | high |
| `widthRatio` (`gfdFade`) | `0x0D0` | `FieldD0` | high |
| `rgbaIn` | `0x0D8` | `FieldD8` | high |
| `rgbaCenter` | `0x0DC` | `FieldDC` | high |
| `rgbaOut` | `0x0E0` | `FieldE0` | high |
| **Track** `radius` | `0x024` | `Field24` | high |
| `widthRatio` | `0x028` | `Field28` | high |
| `rgbaIn` (`gfdCurve4`) | `0x02C` | `Field2C` | high |
| `rgbaOut` (`gfdCurve4`) | `0x090` | `Field90` | high |
| `speed` | `0x0F4` | `FieldF4` | high |
| `accele` | `0x0F8` | `FieldF8` | high |
| `repeatTexU` | `0x0FC` | `FieldFC` | high |
| `moveTexV` | `0x100` | `Field100` | high |
| **Fill** `occurrence` | `0x024` | `Field24` | high |
| `radius` (`gfdCurve`) | `0x028` | `Field28` | high |
| `rgbaIn` (`gfdCurve4`) | `0x07C` | `Field7C` | high |
| `rgbaOut` (`gfdCurve4`) | `0x0E0` | `FieldE0` | high |
| **Hoop** `occurrence` | `0x024` | (not modelled) | high |
| `radius` | `0x028` | `Field28` | high |
| `speed` | `0x02C` | `Field2C` | high |
| `accele` | `0x030` | `Field30` | high |
| `width` (`gfdCurve`) | `0x034` | `Field34` | high |
| `widthRatio` (`gfdFade`) | `0x088` | `Field88` | high |
| `rgbaIn` (`gfdCurve4`) | `0x090` | `Field90` | high |
| `rgbaCenter` (`gfdCurve4`) | `0x0F4` | `FieldF4` | high |
| `rgbaOut` (`gfdCurve4`) | `0x158` | `Field158` | high |
| `repeatTexU` | `0x1BC` | `Field1BC` | high |
| `moveTexV` | `0x1C0` | `Field1C0` | high |

## Epl leaf: LightningPolygon (`gfdEPLPolygonThunder`)

Reader `gfdEPLPolygonThunderStreamRead_0` at **`0x966D8C`**, writer at
`0x966238`. Variants from `gfdEPLPolygonThunderTypeTable` at `0x1082B04`.

`gfdEPLPolygonThunderBasicParams` → `EplLightningPolygon`, all **high**:

| Official | Offset | C# member |
|---|---|---|
| `flags` | `0x000` | `Field00` |
| `life` | `0x004` | `Field04` |
| `number` | `0x008` | `Field08` |
| `count` (`gfdRange`) | `0x00C` | `Field0C` |
| `fade` (`gfdFade`) | `0x014` | `Field14` |
| `blend` | `0x01C` | `Field1C` |
| `split` | `0x020` | `Feild20` |
| `wave` (`gfdRange`) | `0x024` | `Field24` |
| `waveHeight` (`gfdRange`) | `0x02C` | `Field2C` |
| `uneven` (`gfdRange`) | `0x034` | `Field34` |
| `bothEnds` (`gfdFade`) | `0x03C` | `Field3C` |
| `widthCore` | `0x044` | `Field44` |
| `widthEdge` | `0x048` | `Field48` |
| `rgbaCore` | `0x04C` | `Field4C` |
| `rgbaBorder` | `0x050` | `Field50` |
| `rgbaEdge` | `0x054` | `Field54` |
| `adjust.scale` | `0x058` | `Field58` |
| `adjust.speed` | `0x05C` | `Field5C` |
| `seed` | `0x060` | `Field60` |

(`Feild20` is a typo in our source for `split`.)

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Bar` | `EplLightningPolygonRod` | high |
| 2 | `Sphere` | `EplLightningPolygonBall` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Bar** `length` (`gfdRange`) | `0x064` | `Field64` | high |
| **Sphere** `radius` (`gfdCurve`) | `0x064` | `Field64` | high |
| `round` | `0x0B8` | `FieldB8` | high |
| `rotateYSpeed` | `0x0C0` | `FieldC0` | high |
| `rotateYAccele` | `0x0C8` | `FieldC8` | high |
| `rotateYType` | `0x0CC` | `FieldCC` | high |
| `rotateZSpeed` | `0x0D0` | `FieldD0` | high |
| `rotateZAccele` | `0x0D8` | `FieldD8` | high |
| `rotateZType` | `0x0DC` | `FieldDC` | high |

## Epl leaf: TrajectoryPolygon (`gfdEPLPolygonTrack`)

Reader `gfdEPLPolygonTrackStreamRead_0` at **`0x965D14`**, writer at `0x9643A0`.

`gfdEPLPolygonTrackBasicParams` → `EplTrajectoryPolygon`, all **high**:

| Official | Offset | C# member |
|---|---|---|
| `flags` | `0x000` | `Field00` |
| `number` | `0x004` | `Field04` |
| `thickCore` | `0x008` | `Field08` |
| `thickOutside` | `0x00C` | `Field0C` |
| `rgbCore` (`gfdCurve4`) | `0x010` | `Field10` |
| `rgbOutside` (`gfdCurve4`) | `0x074` | `Field74` |
| `alpha` (`gfdCurve`) | `0x0D8` | `FieldD8` |
| `split` | `0x12C` | `Field12C` |
| `repeatTexV` | `0x130` | `Field130` |
| `moveTexV` | `0x134` | `Field134` |
| `interval` | `0x138` | `Field138` |

The variant block at `0x140` is a full `gfdEPLBehavior<Variant>Params`, the
same six variants as Particle. Our C# does not model it yet.

## Epl leaf: Model, Camera, Light, Helper

### `gfdEPLMesh` → `EplModel`

Reader `gfdEPLMeshStreamRead_0` at **`0x95BA70`**, writer at `0x95B4D4`.
`gfdEPLMeshBasicParams`: `flags` `0x00`, `adjust` `0x04`.

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| `flags` | `0x00` | `Field00` | high |
| `adjust.scale` | `0x04` | `Field04` | high |
| `adjust.speed` | `0x08` | `Field08` | high |
| `distance` (2D only) | `0x0C` | `EplModel2DData.Field0C` | high |

Variants: 1 = `3D` (`gfdEPLMesh3DParams`, reader `0x95AFE8`), 2 = `2D`
(`gfdEPLMesh2DParams`, reader `0x95B660`). High. Everything our `EplModel`
reads beyond `Field08` (`Field0C` to `Field2C`, and the whole `_P5R` set) is a
later-version addition with no counterpart in this build, **low**.

### `gfdEPLCamera` → `EplCamera`

Reader `gfdEPLCameraStreamRead_0` at **`0x95AE00`**, writer at `0x959AB0`.
`gfdEPLCameraBasicParams`: `flags` `0x00`, `adjust` `0x04`, `seed` `0x0C`.

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| `flags` | `0x00` | `Field00` | high |
| `adjust.scale` | `0x04` | `Field04` | high |
| `adjust.speed` | `0x08` | `Field08` | high |
| `seed` | `0x0C` | `Field0C` | high |

Variants, with official name strings read straight out of
`gfdEPLCameraTypeTable` at `0x1082960`:

| Value | Official name | Params | Our class | Conf. |
|---|---|---|---|---|
| 1 | `MESH` | `gfdEPLCameraMeshParams` | `EplCameraMeshParams` | high |
| 2 | `QUAKE` | `gfdEPLCameraQuakeParams` | `EplCameraQuakeParams` | high |

`gfdEPLCameraQuakeParams.quake` is a `gfdQuakeParams` at `0x10`, which names
all five of our placeholders:

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| `quake.power` | `0x10` | `Field10` | high |
| `quake.pitchWeight` | `0x14` | `Field14` | high |
| `quake.totalTime` | `0x18` | `Field18` | high |
| `quake.fadeInTime` | `0x1C` | `Field1C` | high |
| `quake.fadeOutTime` | `0x20` | `Field20` | high |

### `gfdEPLLight` → `EplLight`

Reader `gfdEPLLightStreamRead_0` at **`0x957EF0`**, writer at `0x957A14`.
`gfdEPLLightBasicParams`: `flags` `0x00`, `adjust` `0x04`.

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| `flags` | `0x00` | `Field00` | high |
| `adjust.scale` | `0x04` | `Field04` | high |
| `adjust.speed` | `0x08` | `Field08` | high |

Variants, names from `gfdEPLLightTypeTable` at `0x1082918`:

| Value | Official name | Our class | Conf. |
|---|---|---|---|
| 1 | `MESH(ps3 only)` | `EplLightMeshData` | high |
| 2 | `SCENE` | `EplLightSceneData` | high |

Our types 3 (`EplLightPointData`) and 4 (`EplLightSpotData`) do not exist in
this build; no evidence either way for them.

`gfdEPLLightSceneParams` → `EplLightSceneData`, all **high**:

| Official | Offset | C# member |
|---|---|---|
| `life` | `0x0C` | `Field0C` |
| `fade` (`gfdFade`) | `0x10` | `Field10` |
| `ambient` (RGBA u32) | `0x18` | `Field18` |
| `diffuse` (RGBA u32) | `0x1C` | `Field1C` |
| `specular` (RGBA u32) | `0x20` | `Field20` |
| `rotate[0..2]` | `0x24` | `Field24`, `Field28`, `Field2C` |

### `gfdEPLHelper` → `EplHelper`

Reader `gfdEPLHelperStreamRead_0` at **`0x94F210`**, writer at `0x94E39C`.
`gfdEPLHelperBasicParams`: `flags` `0x00`, `adjust` `0x04`, `scale` `0x0C`.

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| `flags` | `0x00` | `Field00` | high |
| `adjust.scale` | `0x04` | `Field04` | high |
| `adjust.speed` | `0x08` | `Field08` | high |
| `scale` | `0x0C` | `Field0C` | high |

`Field84` and `Field88` are the two "has asset" booleans in front of
`material[0]` and `material[1]` (`gfdEPLHelper_tag.material` is a two-element
array). Medium: the pairing is from the struct, the offsets are ours.

### `gfdEPLPolygonBoard` → `EplBoardPolygon`

Reader `gfdEPLPolygonBoardStreamRead_0` at **`0x960174`**, writer at `0x95F554`.
`gfdEPLPolygonBoardBasicParams`, all **high**: `flags` `0x00` → `Field00`,
`life` `0x04` → `Field04`, `fade` `0x08` → `Field08`, `blend` `0x10` →
`Field10`, `adjust.scale` `0x14` → `Field14`, `adjust.speed` `0x18` →
`Field18`, `seed` `0x1C` → `Field1C`.

| Value | Official | Our class | Conf. |
|---|---|---|---|
| 1 | `Square` | `EplSquareBoardPolygon` | high |
| 2 | `Oblong` | `EplRectangleBoardPolygon` | high |

| Official | Offset | C# member | Conf. |
|---|---|---|---|
| **Square** `pivot` (`gfdCommonVec2`) | `0x020` | `Field20`, `Field24` | high |
| `length` (`gfdCurve4`) | `0x028` | `Field28` | high |
| `rgb` (`gfdCurve4`) | `0x08C` | `Field8C` | high |
| `alpha` | `0x0F0` | `FieldF0` | high |
| `rotateStart` | `0x0F4` | `FieldF4` | high |
| `rotateSpeed` | `0x0FC` | `FieldFC` | high |
| `rotateAccele` | `0x104` | `Field104` | high |
| `rotateType` | `0x108` | `Field108` | high |
| **Oblong** `pivot` | `0x020` | `Field20`, `Field24` | high |
| `width` (`gfdCurve4`) | `0x028` | `Field28` | high |
| `height` (`gfdCurve4`) | `0x08C` | `Field8C` | high |
| `rgb` (`gfdCurve4`) | `0x0F0` | `FieldF0` | high |
| `alpha` | `0x154` | `Field154` | high |
| `rotateStart` | `0x158` | `Field158` | high |
| `rotateSpeed` | `0x160` | `Field160` | high |
| `rotateAccele` | `0x168` | `Field168` | high |
| `rotateType` | `0x16C` | `Field16C` | high |

## Priority 2: the flag enumerations

**The flag bit names do not survive in this image.** I checked four
independent ways and found nothing:

1. **Type library enums.** The IDB holds 986 enum types; exactly one has
  GFD-prefixed members (`gfdDDDStoGTFSwOp`, a DDS-to-GTF swizzle option,
  unrelated). Every `gfd*Flag` type, including `gfdMaterialFlag`,
  `gfdMaterialFlag2`, `gfdGeometryFlag`, `gfdMeshFlag`, `gfdAnimationFlag`,
  `gfdAnimControllerFlag`, `gfdLightFlag`, `gfdEPLFlag`, `gfdEPLLeafFlag` and
  all eleven `gfdEPL<Category>Flag`, is a bare `typedef gfdUInt32`. The
  compiler emitted no enumerator names for them.
2. **String search for enumerator names.** No string in the image matches
  `^GFD[A-Z]+_[A-Z0-9_]+$`. The only `GFDEPL*` and `GFD*_MAX` strings are
  bounds asserts (`type < GFDEPLPOLYGONGLITTER_TYPE_MAX`), which name the
  *count*, not the bits.
3. **Assert strings mentioning a flag by name.** Every `FLAG`/`BIT` string in
  the binary belongs to game code (`fldNpcCtrlFlag_READY`,
  `BTL_FORMATION_FLAG_USE`, `EVTWGELEM_FLAG_LINKEDDATA_U8_TO_FLOAT`, and so
  on), not to the `gfd` graphics layer.
4. **Switch dispatch on individual bits.** The one place that consumes the
  material bits wholesale, `gfdMaterialGetShaderFlags_0` at `0x8CE764`, maps
  material bits onto shader bits numerically with no names on either side.

So the only bits recoverable are those with a **named setter or getter**. Those
are listed below and are genuinely high confidence; everything else in
`MaterialAttribute.cs`, `Mesh.cs`, `AnimationFlags.cs`, `AnimationPackFlags.cs`
and `FieldTexturePS3.cs` stays as it is. I am not going to invent names for
them.

### Bits with a named setter

| Flag word | Bit | Official meaning (from the symbol) | C# name | Conf. |
|---|---|---|---|---|
| `gfdMaterialFlag` | `0x1000` (12) | `gfdMaterialSetWire` | `RenderWireframe` | high |
| `gfdMaterialFlag` | `0x2000` (13) | `gfdMaterialSetAlphaTestEnable` | `AlphaTest` | high |
| `gfdMaterialFlag` | `0x10000` (16) | gates `extension` in the reader | `HasAttributes` | high |
| `gfdMaterialFlag` | `0x100000 << i` (20-29) | gates `textures[i]` in the reader | `HasDiffuseMap` to `HasReserveMap` | high |
| `gfdMaterialFlag2` | `0x80` (7) | `gfdMaterialSetColorOp(op == 1)`, i.e. a colour-op LERP select | `LerpVertexColor` | medium |
| `gfdLightFlag` | `0x1` (0) | `gfdLightEnable` | `LightFlags` bit 0 | high |
| `gfdLightFlag` | `0x2` (1) | set by `gfdLightSetAttenuationDistance`; selects the distance pair over the coefficient triple | `LightFlags.Bit2` in our reader | high |
| `gfdAnimControllerFlag` | `0x4` (2) | gates `neck` in `gfdAnimControllerStreamRead` | `AnimationPackFlags.Bit2` | high |
| `gfdEPLFlag` | `0x4` (2) | unconditionally ORed in by `gfdEPLStreamReadInside` |  | high |

Nothing further is recoverable for `gfdMaterialFlag` bits 11, 14, 15 and 31,
for `gfdMaterialFlag2` bits 11-15, for the remaining `gfdGeometryFlag` bits,
for `gfdAnimationFlag` beyond the read-gating bits already documented in the
first pass, or for `FieldTextureFlags` (see below).

### Flag words for which nothing at all survives

`gfdEPLFlag`, `gfdEPLLeafFlag`, `gfdEPLBehaviorFlag`, `gfdEPLParticleFlag`,
`gfdEPLPostEffectFlag`, `gfdEPLPostEffectLensFlareFlag`,
`gfdEPLPolygonBoardFlag`, `gfdEPLPolygonCircleFlag`, `gfdEPLPolygonFlashFlag`,
`gfdEPLPolygonGlitterFlag`, `gfdEPLPolygonThunderFlag`,
`gfdEPLPolygonTrackFlag`, `gfdEPLPolygonWindFlag`, `gfdEPLCameraFlag`,
`gfdEPLLightFlag`, `gfdEPLMeshFlag`, `gfdEPLHelperFlag`,
`gfdEPLObjectParticleFlag`, `gfdEPLTextureFlag`, `gfdEPLDisplacementFlag`.

All of these exist as named *types* at the right struct offsets, so we know
which field is the flag word and what it is called (`flags`); we do not know
what any individual bit means.

## Priority 3: leftovers in structures covered by the first pass

### `gfdKeyMOTION` → `GFDLibrary.Animations.KeyType22`

Reader `gfdKeyMOTIONStreamRead_0` at **`0x894448`**, writer at `0x8942AC`. This
names all twelve of our placeholders. `gfdKeyCONTROLLER` is
`{flags, anim, blend, weight, speed}`.

| # | Official field | Type | Offset | C# member | Conf. |
|---|---|---|---|---|---|
| 0 | `type` | `gfdKeyMotionType` u32 | `0x04` | `Field04` | high |
| 1 | `data.flags` | u32 | `0x08` | `Field08` | high |
| 2 | `data.anim` | u16 on the wire | `0x0C` | `Field0C` | high |
| 3 | `data.blend` | f32 | `0x10` | `Field10` | high |
| 4 | `data.weight` | f32 | `0x14` | `Field14` | high |
| 5 | `data.speed` | f32 | `0x18` | `Field18` | high |
| 6 | `next.flags` | u32 | `0x1C` | `Field1C` | high |
| 7 | `next.anim` | u16 on the wire | `0x20` | `Field20` | high |
| 8 | `next.blend` | f32 | `0x24` | `Field24` | high |
| 9 | `next.weight` | f32 | `0x28` | `Field28` | high |
| 10 | `next.speed` | f32 | `0x2C` | `Field2C` | high |
| 11 | `interp` | `gfdKeyInterp`, u8 on the wire | `0x00` | `Field00` | high |

Note `anim` is declared `gfdUInt32` but streamed as a `u16`, which is why our
`Field0C`/`Field20` are `short`. That is correct behaviour, not a bug.

### `gfdShaderID` → `GFDLibrary.Shaders.Shader`

`gfdShaderID_tag`: `type` (u16) `0x00`, `flag[3]` (u32) `0x04`, `texcoord.in`
`0x10`, `texcoord.out` `0x14`.

| Official field | C# member | Conf. |
|---|---|---|
| `id.type` | `ShaderType` | high |
| `id.flag[0]` | `MatFlags0` | high |
| `id.flag[1]` | `MatFlags1` | high |
| `id.flag[2]` | `MatFlags2` | high |
| `id.texcoord.in` | `Texcoord0` | high |
| `id.texcoord.out` | `Texcoord1` | high |

`Field06` is most likely the two-byte hole between `type` (u16 at `0x00`) and
`flag[0]` (u32 at `0x04`), written out because the whole `gfdShaderID` is
blitted. **Low**  the struct has no member there, so this is positional
reasoning only. `ShaderPS3.Field06_2` and `ShaderPS3.Field1C` have no
counterpart in `gfdShaderID` at all and I could not place them; **low**.

There is no `gfdShaderStreamRead`. The shader cache is read by
`gfdShaderCacheLoadInside` and the per-shader bodies are opaque blobs
(`gfdShaderVertex_tag` and `gfdShaderFragment_tag` are `_BYTE` gaps in the type
library), so the C# side is already as good as the binary allows.

### `gfdCamera` → `Camera.Field190`

`gfdCameraSetRoll_0` writes `+400` (`0x190`), which is exactly the field the
reader fills under the `> 0x1104060` gate. So **`Field190` is `roll`**, and
this upgrades the first pass's medium to **high**. `gfdCameraSetFovy_0` writes
`+392` (`0x188`), confirming `FieldOfView`. `Field198`, `Field19C` and
`Field1A0` are read only above `0x2110050` and do not exist here; **no
evidence**.

### `gfdLight` → the `Light.cs` placeholders

Setter targets pin most of these. `gfdLight_tag` itself is opaque, so the names
are the setters' names rather than struct members: **high** where a setter
writes exactly the offset the reader fills.

| Offset | Written by | Read when | C# member | Conf. |
|---|---|---|---|---|
| `0x10`, `0x14`, `0x18` | `gfdLightSetPosition` | types 2 and 3 | `Field10`, `Field04`, `Field08` | high |
| `0x20`, `0x24`, `0x28` | (direction triple) | types 1 and 3 | `Field20`, `Field04`, `Field08` | medium |
| `0x60`, `0x64`, `0x68` | `gfdLightSetAttenuation` | when `flags & 2` is clear | `Field60`, `Field64`, `Field68` | high |
| `0x6C`, `0x70` | `gfdLightSetAttenuationDistance` | when `flags & 2` is set | `AttenuationStart`, `AttenuationEnd` | high |
| `0x74`, `0x78` | `gfdLightSetCone` | type 3 | `AngleInnerCone`, `AngleOuterCone` | high |
| `0x7C` | `gfdLightEnable` and others | always | `Flags` | high |
| `0x84` | `gfdLightSetScale` | not streamed |  | high |
| `0x88` | `gfdLightSetAlpha` | not streamed |  | high |

So `Field60`/`Field64`/`Field68` are the **attenuation coefficients** (the
constant, linear and quadratic terms), confirming the first pass's guess.

There is a real modelling problem here worth flagging: our `Field04` and
`Field08` are reused for two different official fields. For a point light they
are `position.y` and `position.z`; for a directional or spot light they are
`direction.y` and `direction.z`. `Field10` is `position.x` and `Field20` is
`direction.x`. The file is read and written consistently, so nothing is
corrupted, but the two vectors should be modelled separately.

`Light.Field98` and `Light.Field9C` are read only above `0x2110191` and
`0x2110203` and do not exist here; **no evidence**.

### Items with no evidence in this build

Stated plainly rather than guessed:

- **`FieldTexturePS3.cs`** (`Field00`, `Field08`, `Field0C`, `Field1A`,
 `Field1B`, `Field1C`, `Field24`, and `FieldTextureFlags` bits 1-7). There is
 no `gfdFieldTexture*` symbol, no `gfdField*` function of any kind, and no
 matching struct in the type library. This format is not handled by the `gfd`
 layer in this executable. Nothing recovered.
- **`MaterialParameterSet_Metaphor.cs`** (`Field0` through `Field6`). The
 Metaphor material parameter sets post-date this build by two major format
 versions; the gate in our reader is `Version >= 0x2000000` and this image is
 a `0x11xxxxx`-era build. Nothing recovered.
- **`Model.Field100_10`** (gate `>= 0x2040001`), **`Material.Field6C_2`**
 (gate `> 0x2110160`), **`Camera.Field198/19C/1A0`** (gate `> 0x2110050`),
 **`Light.Field98/9C`**, **`Epl.Field40/Field60/Field70`**,
 **`EplLeaf.Field34/Field3C`**, **`EplLeafCommonData2.Field62`**, and the
 whole `EplModel` `_P5R` block. All are later-version additions absent from
 this executable. Nothing recovered.
- **`AnimationBit28DataEntry` / `AnimationBit29Data` / `AnimationBit31Data`**
 were already fully named in the first pass (`gfdAnimationEffect`,
 `gfdAnimationNeck`, `gfdAnimationTranslation`); there is nothing left in them.
- **`EplAnimation.cs`** (`Field00`, `Field04`, and
 `EplAnimationController.Field00/Field04/Field0C`). Our `EplAnimation` is
 `gfdTimeLine` (reader `gfdTimeLineStreamRead_0` at `0x88ABAC`) and
 `EplAnimationController` is `gfdTimeLineTrack`. Both structs exist
 (`gfdTimeLine`, `gfdTimeLineTrack_tag`) but I did not decompile the timeline
 serialiser in this pass, so I am leaving these unmapped rather than guessing.
 This is the one remaining Epl gap and the obvious next target.
- **`ChunkType000100F9Entry1`** particle field names (`SphereRadius`, `Mass`,
 `RestoringForce`, `WindRate`). Unchanged from the first pass: file order and
 widths are certain, the names are still ours. `gfdPhysicsSector_tag_0` names
 the arrays but not the particle members.

## Additional serialiser addresses (Epl family)

| Structure | `StreamRead` | `StreamWrite` |
|---|---|---|
| `gfdEPL` | `0x8A7310` (inside: `0x8A6F20`) | `0x8A3FE8` |
| `gfdEPLLeaf` | `0x8A4A70` | `0x8A39F8` |
| `gfdEPLParts` | `0x8A36DC` | `0x8A3570` |
| `gfdEPLCurve` | `0x8A2CDC` | `0x8A277C` |
| `gfdEPLCurve4` | `0x8A29E0` | `0x8A23F4` |
| `gfdEPLRange` | `0x8A2990` | `0x8A2390` |
| `gfdEPLFade` | `0x8A2940` | `0x8A232C` |
| `gfdEPLTexture` | `0x8A1BF4` | `0x8A165C` |
| `gfdEPLDisplacement` | `0x8A121C` | `0x8A07B8` |
| `gfdEPLParticle` | `0x9754D4` | `0x9740BC` |
| `gfdEPLDirectionalParticle` | `0x953E54` | (shares Particle's) |
| `gfdEPLObjectParticle` | `0x956424` | `0x9542C8` |
| `gfdEPLBehaviorParams` | `0x96E7AC` | `0x96DFDC` |
| `gfdEPLBehaviorSmoke` | `0x96E6DC` | `0x96DF08` |
| `gfdEPLBehaviorSpark` | `0x96E5CC` | `0x96DE34` |
| `gfdEPLBehaviorSpiral` | `0x96E4D8` | `0x96DD4C` |
| `gfdEPLBehaviorSphere` | `0x96E408` | `0x96DC90` |
| `gfdEPLBehaviorRing` | `0x96E2FC` | `0x96DBA8` |
| `gfdEPLBehaviorLine` | `0x96E258` | `0x96DAD4` |
| `gfdEPLPostEffect` | `0x950AA4` | `0x94FFDC` |
| `gfdEPLPolygonGlitter` | `0x95E450` | `0x95CDB4` |
| `gfdEPLPolygonWind` | `0x962918` | `0x96173C` |
| `gfdEPLPolygonFlash` | `0x96C000` | `0x96A98C` |
| `gfdEPLPolygonCircle` | `0x968F6C` | `0x9681FC` |
| `gfdEPLPolygonThunder` | `0x966D8C` | `0x966238` |
| `gfdEPLPolygonTrack` | `0x965D14` | `0x9643A0` |
| `gfdEPLPolygonBoard` | `0x960174` | `0x95F554` |
| `gfdEPLMesh` | `0x95BA70` | `0x95B4D4` |
| `gfdEPLCamera` | `0x95AE00` | `0x959AB0` |
| `gfdEPLLight` | `0x957EF0` | `0x957A14` |
| `gfdEPLHelper` | `0x94F210` | `0x94E39C` |
| `gfdTimeLine` | `0x88ABAC` | `0x88A214` |

Dispatch tables used above: `gfdEPLPartsCategoryFuncTable` at `0x1470F34`,
`gfdEPLBehaviorTypeTable` at `0x1082C68`, `gfdEPLPostEffectTypeTable` at
`0x10827B0`, `gfdEPLPolygonGlitterTypeTable` at `0x10829E4`,
`gfdEPLPolygonWindTypeTable` at `0x1082AA4`, `gfdEPLPolygonFlashTypeTable` at
`0x1082BD8`, `gfdEPLPolygonCircleTypeTable` at `0x1082B4C`,
`gfdEPLPolygonThunderTypeTable` at `0x1082B04`,
`gfdEPLPolygonBoardTypeTable` at `0x1082A5C`, `gfdEPLMeshTypeTable` at
`0x10829A8`, `gfdEPLCameraTypeTable` at `0x1082960`, `gfdEPLLightTypeTable` at
`0x1082918`.

---
---

# Third pass: debug printers as a naming source

Prompted by the `gfdVertexFormat` correction. Where a structure has a debug
printer or a to-string helper, that beats stride arithmetic or any other
inference, because the printer names the bits outright. This pass applies that
idea systematically. The `gfdVertexFormat` table earlier in this document has
been corrected and upgraded in place; what follows is the rest of the sweep.

## What the sweep covered

Every function in the image that calls `gfdPrintf`, filtered to the `gfd`
layer. Thirty-six functions, the substantial ones being:

| Calls | Address | Function |
|---|---|---|
| 97 | `0x8CBE94` | `gfdMaterialPrintf_0` |
| 50 | `0x8E5958` | `gfdNodePrintf_0` |
| 18 | `0x91D8D0` | `gfdThJobGeometryCreateJobData_0` |
| 13 | `0x875178` | `gfdPropertyPrintf_0` |
| 13 | `0x89E2C0` | `gfdPhotonGeometryArrayBuild_0` |
| 12 | `0x8E1B24` | `gfdNodePrintfHierarchyRecursive_0` |
| 10 | `0x880F1C` | `gfdTMXConvertToDDS_0` |
| 8 | `0x886AB4` | `gfdTaskPrintfRecursive_0` |

Plus a scan of every named global that looks like an array of string pointers.

## The one real new recovery: `texSlotName`

`gfdMaterialPrintf_0` walks the ten texture slots (stride `0x50`, the size of
`gfdMaterialTexture`) and labels each from a global the symbol table names
**`texSlotName`**, at **`0x1070D2C`**, pointing at a run of eight-byte padded
strings at `0xF24520`. Ten entries, in slot order:

| Slot | Flag bit | Official name | C# `Material` member | Conf. |
|---|---|---|---|---|
| 0 | `0x00100000` (20) | `diffuse` | `DiffuseMap` | high |
| 1 | `0x00200000` (21) | `normal` | `NormalMap` | high |
| 2 | `0x00400000` (22) | `gloss` | `GlossMap` | high |
| 3 | `0x00800000` (23) | `reflect` | `ReflectionMap` | high |
| 4 | `0x01000000` (24) | `multi` | `MultiMap` | high |
| 5 | `0x02000000` (25) | `glow` | `GlowMap` | high |
| 6 | `0x04000000` (26) | `dark` | `DarkMap` | high |
| 7 | `0x08000000` (27) | `detail` | `DetailMap` | high |
| 8 | `0x10000000` (28) | `light` | `LightMap` | high |
| 9 | `0x20000000` (29) | `reserve` | `ReserveMap` | high |

High confidence: the symbol is named `texSlotName`, the table has exactly ten
entries, and the printer indexes it with the same slot index the material
reader uses for `textures[i]`.

Three of our names are wrong, and two of the corrections are self-consistent
with findings from the first pass:

- Slot 2 is **`gloss`**, not specular. (`gfdMaterial` already has a separate
  `specular` colour member, so "SpecularMap" conflates two different things.)
- Slot 4 is **`multi`**, the multi-texture slot. This corroborates the first
  pass's reading of `blend.multiple` as the multi-texture blend mode, which we
  currently call `HighlightMapBlendMode`. The two should be renamed together.
- Slot 8 is **`light`**, a light map, not a shadow map. This also lines up with
  the later-generation `FLAG1_LIGHTMAP_MODULATE2` and `gfdEnvironmentLightMap`
  names that already exist in our `MaterialFlags.cs`.
- Slot 9 is **`reserve`**, which is to say the tenth slot was spare. Our
  `TextureMap10` is as good a name as the original.

## Enumerations confirmed by the same string block

The block around `0xF244D8`, used by `gfdMaterialPrintf_0`, spells out the
alpha-test functions in order. They match our `AlphaClipMode` exactly, which
upgrades that enum from "matches by position" to named:

| Value | Official | C# `AlphaClipMode` | Conf. |
|---|---|---|---|
| 0 | `NEVER` | `Never` | high |
| 1 | `LESS` | `Less` | high |
| 2 | `EQUAL` | `Equal` | high |
| 3 | `LEQUAL` | `LEqual` | high |
| 4 | `GREATER` | `Greater` | high |
| 5 | `NOTEQUAL` | `NotEqual` | high |
| 6 | `GEQUAL` | `GEqual` | high |
| 7 | `ALWAYS` | `Always` | high |

The same block holds `(BACK)` and `(NONE)` for `gfdCulling`, and `(ambient)`,
`(diffuse)`, `(emissive)`, `(specular)` for the four colour vectors, which
re-confirms the `gfdMaterial` colour order already documented in the first
pass.

## What the sweep did *not* yield

This is the important half, and it does not change the Priority 2 conclusion.

- **`gfdMaterialPrintf_0` does not print flag bit names.** It prints the
  colours, the blend and alpha-test state, the shader id (unpacking
  `texcoord.in` and `texcoord.out` three bits at a time, ten groups each), and
  the ten texture slots. The flag words themselves are printed as raw hex. So
  there is still no name for any `gfdMaterialFlag` or `gfdMaterialFlag2` bit
  beyond the two with named setters (`0x1000` wire, `0x2000` alpha-test).
- **No `gfd` printer names any Epl flag bit.** The eleven
  `gfdEPL<Category>Flag` words have no printer at all.
- **The named-global scan found nothing else in the `gfd` layer.**
  `texSlotName` was the only string-pointer table belonging to the graphics
  library; every other hit (`CAMERA_MODE_NAME`, `FLD_TABLE_FILE_NAME`,
  `DagOpcodeNames`, the `btl::AuthenticCommand` tables, and so on) is game or
  shader-compiler code.
- `gfdNodePrintf_0` and `gfdPropertyPrintf_0` print values, not bit names, so
  they add nothing to the flag problem. `gfdPropertyPrintf_0` does confirm the
  nine `gfdPropertyType` cases already documented.

## Method note

Two things worth carrying forward:

1. **Prefer a printer to arithmetic.** Stride accumulation tells you a size,
   never an order and never a name. A debug printer or a `GetName` helper tells
   you both. The same applies to the per-category `gfdEPL*TypeTable` entries
   used in the second pass: `gfdEPLCameraTypeTable` and `gfdEPLLightTypeTable`
   carry real name strings (`MESH`, `QUAKE`, `SCENE`) while the others carry
   null name pointers, which is why those two variant tables are the only ones
   named rather than inferred from the handler symbol.
2. **Round-tripping is not an ordering proof.** A reader and writer that agree
   with each other will round trip byte for byte whatever order they use. Only
   a function that consumes the data for its real purpose, such as
   `gfdDeviceRenderTransferImDataToVertexBuffer_0` building vertex element
   descriptors, can settle a layout question.
