using System.Collections.Generic;
using System;

namespace AtlusGfdLib
{
    public sealed class Model : Resource
    {
        public TextureDictionary TextureDictionary { get; set; }

        public MaterialDictionary MaterialDictionary { get; set; }

        public Scene Scene { get; set; }

        public AnimationPackage AnimationPackage { get; set; }
        internal ChunkType000100F9 ChunkType000100F9 { get; set; }

        public Model(uint version)
            : base(ResourceType.Model, version)
        {
        }
    }
}
