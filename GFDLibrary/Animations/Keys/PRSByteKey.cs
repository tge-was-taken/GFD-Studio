using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class PRSByteKey : PRSKey
    {
        public byte Byte { get; set; }

        public PRSByteKey() : base( KeyType.PRSByte ) { }

        internal override void Read( ResourceReader reader )
        {
            base.Read( reader );
            Byte = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            base.Write( writer );
            writer.WriteByte( Byte );
        }
    }
}