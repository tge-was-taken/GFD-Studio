using System;
using System.CommandLine;
using System.IO;
using GFDStudio.Cli.Commands;
using GFDLibrary;
using GFDStudio.GUI;

namespace GFDStudio.Cli
{
    internal static class Program
    {
        /// <summary>
        /// The answers the prompts in the core take. Commands fill this in from their arguments
        /// before handing over to anything that might ask.
        /// </summary>
        public static CliUserDialogs Dialogs { get; } = new();

        public static CliUserOptions Options { get; } = new();

        /// <summary>
        /// What a command prints as its result.
        /// </summary>
        public static TextWriter Output { get; } = Console.Out;

        public static int Main( string[] args )
        {
            // The core asks its questions through these, and throws outright when they are
            // missing, so they go in before anything else runs.
            UserDialogs.Current = Dialogs;
            UserOptions.Current = Options;

            var root = new RootCommand( "GFD Studio on the command line." )
            {
                InfoCommand.Create(),
                ConvertCommand.Create(),
                ExportCommand.Create(),
                ReplaceCommand.Create(),
                TexturesCommand.Create(),
                BatchCommand.Create(),
                RoundtripCommand.Create(),
            };

            // Catherine Full Body shares its version numbers with P3D and P5R, so which one a
            // file is has to be said rather than read. It applies to every command.
            var catherine = new Option<bool>( "--catherine-full-body" )
            {
                Description = "Read files as Catherine Full Body, which shares its version numbers with P3D and P5R.",
                Recursive = true,
            };

            root.Options.Add( catherine );

            var parse = root.Parse( args );
            ResourceVersion.TreatAsCatherineFullBody = parse.GetValue( catherine );

            return CommandHelpers.Guard( () => parse.Invoke() );
        }
    }
}
