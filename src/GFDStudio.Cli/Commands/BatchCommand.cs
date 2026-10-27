using System;
using System.CommandLine;
using System.IO;
using GFDStudio.IO;

namespace GFDStudio.Cli.Commands
{
    /// <summary>
    /// The jobs that run over a whole directory of files. Each one asks for what it needs through
    /// the dialogs, so the answers go in here in the order it will ask for them.
    /// </summary>
    public static class BatchCommand
    {
        public static Command Create()
            => new( "batch", "Run one of the jobs that work over a whole directory of files." )
            {
                RetargetAnimations(),
                ConvertAnimationsToP5(),
                RescaleAnimations(),
                ConvertMaterials(),
                CopySplitAnimations(),
                ExportTextures(),
                ReplaceTextures(),
            };

        private static Command RetargetAnimations()
        {
            var directory = DirectoryArgument( "The directory of GAP files to retarget. The files are replaced in place." );
            var original = CommandHelpers.Text( "--original", "The model the animations were made for.", required: true );
            var model = CommandHelpers.Text( "--new", "The model to retarget them to.", required: true );
            var yes = CommandHelpers.Switch( "--yes", "Fix arms." );
            var no = CommandHelpers.Switch( "--no", "Do not fix arms." );

            var command = new Command( "retarget-animations",
                                       "Rewrite every GAP under a directory to follow a different model's skeleton." )
            {
                directory, original, model, yes, no,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddOpenFile( parse.GetValue( original ) );
                Program.Dialogs.AddOpenFile( parse.GetValue( model ) );
                Program.Dialogs.YesNo = CommandHelpers.Toggle( parse.GetValue( yes ), parse.GetValue( no ) );
                Program.Dialogs.AddFolder( Directory( parse, directory ) );

                BatchTools.RetargetAnimationPacks();
                return 0;
            } ) );

            return command;
        }

        private static Command ConvertAnimationsToP5()
        {
            var directory = DirectoryArgument( "The directory of GAP files to convert. The files are replaced in place." );

            var command = new Command( "convert-animations-to-p5",
                                       "Convert every GAP under a directory from the Royal format to the original." )
            {
                directory,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddFolder( Directory( parse, directory ) );
                BatchTools.ConvertAnimationPacksToP5();
                return 0;
            } ) );

            return command;
        }

        private static Command RescaleAnimations()
        {
            var directory = DirectoryArgument( "The directory of GAP files to rescale. The files are replaced in place." );
            var scale = CommandHelpers.Text( "--scale", "The scale to apply, as x,y,z. Defaults to 1,1,1." );
            var position = CommandHelpers.Text( "--position", "The offset to apply, as x,y,z. Defaults to 0,0,0." );
            var rotation = CommandHelpers.Text( "--rotation", "Euler degrees as x,y,z, or four numbers for a quaternion." );

            var command = new Command( "rescale-animations",
                                       "Apply a scale, position and rotation to every GAP under a directory." )
            {
                directory, scale, position, rotation,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddFolder( Directory( parse, directory ) );
                Program.Dialogs.Transform = TransformParser.Parse( parse.GetValue( scale ),
                                                                   parse.GetValue( position ),
                                                                   parse.GetValue( rotation ) )
                                            ?? throw new CliException( "Pass at least one of --scale, --position or --rotation." );

                BatchTools.RescaleAnimationPacks();
                return 0;
            } ) );

            return command;
        }

        private static Command ConvertMaterials()
        {
            var directory = DirectoryArgument( "The directory of GMD files to convert. The files are replaced in place." );
            var version = CommandHelpers.Text( "--version", "Resource version to convert to, such as 0x01105100." );
            var preset = CommandHelpers.Text( "--preset", "Material preset YAML to convert against." );

            var command = new Command( "convert-materials", "Apply one material preset to every GMD under a directory." )
            {
                directory, version, preset,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddFolder( Directory( parse, directory ) );
                Program.Dialogs.ModelVersion = ConvertCommand.ParseVersion( parse.GetValue( version ) );
                Program.Dialogs.MaterialPreset = parse.GetValue( preset );

                BatchTools.ConvertModelMaterials();
                return 0;
            } ) );

            return command;
        }

        private static Command CopySplitAnimations()
        {
            var directory = DirectoryArgument( "The directory of model files to write the new GAP files for." );
            var a = CommandHelpers.Text( "--a", "The GAP ending with a.", required: true );
            var b = CommandHelpers.Text( "--b", "The GAP ending with b.", required: true );
            var c = CommandHelpers.Text( "--c", "The GAP ending with c.", required: true );
            var yes = CommandHelpers.Switch( "--yes", "The GAP ID is 52." );
            var no = CommandHelpers.Switch( "--no", "The GAP ID is 51." );

            var command = new Command( "copy-split-animations",
                                       "Write the a, b and c halves of a battle GAP retargeted to each model under a directory." )
            {
                directory, a, b, c, yes, no,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddOpenFile( parse.GetValue( a ) );
                Program.Dialogs.AddOpenFile( parse.GetValue( b ) );
                Program.Dialogs.AddOpenFile( parse.GetValue( c ) );
                Program.Dialogs.YesNo = CommandHelpers.Toggle( parse.GetValue( yes ), parse.GetValue( no ) );
                Program.Dialogs.AddFolder( Directory( parse, directory ) );

                BatchTools.CopySplitAnimationPackToModels();
                return 0;
            } ) );

            return command;
        }

        private static Command ExportTextures()
        {
            var directory = DirectoryArgument( "The directory of models to take the textures from." );
            var output = CommandHelpers.Text( "--out", "The directory to write the textures into.", required: true );

            var command = new Command( "export-textures",
                                       "Write every texture of every model under a directory into one folder." )
            {
                directory, output,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                System.IO.Directory.CreateDirectory( parse.GetValue( output ) );

                Program.Dialogs.AddFolder( Directory( parse, directory ) );
                Program.Dialogs.AddFolder( parse.GetValue( output ) );

                BatchTools.ExportTexturesFromModels();
                return 0;
            } ) );

            return command;
        }

        private static Command ReplaceTextures()
        {
            var directory = DirectoryArgument( "The directory of models to replace textures in. The files are replaced in place." );
            var from = CommandHelpers.Text( "--from", "The directory holding the replacement DDS files.", required: true );

            var command = new Command( "replace-textures",
                                       "Swap a DDS of the same name into every model under a directory." )
            {
                directory, from,
            };

            command.SetAction( parse => CommandHelpers.Guard( () =>
            {
                Program.Dialogs.AddFolder( Directory( parse, directory ) );
                Program.Dialogs.AddFolder( parse.GetValue( from ) );

                BatchTools.ReplaceTexturesInModels();
                return 0;
            } ) );

            return command;
        }

        private static Argument<string> DirectoryArgument( string description )
            => new( "dir" ) { Description = description };

        private static string Directory( ParseResult parse, Argument<string> argument )
        {
            var path = parse.GetValue( argument );

            if ( !System.IO.Directory.Exists( path ) )
                throw new CliException( $"No directory at \"{path}\"." );

            return path;
        }
    }
}
