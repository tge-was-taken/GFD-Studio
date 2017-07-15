using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public struct ByteVector4 : IEquatable<ByteVector4>
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public ByteVector4( byte x, byte y, byte z, byte w )
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public ByteVector4( ByteVector3 vec3, byte w )
        {
            X = vec3.X;
            Y = vec3.Y;
            Z = vec3.Z;
            W = w;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}, {W}]";
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( ByteVector4 ) )
                return false;

            return Equals( ( ByteVector4 )obj );
        }

        public bool Equals( ByteVector4 other )
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
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
