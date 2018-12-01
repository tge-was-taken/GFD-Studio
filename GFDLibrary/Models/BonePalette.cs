using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GFDLibrary.Models
{
    public sealed class BonePalette
    {
        public int BoneCount => InverseBindMatrices.Length;

        public Matrix4x4[] InverseBindMatrices { get; set; }

        public ushort[] BoneToNodeIndices { get; set; }

        public BonePalette()
        {
            
        }

        public BonePalette( int matrixCount )
        {
            InverseBindMatrices = new Matrix4x4[matrixCount];
            BoneToNodeIndices = new ushort[matrixCount];
        }

        public BonePalette( IEnumerable<Bone> bones )
        {
            InverseBindMatrices = bones.Select( x => x.InverseBindMatrix ).ToArray();
            BoneToNodeIndices = bones.Select( x => x.NodeIndex ).ToArray();
        }
    }
}