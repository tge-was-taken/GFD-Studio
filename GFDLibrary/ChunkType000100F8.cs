namespace GFDLibrary
{
    public sealed class ChunkType000100F8 : Resource
    {
        public byte[] RawData { get; set; }

        public ChunkType000100F8( uint version ) : base( ResourceType.ChunkType000100F8, version )
        {
        }
    }
}