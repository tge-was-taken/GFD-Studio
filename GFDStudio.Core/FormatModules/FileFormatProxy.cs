using System.IO;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Proxy class for file formats that do not have a direct type representation.
    /// </summary>
    public class FileFormatProxy
    {
        public FileFormatProxy(Stream stream, string filename = null)
        {
            Stream = stream;
            Filename = filename;
        }

        public Stream Stream { get; }
        public string Filename { get; }
    }
}
