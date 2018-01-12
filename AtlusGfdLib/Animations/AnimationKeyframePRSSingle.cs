using System.Numerics;

namespace AtlusGfdLib
{
    public struct AnimationKeyframePRSSingle : IAnimationKeyframe
    {
        public AnimationKeyframeType KeyframeType => AnimationKeyframeType.PRSSingle;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}