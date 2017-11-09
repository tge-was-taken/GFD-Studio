using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib.IO
{
    internal struct FileHeader
    {
        public string Magic;
        public uint Version;
        public FileType Type;
        public int Unknown;

        public const int    CSIZE              = 16;
        public const string CMAGIC_FS          = "GFS0";
        public const string CMAGIC_SHADERCACHE = "GSC0";
    }

    internal enum FileType
    {
        ModelResourceBundle = 1,
        ShaderCachePS3 = 2,
        ShaderCachePSP2 = 4,
    }
}
