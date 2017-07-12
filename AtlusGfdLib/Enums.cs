using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public enum AnimationControllerType
    {
        Invalid = 0,
        PRS = 1,
        Material = 2,
        Camera = 3,
        Morph = 4
    }

    public enum ResourceType
    {
        Invalid = 0,
        Model,
        AnimationList,
        TextureDictionary,
        MaterialDictionary,
        Scene,
        ShaderCache,
    }
}
