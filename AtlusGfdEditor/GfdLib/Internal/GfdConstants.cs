using System;
using System.Collections.Generic;
using AtlusGfdEditor.Framework.IO;

namespace AtlusGfdEditor.GfdLib.Internal
{
    static class GfdConstants
    {
        public static uint TextureDataFooter = 0x01010000;

        public static Dictionary<Type, GfdResourceType> TypeToGfdResourceTypeMap = new Dictionary<Type, GfdResourceType>()
        {
            { typeof(GfdScene),                 GfdResourceType.Scene },
            { typeof(GfdMaterialDictionary),    GfdResourceType.MaterialDictionary },
            { typeof(GfdTextureDictionary),     GfdResourceType.TextureDictionary },
            { typeof(GfdAnimationList),         GfdResourceType.AnimationList }
        };
    }
}
