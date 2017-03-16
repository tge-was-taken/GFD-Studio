using System.IO;
using AtlusGfdEditor.GfdLib.Internal;

namespace AtlusGfdEditor.GfdLib
{
    public class GfdResource
    {
        public GfdResourceType Type { get; }
        public uint Version { get; }

        internal GfdResource(GfdResourceType type, uint version)
        {
            Type = type;
            Version = version;
        }

        public static GfdResource Load(string filepath)
        {
            using (var stream = File.OpenRead(filepath))
                return new GfdResourceReader().ReadFromStream(stream);
        }

        public static GfdResource Load(Stream stream)
        {
            return new GfdResourceReader().ReadFromStream(stream);
        }
    }
}
