using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary.Animations
{
    public class Vector3Key : Key
    {
        public Vector3 Value { get; set; }

        public Vector3Key() : this( KeyType.Vector3 ) { }

        public Vector3Key( KeyType type ) : base( type )
        {
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
