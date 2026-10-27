using System.Numerics;

namespace GFDLibrary.Misc
{
    public class ChunkType000100F9Entry2
    {
        public short CapsuleType { get; set; }

        public float CapsuleRadius { get; set; }

        public float CapsuleHeight { get; set; }

        public Matrix4x4 Matrix { get; set; }

        public string NodeName { get; set; }
    }
}