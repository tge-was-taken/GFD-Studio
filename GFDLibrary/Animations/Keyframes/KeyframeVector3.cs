using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeVector3 : Keyframe
    {
        public override KeyframeType Type { get; internal set; }

        public Vector3 Value { get; set; }

        public KeyframeVector3() : this( KeyframeType.Vector3 ) { }

        public KeyframeVector3( KeyframeType type )
        {
            Type = type;
        }

        internal override void Read( ResourceReader reader )
        {
            Value = reader.ReadVector3();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteVector3( Value );
        }
    }
}
