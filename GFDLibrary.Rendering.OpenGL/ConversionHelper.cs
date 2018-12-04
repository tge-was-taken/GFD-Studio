using OpenTK;

namespace GFDLibrary.Rendering.OpenGL
{
    public static class ConversionHelper
    {
        public static unsafe Matrix4 Convert( this System.Numerics.Matrix4x4 value ) => *( Matrix4* )&value;
        public static unsafe Vector3 Convert( this System.Numerics.Vector3 value ) => *( Vector3* ) &value;
        public static unsafe Vector4 Convert( this System.Numerics.Vector4 value ) => *( Vector4* )&value;
        public static unsafe Quaternion Convert( this System.Numerics.Quaternion value ) => *( Quaternion* )&value;

        public static unsafe System.Numerics.Quaternion Convert( this Quaternion value ) => *( System.Numerics.Quaternion* )&value;
        public static unsafe System.Numerics.Vector3 Convert( this Vector3 value ) => *( System.Numerics.Vector3* )&value;
    }
}
