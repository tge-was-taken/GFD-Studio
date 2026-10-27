using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class Single3ByteKey : Key
    {
        public float Field00 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public byte Byte { get; set; }

        public Single3ByteKey() : base( KeyType.Single4Byte )
        {

        }

        internal override void Read( ResourceReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Byte = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteByte( Byte );
        }
    }
}