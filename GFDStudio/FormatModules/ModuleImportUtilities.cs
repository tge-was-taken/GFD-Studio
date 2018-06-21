using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Utilities for importing files using the module system.
    /// </summary>
    public static class ModuleImportUtilities
    {
        /// <summary>
        /// Gets a list of modules with the given extension.
        /// </summary>
        /// <param name="extension">The extension to match.</param>
        /// <returns>A list of modules with the given extension.</returns>
        public static List<IFormatModule> GetModulesWithExtension( string extension )
        {
            extension = extension.TrimStart( '.' );
            return FormatModuleRegistry.Modules.Where( x => x.Extensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) ).ToList();
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified file.
        /// </summary>
        /// <param name="filepath">The file to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( string filepath, out IFormatModule module )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return TryGetModuleForImport( stream, out module, Path.GetFileName( filepath ) );
            }
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified data.
        /// </summary>
        /// <param name="data">The data to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <param name="filename">Optional filename parameter. Might be required by some modules.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( byte[] data, out IFormatModule module, string filename = null )
        {
            using ( var stream = new MemoryStream( data ) )
            {
                return TryGetModuleForImport( stream, out module, filename );
            }
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified stream data.
        /// </summary>
        /// <param name="stream">The stream containing data to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <param name="filename">Optional filename parameter. Might be required by some modules.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( Stream stream, out IFormatModule module, string filename = null )
        {
            // try to find a module that can import this file
            module = FormatModuleRegistry.Modules.SingleOrDefault( x => x.CanImport( stream, filename ) );

            // simplicity is nice sometimes c:
            return module != null;
        }

        /// <summary>
        /// Imports a file from a given path using a registered module associated with type <typeparamref name="T"/>. Returns null on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ImportFile<T>( string filePath ) where T : class
        {
            if ( !TryGetModuleForImport( filePath, out var module ) )
                return null;

            return module.Import( filePath ) as T;
        }

        /// <summary>
        /// Selects a file to import using a registered module associated with type <typeparamref name="T"/>. Returns null on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <returns></returns>
        public static T SelectImportFile<T>( string title = "Select file to import" ) where T : class
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter             = ModuleFilterGenerator.GenerateFilter( new[] { FormatModuleUsageFlags.Import }, typeof( T ) );
                dialog.AutoUpgradeEnabled = true;
                dialog.CheckPathExists    = true;
                dialog.Title              = title;
                dialog.ValidateNames      = true;
                dialog.AddExtension       = true;

                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                return ImportFile<T>( dialog.FileName );
            }
        }
    }
}
