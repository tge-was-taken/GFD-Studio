using System;

namespace GFDLibrary
{
    [Flags]
    public enum AnimationFlags
    {
        Flag1           = 1 << 00,
        Flag2           = 1 << 01,
        Flag4           = 1 << 02,
        Flag8           = 1 << 03,
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
        HasProperties   = 1 << 23,
        Flag1000000     = 1 << 24,
        HasSpeed        = 1 << 25,
        Flag4000000     = 1 << 26,
        Flag10000000    = 1 << 28,
        Flag20000000    = 1 << 29,
        HasBoundingBox  = 1 << 30,
        Flag80000000    = 1 << 31,
    }
}