using System.Collections.Generic;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class AnimationKeyframeTrack
    {
        // 00
        public AnimationKeyframeType KeyframeType { get; set; }

        // 04 = Keyframe count

        // 08
        public List<IAnimationKeyframe> Keyframes { get; set; }

        // 0C
        public List<float> KeyframeTimings { get; set; }

        // 10
        public Vector3 BasePosition { get; set; }

        // 1C
        public Vector3 BaseScale { get; set; }

        public AnimationKeyframeTrack()
        {
            KeyframeTimings = new List< float >();
            Keyframes = new List< IAnimationKeyframe >();
        }
    }
}