using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AtlusGfdEditor.Framework.IO
{
    public unsafe class BinaryStreamSourceReader : BinarySourceReader
    {
        private const int BUFFER_SIZE = 4096;
        private Stream m_Stream;
        private long m_BufferBase;

        public BinaryStreamSourceReader(Stream stream, Endianness endian)
            : base(endian)
        {
            m_Stream = stream;
            m_pBuffer = (byte*)Marshal.AllocHGlobal(BUFFER_SIZE);
            m_Position = 0;
            m_BufferBase = 0;
            m_Size = m_Stream.Length;

            CopyToBuffer(0);
        }

        protected void CopyToBuffer(long offset, int bufferSize = BUFFER_SIZE)
        {
            m_BufferBase = offset;
            m_Stream.Position = offset;

            byte[] buffer = new byte[bufferSize];
            m_Stream.Read(buffer, 0, bufferSize);

            Marshal.Copy(buffer, 0, (IntPtr)m_pBuffer, bufferSize);
        }

        protected override byte* GetPointerAtOffset(int valueSize = 0)
        {
            if ((m_Position + valueSize) > (m_BufferBase + BUFFER_SIZE))
            {
                CopyToBuffer(m_Position);

                if ((m_Position + valueSize) > (m_BufferBase + BUFFER_SIZE))
                {
                    Debug.Assert(false);
                }
            }

            Debug.Assert((m_Position - m_BufferBase) >= 0);

            return (m_pBuffer + (m_Position - m_BufferBase));
        }

        public override byte[] ReadByteArray(int count)
        {
            int size = sizeof(byte) * count;
            AssertStateBeforeReadOrSeek(size);

            byte[] buffer = new byte[count];

            for (int i = 0; i < size / BUFFER_SIZE; i++)
            {
                Marshal.Copy((IntPtr)GetPointerAtOffset(BUFFER_SIZE), buffer, BUFFER_SIZE * i, BUFFER_SIZE);
                m_Position += BUFFER_SIZE;
            }

            int remainder = size % BUFFER_SIZE;

            Marshal.Copy((IntPtr)GetPointerAtOffset(remainder), buffer, size - remainder, remainder);
            m_Position += remainder;

            return buffer;
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                Marshal.FreeHGlobal((IntPtr)m_pBuffer);
                m_pBuffer = null;
                m_Position = 0;
                m_Size = 0;
                m_Disposed = true;
                m_BufferBase = 0;

                if (disposing)
                {
                    m_Stream.Dispose();
                }
            }
        }
    }
}
