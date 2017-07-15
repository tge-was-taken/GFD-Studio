namespace AtlusGfdLib
{
    internal struct ChunkHeader
    {
        public uint Version;
        public ChunkType Type;
        public int Size;
        public int Unknown;

        public const int CSIZE = 16;
        public const int SIZE_OFFSET = 8;
    }

    internal enum ChunkType : uint
    {
        Invalid            = 0,
        Scene              = 0x00010003,
        MaterialDictionary = 0x000100FB,
        TextureDictionary  = 0x000100FC,
        AnimationPackage   = 0x000100FD
    }
}
