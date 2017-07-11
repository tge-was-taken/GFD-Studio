
namespace AtlusGfdLib
{
    public struct Color
    {
        public float R, G, B, A;

        public Color(float red, float green, float blue, float alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color(int red, int green, int blue, int alpha)
        {
            R = ((float)red)   / byte.MaxValue; 
            G = ((float)green) / byte.MaxValue; 
            B = ((float)blue)  / byte.MaxValue; 
            A = ((float)alpha) / byte.MaxValue;
        }

        public Color( System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public static explicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb((int)( color.A * byte.MaxValue), 
                                  (int)( color.R * byte.MaxValue), 
                                  (int)( color.G * byte.MaxValue), 
                                  (int)( color.B * byte.MaxValue));
        }

        public static explicit operator Color( System.Drawing.Color color)
        {
            return new Color(color);
        }
    }
}