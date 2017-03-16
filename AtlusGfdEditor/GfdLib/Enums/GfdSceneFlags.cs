using System;

namespace AtlusGfdEditor.GfdLib
{
    [Flags]
    enum GfdSceneFlags
    {
        BoundingBox = 1 << 0,
        Skinning    = 1 << 2,
        Morphers    = 1 << 3
    }
}
