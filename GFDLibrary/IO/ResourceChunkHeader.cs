using System.Diagnostics;
using System.IO;

namespace GFDLibrary.IO
{
    public class ResourceChunkHeader
    {
        public const int SIZE = 16;

        public uint Version { get; set; }
        public ResourceChunkType Type { get; set; }
        public int Length { get; set; }
        public int Reserved { get; set; }

        internal ResourceChunkHeader()
        {         
        }

        public ResourceChunkHeader( uint version, ResourceChunkType type, int length )
        {
            Version = version;
            Type = type;
            Length = length;
            Reserved = 0;
        }

        internal void Read( BinaryReader reader )
        {
            Version = reader.ReadUInt32();
            Type = ( ResourceChunkType )reader.ReadInt32();
            Length = reader.ReadInt32();
            Reserved = reader.ReadInt32();
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( Version );
            writer.Write( ( int )Type );
            writer.Write( Length );
            writer.Write( Reserved );
        }
    }
}