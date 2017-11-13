using System.IO;

namespace AtlusGfdLib.IO
{
    public static class StreamExtensions
    {
        public static StreamView CreateSubStream(this Stream stream, long position, long length)
        {
            return new StreamView(stream, position, length);
        }
    }
}
