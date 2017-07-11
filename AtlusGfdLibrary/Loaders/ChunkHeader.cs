namespace AtlusGfdLib
{
    internal struct ChunkHeader
    {
        public uint Version;
        public ChunkType Type;
        public int Size;
        public int Unknown;

        public const int SIZE = 12;
    }
}
