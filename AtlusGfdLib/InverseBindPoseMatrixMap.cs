using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class InverseBindPoseMatrixMap
    {
        public Matrix4x4[] InverseBindPoseMatrices { get; set; }

        public ushort[] RemapIndices { get; set; }

        public InverseBindPoseMatrixMap( int matrixCount )
        {
            InverseBindPoseMatrices = new Matrix4x4[matrixCount];
            RemapIndices = new ushort[matrixCount];
        }
    }
}