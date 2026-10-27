using System;
using System.CommandLine;
using System.IO;
using GFDLibrary;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// The replace dialog, answered up front, including the toggles that only mean anything for
    /// a material.
    /// </summary>
    public static class ReplaceCommand
    {
        public static Command Create()
        {
            var file = new Argument<string>( "file" ) { Description = "The file to replace in." };
            var with = CommandHelpers.Text( "--with", "The file to take the new contents from." );
            var output = CommandHelpers.Text( "--out", "Where to write the result." );
            var format = CommandHelpers.Text( "--format", "Which format --with is in. Taken from its extension when left out." );
            var retainNames = CommandHelpers.Switch( "--retain-texture-names", "Keep the original material's name and texture names." );
            var retainColors = CommandHelpers.Switch( "--retain-colour-values", "Keep the original material's colour parameters." );
            var list = CommandHelpers.Switch( "--list", "List the formats this file can be replaced from and stop." );
            var version = CommandHelpers.Text( "--version", "Resource version to convert a scene to, such as 0x01105100." );
            var preset = CommandHelpers.Text( "--preset", "Material preset YAML to convert a scene against." );

            var command = new Command( "replace", "Replace the contents of a file from another file, then write the result out." )
            {
                file, with, output, format, retainNames, retainColors, list, version, preset,
            };

            command.SetAction( parse => CommandHelpers.Guard( () => Run( parse.GetValue( file ),
                                                                        parse.GetValue( with ),
                                                                        parse.GetValue( output ),
                                                                        parse.GetValue( format ),
                                                                        parse.GetValue( retainNames ),
                                                                        parse.GetValue( retainColors ),
                                                                        parse.GetValue( list ),
                                                                        parse.GetValue( version ),
                                                                        parse.GetValue( preset ) ) ) );
            return command;
        }

        private static int Run( string path, string replacement, string output, string format,
                                bool retainTextureNames, bool retainColorValues, bool list, string version, string preset )
        {
            var node = ResourceFiles.Load( path );
            var formats = node.ReplaceFormats();

            if ( list )
            {
                foreach ( var available in formats )
                    Program.Output.WriteLine( $"{available.Extension,-10}{available.Name}" );

                return 0;
            }

            if ( replacement == null )
                throw new CliException( "Pass --with naming the file to take the new contents from." );

            if ( output == null )
                throw new CliException( "Pass --out with where to write the result." );

            if ( !File.Exists( replacement ) )
                throw new CliException( $"No file at \"{replacement}\"." );

            Program.Options.RetainMaterialTextureNames = retainTextureNames;
            Program.Options.RetainMaterialColorValues = retainColorValues;
            Program.Dialogs.Format = format;
            Program.Dialogs.ModelVersion = ConvertCommand.ParseVersion( version );
            Program.Dialogs.MaterialPreset = preset;

            var source = Program.Dialogs.MatchFormat( formats, replacement );

            if ( source.Type == null )
            {
                var data = YamlSerializer.LoadYamlFile( replacement, node.DataType );
                node.ReplaceProcessing( node.DataType, data, retainTextureNames, retainColorValues );
            }
            else
            {
                node.Replace( replacement, source.Type, retainTextureNames, retainColorValues );
            }

            // The format of the result follows the file it is written to, not the replacement.
            Program.Dialogs.Format = null;
            ResourceFiles.Save( node, output, Program.Dialogs.MatchFormat( node.ExportFormats(), output ).Type );

            Program.Output.WriteLine( $"Wrote {output}" );
            return 0;
        }
    }
}
