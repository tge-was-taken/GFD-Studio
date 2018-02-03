using System.Numerics;

namespace AtlusGfdLibrary.Keyframes
{
    public struct NodePRSHalfKeyframe : IKeyframe
    {
        public KeyframeKind Kind => KeyframeKind.NodePRSHalf;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}