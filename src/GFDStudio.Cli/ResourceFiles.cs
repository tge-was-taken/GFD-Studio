using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.DataViewNodes;
using GFDStudio.IO;

namespace GFDStudio.Cli
{
    /// <summary>
    /// Opening a file the way the GUI does, minus the tree.
    /// </summary>
    public static class ResourceFiles
    {
        /// <summary>
        /// Loads a file into the view node that knows how to export and replace it. A scene in a
        /// format only the converter reads, such as Collada, is converted to a model pack first.
        /// </summary>
        public static DataViewNode Load( string path )
        {
            if ( !File.Exists( path ) )
                throw new CliException( $"No file at \"{path}\"." );

            if ( IsAssimpScene( path ) )
            {
                var converted = ModelConverterUtility.ConvertAssimpModel( path, null )
                                ?? throw new CliException( $"\"{path}\" could not be converted to a model pack." );

                return DataViewNodeFactory.Create( Path.GetFileName( path ), converted );
            }

            // The factory's own file overload leaves the file open, which a command writing back
            // to the same path would trip over. The stream is deliberately left undisposed: a
            // node for a format with no reader of its own keeps hold of it.
            var stream = new MemoryStream( File.ReadAllBytes( path ) );

            if ( !DataViewNodeFactory.TryCreate( Path.GetFileName( path ), stream, null, out var node, out var error ) )
                throw new CliException( error == null
                    ? $"Nothing here reads \"{path}\"."
                    : $"\"{path}\" could not be read: {error.Message}" );

            // Several parts of a Metaphor model pack live in files beside it.
            if ( node.Data is ModelPack pack && pack.Version >= 0x02000000 )
                CollectModelParts.CollectDisconnectedModelPartsForModelPack( pack, path );

            return node;
        }

        public static ModelPack LoadModelPack( string path )
            => Load( path ).Data as ModelPack ?? throw new CliException( $"\"{path}\" is not a model pack." );

        public static bool IsAssimpScene( string path )
            => Extensions( typeof( AssimpScene ) ).Any( x => PathUtilities.MatchesAnyExtension( path, new[] { x } ) );

        public static string[] Extensions( Type type )
            => FormatModuleRegistry.ModuleByType.TryGetValue( type, out var module ) ? module.Extensions : Array.Empty<string>();

        /// <summary>
        /// The game a resource version belongs to, where it is one the library knows by name.
        /// </summary>
        public static string VersionName( uint version )
        {
            var names = typeof( ResourceVersion )
                        .GetFields( BindingFlags.Public | BindingFlags.Static )
                        .Where( x => x.IsLiteral && x.FieldType == typeof( uint ) && (uint)x.GetRawConstantValue() == version )
                        .Select( x => x.Name )
                        .ToList();

            return names.Count == 0 ? "unrecognised" : string.Join( " / ", names );
        }

        /// <summary>
        /// Writes a node out, taking YAML itself the way the export dialog does.
        /// </summary>
        public static void Save( DataViewNode node, string path, Type formatType )
        {
            var directory = Path.GetDirectoryName( Path.GetFullPath( path ) );
            if ( !string.IsNullOrEmpty( directory ) )
                Directory.CreateDirectory( directory );

            if ( formatType == null )
                YamlSerializer.SaveYamlFile( node.Data, path );
            else
                node.Export( path, formatType );
        }

        public static IEnumerable<string> DdsFiles( string directory )
        {
            if ( !Directory.Exists( directory ) )
                throw new CliException( $"No directory at \"{directory}\"." );

            return Directory.EnumerateFiles( directory, "*.dds", SearchOption.AllDirectories );
        }
    }
}
