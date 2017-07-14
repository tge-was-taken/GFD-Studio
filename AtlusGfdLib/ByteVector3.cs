using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public struct ByteVector3
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
    }
}
