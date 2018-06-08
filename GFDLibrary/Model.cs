using System;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class Model : Resource
    {
        public override ResourceType ResourceType => ResourceType.Model;

        public TextureDictionary Textures { get; set; }

        public MaterialDictionary Materials { get; set; }

        public Scene Scene { get; set; }

        public AnimationPack AnimationPack { get; set; }

        public ChunkType000100F8 ChunkType000100F8 { get; set; }

        public ChunkType000100F9 ChunkType000100F9 { get; set; }

        public Model()
        {         
        }

        public Model(uint version) : base(version )
        {
        }

        public void ReplaceWith( Model other )
        {
            if ( Textures == null || other.Textures == null )
                Textures = other.Textures;
            else
                Textures.ReplaceWith( other.Textures );

            if ( Materials == null || other.Materials == null )
                Materials = other.Materials;
            else
                Materials.ReplaceWith( other.Materials );

            if ( Scene == null || other.Scene == null )
                Scene = other.Scene;
            else
                Scene.ReplaceWith( other.Scene );

            if ( other.AnimationPack != null )
            {
                if ( AnimationPack == null )
                {
                    AnimationPack = other.AnimationPack;
                }
                else
                {
                    // TODO
                }
            }

            if ( other.ChunkType000100F8 != null )
            {
                if ( ChunkType000100F8 == null )
                {
                    ChunkType000100F8 = other.ChunkType000100F8;
                }
                else
                {
                    // TODO
                }
            }

            if ( other.ChunkType000100F9 != null )
            {
                if ( ChunkType000100F9 == null )
                {
                    ChunkType000100F9 = other.ChunkType000100F9;
                }
                else
                {
                    // TODO
                }
            }
        }

        internal override void Read( ResourceReader reader )
        {
            while ( ( reader.Position + ResourceChunkHeader.SIZE ) < reader.BaseStream.Length )
            {
                var chunk = reader.ReadChunkHeader();
                if ( chunk.Type == ResourceChunkType.Invalid || !Enum.IsDefined(typeof(ResourceChunkType), chunk.Type) )
                    break;

                var chunkDataLength = chunk.Length - 16;
                var chunkDataStart = reader.Position;

                switch ( chunk.Type )
                {
                    case ResourceChunkType.TextureDictionary:
                        Textures = reader.ReadResource<TextureDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.MaterialDictionary:
                        Materials = reader.ReadResource<MaterialDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.Scene:
                        Scene = reader.ReadResource<Scene>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F9:
                        ChunkType000100F9 = reader.ReadResource<ChunkType000100F9>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F8:
                        ChunkType000100F8 = new ChunkType000100F8( chunk.Version ) { RawData = reader.ReadBytes( chunkDataLength ) };
                        break;
                    case ResourceChunkType.AnimationPack:
                        {
                            AnimationPack = reader.ReadResource<AnimationPack>( chunk.Version );

                            if ( AnimationPack.ErrorsOccuredDuringLoad && Scene != null )
                            {
                                // Invalid data, let's not make model uneditable because the animation support sucks, now shall we?
                                reader.SeekBegin( chunkDataStart );
                                AnimationPack = new AnimationPack( chunk.Version ) { RawData = reader.ReadBytes( chunkDataLength ) };
                            }
                        }
                        break;
                    default:
                        reader.SeekCurrent( chunkDataLength );
                        continue;
                }
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            if ( Textures != null )
                writer.WriteResourceChunk( Textures );

            if ( Materials != null )
                writer.WriteResourceChunk( Materials );

            if ( Scene != null )
                writer.WriteResourceChunk( Scene );

            if ( ChunkType000100F9 != null )
                writer.WriteResourceChunk( ChunkType000100F9 );

            if ( ChunkType000100F8 != null )
                writer.WriteResourceChunk( ChunkType000100F8 );

            if ( AnimationPack != null )
                writer.WriteResourceChunk( AnimationPack );

            bool animationOnly = AnimationPack != null &&
                                 Textures == null &&
                                 Materials == null &&
                                 Scene == null &&
                                 ChunkType000100F9 == null &&
                                 ChunkType000100F8 == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                writer.WriteEndChunk( Version );
        }
    }
}
