using System.IO;
using AtlusGfdLib.IO;

namespace AtlusGfdLib
{
    public abstract class Resource
    {
        public ResourceType Type { get; }

        public uint Version { get; set; }

        protected Resource(ResourceType type, uint version)
        {
            Type = type;
            Version = version;
        }

        public static Resource Load(string filepath)
        {
            using ( var stream = File.OpenRead( filepath ) )
                return Load( stream );
        }

        public static Resource Load(Stream stream)
        {
            return ResourceReader.ReadFromStream( stream, IO.Endianness.BigEndian );
        }

        public static void Save( Resource resource, string filepath )
        {
            using ( var stream = File.Create( filepath ) )
                Save( resource, stream );
        }

        public static void Save( Resource resource, Stream stream )
        {
            ResourceWriter.WriteToStream( resource, stream, Endianness.BigEndian );
        }
    }

    public enum ResourceType
    {
        Invalid = 0,
        Model,
        AnimationList,
        TextureDictionary,
        MaterialDictionary,
        Scene,
        ShaderCache,
    }
}
