using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDLibrary.Utilities
{
    public static class MathHelper
    {
        public static float RadiansToDegrees( float radians ) => radians * 57.2958f;
        public static double RadiansToDegrees( double radians ) => radians * 57.2958d;
        public static float  DegreesToRadians( float  degrees ) => degrees / 57.2958f;
        public static double DegreesToRadians( double degrees ) => degrees / 57.2958d;
    }
}
