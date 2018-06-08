using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeSingle4Byte : Keyframe
    {
        public override KeyframeType Type
        {
            get => KeyframeType.Single4Byte;
            internal set { }
        }

        public float Field00 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public float Field0C { get; set; }

        public byte Byte { get; set; }

        public KeyframeSingle4Byte() { }

        internal override void Read( ResourceReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Byte = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteByte( Byte );
        }
    }
}