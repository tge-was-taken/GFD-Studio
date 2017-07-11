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

        internal Model(uint version)
            : base(ResourceType.Model, version)
        {
        }
    }
}
