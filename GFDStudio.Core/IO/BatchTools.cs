using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDLibrary.Textures;
using GFDStudio.FormatModules;
using GFDStudio.GUI;

namespace GFDStudio.IO
{
    /// <summary>
    /// The jobs that run over a whole directory of files. They ask for what they need through
    /// <see cref="UserDialogs"/>, so either front end can offer them.
    /// </summary>
    public static class BatchTools
    {
        private static readonly string[] ModelExtensions = { ".GFS", ".GMD" };

        public static void RetargetAnimationPacks()
        {
            var originalScene = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the original model file." )?.Model;
            if ( originalScene == null )
                return;

            var newScene = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the new model file." )?.Model;
            if ( newScene == null )
                return;

            var fixArms = UserDialogs.Current.AskYesNo( "Fix arms? If unsure, select No.", "Question" );

            var directoryPath = UserDialogs.Current.SelectFolder(
                "Select a directory containing GAP files, or subdirectories containing GAP files to retarget to the new model.\n" +
                "Note that this will replace the original files." );

            if ( directoryPath == null )
                return;

            var failures = ForEachFile( "Retargeting animations", directoryPath, "*.GAP", filePath =>
            {
                var animationPack = Resource.Load<AnimationPack>( filePath );
                animationPack.Retarget( originalScene, newScene, fixArms );
                animationPack.Save( filePath );
            } );

            // Retargeting Metaphor animation packs in particular is quite memory intensive.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Report( failures, "All animation packs successfully retargeted!" );
        }

        public static void ConvertAnimationPacksToP5()
        {
            var directoryPath = UserDialogs.Current.SelectFolder(
                "Select a directory containing GAP files, or subdirectories containing GAP files to convert the animations.\n" +
                "Note that this will replace the original files." );

            if ( directoryPath == null )
                return;

            var failures = ForEachFile( "Converting animations", directoryPath, "*.GAP", filePath =>
            {
                var animationPack = Resource.Load<AnimationPack>( filePath );
                animationPack.ConvertToP5();
                animationPack.Save( filePath );
            } );

            Report( failures, "All animation packs successfully converted!" );
        }

        public static void RescaleAnimationPacks()
        {
            var directoryPath = UserDialogs.Current.SelectFolder(
                "Select a directory containing GAP files, or subdirectories containing GAP files to rescale.\n" +
                "Note that this will replace the original files." );

            if ( directoryPath == null )
                return;

            var transform = UserDialogs.Current.AskTransform( "Rescale animation packs" );
            if ( transform == null )
                return;

            var failures = ForEachFile( "Rescaling animations", directoryPath, "*.GAP", filePath =>
            {
                var animationPack = Resource.Load<AnimationPack>( filePath );
                animationPack.Rescale( transform.Scale, transform.Position, transform.Rotation );
                animationPack.Save( filePath );
            } );

            Report( failures, "All animation packs successfully rescaled!" );
        }

        public static void ConvertModelMaterials()
        {
            var directoryPath = UserDialogs.Current.SelectFolder(
                "Select a directory containing GMD files, or subdirectories containing GMD files to convert the materials.\n" +
                "Note that this will replace the original files." );

            if ( directoryPath == null )
                return;

            var options = UserDialogs.Current.AskModelConversionOptions( null, null );
            if ( options == null )
                return;

            var failures = ForEachFile( "Converting materials", directoryPath, "*.GMD", filePath =>
            {
                var model = Resource.Load<ModelPack>( filePath );
                model.Materials = MaterialDictionary.ConvertAllToMaterialPreset( model.Materials, options );
                model.Save( filePath );
            } );

            Report( failures, "All model materials successfully converted!" );
        }

        public static void CopySplitAnimationPackToModels()
        {
            var aGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with a." );
            if ( aGap == null )
                return;

            var bGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with b." );
            if ( bGap == null )
                return;

            var cGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with c." );
            if ( cGap == null )
                return;

            var isGap52 = UserDialogs.Current.AskYesNo( "GAP ID 52? If unsure, select No.", "Question" );

            var directoryPath = UserDialogs.Current.SelectFolder(
                "Select a directory containing Model files, or subdirectories containing model files to make the new GAP files for.\n" +
                "Note that this will replace the original files." );

            if ( directoryPath == null )
                return;

            var gapId = isGap52 ? "_052_" : "_051_";

            var failures = ForEachFile( "Copying split animation packs", directoryPath, "*.GMD", filePath =>
            {
                var name = Path.GetFileName( filePath );
                var targetCharacterId = name.Split( "_" )[0].Remove( 0, 1 );
                var targetGapName = name.Split( "_" )[1];
                var targetGapDirectory = Path.Join( Path.GetDirectoryName( filePath ), "battle" );

                var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath )?.Model;
                if ( targetModel == null )
                    return;

                foreach ( var (pack, suffix) in new[] { (aGap, "a"), (bGap, "b"), (cGap, "c") } )
                {
                    pack.FixTargetIds( targetModel );
                    pack.Save( Path.Join( targetGapDirectory, $"bb{targetCharacterId}{gapId}{targetGapName}{suffix}.GAP" ) );
                }
            } );

            Report( failures, "All split GAPs successfully generated!" );
        }

