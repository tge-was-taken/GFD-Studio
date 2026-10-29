using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Textures;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// Says what a file is and what is in it.
    /// </summary>
    public static class InfoCommand
    {
        public static Command Create()
        {
            var file = new Argument<string>( "file" ) { Description = "The file to look at." };
            var formats = CommandHelpers.Switch( "--formats", "Also list the formats this file can be exported to and replaced from." );

            var command = new Command( "info", "Identify a file and summarise what it holds." ) { file, formats };
            command.SetAction( parse => CommandHelpers.Guard( () => Run( parse.GetValue( file ), parse.GetValue( formats ) ) ) );
            return command;
        }

        private static int Run( string path, bool showFormats )
        {
            var node = ResourceFiles.Load( path );

            var rows = new List<(string Name, string Value)>
            {
                ("File", path),
                ("Type", node.DataType.Name),
            };

            if ( node.Data is Resource resource )
                rows.Add( ("Version", $"0x{resource.Version:X8} ({ResourceFiles.VersionName( resource.Version )})") );

            rows.AddRange( Contents( node.Data ) );

            if ( showFormats )
            {
                rows.Add( ("Export", CliUserDialogs.Describe( node.ExportFormats() )) );
                rows.Add( ("Replace", CliUserDialogs.Describe( node.ReplaceFormats() )) );
            }

            var width = rows.Max( x => x.Name.Length );
            foreach ( var (name, value) in rows )
                Program.Output.WriteLine( $"{( name + ":" ).PadRight( width + 2 )}{value}" );

            return 0;
        }

        /// <summary>
        /// The counts worth knowing for each kind of resource.
        /// </summary>
        private static IEnumerable<(string, string)> Contents( object data )
        {
            switch ( data )
            {
                case ModelPack pack:
                    yield return ("Textures", Count( pack.Textures?.Count ));
                    yield return ("Materials", Count( pack.Materials?.Count ));

                    foreach ( var row in Contents( pack.Model ) )
                        yield return row;

                    yield return ("Animations", Count( pack.AnimationPack?.Animations?.Count ));
                    yield return ("Blend animations", Count( pack.AnimationPack?.BlendAnimations?.Count ));
                    break;

                case Model model:
                    yield return ("Nodes", Count( model.Nodes?.Count() ));
                    yield return ("Meshes", Count( model.Meshes?.Count() ));
                    yield return ("Bones", Count( model.Bones?.Count ));
                    break;

                case TextureDictionary textures:
                    yield return ("Textures", textures.Count.ToString());

                    foreach ( var texture in textures.Textures )
                        yield return ($"  {texture.Name}", $"{texture.Format}, {texture.Data.Length} bytes");

                    break;

                case MaterialDictionary materials:
                    yield return ("Materials", materials.Count.ToString());
                    break;

                case AnimationPack animations:
                    yield return ("Animations", Count( animations.Animations?.Count ));
                    yield return ("Blend animations", Count( animations.BlendAnimations?.Count ));
                    break;

                case Animation animation:
                    yield return ("Duration", $"{animation.Duration:0.###}s");
                    yield return ("Controllers", Count( animation.Joints?.Count ));
                    break;
            }
        }

        private static string Count( int? value ) => value?.ToString() ?? "none";
    }
}
