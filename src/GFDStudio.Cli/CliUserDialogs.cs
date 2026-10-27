using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDStudio.GUI;

namespace GFDStudio.Cli
{
    /// <summary>
    /// The prompts answered from the command line. Nothing here waits for a person: an answer
    /// that was not given in advance is an error naming the flag that would have given it.
    /// </summary>
    public sealed class CliUserDialogs : IUserDialogs
    {
        private readonly Queue<string> mOpenFiles = new();
        private readonly Queue<string> mFolders = new();
        private readonly object mProgressLock = new();

        public bool? YesNo { get; set; }

        public TransformRequest Transform { get; set; }

        /// <summary>
        /// The version to convert an imported scene to, overriding the one the original model or
        /// the defaults would give.
        /// </summary>
        public uint? ModelVersion { get; set; }

        /// <summary>
        /// A material preset YAML file to convert against, instead of the default for the version.
        /// </summary>
        public string MaterialPreset { get; set; }

        /// <summary>
        /// The export or replace format the user named, matched against the node's own list.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Queues an answer for the next prompt that asks for a file to read, or for a
        /// directory. A job asks for these in a fixed order, which is the order they go in.
        /// </summary>
        public void AddOpenFile( string path ) => mOpenFiles.Enqueue( path );

        public void AddFolder( string path ) => mFolders.Enqueue( path );

        public void ShowMessage( string text, string caption, UserMessageKind kind )
            => Console.Error.WriteLine( $"{caption}: {text}" );

        public bool AskYesNo( string text, string caption, bool defaultYes = false )
        {
            if ( YesNo == null )
                throw new CliException( $"This asks a question that has no answer: \"{text}\" Pass --yes or --no." );

            Console.Error.WriteLine( $"{text} {( YesNo.Value ? "Yes" : "No" )}" );
            return YesNo.Value;
        }

        public FileDialogResult OpenFile( FileDialogRequest request )
            => new( new[] { Take( mOpenFiles, request.Title, "a file to read" ) }, 1 );

        public FileDialogResult SaveFile( FileDialogRequest request )
            => throw new CliException( $"This asks where to write a file ({Describe( request.Title )}), " +
                                       "which this command cannot answer. Pass --out." );

        public string SelectFolder( string title )
            => Take( mFolders, title, "a directory" );

        public ModelConverterOptions AskModelConversionOptions( Assimp.Scene scene, ModelPack originalModel )
        {
            var viewModel = ModelConverterOptionsBuilder.Build( scene, originalModel );

            if ( ModelVersion != null )
                viewModel.Version = ModelVersion.Value;

            // The default preset is picked when the view model is built, before the version it
            // should follow is known, so it is chosen again here.
            var presetPath = MaterialPreset ?? MaterialPresetHelper.GetDefaultMaterialPresetForVersion( viewModel.Version );
            if ( !File.Exists( presetPath ) )
                throw new CliException( $"No material preset at \"{presetPath}\". Pass --preset with one." );

            viewModel.DefaultMaterial.Preset = new ModelConverterMaterialPresetViewModel( presetPath );

            Console.Error.WriteLine( $"Converting as version 0x{viewModel.Version:X8} with preset {Path.GetFileNameWithoutExtension( presetPath )}" );
            return viewModel.MapToModelConverterOptions();
        }

        public NewTextureMap AskNewTextureMap()
            => throw new CliException( "This asks for a new texture map, which no command can answer yet." );

        /// <summary>
        /// The export and replace commands pick the format themselves and call the view node
        /// directly, so these are only reached by something the command line does not drive.
        /// </summary>
        public ExportOptionsResult AskExportOptions( ExportOptionsRequest request )
            => throw new CliException( $"This asks where to export to, and in which format. Pass --out, and --format " +
                                       $"with one of {Describe( request.Formats )}." );

        public ReplaceOptionsResult AskReplaceOptions( ReplaceOptionsRequest request )
            => throw new CliException( $"This asks what to replace from. Pass --with, and --format with one of " +
                                       $"{Describe( request.Formats )}." );

        public global::System.Numerics.Vector3? AskDirection( string title, string prompt, global::System.Numerics.Vector3 initial )
            => throw new CliException( $"{title} needs a direction, which this command cannot ask for." );

        public TransformRequest AskTransform( string title )
            => Transform ?? throw new CliException( $"This asks for a transform ({Describe( title )}). Pass --scale, --position and --rotation." );

        /// <summary>
        /// Runs the job here and now, with its progress on standard error so that it stays out of
        /// whatever the command writes to standard output.
        /// </summary>
        public void RunWithProgress( string title, Func<IProgressReport, Task> work )
        {
            Console.Error.WriteLine( $"{title}..." );
            work( new ConsoleProgress( mProgressLock ) ).GetAwaiter().GetResult();
            Console.Error.WriteLine( $"{title}: done" );
        }

