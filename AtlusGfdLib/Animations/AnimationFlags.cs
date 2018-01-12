using System;

namespace AtlusGfdLib
{
    [Flags]
    public enum AnimationFlags
    {
        HasProperties   = 1 << 23,
        Flag1000000  = 1 << 24,
        Flag2000000  = 1 << 25,
        Flag4000000  = 1 << 26,
        Flag10000000 = 1 << 28,
        Flag20000000 = 1 << 29,
        HasBoundingBox = 1 << 30,
        Flag80000000 = 1 << 31,
    }
}