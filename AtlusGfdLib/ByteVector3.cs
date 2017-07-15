using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public struct ByteVector3 : IEquatable<ByteVector3>
    {
        public byte X;
        public byte Y;
        public byte Z;

        public ByteVector3( byte x, byte y, byte z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ByteVector3( ByteVector4 vec4 )
        {
            X = vec4.X;
            Y = vec4.Y;
            Z = vec4.Z;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( ByteVector3 ) )
                return false;

            return Equals( ( ByteVector3 )obj );
        }

        public bool Equals( ByteVector3 other )
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 33 + X.GetHashCode();
                hash = hash * 33 + Y.GetHashCode();
                hash = hash * 33 + Z.GetHashCode();
                return hash;
            }
        }
    }
}
