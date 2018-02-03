using System.Numerics;

namespace AtlusGfdLibrary.Keyframes
{
    public struct NodePRKeyframe : IKeyframe
    {
        public KeyframeKind Kind => KeyframeKind.NodePR;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}