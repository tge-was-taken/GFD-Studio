using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframePRSByte : KeyframePRS
    {
        public override KeyframeType Type => KeyframeType.PRSByte;

        public byte Byte { get; set; }

        public KeyframePRSByte() : base( KeyframeType.PRSByte ) { }

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