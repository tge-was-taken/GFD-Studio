namespace AtlusGfdLib.IO
{
    internal struct ResourceChunkHeader
    {
        public uint Version;
        public ResourceChunkType Type;
        public int Size;
        public int Unknown;

        public const int CSIZE = 16;
        public const int SIZE_OFFSET = 8;
    }

    internal enum ResourceChunkType : uint
    {
        Invalid            = 0,
        Scene              = 0x00010003,
        MaterialDictionary = 0x000100FB,
        TextureDictionary  = 0x000100FC,
        AnimationPackage   = 0x000100FD,
        Type000100F9       = 0x000100F9,
    }
}
