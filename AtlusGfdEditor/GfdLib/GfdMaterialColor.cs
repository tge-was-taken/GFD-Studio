using System.Drawing;

namespace AtlusGfdEditor.GfdLib
{
    public struct GfdMaterialColor
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Intensity;

        public GfdMaterialColor(float red, float green, float blue, float intensity)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Intensity = intensity;
        }

        public GfdMaterialColor(int red, int green, int blue, int intensity)
        {
            Red         = ((float)red)          / byte.MaxValue; ;
            Green       = ((float)green)        / byte.MaxValue; ;
            Blue        = ((float)blue)         / byte.MaxValue; ;
            Intensity   = ((float)intensity)    / byte.MaxValue;
        }

        public GfdMaterialColor(Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public static explicit operator Color(GfdMaterialColor color)
        {
            return Color.FromArgb((int)(color.Intensity    * byte.MaxValue), 
                                  (int)(color.Red          * byte.MaxValue), 
                                  (int)(color.Green        * byte.MaxValue), 
                                  (int)(color.Blue         * byte.MaxValue));
        }

        public static explicit operator GfdMaterialColor(Color color)
        {
            return new GfdMaterialColor(color);
        }
    }
}