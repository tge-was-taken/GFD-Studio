using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Material
    {
        private string mName;
        public string Name
        {
            get => mName;
            set => mName = value ?? throw new ArgumentNullException( nameof( value ) );
        }

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
        public Vector4 Ambient { get; set; }

        // 0x10
        public Vector4 Diffuse { get; set; }

        // 0x20
        public Vector4 Specular { get; set; }

        // 0x30
        public Vector4 Emissive { get; set; }

        // 0x40
        public float Field40 { get; set; }

        // 0x44
        public float Field44 { get; set; }

        // 0x48
        public byte Field48 { get; set; }

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
        public int Field6C { get; set; }

        // 0x70
        public int Field70 { get; set; }

        // 0x50
        public short Field50 { get; set; }

        // 0x98
        public int Field98 { get; set; }

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

        private List<MaterialProperty> mProperties;
        public List<MaterialProperty> Properties
        {
            get => mProperties;
            set
            {
                mProperties = value;
                ValidateFlags();
            }
        }

        public Material( string name )
            : this()
        {
            Name = name;
        }

        internal Material()
        {
            Field40 = 1.0f;
            Field44 = 0;
            Field49 = 1;
            Field4B = 1;
            Field4D = 1;
            Field48 = 0;
            Field4A = 0;
            Field4C = 0;
            Field5C = 0;
            Flags = MaterialFlags.Flag1 | MaterialFlags.Flag2;
            Field92 = 4;
            Field70 = -1;
            Field98 = -1;
            Field6C = -1;
        }

        public override string ToString()
        {
            return Name;
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

            if ( Properties == null )
            {
                mFlags &= ~MaterialFlags.HasProperties;
            }
            else
            {
                mFlags |= MaterialFlags.HasProperties;
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
    }

    [Flags]
    public enum MaterialFlags : uint
    {
        Flag1            = 1 << 00,
        Flag2            = 1 << 01,
        Flag4            = 1 << 02,
        Flag8            = 1 << 03,
        Flag10           = 1 << 04,
        Flag20           = 1 << 05,
        Flag40           = 1 << 06,
        Flag80           = 1 << 07,
        Flag100          = 1 << 08,
        Flag200          = 1 << 09,
        Flag400          = 1 << 10,
        Flag800          = 1 << 11,
        Flag1000         = 1 << 12,
        Flag2000         = 1 << 13,
        Flag4000         = 1 << 14,
        Flag8000         = 1 << 15,
        HasProperties    = 1 << 16,
        Flag20000        = 1 << 17,
        Flag40000        = 1 << 18,
        Flag80000        = 1 << 19,
        HasDiffuseMap    = 1 << 20,
        HasNormalMap     = 1 << 21,
        HasSpecularMap   = 1 << 22,
        HasReflectionMap = 1 << 23,
        HasHighlightMap  = 1 << 24,
        HasGlowMap       = 1 << 25,
        HasNightMap      = 1 << 26,
        HasDetailMap     = 1 << 27,
        HasShadowMap     = 1 << 28,
        Flag20000000     = 1 << 29,
        Flag40000000     = 1 << 30,
        Flag80000000     = 1u << 31
    }
}