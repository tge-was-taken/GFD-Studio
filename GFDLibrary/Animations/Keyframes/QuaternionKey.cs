using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class QuaternionKey : Key
    {
        public override KeyType Type { get; internal set; }

        public Quaternion Value { get; set; }

        public QuaternionKey() : this( KeyType.Quaternion ) { }

        public QuaternionKey( KeyType type )
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