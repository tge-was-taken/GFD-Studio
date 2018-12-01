namespace GFDLibrary.Textures.Swizzle
{
    internal static class SwizzleUtilities
    {
        public static int Morton( int t, int sx, int sy )
        {
            int num1;
            int num2 = num1 = 1;
            int num3 = t;
            int num4 = sx;
            int num5 = sy;
            int num6 = 0;
            int num7 = 0;

            while ( num4 > 1 || num5 > 1 )
            {
                if ( num4 > 1 )
                {
                    num6 +=  num2 * ( num3 & 1 );
                    num3 >>= 1;
                    num2 *=  2;
                    num4 >>= 1;
                }
                if ( num5 > 1 )
                {
                    num7 +=  num1 * ( num3 & 1 );
                    num3 >>= 1;
                    num1 *=  2;
                    num5 >>= 1;
                }
            }

            return num7 * sx + num6;
        }
    }
}
