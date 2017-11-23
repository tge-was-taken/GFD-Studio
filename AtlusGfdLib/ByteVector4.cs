using System;

namespace AtlusGfdLib
{
    /// <summary>
    /// Represents a vector consisting out of 4 byte components.
    /// </summary>
    public struct ByteVector4 : IEquatable<ByteVector4>
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
        /// The W component of the vector.
        /// </summary>
        public byte W;

        /// <summary>
        /// Creates a new <see cref="ByteVector4"/> with the specified component values.
        /// </summary>
        /// <param name="x">The value of the X component.</param>
        /// <param name="y">The value of the Y component.</param>
        /// <param name="z">The value of the Z component.</param>
        /// <param name="w">The value of the W component.</param>
        public ByteVector4( byte x, byte y, byte z, byte w )
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Creates a new <see cref="ByteVector4"/> by taking the X, Y and Z components from a <see cref="ByteVector3"/> and using the <paramref name="w"/> for the W component.
        /// </summary>
        /// <param name="vec3">The <see cref="ByteVector3"/> whose X, Y and Z components are used.</param>
        /// <param name="w">The value of the W component.</param>
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
