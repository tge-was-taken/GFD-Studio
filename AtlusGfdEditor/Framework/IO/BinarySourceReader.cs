using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AtlusGfdEditor.Framework.IO
{
    public unsafe abstract class BinarySourceReader : IDisposable
    {
        protected byte* m_pBuffer;
        protected long m_Position;
        protected long m_Size;
        protected bool m_Disposed = false; 

        public Endianness Endianness { get; set; }

        public long Position
        {
            get { return m_Position; }
        }

        public long SourceLength
        {
            get { return m_Size; }
        }

        public BinarySourceReader(Endianness endian)
        {
            Endianness = endian;
        }

        ~BinarySourceReader()
        {
            Dispose(false);
        }

        public sbyte ReadSByte()
        {
            int size = sizeof(sbyte);
            AssertStateBeforeReadOrSeek(size);

            sbyte value;
            value = *(sbyte*)GetPointerAtOffset(size);
            m_Position += size;

            return value;
        }

        public byte ReadByte()
        {
            int size = sizeof(byte);
            AssertStateBeforeReadOrSeek(size);

            byte value;
            value = *GetPointerAtOffset(size);
            m_Position += size;

            return value;
        }

        public short ReadInt16()
        {
            int size = sizeof(short);
            AssertStateBeforeReadOrSeek(size);

            short value;
            value = *(short*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public ushort ReadUInt16()
        {
            int size = sizeof(ushort);
            AssertStateBeforeReadOrSeek(size);

            ushort value;
            value = *(ushort*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public int ReadInt32()
        {
            int size = sizeof(int);
            AssertStateBeforeReadOrSeek(size);

            int value;
            value = *(int*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public uint ReadUInt32()
        {
            int size = sizeof(uint);
            AssertStateBeforeReadOrSeek(size);

            uint value;
            value = *(uint*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public long ReadInt64()
        {
            int size = sizeof(long);
            AssertStateBeforeReadOrSeek(size);

            long value;
            value = *(long*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public ulong ReadUInt64()
        {
            int size = sizeof(ulong);
            AssertStateBeforeReadOrSeek(size);

            ulong value;
            value = *(uint*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                value = EndiannessHelper.SwapEndianness(value);

            return value;
        }

        public float ReadSingle()
        {
            int size = sizeof(float);
            AssertStateBeforeReadOrSeek(size);

            float value;
            value = *(float*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                unsafe { *(uint*)&value = EndiannessHelper.SwapEndianness(*(uint*)&value); }

            return value;
        }

        public double ReadDouble()
        {
            int size = sizeof(double);
            AssertStateBeforeReadOrSeek(size);

            double value;
            value = *(double*)GetPointerAtOffset(size);
            m_Position += size;

            if (Endianness == Endianness.BigEndian)
                unsafe { *(ulong*)&value = EndiannessHelper.SwapEndianness(*(ulong*)&value); }

            return value;
        }

        public string ReadString()
        {
            StringBuilder strBuilder = new StringBuilder();
            char c = (char)ReadByte();
            while (c != '\0')
            {
                strBuilder.Append(c);
                c = (char)ReadByte();
            }

            return strBuilder.ToString();
        }

        public string ReadString(int fixedLength)
        {
            StringBuilder strBuilder = new StringBuilder();
            byte[] strBytes = ReadArray(fixedLength, ReadByte);

            for (int i = 0; i < strBytes.Length; i++)
                if (strBytes[i] != 0)
                    strBuilder.Append((char)strBytes[i]);

            return strBuilder.ToString();
        }

        public T[] ReadArray<T>(int elementCount, Func<T> readCallback)
        {
            T[] array = new T[elementCount];
            for (int i = 0; i < array.Length; i++)
                array[i] = readCallback();

            return array;
        }

        public sbyte[] ReadSByteArray(int count)
        {
            return ReadArray(count, ReadSByte);
        }

        public virtual byte[] ReadByteArray(int count)
        {
            int size = sizeof(byte) * count;
            AssertStateBeforeReadOrSeek(size);

            byte[] buffer = new byte[count];
            Marshal.Copy((IntPtr)GetPointerAtOffset(size), buffer, 0, count);
            m_Position += size;

            return buffer;
        }

        public short[] ReadInt16Array(int count)
        {
            return ReadArray(count, ReadInt16);
        }

        public ushort[] ReadUInt16Array(int count)
        {
            return ReadArray(count, ReadUInt16);
        }

        public int[] ReadInt32Array(int count)
        {
            return ReadArray(count, ReadInt32);
        }

        public uint[] ReadUInt32Array(int count)
        {
            return ReadArray(count, ReadUInt32);
        }

        public long[] ReadInt64Array(int count)
        {
            return ReadArray(count, ReadInt64);
        }

        public ulong[] ReadUInt64Array(int count)
        {
            return ReadArray(count, ReadUInt64);
        }

        public float[] ReadSingleArray(int count)
        {
            return ReadArray(count, ReadSingle);
        }

        public double[] ReadDoubleArray(int count)
        {
            return ReadArray(count, ReadDouble);
        }

        public string[] ReadStringArray(int count)
        {
            return ReadArray(count, ReadString);
        }

        public string[] ReadString(int count, int fixedLength)
        {
            string[] array = new string[count];
            for (int i = 0; i < array.Length; i++)
                array[i] = ReadString(fixedLength);

            return array;
        }

        public void SeekSet(long position)
        {
            AssertStateBeforeReadOrSeek(position - m_Position, true);
            m_Position = position;
        }

        public void SeekCurrent(long offset)
        {
            AssertStateBeforeReadOrSeek(offset, true);
            m_Position += offset;
        }

        public void Rewind()
        {
            m_Position = 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract byte* GetPointerAtOffset(int valueSize = 0);

        protected abstract void Dispose(bool disposing);

        protected void AssertStateBeforeReadOrSeek(long offset, bool isSeeking = false)
        {
            if (m_pBuffer == null)
            {
                Debug.Assert(false);
            }
            else if ((m_Position + offset) > m_Size)
            {
                Debug.Assert(false);
            }
        }

        public static BinarySourceReader GetReader(string filename, Endianness endianness)
        {
            return new BinaryFileSourceReader(filename, endianness);
        }

        public static BinarySourceReader GetReader(Stream stream, Endianness endianness)
        {
            if (stream is MemoryStream)
                return new BinaryMemoryStreamSourceReader((MemoryStream)stream, endianness);
            else
                return new BinaryStreamSourceReader(stream, endianness);
        }

        public static BinarySourceReader GetReader(byte[] data, Endianness endianness)
        {
            return new BinaryMemoryStreamSourceReader(data, endianness);
        }
    }
}
