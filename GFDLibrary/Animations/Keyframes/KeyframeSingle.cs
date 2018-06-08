using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeSingle : Keyframe
    {
        public override KeyframeType Type { get; internal set; }

        public float Value { get; set; }

        public KeyframeSingle() : this( KeyframeType.Single ) { }

        public KeyframeSingle(KeyframeType type)
        {
            Type = type;
        }

        internal override void Read( ResourceReader reader )
        {
            Value = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteSingle( Value );
        }
    }
}