using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class QuaternionKey : Key
    {
        public Quaternion Value { get; set; }

        public QuaternionKey() : this( KeyType.Quaternion ) { }

        public QuaternionKey( KeyType type ) : base( type )
        {
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