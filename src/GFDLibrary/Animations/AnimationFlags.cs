using System;

namespace GFDLibrary.Animations
{
    [Flags]
    public enum AnimationFlags
    {
        Bit0 = 1 << 00,
        Bit1 = 1 << 01,
        Bit2 = 1 << 02,
        Bit3 = 1 << 03,
        Bit4 = 1 << 04,
        Bit5 = 1 << 05,
        Bit6 = 1 << 06,
        Bit7 = 1 << 07,
        Bit8 = 1 << 08,
        Bit9 = 1 << 09,
        Bit10 = 1 << 10,
        Bit11 = 1 << 11,
        Bit12 = 1 << 12,
        Bit13 = 1 << 13,
        Bit14 = 1 << 14,
        Bit15 = 1 << 15,
        Bit16 = 1 << 16,
        Bit17 = 1 << 17,
        Bit18 = 1 << 18,
        Bit19 = 1 << 19,
        Bit20 = 1 << 20,
        Bit21 = 1 << 21,
        Bit22 = 1 << 22,
        HasProperties = 1 << 23,
        Bit24 = 1 << 24,
        HasSpeed = 1 << 25,
        Bit26 = 1 << 26,
        Bit28 = 1 << 28,
        Bit29 = 1 << 29,
        HasBoundingBox = 1 << 30,
        Bit31 = 1 << 31,
    }
}