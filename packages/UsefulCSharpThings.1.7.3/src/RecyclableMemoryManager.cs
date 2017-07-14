using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IO;

namespace UsefulThings
{
    /// <summary>
    /// Provides more convenient access to Microsoft's RecyclableMemoryStream methods.  https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream
    /// </summary>
    public static class RecyclableMemoryManager
    {
        static RecyclableMemoryStreamManager RecyclableStreamManager = new RecyclableMemoryStreamManager(RecyclableMemoryStreamManager.DefaultBlockSize, RecyclableMemoryStreamManager.DefaultLargeBufferMultiple, 64*1024*1024);

        static RecyclableMemoryManager()
        {
            //RecyclableStreamManager.MaximumFreeLargePoolBytes = 400*1024*1024; // ~400mb
            //RecyclableStreamManager.MaximumFreeSmallPoolBytes = 200*1024*1024; // ~200mb
        }

        /// <summary>
        /// Gets an empty RecyclableMemoryStream from pool. Not necessarily an unallocated stream.
        /// </summary>
        /// <returns>RecyclableMemoryStream.</returns>
        public static MemoryStream GetStream()
        {
            return RecyclableStreamManager.GetStream();
        }


        /// <summary>
        /// Gets a RecyclableMemoryStream from the pool and bases it on data.
        /// </summary>
        /// <param name="data">Data to base stream on.</param>
        /// <param name="offset">Offset in array to start at.</param>
        /// <param name="length">Length to read from data into stream.</param>
        /// <param name="name">Name of stream in pool for later identification.</param>
        /// <returns>RecyclableMemoryStream containing data.</returns>
        public static MemoryStream GetStream(byte[] data, int offset = 0, int length = -1, string name = "")
        {
            return RecyclableStreamManager.GetStream(name, data, offset, length == -1 ? data.Length : length);
        }


        /// <summary>
        /// Gets a RecyclableMemoryStream of a given size from pool.
        /// </summary>
        /// <param name="requiredSize">Starting size stream must have.</param>
        /// <param name="name">Name of stream in pool for later identification.</param>
        /// <returns>RecyclableMemoryStream of at least given size.</returns>
        public static MemoryStream GetStream(int requiredSize, string name = "")
        {
            return RecyclableStreamManager.GetStream(name, requiredSize);
        }
    }
}
