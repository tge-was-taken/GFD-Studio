using System.Numerics;

namespace GFDLibrary
{
    public class Bone
    {
        public ushort NodeIndex { get; set; }

        public Matrix4x4 InverseBindMatrix { get; set; }

        public Bone( ushort nodeIndex, Matrix4x4 inverseBindMatrix )
        {
            NodeIndex = nodeIndex;
            InverseBindMatrix = inverseBindMatrix;
        }
    }
}