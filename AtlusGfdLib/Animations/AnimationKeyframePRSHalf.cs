using System.Numerics;

namespace AtlusGfdLib
{
    public struct AnimationKeyframePRSHalf : IAnimationKeyframe
    {
        public AnimationKeyframeType KeyframeType => AnimationKeyframeType.PRSHalf;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}