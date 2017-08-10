using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.Modules
{
    /// <summary>
    /// Generates filters based on the currently registered modules.
    /// </summary>
    public static class ModuleFilterGenerator
    {
        private static StringBuilder sBuilder = new StringBuilder();

        /// <summary>
        /// Generate a file filter using the modules that match the specified flags.
        /// </summary>
        /// <param name="flags">The module flags used to filter which modules to include in the filter.</param>
        /// <returns>A file filter for use in file dialogs.</returns>
        public static string GenerateFilter( FormatModuleUsageFlags flags )
        {
            return GenerateFilter( flags, null );
        }

        /// <summary>
        /// Generate a file filter using the modules that match the specified flags.
        /// </summary>
        /// <param name="flags">The module flags used to filter which modules to include in the filter.</param>
        /// <param name="objectTypes">The module object types used to filter which modules to include in the filter.</param>
        /// <returns>A file filter for use in file dialogs.</returns>
        public static string GenerateFilter( FormatModuleUsageFlags flags, params Type[] objectTypes )
        {
            return GenerateFilterInternal( new[] { flags }, objectTypes );
        }

        public static string GenerateFilter( params FormatModuleUsageFlags[] flags )
        {
            return GenerateFilterInternal( flags );
        }

        public static string GenerateFilter( FormatModuleUsageFlags[] flags, params Type[] objectTypes )
        {
            return GenerateFilterInternal( flags, objectTypes );
        }

        private static string GenerateFilterInternal( FormatModuleUsageFlags[] flags, Type[] objectTypes = null )
        {
            if ( sBuilder.Length != 0 )
                sBuilder.Clear();

            bool isFirst = true;
            foreach ( var module in ModuleRegistry.Modules )
            {
                // skip module if it does not have the any of the flags requested
                if ( !flags.Any( x => module.UsageFlags.HasFlag( x ) ) )
                    continue;

                if ( objectTypes != null )
                {
                    // skip module if it does not match with one of the object types
                    if ( !objectTypes.Contains( module.ObjectType ) )
                        continue;
                }

                if ( !isFirst )
                {
                    // add seperator for the previous module if this is not the first iteration
                    sBuilder.Append( '|' );
                }
                else
                {
                    isFirst = false;
                }

                // name part
                sBuilder.Append( module.Name );
                sBuilder.Append( '|' );

                // file extension part
                for ( int extensionIndex = 0; extensionIndex < module.Extensions.Length; extensionIndex++ )
                {
                    sBuilder.Append( $"*.{module.Extensions[extensionIndex]}" );

                    // add seperator if this is not the last extension
                    if ( extensionIndex != ( module.Extensions.Length - 1 ) )
                    {
                        sBuilder.Append( ';' );
                    }
                }
            }

            return sBuilder.ToString();
        }
    }
}
