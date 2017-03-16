namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdMaterial
    {
        public GfdString Name { get; set; }
        public uint Flags { get; set; }
        public GfdMaterialColor Ambient { get; set; }
        public GfdMaterialColor Diffuse { get; set; }
        public GfdMaterialColor Specular { get; set; }
        public GfdMaterialColor Field30 { get; set; }
        public float Field40 { get; set; }
        public float Field44 { get; set; }
        public byte[] Field48 { get; set; }
        public uint Field58 { get; set; }
        public uint Field5C { get; set; }
        public ushort Field60 { get; set; }
        public ushort Field62 { get; set; }
        public GfdMaterialTextureReference[] TextureReferences { get; }

        internal GfdMaterial()
        {
            TextureReferences = new GfdMaterialTextureReference[11];
        }
    }
}