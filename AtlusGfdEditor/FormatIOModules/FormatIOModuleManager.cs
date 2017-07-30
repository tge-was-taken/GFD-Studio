using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.FormatIOModules
{
    public static class FormatIOModuleManager
    {
        private static List<IFormatIOModule> sModules = new List<IFormatIOModule>();

        /// <summary>
        /// Gets the registered modules.
        /// </summary>
        public static IReadOnlyList<IFormatIOModule> Modules => sModules;

        static FormatIOModuleManager()
        {
            RegisterModules
            (
                new ModelFormatIOModule(),
                new ArchiveFormatIOModule()
            );
        }

        //
        // Methods for registration of modules
        //

        /// <summary>
        /// Registers a format IO module.
        /// </summary>
        /// <param name="module">The module to register.</param>
        public static void RegisterModule( IFormatIOModule module )
        {
            sModules.Add( module );
        }

        /// <summary>
        /// Registers multiple format IO modules.
        /// </summary>
        /// <param name="modules">The modules to register.</param>
        /// <remarks>Equivalent to registering each module seperately.</remarks>
        public static void RegisterModules( params IFormatIOModule[] modules )
        {
            foreach ( var module in modules )
            {
                RegisterModule( module );
            }
        }

        //
        // Methods for finding the appropriate module by object type
        //
        public static bool TryGetModuleByObjectType( Type objectType, out IFormatIOModule module )
        {
            // find module whose ObjectType matches with the given type
            module = sModules.SingleOrDefault( x => x.ObjectType == objectType );

            return module != null;
        }

        //
        // Methods for finding the appropriate module by module type
        //
        public static bool TryGetModuleByModuleType( Type moduleType, out IFormatIOModule module )
        {
            // find module whose type matches the given type
            module = sModules.SingleOrDefault( x => x.GetType() == moduleType );

            return module != null;
        }

        //
        // Methods for finding the appropriate module for importing
        //

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified file.
        /// </summary>
        /// <param name="filepath">The file to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( string filepath, out IFormatIOModule module )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return TryGetModuleForImport( stream, out module, Path.GetFileName(filepath) ); 
            }
        }

        /// <summary>
        /// Attemps to get the appropriate module for importing the specified data.
        /// </summary>
        /// <param name="data">The data to import.</param>
        /// <param name="module">The out parameter containing the found module, if none are found then it will be null.</param>
        /// <param name="filename">Optional filename parameter. Might be required by some modules.</param>
        /// <returns>Whether or not a module was found.</returns>
        public static bool TryGetModuleForImport( byte[] data, out IFormatIOModule module, string filename = null )
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
        public static bool TryGetModuleForImport( Stream stream, out IFormatIOModule module, string filename = null )
        {
            // try to find a module that can import this file
            module = Modules.SingleOrDefault( x => x.CanImport( stream, filename ) );

            // simplicity is nice sometimes c:
            return module != null;
        }

        // Methods for generating file filters
        public static string GenerateFileFilter( FormatModuleUsageFlags flags )
        {
            var stringBuilder = new StringBuilder();

            for ( int moduleIndex = 0; moduleIndex < Modules.Count; moduleIndex++ )
            {
                var module = Modules[moduleIndex];

                // skip module if it does not have the flags requested
                if ( !module.UsageFlags.HasFlag( flags ) )
                    continue;

                // name part
                stringBuilder.Append( module.Name );
                stringBuilder.Append( '|' );

                // file extension part
                for ( int extensionIndex = 0; extensionIndex < module.Extensions.Length; extensionIndex++ )
                {
                    stringBuilder.Append( $"*.{module.Extensions[extensionIndex]}" );

                    // add seperator if this is not the last extension
                    if ( extensionIndex != (module.Extensions.Length - 1) )
                    {
                        stringBuilder.Append( ';' );
                    }
                }

                // add seperator if this is not the last module
                if ( moduleIndex != ( Modules.Count - 1 ) )
                {
                    stringBuilder.Append( '|' );
                }
            }

            return stringBuilder.ToString();
        }
    }
}
