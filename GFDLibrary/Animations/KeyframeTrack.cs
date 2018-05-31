using System.Collections.Generic;
using System.Numerics;

namespace GFDLibrary
{
    public sealed class KeyframeTrack
    {
        // 00
        public KeyframeKind KeyframeKind { get; set; }

        // 04 = Keyframe count

        // 08
        public List<IKeyframe> Keyframes { get; set; }

        // 0C
        public List<float> KeyframeTimings { get; set; }

        // 10
        public Vector3 BasePosition { get; set; }

        // 1C
        public Vector3 BaseScale { get; set; }

        public KeyframeTrack()
        {
            KeyframeTimings = new List< float >();
            Keyframes = new List< IKeyframe >();
        }
    }
}