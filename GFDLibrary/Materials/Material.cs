using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using GFDLibrary.IO;
using GFDLibrary.Models;
using GFDLibrary.Models.Conversion;
using YamlDotNet.Core;

namespace GFDLibrary.Materials
{
    public sealed class MaterialLegacyParameters : Resource
    {
        public override ResourceType ResourceType => ResourceType.MaterialParameterSetLegacy;
        // 0x00
        public Vector4 AmbientColor { get; set; }

        // 0x10
        public Vector4 DiffuseColor { get; set; }

        // 0x20
        public Vector4 SpecularColor { get; set; }

        // 0x30
        public Vector4 EmissiveColor { get; set; }

        // 0x40
        public float Field40 { get; set; }

        // 0x44
        public float Field44 { get; set; }

        public MaterialLegacyParameters()
        {
            Field40 = 1.0f;
            Field44 = 0;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            AmbientColor = reader.ReadVector4();
            DiffuseColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            Field40 = reader.ReadSingle();
            Field44 = reader.ReadSingle();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteSingle( Field40 );
            writer.WriteSingle( Field44 );
        }
    }
    public sealed class Material : Resource
    {
        public override ResourceType ResourceType => ResourceType.Material;

        public string Name { get; set; }

        // 0x54
        private MaterialFlags mFlags;
        public MaterialFlags Flags
        {
            get => mFlags;
            set
            {
                mFlags = value;
                ValidateFlags();
            }
        }

        // 0x48
        public MaterialDrawMethod DrawMethod { get; set; }

        public MaterialLegacyParameters LegacyParameters { get; set; }

        // 0x49
        public byte Field49 { get; set; }

        // 0x4A
        public byte Field4A { get; set; }

        // 0x4B
        public byte Field4B { get; set; }

        // 0x4C
        public byte Field4C { get; set; }

        // 0x4D
        public HighlightMapMode Field4D { get; set; }

        // 0x90
        public short Field90 { get; set; }

        // 0x92
        public AlphaClipMode Field92 { get; set; }

        // 0x94
        //public short Field94 { get; set; }
        private MaterialFlags2 Field94;

        public MaterialFlags2 Flags2
        {
            get => Field94;
            set
            {
                Field94 = value;
            }
        }

        // 0x96
        public short Field96 { get; set; }

        // 0x5C
        public short Field5C { get; set; }

        // 0x6C
        public uint Field6C { get; set; }

        // 0x70
        public uint Field70 { get; set; }

        // 0x50
        public short DisableBackfaceCulling { get; set; }

        // 0x98
        public uint Field98 { get; set; }

