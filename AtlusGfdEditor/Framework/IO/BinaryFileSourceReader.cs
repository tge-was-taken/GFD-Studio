using System.IO;
using System.IO.MemoryMappedFiles;

namespace AtlusGfdEditor.Framework.IO
{
    public unsafe class BinaryFileSourceReader : BinarySourceReader
    {
        private string m_Filename;
        private MemoryMappedFile m_MemMap;
        private MemoryMappedViewAccessor m_Accessor;

        public BinaryFileSourceReader(string filename, Endianness endian)
            : base(endian)
        {
            m_Filename = filename;
            m_MemMap = MemoryMappedFile.CreateFromFile(filename, FileMode.Open);
            m_Accessor = m_MemMap.CreateViewAccessor();
            m_Accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref m_pBuffer);
            m_Size = (long)m_Accessor.SafeMemoryMappedViewHandle.ByteLength;
        }

        protected override byte* GetPointerAtOffset(int valueSize = 0)
        {
            return (m_pBuffer + m_Position);
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                m_Accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                m_pBuffer = null;
                m_Position = 0;
                m_Size = 0;
                m_Filename = null;

                if (disposing)
                {
                    m_Accessor.Dispose();
                    m_Accessor = null;
                    m_MemMap.Dispose();
                    m_MemMap = null;
                }

                m_Disposed = true;
            }
        }
    }
}
