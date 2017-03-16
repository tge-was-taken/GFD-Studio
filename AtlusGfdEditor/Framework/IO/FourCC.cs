using System;
using System.IO;
using System.Text;

namespace AtlusGfdEditor.Framework.IO
{
    public class FourCC
    {
        public uint Value { get; }

        public FourCC(string value)
        {
            if (value.Length > 4)
                throw new ArgumentException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                Value |= (uint)((byte)value[i] << (i * 8));
            }
        }

        public FourCC(char[] value)
        {
            if (value.Length > 4)
                throw new ArgumentException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                Value |= (uint)((byte)value[i] << (i * 8));
            }
        }

        public FourCC(byte[] value)
        {
            if (value.Length > 4)
                throw new ArgumentException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                Value |= (byte)(value[i] << (i * 8));
            }
        }

        public bool Matches(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != (Value >> (i * 8) & 0xFF))
                    return false;
            }

            return true;
        }

        public bool Matches(uint value)
        {
            return Value == value;
        }

        public bool Matches(Stream stream)
        {
            for (int i = 0; i < 4; i++)
            {
                byte value = (byte)stream.ReadByte();
                if (value != ((Value >> (i * 8)) & 0xFF))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(Value));
        }
    }
}
