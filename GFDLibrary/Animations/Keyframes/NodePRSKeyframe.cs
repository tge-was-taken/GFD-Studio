using System.Numerics;

namespace GFDLibrary.IO.Keyframes
{
    public struct NodePRSKeyframe : IKeyframe
    {
        public KeyframeKind Kind => KeyframeKind.NodePRS;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}