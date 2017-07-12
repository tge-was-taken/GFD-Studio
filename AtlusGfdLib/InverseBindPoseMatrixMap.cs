using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class MatrixMap
    {
        public Matrix4x4[] Matrices { get; set; }

        public ushort[] RemapIndices { get; set; }

        public MatrixMap( int matrixCount )
        {
            Matrices = new Matrix4x4[matrixCount];
            RemapIndices = new ushort[matrixCount];
        }
    }
}