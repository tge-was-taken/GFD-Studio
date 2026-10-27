using System;
using System.CommandLine;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Textures;
using GFDStudio.FormatModules;
using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// Conversion driven by the file extensions alone, which is what a script wants.
    /// </summary>
    public static class ConvertCommand
    {
        /// <summary>
        /// What a loose texture file holds, by extension. More than one format module claims the
        /// dds extension, so the decoder is asked directly rather than through them.
        /// </summary>
        private static readonly Dictionary<string, TextureFormat> TextureFormats = new( StringComparer.OrdinalIgnoreCase )
        {
            ["dds"] = TextureFormat.DDS,
            ["gnf"] = TextureFormat.GNF,
            ["tga"] = TextureFormat.TGA,
            ["tmx"] = TextureFormat.TMX,
            ["gxt"] = TextureFormat.GXT,
        };

        public static Command Create()
        {
            var input = new Argument<string>( "in" ) { Description = "The file to convert." };
            var output = new Argument<string>( "out" ) { Description = "The file to write, whose extension picks the format." };
            var version = CommandHelpers.Text( "--version", "Resource version to convert a scene to, such as 0x01105100." );
            var preset = CommandHelpers.Text( "--preset", "Material preset YAML to convert a scene against." );

            var command = new Command( "convert", "Convert a file, choosing the format from the extensions on both sides." )
            {
                input, output, version, preset,
            };

            command.SetAction( parse => CommandHelpers.Guard( () => Run( parse.GetValue( input ),
                                                                        parse.GetValue( output ),
                                                                        parse.GetValue( version ),
                                                                        parse.GetValue( preset ) ) ) );
            return command;
        }

        private static int Run( string input, string output, string version, string preset )
        {
            Program.Dialogs.ModelVersion = ParseVersion( version );
            Program.Dialogs.MaterialPreset = preset;

            if ( TryConvertTexture( input, output ) )
            {
                Program.Output.WriteLine( $"Wrote {output}" );
                return 0;
            }

            var node = LoadForConversion( input, output );
            ResourceFiles.Save( node, output, Program.Dialogs.MatchFormat( node.ExportFormats(), output ).Type );

            Program.Output.WriteLine( $"Wrote {output}" );
            return 0;
        }

        /// <summary>
        /// YAML carries no hint of what it holds, so the type comes from what the output side
        /// wants instead.
        /// </summary>
        private static DataViewNode LoadForConversion( string input, string output )
        {
            if ( !CliUserDialogs.PathExtension( input ).Equals( "yml", StringComparison.OrdinalIgnoreCase ) )
                return ResourceFiles.Load( input );

            if ( !File.Exists( input ) )
                throw new CliException( $"No file at \"{input}\"." );

            var outputExtension = CliUserDialogs.PathExtension( output );
            var modules = ModuleImportUtilities.GetModulesWithExtension( outputExtension )
                                               .Where( x => x.UsageFlags.HasFlag( FormatModuleUsageFlags.Export ) )
                                               .ToList();

            if ( modules.Count != 1 )
                throw new CliException( $"YAML says nothing about what it holds, and \"{outputExtension}\" does not pin it " +
                                        "down either. Convert to a format only one type writes, such as .gmd." );

            var data = YamlSerializer.LoadYamlFile( input, modules[0].ModelType );
            return DataViewNodeFactory.Create( Path.GetFileName( input ), data );
        }

        /// <summary>
        /// Textures do not go through a view node, since a loose DDS or PNG has none.
        /// </summary>
        private static bool TryConvertTexture( string input, string output )
        {
            var from = CliUserDialogs.PathExtension( input );
            var to = CliUserDialogs.PathExtension( output );

            if ( to.Equals( "png", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( !TextureFormats.TryGetValue( from, out var format ) )
                    return false;

                if ( !File.Exists( input ) )
                    throw new CliException( $"No file at \"{input}\"." );

                PngCodec.Save( TextureDecoder.Decode( File.ReadAllBytes( input ), format ), output );
                return true;
            }

            if ( from.Equals( "png", StringComparison.OrdinalIgnoreCase ) && to.Equals( "dds", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( !File.Exists( input ) )
                    throw new CliException( $"No file at \"{input}\"." );

                var texture = TextureEncoder.Encode( Path.GetFileName( output ), TextureFormat.DDS, PngCodec.Load( input ) );
                File.WriteAllBytes( output, texture.Data );
                return true;
            }

            return false;
        }

        public static uint? ParseVersion( string text )
        {
            if ( text == null )
                return null;

            var trimmed = text.StartsWith( "0x", StringComparison.OrdinalIgnoreCase ) ? text[2..] : text;

            return uint.TryParse( trimmed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var version )
                   ? version
                   : throw new CliException( $"\"--version\" takes a hexadecimal resource version, such as 0x01105100, not \"{text}\"." );
        }
    }
}
