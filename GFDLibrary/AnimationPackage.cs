using System.Collections.Generic;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class AnimationPackage : Resource
    {
        public override ResourceType ResourceType => ResourceType.AnimationPackage;

        // Just store raw data for now
        public byte[] RawData { get; set; }

        public AnimationPackageFlags Flags { get; set; }

        public List<Animation> Animations { get; set; }

        public List<Animation> BlendAnimations { get; set; }

        public AnimationExtraData ExtraData { get; set; }

        internal AnimationPackage(uint version)
            : base(version)
        {
            Animations = new List< Animation >();
            BlendAnimations = new List< Animation >();
        }

        internal override void Read( ResourceReader reader )
        {
            throw new System.NotImplementedException();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.Write( RawData );
        }
    }
}