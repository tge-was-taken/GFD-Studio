using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class SingleByteKey : Key
    {
        public float Field00 { get; set; }

        public byte Byte { get; set; }

        public SingleByteKey() : base( KeyType.SingleByte )
        {

        }

        internal override void Read( ResourceReader reader )
        {
            Field00 = reader.ReadSingle();
            Byte = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Field00 );
            writer.WriteByte( Byte );
        }
    }
}