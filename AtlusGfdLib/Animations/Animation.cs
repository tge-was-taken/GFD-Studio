using System.Collections.Generic;
using AtlusGfdLib.IO.Resource;

namespace AtlusGfdLib
{
    public sealed class Animation
    {
        // 00
        public AnimationFlags Flags { get; set; }

        // 04
        public float Duration { get; set; }

        // 08 = Controller count

        // 0C
        public List<AnimationController> Controllers { get; set; }

        // 10
        public List<AnimationFlag10000000DataEntry> Field10 { get; set; }

        // 14
        public AnimationExtraData Field14 { get; set; }

        // 18
        public BoundingBox? BoundingBox { get; set; }

        // 1C
        public AnimationFlag80000000Data Field1C { get; set; }

        // 20
        public UserPropertyCollection Properties { get; set; }

        // 24
        public float Field24 { get; set; }

        public Animation()
        {
            Controllers = new List< AnimationController >();
            Field10 = new List< AnimationFlag10000000DataEntry >();
            Field24 = 1.0f;
        }
    }
}