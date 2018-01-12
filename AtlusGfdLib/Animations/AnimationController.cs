using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class AnimationController
    {
        // 00
        public AnimationControllerType Type { get; set; }

        // 04
        public int TargetId { get; set; }

        // 08
        public string TargetName { get; set; }

        // 1C
        public List<AnimationKeyframeTrack> Tracks { get; set; }

        public AnimationController()
        {
            Tracks = new List<AnimationKeyframeTrack>();
        }

        public override string ToString()
        {
            return $"{Type} {TargetId} {TargetName}";
        }
    }

    public enum AnimationControllerType
    {
        Invalid = 0,
        PRS = 1,
        Material = 2,
        Camera = 3,
        Morph = 4
    }
}