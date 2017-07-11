using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib.Loaders
{
    internal struct FileHeader
    {
        public string Magic;
        public uint Version;
        public FileType Type;
        public int Unknown;

        public const int CSIZE                 = 16;
        public const string MAGIC_FS       = "GFS0";
        public const string MAGIC_SHADERCACHE = "GSC0";
    }
}
