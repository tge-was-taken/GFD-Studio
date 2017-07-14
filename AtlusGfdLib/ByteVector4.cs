using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public struct ByteVector4
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
    }
}
