using System.Numerics;

namespace AtlusGfdLib
{
    public struct AnimationKeyframePRSingle : IAnimationKeyframe
    {
        public AnimationKeyframeType KeyframeType => AnimationKeyframeType.PRSingle;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}