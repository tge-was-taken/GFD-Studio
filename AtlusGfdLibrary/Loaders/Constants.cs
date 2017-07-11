using System;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    internal static class Constants
    {
        public static Dictionary<Type, ResourceType> TypeToGfdResourceTypeMap = new Dictionary<Type, ResourceType>()
        {
            { typeof(Scene),                 ResourceType.Scene },
            { typeof(MaterialDictionary),    ResourceType.MaterialDictionary },
            { typeof(TextureDictionary),     ResourceType.TextureDictionary },
            { typeof(AnimationPackage),         ResourceType.AnimationList }
        };
    }
}
