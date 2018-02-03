namespace AtlusGfdLibrary
{
    public sealed class TextureMap : Resource
    {
        public string Name { get; set; }

        // 0x44
        public int Field44 { get; set; }

        // 0x48
        public byte Field48 { get; set; }

        // 0x49
        public byte Field49 { get; set; }

        // 0x4A
        public byte Field4A { get; set; }

        // 0x4B
        public byte Field4B { get; set; }
        
        // 0x4C
        public float Field4C { get; set; }

        // 0x50
        public float Field50 { get; set; }

        // 0x54
        public float Field54 { get; set; }

        // 0x58
        public float Field58 { get; set; }

        // 0x5C
        public float Field5C { get; set; }

        // 0x60
        public float Field60 { get; set; }

        // 0x64
        public float Field64 { get; set; }

        // 0x68
        public float Field68 { get; set; }

        // 0x6C
        public float Field6C { get; set; }

        // 0x70
        public float Field70 { get; set; }

        // 0x74
        public float Field74 { get; set; }

        // 0x78
        public float Field78 { get; set; }

        // 0x7C
        public float Field7C { get; set; }

        // 0x80
        public float Field80 { get; set; }

        // 0x84
        public float Field84 { get; set; }

        // 0x88
        public float Field88 { get; set; }

        internal TextureMap() : base(ResourceType.TextureMap, PERSONA5_RESOURCE_VERSION)
        {
        }

        public TextureMap( string name ) : this()
        {
            Name = name;
            Field44 = 0;
            Field48 = 1;
            Field49 = 1;
            Field4A = 0;
            Field4B = 0;
            Field4C = 1;
            Field50 = 0;
            Field54 = 0;
            Field58 = 0;
            Field5C = 0;
            Field60 = 1;
            Field64 = 0;
            Field68 = 0;
            Field6C = 0;
            Field70 = 0;
            Field74 = 0;
            Field78 = 0;
            Field7C = 0;
            Field80 = 0;
            Field84 = 0;
            Field88 = 0;
        }
    }
}
