using System.Diagnostics;
using System.IO;

namespace GFDLibrary.IO
{
    public class ResourceFileHeader
    {
        public const int SIZE = 16;

        public ResourceFileIdentifier Identifier { get; set; }
        public uint Version { get; set; }
        public ResourceType Type { get; set; }

        internal void Read( BinaryReader reader )
        {
            Identifier = ( ResourceFileIdentifier )reader.ReadInt32();
            Version = reader.ReadUInt32();
            Type = ( ResourceType )reader.ReadInt32();
            Trace.Assert( reader.ReadInt32() == 0 );
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( (int)Identifier );
            writer.Write( Version );
            writer.Write( ( int )Type );
            writer.Write( 0 );
        }
    }
}