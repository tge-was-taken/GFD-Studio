using System;

namespace AtlusGfdLibrary
{
    /// <summary>
    /// Represents a vector consisting out of 3 byte components.
    /// </summary>
    public struct ByteVector3 : IEquatable<ByteVector3>
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public byte X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public byte Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public byte Z;

        /// <summary>
        /// Creates a new <see cref="ByteVector3"/> with the specified component values.
        /// </summary>
        /// <param name="x">The value of the X component.</param>
        /// <param name="y">The value of the Y component.</param>
        /// <param name="z">The value of the Z component.</param>
        public ByteVector3( byte x, byte y, byte z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new <see cref="ByteVector3"/> with the specified component values taken from a <see cref="ByteVector4"/>.
        /// </summary>
        /// <param name="vec4">The <see cref="ByteVector4"/> whose component values to use.</param>
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
