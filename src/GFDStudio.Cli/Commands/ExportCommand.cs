using System;
using System.CommandLine;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// The export dialog, answered up front.
    /// </summary>
    public static class ExportCommand
    {
        public static Command Create()
        {
            var file = new Argument<string>( "file" ) { Description = "The file to export." };
            var output = CommandHelpers.Text( "--out", "Where to write the exported file." );
            var format = CommandHelpers.Text( "--format", "Which format to use, by name or extension. Taken from --out when left out." );
            var list = CommandHelpers.Switch( "--list", "List the formats this file can be exported to and stop." );

            var command = new Command( "export", "Export a file in one of the formats it offers." ) { file, output, format, list };
            command.SetAction( parse => CommandHelpers.Guard( () => Run( parse.GetValue( file ),
                                                                        parse.GetValue( output ),
                                                                        parse.GetValue( format ),
                                                                        parse.GetValue( list ) ) ) );
            return command;
        }

        private static int Run( string path, string output, string format, bool list )
        {
            var node = ResourceFiles.Load( path );
            var formats = node.ExportFormats();

            if ( list )
            {
                foreach ( var available in formats )
                    Program.Output.WriteLine( $"{available.Extension,-10}{available.Name}" );

                return 0;
            }

            if ( output == null )
                throw new CliException( "Pass --out with where to write the exported file, or --list to see the formats." );

            Program.Dialogs.Format = format;
            ResourceFiles.Save( node, output, Program.Dialogs.MatchFormat( formats, output ).Type );

            Program.Output.WriteLine( $"Wrote {output}" );
            return 0;
        }
    }
}
