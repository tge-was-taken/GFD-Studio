using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GFDLibrary.Animations;
using GFDLibrary.IO;
using GFDLibrary.Materials;
using GFDLibrary.Misc;
using GFDLibrary.Models;
using GFDLibrary.Textures;
using GFDLibrary.Textures.Texpack;

namespace GFDLibrary
{
    public sealed class ModelPack : Resource
    {
        public override ResourceType ResourceType => ResourceType.ModelPack;

        public TextureDictionary Textures { get; set; }

        public MaterialDictionary Materials { get; set; }

        public Model Model { get; set; }

        /// <summary>
        /// Points every mesh that named the old material at the new name. A mesh names its
        /// material by string, so a rename that does not follow through leaves it dangling.
        /// </summary>
        public void RenameMaterialReferences( string oldName, string newName )
        {
            if ( !Renameable( oldName, newName ) || Model == null )
                return;

            foreach ( var mesh in Model.Meshes.Where( x => x.MaterialName == oldName ) )
                mesh.MaterialName = newName;
        }

        /// <summary>
        /// The same for a texture, which every material's texture maps name by string.
        /// </summary>
        public void RenameTextureReferences( string oldName, string newName )
        {
            if ( !Renameable( oldName, newName ) || Materials == null )
                return;

            foreach ( var map in Materials.Materials.SelectMany( x => x.TextureMaps )
                                          .Where( x => x != null && x.Name == oldName ) )
                map.Name = newName;
        }

        private static bool Renameable( string oldName, string newName )
            => !string.IsNullOrEmpty( oldName ) && !string.IsNullOrEmpty( newName ) && oldName != newName;

        public AnimationPack AnimationPack { get; set; }

        public ChunkType000100F8 ChunkType000100F8 { get; set; }

        public ChunkType000100F9 ChunkType000100F9 { get; set; }

        public ModelPack()
        {         
        }

        public ModelPack(uint version) : base(version )
        {
        }

        public void ReplaceWith( ModelPack other )
        {
            if ( Textures == null || other.Textures == null )
                Textures = other.Textures;
            else
                Textures.ReplaceWith( other.Textures );

            if ( Materials == null || other.Materials == null )
                Materials = other.Materials;
            else
                Materials.ReplaceWith( other.Materials );

            if ( Model == null || other.Model == null )
                Model = other.Model;
            else
                Model.ReplaceWith( other.Model );

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

        public void MergeWith(ModelPack other)
        {
            if (Version != other.Version)
                throw new InvalidOperationException(
                    $"Cannot merge model packs with different versions: {Version:X} vs {other.Version:X}");

            // Textures: add new by name only
            if (other.Textures != null)
            {
                if (Textures == null)
                {
                    Textures = new TextureDictionary();
                }

                foreach (var texture in other.Textures)
                {
                    if (!Textures.ContainsTexture(texture.Key))
                        Textures.Add(texture.Value);
                }
            }

            // Materials: add new by name only
            if (other.Materials != null)
            {
                if (Materials == null)
                {
                    Materials = new MaterialDictionary();
                }

                foreach (var material in other.Materials)
                {
                    if (!Materials.ContainsKey(material.Key))
                        Materials.Add(material.Value);
                }
            }

            // Model: delegate to Model.MergeWith
            if (other.Model != null)
            {
                if (Model == null)
                {
                    Model = other.Model;
                }
                else
                {
                    Model.MergeWith(other.Model);
                }
            }

            // AnimationPack: merge by index
            if (other.AnimationPack != null)
            {
                if (AnimationPack == null)
                {
                    AnimationPack = other.AnimationPack;
                }
                else
                {
                    AnimationPack.MergeWith(other.AnimationPack);
                }
            }

            // Chunks: keep base versions
        }

        protected override void ReadCore( ResourceReader reader )
        {
            while ( ( reader.Position + ResourceChunkHeader.SIZE ) < reader.BaseStream.Length )
            {
                var chunk = reader.ReadChunkHeader();
                if ( chunk.Type == ResourceChunkType.Invalid || !Enum.IsDefined(typeof(ResourceChunkType), chunk.Type) )
                    break;

                var chunkDataLength = chunk.Length - 16;
                var chunkDataStart = reader.Position;
                var chunkDataEnd = chunkDataStart + chunkDataLength;

                switch ( chunk.Type )
                {
                    case ResourceChunkType.TextureDictionary:
                        Textures = reader.ReadResource<TextureDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.MaterialDictionary:
                        Materials = reader.ReadResource<MaterialDictionary>( chunk.Version );
                        break;
                    case ResourceChunkType.Model:
                        Model = reader.ReadResource<Model>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F9:
                        ChunkType000100F9 = reader.ReadResource<ChunkType000100F9>( chunk.Version );
                        break;
                    case ResourceChunkType.ChunkType000100F8:
                        ChunkType000100F8 = new ChunkType000100F8( chunk.Version ) { Data = reader.ReadBytes( chunkDataLength ) };
                        break;
                    case ResourceChunkType.AnimationPack:
                        {
                            AnimationPack = reader.ReadResource<AnimationPack>( chunk.Version );

                            if ( AnimationPack.ErrorsOccuredDuringLoad && Model != null )
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

            // Populate (semantic) metadata fields
            if ( Materials is not null )
            {
                if ( Model is not null )
                {
                    foreach ( var mat in Materials.Values )
                    {
                        var meshesWithMaterial = Model.Nodes
                            .SelectMany( n => n.Meshes )
                            .Where( m => m.MaterialName == mat.Name )
                            .ToList();
                        if (meshesWithMaterial.Count > 0 )
                        {
                            mat.RuntimeMetadata.IsCustomMaterial = false;
                            mat.RuntimeMetadata.GeometryFlags = meshesWithMaterial.Max( m => m.Flags );
                            mat.RuntimeMetadata.VertexAttributeFlags = meshesWithMaterial.Max( m => m.VertexAttributeFlags );
                        }
                    }
                }
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            if ( Textures != null )
                writer.WriteResourceChunk( Textures );

            if ( Materials != null )
                writer.WriteResourceChunk( Materials );

            if ( Model != null )
                writer.WriteResourceChunk( Model );

            if ( ChunkType000100F9 != null )
                writer.WriteResourceChunk( ChunkType000100F9 );

            if ( ChunkType000100F8 != null )
                writer.WriteResourceChunk( ChunkType000100F8 );

            if ( AnimationPack != null )
                writer.WriteResourceChunk( AnimationPack );

            bool animationOnly = AnimationPack != null &&
                                 Textures == null &&
                                 Materials == null &&
                                 Model == null &&
                                 ChunkType000100F9 == null &&
                                 ChunkType000100F8 == null;

            // end chunk is not present in gap files
            if ( !animationOnly )
                writer.WriteEndChunk( Version );
        }
    }
}
