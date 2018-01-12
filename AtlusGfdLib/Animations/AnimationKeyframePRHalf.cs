using System.Numerics;

namespace AtlusGfdLib
{
    public struct AnimationKeyframePRHalf : IAnimationKeyframe
    {
        public AnimationKeyframeType KeyframeType => AnimationKeyframeType.PRHalf;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}