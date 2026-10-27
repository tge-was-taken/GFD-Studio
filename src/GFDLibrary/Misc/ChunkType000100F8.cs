using GFDLibrary.IO;

namespace GFDLibrary.Misc
{
    /// <summary>
    /// Only used by 1 model (c0003_051_00.GMD)
    /// </summary>
    public sealed class ChunkType000100F8 : Resource
    {
        public byte[] Data { get; set; }

        public override ResourceType ResourceType => ResourceType.ChunkType000100F8;

        public ChunkType000100F8( uint version ) : base( version )
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            throw new System.NotImplementedException();
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( Data != null )
                writer.WriteBytes( Data );
        }
    }
}