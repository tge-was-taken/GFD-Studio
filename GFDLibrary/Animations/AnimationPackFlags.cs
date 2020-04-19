using System;

namespace GFDLibrary.Animations
{
    [Flags]
    public enum AnimationPackFlags
    {
        Flag1           = 1 << 0,
        Flag2           = 1 << 1,
        Flag4           = 1 << 2,
        Flag8           = 1 << 3,
        Flag10          = 1 << 04,
        Flag20          = 1 << 05,
        Flag40          = 1 << 06,
        Flag80          = 1 << 07,
        Flag100         = 1 << 08,
        Flag200         = 1 << 09,
        Flag400         = 1 << 10,
        Flag800         = 1 << 11,
        Flag1000        = 1 << 12,
        Flag2000        = 1 << 13,
        Flag4000        = 1 << 14,
        Flag8000        = 1 << 15,
        Flag10000       = 1 << 16,
        Flag20000       = 1 << 17,
        Flag40000       = 1 << 18,
        Flag80000       = 1 << 19,
        Flag100000      = 1 << 20,
        Flag200000      = 1 << 21,
        Flag400000      = 1 << 22,
        Flag800000      = 1 << 23,
        Flag1000000     = 1 << 24,
        Flag2000000     = 1 << 25,
        Flag4000000     = 1 << 26,
        Flag10000000    = 1 << 28,
        Flag20000000    = 1 << 29,
        Flag40000000    = 1 << 30,
        Flag80000000    = 1 << 31,
    }
}