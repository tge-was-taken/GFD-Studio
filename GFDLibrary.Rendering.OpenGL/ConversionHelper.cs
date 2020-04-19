using OpenTK;

namespace GFDLibrary.Rendering.OpenGL
{
    public static class ConversionHelper
    {
        public static unsafe Matrix4 ToOpenTK( this System.Numerics.Matrix4x4 value ) => *( Matrix4* )&value;
        public static unsafe Vector3 ToOpenTK( this System.Numerics.Vector3 value ) => *( Vector3* ) &value;
        public static unsafe Vector4 ToOpenTK( this System.Numerics.Vector4 value ) => *( Vector4* )&value;
        public static unsafe Quaternion ToOpenTK( this System.Numerics.Quaternion value ) => *( Quaternion* )&value;

        public static unsafe System.Numerics.Quaternion ToNumerics( this Quaternion value ) => *( System.Numerics.Quaternion* )&value;
        public static unsafe System.Numerics.Vector3 ToNumerics( this Vector3 value ) => *( System.Numerics.Vector3* )&value;
    }
}
