using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public class KeyframeQuaternion : Keyframe
    {
        public override KeyframeType Type { get; internal set; }

        public Quaternion Value { get; set; }

        public KeyframeQuaternion() : this( KeyframeType.Quaternion ) { }

        public KeyframeQuaternion( KeyframeType type )
        {
            Type = type;
        }

        internal override void Read( ResourceReader reader )
        {
            Value = reader.ReadQuaternion();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteQuaternion( Value );
        }
    }
}