namespace GFDLibrary.IO.Resource
{
    internal struct ResourceChunkHeader
    {
        public uint Version;
        public ResourceChunkType Type;
        public int Size;
        public int Unknown;

        public const int SIZE = 16;
        public const int SIZE_OFFSET = 8;
    }

    internal enum ResourceChunkType : uint
    {
        Invalid            = 0,
        Scene              = 0x00010003,
        ChunkType000100F8  = 0x000100F8,
        ChunkType000100F9  = 0x000100F9,
        MaterialDictionary = 0x000100FB,
        TextureDictionary  = 0x000100FC,
        AnimationPackage   = 0x000100FD,

        // Custom ones
        CustomGenericResourceBundle = 'R' << 24 | 'I' << 16 | 'G' << 8 | 0,
        CustomTexture,
        CustomMaterial,
        CustomTextureMap,
        CustomMaterialAttribute,
    }
}
