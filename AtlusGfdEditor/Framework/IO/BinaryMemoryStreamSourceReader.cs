
using System.IO;
using System.Runtime.InteropServices;

namespace AtlusGfdEditor.Framework.IO
{
    public unsafe class BinaryMemoryStreamSourceReader : BinarySourceReader
    {
        private GCHandle m_Handle;

        public BinaryMemoryStreamSourceReader(byte[] data, Endianness endian)
            : base(endian)
        {
            m_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            m_pBuffer = (byte*)m_Handle.AddrOfPinnedObject();
            m_Position = 0;
            m_Size = data.Length;
        }

        public BinaryMemoryStreamSourceReader(MemoryStream stream, Endianness endian)
            : this(stream.GetBuffer(), endian)
        {
        }

        protected override byte* GetPointerAtOffset(int valueSize = 0)
        {
            return (m_pBuffer + m_Position);
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                m_Handle.Free();
                m_pBuffer = null;
                m_Position = 0;
                m_Size = 0;
                m_Disposed = true;
            }
        }
    }
}