        private TextureMap mDiffuseMap;
        public TextureMap DiffuseMap
        {
            get => mDiffuseMap;
            set
            {
                mDiffuseMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mNormalMap;
        public TextureMap NormalMap
        {
            get => mNormalMap;
            set
            {
                mNormalMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mSpecularMap;
        public TextureMap SpecularMap
        {
            get => mSpecularMap;
            set
            {
                mSpecularMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mReflectionMap;
        public TextureMap ReflectionMap
        {
            get => mReflectionMap;
            set
            {
                mReflectionMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mHightlightMap;
        public TextureMap HighlightMap
        {
            get => mHightlightMap;
            set
            {
                mHightlightMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mGlowMap;
        public TextureMap GlowMap
        {
            get => mGlowMap;
            set
            {
                mGlowMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mNightMap;
        public TextureMap NightMap
        {
            get => mNightMap;
            set
            {
                mNightMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mDetailMap;
        public TextureMap DetailMap
        {
            get => mDetailMap;
            set
            {
                mDetailMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mShadowMap;
        public TextureMap ShadowMap
        {
            get => mShadowMap;
            set
            {
                mShadowMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mTextureMap10;
        public TextureMap TextureMap10
        {
            get => mTextureMap10;
            set
            {
                mTextureMap10 = value;
                ValidateFlags();
            }
        }

        public IEnumerable<TextureMap> TextureMaps
        {
            get
            {
                yield return DiffuseMap;
                yield return NormalMap;
                yield return SpecularMap;
                yield return ReflectionMap;
                yield return HighlightMap;
                yield return GlowMap;
                yield return NightMap;
                yield return DetailMap;
                yield return ShadowMap;
                yield return TextureMap10;
            }
        }

        private List<MaterialAttribute> mAttributes;
        public List<MaterialAttribute> Attributes
        {
            get => mAttributes;
            set
            {
                mAttributes = value;
                ValidateFlags();
            }
        }

        // METAPHOR REFANTAZIO
        // 0x2dc
        public ushort METAPHOR_MaterialParameterFormat { get; set; }
        public bool METAPHOR_UseMaterialParameterSet { get; set; }
        public MaterialParameterSetBase METAPHOR_MaterialParameterSet { get; set; }
        public float Field6C_2 { get; set; }
        public METAPHOR_MaterialFlags0 MatFlags0 { get; set; }
        public METAPHOR_MaterialFlags1 MatFlags1 { get; set; }
        public METAPHOR_MaterialFlags2 MatFlags2 { get; set; }

        public bool IsPresetMaterial { get; internal set; }

        public Material( string name )
            : this()
        {
            Name = name;
            IsPresetMaterial = true;
        }

        public Material()
        {
            Initialize();
        }

        public Material( uint version ) : base( version )
        {
            Initialize();
        }

        public override string ToString()
        {
            return Name;
        }


        private void Initialize()
        {
            LegacyParameters = new();
            Field49 = 1;
            Field4B = 1;
            Field4D = (HighlightMapMode)1;
            DrawMethod = 0;
            Field4A = 0;
            Field4C = 0;
            Field5C = 0;
            Flags = MaterialFlags.HasAmbientColor | MaterialFlags.HasDiffuseColor;
            Field92 = ( AlphaClipMode)4;
            Field70 = 0xFFFFFFFF;
            Field98 = 0xFFFFFFFF;
            Field6C = 0xFFFFFFFF;
        }

        private void ValidateFlags()
        {
            ValidateMapFlags( DiffuseMap,    MaterialFlags.HasDiffuseMap );
            ValidateMapFlags( NormalMap,     MaterialFlags.HasNormalMap );
            ValidateMapFlags( SpecularMap,   MaterialFlags.HasSpecularMap );
            ValidateMapFlags( ReflectionMap, MaterialFlags.HasReflectionMap );
            ValidateMapFlags( HighlightMap,  MaterialFlags.HasHighlightMap );
            ValidateMapFlags( GlowMap,       MaterialFlags.HasGlowMap );
            ValidateMapFlags( NightMap,      MaterialFlags.HasNightMap );
            ValidateMapFlags( DetailMap,     MaterialFlags.HasDetailMap );
            ValidateMapFlags( ShadowMap,     MaterialFlags.HasShadowMap );
            ValidateMapFlags( TextureMap10,  MaterialFlags.HasTextureMap10 );

            if ( Attributes == null )
            {
                mFlags &= ~MaterialFlags.HasAttributes;
            }
            else
            {
                mFlags |= MaterialFlags.HasAttributes;
            }
        }

        private void ValidateMapFlags( TextureMap map, MaterialFlags flag )
        {
            if ( map == null )
            {
                mFlags &= ~flag;
            }
            else
            {
                mFlags |= flag;
            }
        }

        private bool FUN_141070860()
        {
            if ( !METAPHOR_UseMaterialParameterSet )
                return false;
            if ( DrawMethod == MaterialDrawMethod.Opaque )
                return true;
            return METAPHOR_MaterialParameterSet.IsMaterialTransparent( );
        }

        // from gfdMaterialGetShaderFlags(gfdMaterial*, shaderId, vertexFlags, uint* flags)
        public void GetShaderFlags()
        {
            if ( !METAPHOR_UseMaterialParameterSet )
                return;

            MatFlags1 = METAPHOR_MaterialFlags1.FLAG1_MATERIAL_AMBDIFF;
            MatFlags2 = 0;
            if ( Flags.HasFlag( MaterialFlags.EnableLight ) )
            {
                MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_LIGHT0_DIRECTION;
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_LIGHT;
                if ( Flags2.HasFlag( MaterialFlags2.ShadowMapAdd ) )
                    MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_LIGHTMAP_MODULATE;
                else if ( Flags2.HasFlag( MaterialFlags2.ShadowMapMultiply ) )
                    MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_LIGHTMAP_MODULATE2;
            }
            MatFlags0 |= (METAPHOR_MaterialFlags0)0x4000;
            if (Flags.HasFlag(MaterialFlags.AlphaTest ) )
            {
                MatFlags2 |= Field92 switch
                {
                    AlphaClipMode.Never => METAPHOR_MaterialFlags2.FLAG2_ATEST_NEVER,
                    AlphaClipMode.Less | AlphaClipMode.LEqual => METAPHOR_MaterialFlags2.FLAG2_ATEST_LESS_LEQUAL,
                    AlphaClipMode.Equal => METAPHOR_MaterialFlags2.FLAG2_ATEST_EQUAL,
                    AlphaClipMode.Greater | AlphaClipMode.GEqual => METAPHOR_MaterialFlags2.FLAG2_ATEST_GREATER_GEQUAL,
                    AlphaClipMode.NotEqual => METAPHOR_MaterialFlags2.FLAG2_ATEST_NOTEQUAL,
                    _ => 0
                };
            }



            if ( Flags.HasFlag( MaterialFlags.Diffusitivity ) )
                MatFlags0 |= (METAPHOR_MaterialFlags0.FLAG0_TEMPERARE | (METAPHOR_MaterialFlags0)0x4000);
            if (Flags.HasFlag( MaterialFlags.HasVertexColors ) ) // and vertex flags & 0x40 (support vertex colors)
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_VERTEXCOLOR;

            if (Flags.HasFlag( MaterialFlags.ApplyFog ) )
            {
                // This relies on the graphics flag set in gfdGlobal
                // Add an option to toggle these in settings?
                /*
                if (HasFog)
                {
                    MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_FOG;
                }
                if (HasHeightFog)
                {
                    MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_HEIGHTFOG;
                }
                MatFlags2 |= METAPHOR_MaterialFlags1.FLAG2_MATERIAL_MODULATE_FOG;
                */
            }
            if ( Flags.HasFlag( MaterialFlags.ExtraDistortion ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_OCCLUSION;
            if ( Flags.HasFlag( MaterialFlags.HasEmissiveColor ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_EMISSIVE;
            if ( Flags.HasFlag( MaterialFlags.ReceiveShadow ) ) // gfdGlobal graphics flag 1
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_SHADOW;
            // multiply material mode
            if ( Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                switch ( Field4D )
                {
                    case HighlightMapMode.Lerp: MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_MATERIAL_MULTIPLE_SEMI; break; // Lerp
                    case HighlightMapMode.Add: MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_MATERIAL_MULTIPLE_ADD; break; // Add
                    case HighlightMapMode.Subtract: MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_MATERIAL_MULTIPLE_MOD; break; // Modulate
                    case (HighlightMapMode)5: MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_MATERIAL_MULTIPLE_SUB; break; // Subtract
                }
            }
            // texture flags
            if ( Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE1;
            if ( Flags.HasFlag( MaterialFlags.HasNormalMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE2;
            if ( Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE3;
            if ( Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE4;
            if ( Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE5;
            if ( Flags.HasFlag( MaterialFlags.HasGlowMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE6;
            if ( Flags.HasFlag( MaterialFlags.HasNightMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE7;
            if ( Flags.HasFlag( MaterialFlags.HasDetailMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE8;
            if ( Flags.HasFlag( MaterialFlags.HasShadowMap ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE9;
            if ( Flags.HasFlag( MaterialFlags.HasTextureMap10 ) )
                MatFlags1 |= METAPHOR_MaterialFlags1.FLAG1_TEXTURE10;
            // material parameter flags
            METAPHOR_MaterialParameterSet.SetShaderFlags( this );
            // other
            if ( Flags.HasFlag( MaterialFlags.ReflectionCaster ) )
                MatFlags2 |= METAPHOR_MaterialFlags2.FLAG2_REFLECTION_CASTER;
            if ( Flags2.HasFlag( MaterialFlags2.Grayscale ) )
                MatFlags0 |= METAPHOR_MaterialFlags0.FLAG0_CONSTANTCOLOR | METAPHOR_MaterialFlags0.FLAG0_GRAYSCALE;
        }

        protected override void ReadCore( ResourceReader reader )
        {
            // Read material header
            METAPHOR_MaterialParameterFormat = 1;
            METAPHOR_UseMaterialParameterSet = Version >= 0x2000000;
            if ( Version >= 0x2000000 )
            {
                METAPHOR_MaterialParameterFormat = reader.ReadUInt16();
            }
            Name = reader.ReadStringWithHash( Version );
            var flags = ( MaterialFlags )reader.ReadUInt32();

            if ( Version < 0x1104000 )
            {
                flags = ( MaterialFlags )( ( uint )Flags & 0x7FFFFFFF );
            }

            if (Version < 0x2000000 )
            {
                LegacyParameters = reader.ReadResource<MaterialLegacyParameters>( Version );
            } else
            {
                switch ( METAPHOR_MaterialParameterFormat )
                {
                    case 0:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType0>( Version );
                        break;
                    case 1:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType1>( Version );
                        break;
                    case 2:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType2>( Version );
                        break;
                    case 3:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType3>( Version );
                        break;
                    case 4:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType4>( Version );
                        break;
                    case 5:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType5>( Version );
                        break;
                    case 6:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType6>( Version );
                        break;
                    case 7:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType7>( Version );
                        break;
                    case 8:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType8>( Version );
                        break;
                    case 9:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType9>( Version );
                        break;
                    case 0xa:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType10>( Version );
                        break;
                    case 0xb:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType11>( Version );
                        break;
                    case 0xc:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType12>( Version );
                        break;
                    case 0xd:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType13>( Version );
                        break;
                    case 0xe:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType14>( Version );
                        break;
                    case 0xf:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType15>( Version );
                        break;
                    case 0x10:
                        METAPHOR_MaterialParameterSet = reader.ReadResource<MaterialParameterSetType16>( Version );
                        break;
                    default:
                        throw new InvalidDataException( $"Unknown/Invalid material parameter version {METAPHOR_MaterialParameterFormat}" );
                }
            }

            if ( Version <= 0x1103040 )
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadInt16();
                Field49 = ( byte )reader.ReadInt16();
                Field4A = ( byte )reader.ReadInt16();
                Field4B = ( byte )reader.ReadInt16();
                Field4C = ( byte )reader.ReadInt16();

                if ( Version > 0x108011b )
                {
                    Field4D = (HighlightMapMode)reader.ReadInt16();
                }
            }
            else
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadByte();
                Field49 = reader.ReadByte();
                Field4A = reader.ReadByte();
                Field4B = reader.ReadByte();
                Field4C = reader.ReadByte();
                Field4D = (HighlightMapMode)reader.ReadByte();
            }

            Field90 = reader.ReadInt16();
            Field92 = ( AlphaClipMode)reader.ReadInt16();

            if ( Version <= 0x1104800 )
            {
                Field94 = (MaterialFlags2)1;
                Field96 = ( short )reader.ReadInt32();
            }
            else
            {
                Field94 = (MaterialFlags2)reader.ReadInt16();
                Field96 = reader.ReadInt16();
            }

            Field5C = reader.ReadInt16();
            Field6C = reader.ReadUInt32();
            Field70 = reader.ReadUInt32();
            DisableBackfaceCulling = reader.ReadInt16();

            if ( Version <= 0x1105070 || Version >= 0x1105090 || Version == 0x1105080 )
            {
                Field98 = reader.ReadUInt32();
            }
            Field6C_2 = 0;
            if ( Version > 0x2110160 )
                Field6C_2 = reader.ReadSingle();

            TextureMap ReadTextureMapResource()
            {
                var texMap = reader.ReadResource<TextureMap>( Version );
                if ( METAPHOR_UseMaterialParameterSet )
                    texMap.METAPHOR_ParentMaterialParameterSet = METAPHOR_MaterialParameterSet;
                return texMap;
            }

            if ( flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
                DiffuseMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasNormalMap ) )
                NormalMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasSpecularMap ) )
                SpecularMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasReflectionMap ) )
                ReflectionMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasHighlightMap ) )
                HighlightMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasGlowMap ) )
                GlowMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasNightMap ) )
                NightMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasDetailMap ) )
                DetailMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasShadowMap ) )
                ShadowMap = ReadTextureMapResource();
            if ( flags.HasFlag( MaterialFlags.HasTextureMap10 ) )
                ShadowMap = ReadTextureMapResource();

            if ( flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                Attributes = new List<MaterialAttribute>();
                int attributeCount = reader.ReadInt32();

                for ( int i = 0; i < attributeCount; i++ )
                {
                    var attribute = MaterialAttribute.Read( reader, Version );
                    Attributes.Add( attribute );
                }
            }
            Flags = flags;
            // Trace.Assert( Flags == flags, "Material flags don't match flags from file" );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( Version >= 0x2000000 )
                writer.WriteUInt16( METAPHOR_MaterialParameterFormat );
            writer.WriteStringWithHash( Version, Name );
            writer.WriteUInt32( ( uint )Flags );
            if ( METAPHOR_UseMaterialParameterSet )
                METAPHOR_MaterialParameterSet.Write( writer );
            else
            {
                writer.WriteResource( LegacyParameters );
            }

            if ( Version <= 0x1103040 )
            {
                writer.WriteInt16( ( short )DrawMethod );
                writer.WriteInt16( Field49 );
                writer.WriteInt16( Field4A );
                writer.WriteInt16( Field4B );
                writer.WriteInt16( Field4C );

                if ( Version > 0x108011b )
                {
                    writer.WriteInt16( (byte)Field4D );
                }
            }
            else
            {
                writer.WriteByte( ( byte )DrawMethod );
                writer.WriteByte( Field49 );
                writer.WriteByte( Field4A );
                writer.WriteByte( Field4B );
                writer.WriteByte( Field4C );
                writer.WriteByte( (byte)Field4D );
            }

            writer.WriteInt16( Field90 );
            writer.WriteInt16( ( short ) Field92 );

            if ( Version <= 0x1104800 )
            {
                writer.WriteInt32( Field96 );
            }
            else
            {
                writer.WriteInt16( (short)Field94 );
                writer.WriteInt16( Field96 );
            }

            writer.WriteInt16( Field5C );
            writer.WriteUInt32( Field6C );
            writer.WriteUInt32( Field70 );
            writer.WriteInt16(DisableBackfaceCulling);

            if ( Version <= 0x1105070 || Version >= 0x1105090 )
            {
                writer.WriteUInt32( Field98 );
            }
            if ( Version > 0x2110160 )
                writer.WriteSingle( Field6C_2 );

            if ( Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                writer.WriteResource( DiffuseMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                writer.WriteResource(  NormalMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                writer.WriteResource(  SpecularMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                writer.WriteResource(  ReflectionMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                writer.WriteResource(  HighlightMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                writer.WriteResource(  GlowMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                writer.WriteResource(  NightMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                writer.WriteResource(  DetailMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                writer.WriteResource(  ShadowMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasTextureMap10 ) )
            {
                writer.WriteResource( TextureMap10 );
            }

            if ( Flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                writer.WriteInt32( Attributes.Count );

                foreach ( var attribute in Attributes )
                    MaterialAttribute.Write( writer, attribute );
            }
        }

        public static Material ConvertToMaterialPreset(Material material, ModelPackConverterOptions options)
        {
            Material newMaterial = null;

            var materialName = material.Name;
            var diffuseTexture = material.DiffuseMap;
            var shadowTexture = material.ShadowMap;
            if (shadowTexture == null)
                shadowTexture = material.DiffuseMap;
            var specularTexture = material.SpecularMap;
            if (specularTexture == null)
                specularTexture = material.DiffuseMap;
            if ( diffuseTexture == null ) newMaterial = material;
            else newMaterial = MaterialFactory.CreateMaterial( materialName, diffuseTexture.Name, options );
            return newMaterial;
        }
    }

    [Flags]
    public enum MaterialFlags : uint
    {
        HasAmbientColor = 1 << 00,
        HasDiffuseColor = 1 << 01,
        HasSpecularColor = 1 << 02,
        Transparency = 1 << 03,
        HasVertexColors = 1 << 04,
        ApplyFog = 1 << 05,
        Diffusitivity = 1 << 06,
        HasUVAnimation = 1 << 07,
        HasEmissiveColor = 1 << 08,
        HasReflection = 1 << 09,
        EnableShadow = 1 << 10,
        EnableLight = 1 << 11,
        RenderWireframe = 1 << 12,
        AlphaTest = 1 << 13,
        ReceiveShadow = 1 << 14,
        CastShadow = 1 << 15,
        HasAttributes = 1 << 16,
        HasOutline = 1 << 17,
        SpecularInNormalMap = 1 << 18,
        ReflectionCaster = 1 << 19,
        HasDiffuseMap = 1 << 20,
        HasNormalMap = 1 << 21,
        HasSpecularMap = 1 << 22,
        HasReflectionMap = 1 << 23,
        HasHighlightMap = 1 << 24,
        HasGlowMap = 1 << 25,
        HasNightMap = 1 << 26,
        HasDetailMap = 1 << 27,
        HasShadowMap = 1 << 28,
        HasTextureMap10 = 1 << 29,
        ExtraDistortion = 1 << 30,
        Bit31 = 1u << 31
    }

    [Flags]
    public enum MaterialFlags2 : ushort
    {
        Bloom = 1 << 00,
        ShadowMapAdd = 1 << 01,
        ShadowMapMultiply = 1 << 02,
        DisableHDR = 1 << 03,
        DisableDeferred = 1 << 04,
        DisableOutline = 1 << 05,
        OpaqueAlpha1 = 1 << 06,
        LerpVertexColor = 1 << 07,
        ReflectionMapAdd = 1 << 08,
        Grayscale = 1 << 09,
        DisableFog = 1 << 10,
        Bit11 = 1 << 11,
        Bit12 = 1 << 12,
        Bit13 = 1 << 13,
        Bit14 = 1 << 14,
        Bit15 = 1 << 15
    }

    public enum MaterialDrawMethod
    {
        Opaque,
        Transparent,
        Add,
        Subtract,
        Modulate,
        ModulateTransparent,
        Modulate2Transparent,
        Advanced
    }
    public enum AlphaClipMode
    {
        Never,
        Less,
        Equal,
        LEqual,
        Greater,
        NotEqual,
        GEqual,
        Always
    }
    public enum HighlightMapMode
    {
        Lerp = 1,
        Add = 2,
        Subtract = 3,
        Modulate = 4
    }
}
