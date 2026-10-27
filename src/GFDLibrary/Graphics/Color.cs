using GFDLibrary.Materials;
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

        public uint ABGR
        {
            get
            {
                return (uint)( A << 24 | B << 16 | G << 8 | R );
            }
            set
            {
                A = (byte)( ( value >> 24 ) & 0xFF );
                B = (byte)( ( value >> 16 ) & 0xFF );
                G = (byte)( ( value >> 8 ) & 0xFF );
                R = (byte)( value & 0xFF );
            }
        }

        public uint RGBA
        {
            get
            {
                return (uint)( R << 24 | G << 16 | B << 8 | A );
            }
            set
            {
                R = (byte)( ( value >> 24 ) & 0xFF );
                G = (byte)( ( value >> 16 ) & 0xFF );
                B = (byte)( ( value >> 8 ) & 0xFF );
                A = (byte)( value & 0xFF );
            }
        }

        public Color( byte r, byte g, byte b, byte a = byte.MaxValue )
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color( float r, float g, float b, float a = 1 )
        {
            R = (byte)Math.Round( Math.Clamp( r, 0f, 1f ) * 255f );
            G = (byte)Math.Round( Math.Clamp( g, 0f, 1f ) * 255f );
            B = (byte)Math.Round( Math.Clamp( b, 0f, 1f ) * 255f );
            A = (byte)Math.Round( Math.Clamp( a, 0f, 1f ) * 255f );
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

        public static Color FromABGR(uint bits)
        {
            unchecked
            {
                return new Color()
                {
                    A = (byte)( bits >> 24 ),
                    B = (byte)( bits >> 16 ),
                    G = (byte)( bits >> 8 ),
                    R = (byte)( bits )
                };
            }
        }

        public static Color FromRGBA(uint bits)
        {
            unchecked
            {
                return new Color()
                {
                    R = (byte)( bits >> 24 ),
                    G = (byte)( bits >> 16 ),
                    B = (byte)( bits >> 8 ),
                    A = (byte)( bits )
                };
            }
        }
    }
}
#pragma warning restore S2328 // "GetHashCode" should not reference mutable fields
#pragma warning restore S1104 // Fields should not have public accessibility
