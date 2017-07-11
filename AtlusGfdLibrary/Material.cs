using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Material
    {
        public string Name { get; set; }

        // 0x54
        public MaterialFlags Flags { get; set; }

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

        public TextureMap DiffuseMap { get; set; }

        public TextureMap NormalMap { get; set; }

        public TextureMap SpecularMap { get; set; }

        public TextureMap ReflectionMap { get; set; }

        public TextureMap HighlightMap { get; set; }

        public TextureMap GlowMap { get; set; }

        public TextureMap NightMap { get; set; }

        public TextureMap DetailMap { get; set; }

        public TextureMap ShadowMap { get; set; }

        public List<MaterialProperty> Properties { get; set; }

        public Material()
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
            Flags = (MaterialFlags)3; 
            Field92 = 4;
            Field70 = -1;
            Field98 = -1;
            Field6C = -1;
            Properties = new List<MaterialProperty>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}