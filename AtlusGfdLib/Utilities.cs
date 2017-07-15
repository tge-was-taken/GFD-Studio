using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public static class Utilities
    {
        public static bool NearlyEquals( this float a, float b, float epsilon = 3f )
        {
            return true;

            float absA = Math.Abs( a );
            float absB = Math.Abs( b );
            float diff = Math.Abs( a - b );

            if ( a == b )
            { // shortcut, handles infinities
                return true;
            }
            else if ( a == 0 || b == 0 || diff < float.MinValue )
            {
                return true;
            }
            else
            { // use relative error
                return diff / Math.Min( ( absA + absB ), float.MaxValue ) < epsilon;
            }
        }

        public static bool NearlyEquals( this Vector2 lhs, Vector2 rhs )
        {
            return NearlyEquals( lhs.X, rhs.X ) && NearlyEquals( lhs.Y, rhs.Y );
        }

        public static bool NearlyEquals( this Vector3 lhs, Vector3 rhs )
        {
            return NearlyEquals( lhs.X, rhs.X ) && NearlyEquals( lhs.Y, rhs.Y ) && NearlyEquals( lhs.Z, rhs.Z );
        }

        public static bool NearlyEquals( this Vector4 lhs, Vector4 rhs )
        {
            return NearlyEquals( lhs.X, rhs.X ) && NearlyEquals( lhs.Y, rhs.Y ) && NearlyEquals( lhs.Z, rhs.Z ) && NearlyEquals( lhs.W, rhs.W );
        }

        public static bool NearlyEquals( this Quaternion lhs, Quaternion rhs )
        {
            return NearlyEquals( lhs.X, rhs.X ) && NearlyEquals( lhs.Y, rhs.Y ) && NearlyEquals( lhs.Z, rhs.Z ) && NearlyEquals( lhs.W, rhs.W );
        }

        public static bool NearlyEquals( this Matrix4x4 lhs, Matrix4x4 rhs )
        {
            return NearlyEquals( lhs.M11, lhs.M11 ) && NearlyEquals( lhs.M12, lhs.M12 ) && NearlyEquals( lhs.M13, lhs.M13 ) && NearlyEquals( lhs.M14, lhs.M14 ) &&
                   NearlyEquals( lhs.M21, lhs.M21 ) && NearlyEquals( lhs.M22, lhs.M22 ) && NearlyEquals( lhs.M23, lhs.M23 ) && NearlyEquals( lhs.M24, lhs.M24 ) &&
                   NearlyEquals( lhs.M31, lhs.M31 ) && NearlyEquals( lhs.M32, lhs.M32 ) && NearlyEquals( lhs.M33, lhs.M33 ) && NearlyEquals( lhs.M34, lhs.M34 ) &&
                   NearlyEquals( lhs.M41, lhs.M41 ) && NearlyEquals( lhs.M42, lhs.M42 ) && NearlyEquals( lhs.M43, lhs.M43 ) && NearlyEquals( lhs.M44, lhs.M44 );
        }
    }
}
