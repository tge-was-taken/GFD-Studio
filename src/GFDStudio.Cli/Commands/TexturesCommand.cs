using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using GFDStudio.IO;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// The textures of a single model, in and out.
    /// </summary>
    public static class TexturesCommand
    {
        public static Command Create()
            => new( "textures", "Export the textures of a model, or replace them from a directory." )
            {
                Export(),
                Replace(),
            };

        private static Command Export()
        {
            var model = new Argument<string>( "model" ) { Description = "The model to take the textures from." };
            var output = CommandHelpers.Text( "--out", "The directory to write the textures into.", required: true );

            var command = new Command( "export", "Write every texture of a model into a directory." ) { model, output };
            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                var directory = parse.GetValue( output );
                Directory.CreateDirectory( directory );

                var pack = ResourceFiles.LoadModelPack( parse.GetValue( model ) );
                if ( pack.Textures == null )
                {
                    Console.Error.WriteLine( $"{parse.GetValue( model )} holds no textures." );
                    return 0;
                }

                foreach ( var texture in pack.Textures.Textures )
                    BatchTools.SaveTexture( texture, directory );

                Program.Output.WriteLine( $"Wrote {pack.Textures.Count} textures to {directory}" );
                return 0;
            } ) );

            return command;
        }

        private static Command Replace()
        {
            var model = new Argument<string>( "model" ) { Description = "The model to replace the textures in." };
            var from = CommandHelpers.Text( "--from", "The directory holding the replacement DDS files.", required: true );
            var output = CommandHelpers.Text( "--out", "Where to write the model.", required: true );

            var command = new Command( "replace", "Swap in the DDS files from a directory that share a name with a texture in the model." )
            {
                model, from, output,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                var path = parse.GetValue( model );
                var pack = ResourceFiles.LoadModelPack( path );

                if ( pack.Textures == null )
                    throw new CliException( $"{path} holds no textures to replace." );

                var replacements = ResourceFiles.DdsFiles( parse.GetValue( from ) ).ToList();
                if ( replacements.Count == 0 )
                    throw new CliException( $"No DDS files under \"{parse.GetValue( from )}\"." );

                BatchTools.ReplaceTexturesInModel( pack, replacements );
                pack.Save( parse.GetValue( output ) );

                Program.Output.WriteLine( $"Wrote {parse.GetValue( output )}" );
                return 0;
            } ) );

            return command;
        }
    }
}
