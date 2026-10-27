using System.Numerics;

namespace GFDLibrary.Models
{
    public class Bone
    {
        public ushort NodeIndex { get; set; }

        public Matrix4x4 InverseBindMatrix { get; set; }

        public Bone()
        {
            NodeIndex = ushort.MaxValue;
            InverseBindMatrix = Matrix4x4.Identity;
        }

        public Bone( ushort nodeIndex, Matrix4x4 inverseBindMatrix )
        {
            NodeIndex = nodeIndex;
            InverseBindMatrix = inverseBindMatrix;
        }
    }
}