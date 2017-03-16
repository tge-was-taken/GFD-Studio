using System;

namespace AtlusGfdEditor.GfdLib
{
    [Flags]
    enum GfdMaterialFlags
    {
        Diffuse     = 1 << 20,
        Normal      = 1 << 21,
        Specular    = 1 << 22,
        Reflection  = 1 << 23,
        Highlight   = 1 << 24,
        Glow        = 1 << 25, 
        Night       = 1 << 26,
        Detail      = 1 << 27,
        Shadow      = 1 << 28,
        Flag29      = 1 << 29,
        Flag30      = 1 << 30,
        Flag31      = 1 << 31
    }
}
