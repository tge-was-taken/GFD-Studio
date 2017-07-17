using System.IO;

namespace AtlusGfdLib.IO
{
    public static class StreamExtensions
    {
        public static SubStream CreateSubStream(this Stream stream, long position, long length)
        {
            return new SubStream(stream, position, length);
        }
    }
}
