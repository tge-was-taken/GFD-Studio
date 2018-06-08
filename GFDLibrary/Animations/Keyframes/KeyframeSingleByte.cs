using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeSingleByte : Keyframe
    {
        public override KeyframeType Type
        {
            get => KeyframeType.SingleByte;
            internal set { }
        }

        public float Field00 { get; set; }

        public byte Byte { get; set; }

        public KeyframeSingleByte() { }

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