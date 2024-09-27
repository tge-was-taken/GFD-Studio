using GFDLibrary;
using GFDLibrary.Textures.Texpack;
using GFDStudio.FormatModules;
using System.IO;

namespace GFDStudio.IO
{
    public class CollectModelParts
    {
        public static void CollectDisconnectedModelPartsForModelPack(ModelPack Pack, string filePath)
        {
            string ModelDirectory = Path.GetDirectoryName( filePath );
            Logger.Debug( $"METAPHOR: Looking for loose chunks in {ModelDirectory}" );
            string TexPath = Path.Combine( ModelDirectory, Path.GetFileNameWithoutExtension( filePath ) + ".TEX" );
            if (File.Exists( TexPath ))
            {
                MetaphorTexpack MRFTexpack = ModuleImportUtilities.ImportFile<MetaphorTexpack>( TexPath );
                if ( MRFTexpack != null )
                {
                    foreach ( var tex in MRFTexpack.TextureList )
                    {
                        Logger.Debug( $"METAPHOR: Texture bin got {tex.Key}" );
                        if ( Pack.Textures.TryGetValue( tex.Key, out var modelTexPlaceholder ) )
                            modelTexPlaceholder.Data = tex.Value;

                    }
                }
            }
        }
    }
}
