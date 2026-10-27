using System;
using System.CommandLine;
using System.IO;
using GFDStudio.Cli.Commands;
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
            };

            return CommandHelpers.Guard( () => root.Parse( args ).Invoke() );
        }
    }
}
