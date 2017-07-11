namespace AtlusGfdLib.IO
{
    public enum Endianness
    {
        LittleEndian,
        BigEndian
    }

    public static class EndiannessHelper
    {
        public static short SwapEndianness(short value)
        {
            return (short)((value << 8) | ((value >> 8) & 0xFF));
        }

        public static ushort SwapEndianness(ushort value)
        {
            return (ushort)((value << 8) | (value >> 8));
        }

        public static int SwapEndianness(int value)
        {
            value = (int)((value << 8) & 0xFF00FF00) | ((value >> 8) & 0xFF00FF);
            return (value << 16) | ((value >> 16) & 0xFFFF);
        }

        public static uint SwapEndianness(uint value)
        {
            value = ((value << 8) & 0xFF00FF00) | ((value >> 8) & 0xFF00FF);
            return (value << 16) | (value >> 16);
        }

        public static long SwapEndianness(long value)
        {
            value = (long)(((ulong)(value << 8) & 0xFF00FF00FF00FF00UL) | ((ulong)(value >> 8) & 0x00FF00FF00FF00FFUL));
            value = (long)(((ulong)(value << 16) & 0xFFFF0000FFFF0000UL) | ((ulong)(value >> 16) & 0x0000FFFF0000FFFFUL));
            return (long)((ulong)(value << 32) | ((ulong)(value >> 32) & 0xFFFFFFFFUL));
        }

        public static ulong SwapEndianness(ulong value)
        {
            value = ((value << 8) & 0xFF00FF00FF00FF00UL ) | ((value >> 8) & 0x00FF00FF00FF00FFUL );
            value = ((value << 16) & 0xFFFF0000FFFF0000UL ) | ((value >> 16) & 0x0000FFFF0000FFFFUL );
            return (value << 32) | (value >> 32);
        }

        public unsafe static float SwapEndianness(float value)
        {
            uint temp = SwapEndianness(*(uint*)&value);
            return *(float*)&temp;
        }

        public unsafe static double SwapEndianness(double value)
        {
            ulong temp = SwapEndianness(*(ulong*)&value);
            return *(double*)&temp;
        }

        public unsafe static decimal SwapEndianness(decimal value)
        {
            fixed (byte* pBytes = new byte[32])
            {
                *(ulong*)pBytes = SwapEndianness(*(ulong*)&value);
                *(ulong*)(pBytes + 16) = SwapEndianness(*((ulong*)&value + 16));

                return *(decimal*)pBytes;
            }
        }
    }
}
