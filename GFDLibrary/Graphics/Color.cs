using System;

// ReSharper disable NonReadonlyMemberInGetHashCode
#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable S2328 // "GetHashCode" should not reference mutable fields
namespace GFDLibrary.Graphics
{
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Black = new Color( 0, 0, 0 );
        public static readonly Color Gray = new Color( 127, 127, 127 );
        public static readonly Color White = new Color( 255, 255, 255 );

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color( byte r, byte g, byte b, byte a = byte.MaxValue )
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = R.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ G.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ B.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ A.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals( object obj )
        {
            return obj is Color color && Equals( color );
        }

        public bool Equals( Color other )
        {
            return R == other.R &&
                     G == other.G &&
                     B == other.B &&
                     A == other.A;
        }

        public static bool operator ==( Color color1, Color color2 )
        {
            return color1.Equals( color2 );
        }

        public static bool operator !=( Color color1, Color color2 )
        {
            return !( color1 == color2 );
        }

        public static implicit operator Color( System.Drawing.Color color )
        {
            return new Color( color.R, color.G, color.B, color.A );
        }
    }
}
#pragma warning restore S2328 // "GetHashCode" should not reference mutable fields
#pragma warning restore S1104 // Fields should not have public accessibility
