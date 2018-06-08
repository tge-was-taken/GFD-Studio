using System.IO;
using GFDLibrary.IO;
using GFDLibrary.IO.Common;

namespace GFDLibrary
{
    public abstract class Resource
    {
        public abstract ResourceType ResourceType { get; }

        public uint Version { get; set; }

        protected Resource( uint version )
        {
            Version = version;
        }

        protected Resource()
        {
            Version = ResourceVersion.Persona5;
        }

        public static T Load<T>( string path ) where T : Resource
        {
            return Load( path ) as T;
        }

        public static T Load<T>( Stream stream, bool leaveOpen = false ) where T : Resource
        {
            return Load( stream, leaveOpen ) as T;
        }

        public static ResourceType GetResourceType( Stream stream )
        {
            if ( stream.Length < ResourceFileHeader.SIZE )
                return ResourceType.Invalid;

            ResourceType type;

            using ( var reader = new ResourceReader( stream, true ) )
            {
                var header = reader.ReadFileHeader();
                type = header.Type;
                stream.Position = 0;
            }

            return type;
        }

        public static Resource Load( string path )
        {
            using ( var stream = File.OpenRead( path ) )
                return Load( stream, false );
        }

        public static Resource Load( Stream stream, bool leaveOpen )
        {
            if ( stream.Length < ResourceFileHeader.SIZE )
                throw new InvalidDataException( "Stream is too small to be a valid resource file." );

            using ( var reader = new ResourceReader( stream, leaveOpen ) )
            {
                var header = reader.ReadFileHeader();
                Resource res;

                switch ( header.Type )
                {
                    case ResourceType.Model:
                        res = new Model( header.Version );
                        break;

                    case ResourceType.ShaderCachePS3:
                        res = new ShaderCachePS3( header.Version );
                        break;

                    case ResourceType.ShaderCachePSP2:
                        res = new ShaderCachePSP2( header.Version );
                        break;

                    case ResourceType.ShaderCachePS4:
                        res = new ShaderCachePS4( header.Version );
                        break;

                    // Custom
                    case ResourceType.TextureMap:
                        res = new TextureMap( header.Version );
                        break;
                    case ResourceType.TextureDictionary:
                        res = new TextureDictionary( header.Version );
                        break;
                    case ResourceType.Texture:
                        res = new Texture( header.Version );
                        break;
                    case ResourceType.ShaderPS3:
                        res = new ShaderPS3( header.Version );
                        break;
                    case ResourceType.ShaderPSP2:
                        res = new ShaderPSP2( header.Version );
                        break;
                    case ResourceType.ShaderPS4:
                        res = new ShaderPS4( header.Version );
                        break;
                    case ResourceType.Scene:
                        res = new Scene( header.Version );
                        break;
                    case ResourceType.Node:
                        res = new Node( header.Version );
                        break;
                    case ResourceType.UserPropertyCollection:
                        res = new UserPropertyCollection( header.Version );
                        break;
                    case ResourceType.Morph:
                        res = new Morph( header.Version );
                        break;
                    case ResourceType.MorphTarget:
                        res = new MorphTarget( header.Version );
                        break;
                    case ResourceType.MorphTargetList:
                        res = new MorphTargetList( header.Version );
                        break;
                    case ResourceType.MaterialDictionary:
                        res = new MaterialDictionary( header.Version );
                        break;
                    case ResourceType.ChunkType000100F9:
                        res = new ChunkType000100F9( header.Version );
                        break;
                    case ResourceType.ChunkType000100F8:
                        res = new ChunkType000100F8( header.Version );
                        break;
                    case ResourceType.AnimationPack:
                        res = new AnimationPack( header.Version );
                        break;
                    case ResourceType.MaterialAttribute:
                        return MaterialAttribute.Read( reader, header.Version );
                    case ResourceType.Material:
                        res = new Material( header.Version );
                        break;
                    case ResourceType.Light:
                        res = new Light( header.Version );
                        break;
                    case ResourceType.Geometry:
                        res = new Geometry( header.Version );
                        break;
                    case ResourceType.Camera:
                        res = new Camera( header.Version );
                        break;
                    case ResourceType.Epl:
                        return Epl.Read( reader, header.Version, out _ );
                    case ResourceType.EplLeaf:
                        res = new EplLeaf( header.Version );
                        break;
                    case ResourceType.Animation:
                        res = new Animation( header.Version );
                        break;
                    case ResourceType.AnimationExtraData:
                        res = new AnimationExtraData( header.Version );
                        break;
                    case ResourceType.KeyframeTrack:
                        res = new KeyframeTrack( header.Version );
                        break;
                    case ResourceType.AnimationController:
                        res = new AnimationController( header.Version );
                        break;
                    default:
                        throw new InvalidDataException( "Unknown/Invalid resource type" );
                }

                res.Read( reader );

                return res;
            }
        }

        public void Save( string path )
        {
            using ( var stream = FileUtils.Create( path ) )
                Save( stream, false );
        }

        public void Save( Stream stream, bool leaveOpen )
        {
            using ( var writer = new ResourceWriter( stream, leaveOpen ) )
            {
                writer.WriteFileHeader( ResourceFileIdentifier.Model, Version, ResourceType );
                Write( writer );
            }
        }

        internal abstract void Read( ResourceReader reader );
        internal abstract void Write( ResourceWriter writer );
    }
}
