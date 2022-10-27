using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using GFDLibrary.IO;
using GFDLibrary.Models;
using GFDLibrary.Models.Conversion;

namespace GFDLibrary.Materials
{
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

        // 0x48
        public MaterialDrawMethod DrawMethod { get; set; }

        // 0x49
        public byte Field49 { get; set; }

        // 0x4A
        public byte Field4A { get; set; }

        // 0x4B
        public byte Field4B { get; set; }

        // 0x4C
        public byte Field4C { get; set; }

        // 0x4D
        public byte Field4D { get; set; }

        // 0x90
        public short Field90 { get; set; }

        // 0x92
        public short Field92 { get; set; }

        // 0x94
        public short Field94 { get; set; }

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
            Field40 = 1.0f;
            Field44 = 0;
            Field49 = 1;
            Field4B = 1;
            Field4D = 1;
            DrawMethod = 0;
            Field4A = 0;
            Field4C = 0;
            Field5C = 0;
            Flags = MaterialFlags.Bit0 | MaterialFlags.Bit1;
            Field92 = 4;
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

        protected override void ReadCore( ResourceReader reader )
        {
            // Read material header
            Name = reader.ReadStringWithHash( Version );
            var flags = ( MaterialFlags )reader.ReadUInt32();

            if ( Version < 0x1104000 )
            {
                flags = ( MaterialFlags )( ( uint )Flags & 0x7FFFFFFF );
            }

            AmbientColor = reader.ReadVector4();
            DiffuseColor = reader.ReadVector4();
            SpecularColor = reader.ReadVector4();
            EmissiveColor = reader.ReadVector4();
            Field40 = reader.ReadSingle();
            Field44 = reader.ReadSingle();

            if ( Version <= 0x1103040 )
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadInt16();
                Field49 = ( byte )reader.ReadInt16();
                Field4A = ( byte )reader.ReadInt16();
                Field4B = ( byte )reader.ReadInt16();
                Field4C = ( byte )reader.ReadInt16();

                if ( Version > 0x108011b )
                {
                    Field4D = ( byte )reader.ReadInt16();
                }
            }
            else
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadByte();
                Field49 = reader.ReadByte();
                Field4A = reader.ReadByte();
                Field4B = reader.ReadByte();
                Field4C = reader.ReadByte();
                Field4D = reader.ReadByte();
            }

            Field90 = reader.ReadInt16();
            Field92 = reader.ReadInt16();

            if ( Version <= 0x1104800 )
            {
                Field94 = 1;
                Field96 = ( short )reader.ReadInt32();
            }
            else
            {
                Field94 = reader.ReadInt16();
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

            if ( flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                DiffuseMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                NormalMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                SpecularMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                ReflectionMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                HighlightMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                GlowMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                NightMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                DetailMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                ShadowMap = reader.ReadResource<TextureMap>( Version );
            }

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
            Trace.Assert( Flags == flags, "Material flags don't match flags from file" );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, Name );
            writer.WriteUInt32( ( uint )Flags );
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteSingle( Field40 );
            writer.WriteSingle( Field44 );

            if ( Version <= 0x1103040 )
            {
                writer.WriteInt16( ( short )DrawMethod );
                writer.WriteInt16( Field49 );
                writer.WriteInt16( Field4A );
                writer.WriteInt16( Field4B );
                writer.WriteInt16( Field4C );

                if ( Version > 0x108011b )
                {
                    writer.WriteInt16( Field4D );
                }
            }
            else
            {
                writer.WriteByte( ( byte )DrawMethod );
                writer.WriteByte( Field49 );
                writer.WriteByte( Field4A );
                writer.WriteByte( Field4B );
                writer.WriteByte( Field4C );
                writer.WriteByte( Field4D );
            }

            writer.WriteInt16( Field90 );
            writer.WriteInt16( Field92 );

            if ( Version <= 0x1104800 )
            {
                writer.WriteInt32( Field96 );
            }
            else
            {
                writer.WriteInt16( Field94 );
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
        Bit0            = 1 << 00,
        Bit1            = 1 << 01,
        Bit2            = 1 << 02,
        Bit3            = 1 << 03,
        EnableVertColors = 1 << 04,
        Bit5           = 1 << 05,
        Bit6           = 1 << 06,
        EnableLight      = 1 << 07,
        Bit8          = 1 << 08,
        Bit9          = 1 << 09,
        Bit10          = 1 << 10,
        EnableLight2     = 1 << 11,
        PurpleWireframe  = 1 << 12,
        Bit13         = 1 << 13,
        ReceiveShadow    = 1 << 14,
        CastShadow       = 1 << 15,
        HasAttributes    = 1 << 16,
        Bit17   = 1 << 17,
        Bit18   = 1 << 18,
        DisableBloom     = 1 << 19,
        HasDiffuseMap    = 1 << 20,
        HasNormalMap     = 1 << 21,
        HasSpecularMap   = 1 << 22,
        HasReflectionMap = 1 << 23,
        HasHighlightMap  = 1 << 24,
        HasGlowMap       = 1 << 25,
        HasNightMap      = 1 << 26,
        HasDetailMap     = 1 << 27,
        HasShadowMap     = 1 << 28,
        Bit29= 1 << 29,
        Bit30     = 1 << 30,
        Bit31     = 1u << 31
    }

    public enum MaterialDrawMethod
    {
        Opaque,
        Translucent,
    }
}