        public static void ExportTexturesFromModels()
        {
            var directoryPath = UserDialogs.Current.SelectFolder( "Select a directory containing Model files to extract textures from." );
            if ( directoryPath == null )
                return;

            var outputPath = UserDialogs.Current.SelectFolder( "Select a directory to dump all textures into." );
            if ( outputPath == null )
                return;

            var failures = ForEachFile( "Exporting textures", ModelFiles( directoryPath ), filePath =>
            {
                var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath );
                if ( targetModel == null )
                    return;

                if ( targetModel.Version >= 0x2000000 )
                    CollectModelParts.CollectDisconnectedModelPartsForModelPack( targetModel, filePath );

                if ( targetModel.Textures == null )
                    return;

                foreach ( var texture in targetModel.Textures.Textures )
                    SaveTexture( texture, outputPath );
            } );

            Report( failures, "All textures successfully mass dumped!" );
        }

        public static void ReplaceTexturesInModels()
        {
            var directoryPath = UserDialogs.Current.SelectFolder( "Select a directory containing Model files to replace textures in." );
            if ( directoryPath == null )
                return;

            var replacementPath = UserDialogs.Current.SelectFolder( "Select a directory containing the textures you wish to replace in models." );
            if ( replacementPath == null )
                return;

            var replacements = Directory.EnumerateFiles( replacementPath, "*.dds", SearchOption.AllDirectories ).ToList();

            // From Metaphor onwards a texture is named without its extension, on both sides.
            static string MatchName( uint version, string path )
                => version >= 0x2000000 ? Path.GetFileNameWithoutExtension( path ) : Path.GetFileName( path );

            var failures = ForEachFile( "Replacing textures", ModelFiles( directoryPath ), filePath =>
            {
                var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath );
                if ( targetModel?.Textures == null )
                    return;

                var newTextures = new TextureDictionary( targetModel.Version );

                foreach ( var texture in targetModel.Textures.Textures )
                {
                    var originalName = MatchName( targetModel.Version, texture.Name );
                    var found = replacements.FirstOrDefault( x => MatchName( targetModel.Version, x ) == originalName );

                    if ( found == null )
                    {
                        newTextures.Add( texture );
                        continue;
                    }

                    Logger.Debug( $"Tex Replace: replacing {originalName} with {found}" );
                    newTextures.Add( TextureReplacement.Prepare(
                        targetModel.Version,
                        new Texture( texture.Name, TextureFormat.DDS, File.ReadAllBytes( found ) ) ) );
                }

                targetModel.Textures.ReplaceWith( newTextures );
                targetModel.Save( filePath );
            } );

            Report( failures, "All textures successfully mass replaced in models!" );
        }

        private static void SaveTexture( Texture texture, string outputPath )
        {
            switch ( texture.Format )
            {
                case TextureFormat.DDS:
                    File.WriteAllBytes( Path.Combine( outputPath, Path.ChangeExtension( texture.Name, ".dds" ) ), texture.Data );
                    break;

                case TextureFormat.GNF:
                    File.WriteAllBytes( Path.Combine( outputPath, Path.ChangeExtension( texture.Name, ".gnf" ) ), texture.Data );
                    break;

                case TextureFormat.TGA:
                case TextureFormat.TMX:
                    File.WriteAllBytes( Path.Combine( outputPath, texture.Name ), texture.Data );
                    break;

                default:
                    PngCodec.Save( TextureDecoder.Decode( texture ),
                                   Path.Combine( outputPath, Path.ChangeExtension( texture.Name, ".png" ) ) );
                    break;
            }
        }

        private static List<string> ModelFiles( string directoryPath )
            => Directory.EnumerateFiles( directoryPath, "*.*", SearchOption.AllDirectories )
                        .Where( x => ModelExtensions.Any( x.EndsWith ) )
                        .ToList();

        private static ConcurrentBag<string> ForEachFile( string title, string directoryPath, string pattern, Action<string> work )
            => ForEachFile( title, Directory.EnumerateFiles( directoryPath, pattern, SearchOption.AllDirectories ).ToList(), work );

        /// <summary>
        /// Runs the job over every file, reporting progress and collecting the ones that threw so
        /// that one bad file does not stop the rest.
        /// </summary>
        private static ConcurrentBag<string> ForEachFile( string title, List<string> filePaths, Action<string> work )
        {
            var failures = new ConcurrentBag<string>();

            UserDialogs.Current.RunWithProgress( title, async progress =>
            {
                var processed = 0;

                await Task.WhenAll( filePaths.Select( filePath => Task.Run( () =>
                {
                    if ( progress.CancellationPending )
                        return;

                    progress.Report( (int)( (float)Interlocked.Increment( ref processed ) / filePaths.Count * 100 ),
                                     $"Processing {Path.GetFileName( filePath )}" );

                    try
                    {
                        work( filePath );
                    }
                    catch ( Exception )
                    {
                        failures.Add( filePath );
                    }
                } ) ) );
            } );

            return failures;
        }

        private static void Report( ConcurrentBag<string> failures, string success )
        {
            if ( failures.IsEmpty )
                UserDialogs.Current.ShowMessage( success, "Done", UserMessageKind.Information );
            else
                UserDialogs.Current.ShowMessage( "An error occured while processing the following files:\n" + string.Join( "\n", failures ),
                                                 "Done", UserMessageKind.Warning );
        }
    }
}
