using System.IO;
using AtlusGfdLibrary.IO.Common;
using AtlusGfdLibrary.IO.Resource;

namespace AtlusGfdLibrary
{
    public abstract class Resource
    {
        public const int PERSONA5_RESOURCE_VERSION = 0x01105070;

        public static bool IsValidResource( string filepath )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return IsValidResource( stream );
            }
        }

        public static bool IsValidResource( Stream stream )
        {
            return ResourceReader.IsValidResourceStream( stream );
        }

        public static Resource Load( string filepath )
        {
            using ( var stream = File.OpenRead( filepath ) )
                return Load( stream );
        }

        public static TResource Load<TResource>( string filepath ) where TResource : Resource
        {
            using ( var stream = File.OpenRead( filepath ) )
                return Load<TResource>( stream );
        }

        public static Resource Load( Stream stream )
        {
            return ResourceReader.ReadFromStream( stream, Endianness.BigEndian );
        }

        public static TResource Load<TResource>( Stream stream ) where TResource : Resource
        {
            return ResourceReader.ReadFromStream<TResource>( stream, Endianness.BigEndian );
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

        public ResourceType Type { get; }

        public uint Version { get; set; }

        protected Resource(ResourceType type, uint version)
        {
            Type = type;
            Version = version;
        }
    }

    public enum ResourceType
    {
        Invalid = 0,
        Bundle,
        Model,
        AnimationPackage,
        TextureDictionary,
        MaterialDictionary,
        Scene,
        ShaderCachePS3,
        ShaderCachePSP2,
        ChunkType000100F9,
        Material,
        MaterialAttribute,
        TextureMap,
        Texture
    }
}
