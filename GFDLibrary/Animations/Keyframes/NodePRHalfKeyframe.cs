using System.Numerics;

namespace GFDLibrary.IO.Keyframes
{
    public struct NodePRHalfKeyframe : IKeyframe
    {
        public KeyframeKind Kind => KeyframeKind.NodePRHalf;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}