using System.Collections.Generic;

namespace GFDLibrary
{
    public sealed class AnimationPackage : Resource
    {
        // Just store raw data for now
        public byte[] RawData { get; set; }

        public AnimationPackageFlags Flags { get; set; }

        public List<Animation> Animations { get; set; }

        public List<Animation> BlendAnimations { get; set; }

        public AnimationExtraData ExtraData { get; set; }

        internal AnimationPackage(uint version)
            : base(ResourceType.AnimationPackage, version)
        {
            Animations = new List< Animation >();
            BlendAnimations = new List< Animation >();
        }
    }
}