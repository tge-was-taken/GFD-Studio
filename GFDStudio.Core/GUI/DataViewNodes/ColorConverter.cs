using System.Drawing;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public static class ColorConverter
    {
        public static Vector4 ToFloat( this Color value )
        {
            return new Vector4( ( ( float ) value.R ) / 255f,
                                ( ( float ) value.G ) / 255f,
                                ( ( float ) value.B ) / 255f,
                                ( ( float ) value.A ) / 255f );
        }

        public static Color ToByte( this Vector4 value )
        {
            return Color.FromArgb( ( byte ) ( value.W * 255f ),
                                   ( byte ) ( value.X * 255f ),
                                   ( byte ) ( value.Y * 255f ),
                                   ( byte ) ( value.Z * 255f ) );
        }
    }
}