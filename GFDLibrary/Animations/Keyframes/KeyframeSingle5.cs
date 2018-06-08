using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeSingle5 : Keyframe
    {
        public override KeyframeType Type { get; internal set; }

        public float Field00 { get; set; }

        public float Field04 { get; set; }

        public float Field08 { get; set; }

        public float Field0C { get; set; }

        public float Field10 { get; set; }

        public KeyframeSingle5() : this( KeyframeType.Single5 ) { }

        public KeyframeSingle5( KeyframeType type )
        {
            Type = type;
        }

        internal override void Read( ResourceReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Field00 );
            writer.WriteSingle( Field04 );
            writer.WriteSingle( Field08 );
            writer.WriteSingle( Field0C );
            writer.WriteSingle( Field10 );
        }
    }
}