        /// <summary>
        /// Picks the format the user named, or the one the file extension points at.
        /// </summary>
        public FileFormat MatchFormat( IReadOnlyList<FileFormat> formats, string path )
        {
            if ( Format != null )
            {
                var named = formats.FirstOrDefault( x => Matches( x, Format ) );
                if ( named == null )
                    throw new CliException( $"No format called \"{Format}\". This file offers {Describe( formats )}." );

                return named;
            }

            var extension = PathExtension( path );
            var byExtension = formats.Where( x => string.Equals( x.Extension, extension, StringComparison.OrdinalIgnoreCase ) ).ToList();

            if ( byExtension.Count == 0 )
                throw new CliException( $"Nothing handles \"{extension}\" here. Pass --format with one of {Describe( formats )}." );

            if ( byExtension.Count > 1 )
                throw new CliException( $"More than one format uses \"{extension}\". Pass --format with one of " +
                                        $"{string.Join( ", ", byExtension.Select( x => x.Name ) )}." );

            return byExtension[0];
        }

        private static bool Matches( FileFormat format, string wanted )
            => string.Equals( format.Name, wanted, StringComparison.OrdinalIgnoreCase ) ||
               string.Equals( format.Extension, wanted.TrimStart( '.' ), StringComparison.OrdinalIgnoreCase );

        public static string Describe( IReadOnlyList<FileFormat> formats )
            => string.Join( ", ", formats.Select( x => $"{x.Name} (.{x.Extension})" ) );

        /// <summary>
        /// Matches the double extensions the format modules use, such as ascii.fbx.
        /// </summary>
        public static string PathExtension( string path )
        {
            var name = Path.GetFileName( path );
            var dot = name.IndexOf( '.' );
            return dot < 0 ? string.Empty : name[( dot + 1 )..];
        }

        /// <summary>
        /// The next answer the command queued, or an error saying which prompt went unanswered.
        /// </summary>
        private static string Take( Queue<string> answers, string title, string what )
            => answers.Count == 0
               ? throw new CliException( $"This asks for {what} ({Describe( title )}) that the command did not " +
                                         "supply. See what it takes with --help." )
               : answers.Dequeue();

        private static string Describe( string title )
            => string.IsNullOrWhiteSpace( title ) ? "no further detail" : title.Split( '\n' )[0].Trim();

        /// <summary>
        /// Progress on standard error, at one line per whole percent so that a job over thousands
        /// of files does not write thousands of lines.
        /// </summary>
        private sealed class ConsoleProgress : IProgressReport
        {
            private readonly object mLock;
            private int mLastPercentage = -1;

            public ConsoleProgress( object padlock ) => mLock = padlock;

            public bool CancellationPending => false;

            public void Report( int percentage, string message )
            {
                lock ( mLock )
                {
                    if ( percentage == mLastPercentage )
                        return;

                    mLastPercentage = percentage;
                    Console.Error.WriteLine( $"  {percentage.ToString( CultureInfo.InvariantCulture ).PadLeft( 3 )}%  {message}" );
                }
            }
        }
    }

    /// <summary>
    /// Reads the transform the rescale job asks for off the command line.
    /// </summary>
    public static class TransformParser
    {
        public static TransformRequest Parse( string scale, string position, string rotation )
        {
            if ( scale == null && position == null && rotation == null )
                return null;

            return new TransformRequest( Vector( scale, "--scale", Vector3.One ),
                                         Vector( position, "--position", Vector3.Zero ),
                                         Rotation( rotation ) );
        }

        private static Vector3 Vector( string text, string flag, Vector3 fallback )
        {
            if ( text == null )
                return fallback;

            var parts = Numbers( text, flag );
            if ( parts.Length != 3 )
                throw new CliException( $"\"{flag}\" takes three numbers, as in {flag} 1,1,1." );

            return new Vector3( parts[0], parts[1], parts[2] );
        }

        private static Quaternion Rotation( string text )
        {
            if ( text == null )
                return Quaternion.Identity;

            var parts = Numbers( text, "--rotation" );

            if ( parts.Length == 4 )
                return new Quaternion( parts[0], parts[1], parts[2], parts[3] );

            if ( parts.Length == 3 )
                return Quaternion.CreateFromYawPitchRoll( Radians( parts[1] ), Radians( parts[0] ), Radians( parts[2] ) );

            throw new CliException( "\"--rotation\" takes three numbers as degrees, or four as a quaternion." );
        }

        private static float Radians( float degrees ) => degrees * MathF.PI / 180f;

        private static float[] Numbers( string text, string flag )
            => text.Split( ',', ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries )
                   .Select( x => float.TryParse( x, NumberStyles.Float, CultureInfo.InvariantCulture, out var value )
                                 ? value
                                 : throw new CliException( $"\"{flag}\" has something that is not a number: {x}" ) )
                   .ToArray();
    }
}
