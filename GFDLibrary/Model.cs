using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class Model : Resource
    {
        public override ResourceType ResourceType => ResourceType.Model;

        public TextureDictionary TextureDictionary { get; set; }

        public MaterialDictionary MaterialDictionary { get; set; }

        public Scene Scene { get; set; }

        public AnimationPackage AnimationPackage { get; set; }

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
            if ( TextureDictionary == null || other.TextureDictionary == null )
                TextureDictionary = other.TextureDictionary;
            else
                TextureDictionary.ReplaceWith( other.TextureDictionary );

            if ( MaterialDictionary == null || other.MaterialDictionary == null )
                MaterialDictionary = other.MaterialDictionary;
            else
                MaterialDictionary.ReplaceWith( other.MaterialDictionary );

            if ( Scene == null || other.Scene == null )
                Scene = other.Scene;
            else
                Scene.ReplaceWith( other.Scene );

            if ( other.AnimationPackage != null )
            {
                if ( AnimationPackage == null )
                {
                    AnimationPackage = other.AnimationPackage;
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
                if ( chunk.Type == ResourceChunkType.Invalid )
                    break;

                switch ( chunk.Type )
                {
                    case ResourceChunkType.TextureDictionary:
                        TextureDictionary = reader.Read<TextureDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.MaterialDictionary:
                        MaterialDictionary = reader.Read<MaterialDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.Scene:
                        Scene = reader.Read<Scene>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F9:
                        ChunkType000100F9 = reader.Read<ChunkType000100F9>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F8:
                        ChunkType000100F8 = new ChunkType000100F8( chunk.Version ) { RawData = reader.ReadBytes( chunk.Length - 16 ) };
                        break;
                    case ResourceChunkType.AnimationPackage:
                        AnimationPackage = new AnimationPackage( chunk.Version ) { RawData = reader.ReadBytes( chunk.Length - 16 ) };
                        break;
                    default:
                        reader.SeekCurrent( chunk.Length - 16 );
                        continue;
                }
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            if ( TextureDictionary != null )
                writer.WriteResourceChunk( TextureDictionary );

            if ( MaterialDictionary != null )
                writer.WriteResourceChunk( MaterialDictionary );

            if ( Scene != null )
                writer.WriteResourceChunk( Scene );

            if ( ChunkType000100F9 != null )
                writer.WriteResourceChunk( ChunkType000100F9 );

            if ( ChunkType000100F8 != null )
                writer.WriteResourceChunk( ChunkType000100F8 );

            if ( AnimationPackage != null )
                writer.WriteResourceChunk( AnimationPackage );

            bool animationOnly = AnimationPackage != null &&
                                 TextureDictionary == null &&
                                 MaterialDictionary == null &&
                                 Scene == null &&
                                 ChunkType000100F9 == null &&
                                 ChunkType000100F8 == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                writer.WriteEndChunk( Version );
        }
    }
}
