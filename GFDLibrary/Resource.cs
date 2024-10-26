using System;
using System.IO;
using System.Reflection;
using GFDLibrary.Animations;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.Effects;
using GFDLibrary.IO;
using GFDLibrary.IO.Common;
using GFDLibrary.Lights;
using GFDLibrary.Materials;
using GFDLibrary.Misc;
using GFDLibrary.Models;
using GFDLibrary.Shaders;
using GFDLibrary.Textures;
using GFDLibrary.Textures.Texpack;

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
            return Load( path, typeof( T ) ) as T;
        }

        public static T Load<T>( Stream stream, bool leaveOpen = false ) where T : Resource
        {
            return Load( null, stream, leaveOpen, typeof(T) ) as T;
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
            return Load( path, null );
        }

        private static Resource Load( string path, Type type )
        {
            using ( var stream = File.OpenRead( path ) )
            {
                // Copy file to memory for faster reading
                var memoryStream = new MemoryStream( (int)stream.Length );
                stream.CopyTo( memoryStream );
                memoryStream.Position = 0;

                return Load( path, memoryStream, false, type );
            }
        }

        public static Resource Load( Stream stream, bool leaveOpen )
        {
            return Load( null, stream, leaveOpen, null ); 
        }

        private static Resource Load( string source, Stream stream, bool leaveOpen, Type type )
        {
            if ( stream.Length < ResourceFileHeader.SIZE )
                throw new InvalidDataException( "Stream is too small to be a valid resource file." );

            using ( var reader = new ResourceReader( stream, leaveOpen ) )
            {
                var header = reader.ReadFileHeader();
                Resource res;

                switch ( header.Type )
                {
                    case ResourceType.ModelPack:
                    case ResourceType.ModelPack_Metaphor:
                    case ResourceType.ModelPack_Metaphor_BIG_ENDIAN:
                        res = new ModelPack( header.Version );
                        break;

                    case ResourceType.ShaderCachePS3:
                        if ( type == typeof( Epl ) )
                            res = new Epl( header.Version );
                        else
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
                    case ResourceType.Model:
                        res = new Model( header.Version );
                        break;
                    case ResourceType.Node:
                        return Node.ReadRecursive( reader, header.Version );
                    case ResourceType.UserPropertyDictionary:
                        res = new UserPropertyDictionary( header.Version );
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
                        return new ChunkType000100F8( header.Version ) { Data = reader.ReadBytes( reader.ReadInt32() ) };
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
                    case ResourceType.Mesh:
                        res = new Mesh( header.Version );
                        break;
                    case ResourceType.Camera:
                        res = new Camera( header.Version );
                        break;
                    case ResourceType.Epl:
                        {
                            res = new Epl( header.Version );
                            break;
                        }
                    case ResourceType.EplLeaf:
                        res = new EplLeaf( header.Version );
                        break;
                    case ResourceType.Animation:
                        res = new Animation( header.Version );
                        break;
                    case ResourceType.AnimationBit29Data:
                        res = new AnimationBit29Data( header.Version );
                        break;
                    case ResourceType.AnimationLayer:
                        res = new AnimationLayer( header.Version );
                        break;
                    case ResourceType.AnimationController:
                        res = new AnimationController( header.Version );
                        break;
                    default:
                        throw new InvalidDataException( "Unknown/Invalid resource type" );
                }

                res.ReadCore( reader );

                if ( res.ResourceType == ResourceType.ModelPack && type == typeof(AnimationPack) )
                {
                    // Identify AnimationPack from a file with a model resource header
                    var model = ( ModelPack )res;
                    if ( model.AnimationPack != null && model.ChunkType000100F8 == null && model.ChunkType000100F9 == null &&
                         model.Materials == null && model.Model == null && model.Textures == null )
                    {
                        res = model.AnimationPack;
                    }
                }
                if ( ( res.ResourceType == ResourceType.ModelPack || res.ResourceType == ResourceType.ModelPack_Metaphor )
                    && res.Version >= 0x02000000 )
                {
                    // try loading a TEX from an external file, bundled in the same folder as the model.
                    var model = (ModelPack)res;
                    if (source != null)
                    {
                        string ModelDirectory = Path.GetDirectoryName( source );
                        Logger.Debug( $"METAPHOR: Looking for loose chunks in {ModelDirectory}" );
                        string TexPath = Path.Combine( ModelDirectory, Path.GetFileNameWithoutExtension( source ) + ".TEX" );
                        if ( File.Exists( TexPath ) )
                        {
                            using (var texpackStream = File.OpenRead( TexPath ))
                            {
                                MetaphorTexpack MRFTexpack = new( texpackStream );
                                if (MRFTexpack != null)
                                {
                                    foreach ( var tex in MRFTexpack.TextureList )
                                    {
                                        Logger.Debug( $"METAPHOR: Texture bin got {tex.Key}" );
                                        if ( model.Textures.TryGetValue( tex.Key, out var modelTexPlaceholder ) )
                                        {
                                            modelTexPlaceholder.Data = tex.Value;
                                            modelTexPlaceholder.IsTexSourceExternal = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return res;
            }
        }

        public void Save( string path )
        {
            //replace colons with semi colons since windows doesn't allow colons in filenames
            path = Path.GetDirectoryName( path ) + "\\" + Path.GetFileName( path ).Replace( ':', ';' );

            using ( var stream = FileUtils.Create( path ) )
            using ( var memoryStream = new MemoryStream() )
            {
                // Write to memory and then flush to the file
                Save( memoryStream, true );
                memoryStream.Position = 0;
                memoryStream.CopyTo( stream );
            }
            // METAPHOR: Check if the textures were sourced externally, and if so, export those in a separate TEX file
            // Do this here since Save() is aware of the file system location of the saved model pack
            if ( (ResourceType == ResourceType.ModelPack || ResourceType == ResourceType.ModelPack_Metaphor) 
                && Version >= 0x2000000
                && ((ModelPack)this ).Textures != null )
            {
                MetaphorTexpack extTexpack = new();
                foreach ( var texture in ( (ModelPack)this ).Textures )
                    if ( texture.Value.IsTexSourceExternal )
                        extTexpack.TextureList.Add( texture.Key, texture.Value.Data );
                if ( extTexpack.TextureList.Count > 0 )
                {
                    string ModelDirectory = Path.GetDirectoryName( path );
                    string TexPath = Path.Combine( ModelDirectory, Path.GetFileNameWithoutExtension( path ) + ".TEX" );
                    extTexpack.Save( FileUtils.Create( TexPath ) );
                }
            }
        }

        public void Save( Stream stream, bool leaveOpen )
        {
            using ( var writer = new ResourceWriter( stream, leaveOpen ) )
            {
                ResourceType GetResourceType() => Version >= 0x2000000 ? ResourceType.ModelPack_Metaphor : ResourceType.ModelPack;
                if ( Version >= 0x2000000 )
                    writer.Endianness = Endianness.LittleEndian;
                if ( ResourceType == ResourceType.AnimationPack )
                {
                    // For AnimationPacks we write a model file header, and then a chunk containing the pack data.
                    writer.WriteFileHeader( ResourceFileIdentifier.Model, Version, GetResourceType() );
                    writer.WriteResourceChunk( this );
                    return;
                }

                writer.WriteFileHeader( ResourceFileIdentifier.Model, Version, ResourceType == ResourceType.Model ? GetResourceType() : ResourceType );

                switch ( ResourceType )
                {
                    case ResourceType.Epl:
                        // We have to write this to the file so we can remember it when we load it.
                        //writer.WriteBoolean( ( ( Epl ) this ).IncludesProperties );
                        break;

                    case ResourceType.Node:
                        Node.WriteRecursive( writer, ( Node ) this );
                        return;

                    case ResourceType.ChunkType000100F8:
                        {
                            var chunk = ( ChunkType000100F8 )this;
                            writer.WriteInt32( chunk.Data.Length );
                            writer.WriteBytes( chunk.Data );
                            return;
                        }
                }

                WriteCore( writer );
            }
        }

        internal void Read( ResourceReader reader )
        {
            Logger.Debug( $"Resource: reading {ResourceType} @ 0x{reader.Position:X8}" );
            Logger.IndentDebug();
            ReadCore( reader );
            Logger.UnindentDebug();
        }

        internal void Write( ResourceWriter writer )
        {
            Logger.Debug( $"Resource: writing {ResourceType} @ 0x{writer.Position:X8}" );
            Logger.IndentDebug();
            WriteCore( writer );
            Logger.UnindentDebug();
        }

        protected abstract void ReadCore( ResourceReader reader );
        protected abstract void WriteCore( ResourceWriter writer );
    }
}
