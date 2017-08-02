using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor
{
    public static class Utilities
    {
        public static bool MatchesAnyExtension( string filename, params string[] extensions )
        {
            var fileExtension = Path.GetExtension( filename );
            if ( fileExtension == null )
                return false;

            var fileExtensionWithoutPeriod = fileExtension.TrimStart( '.' );

            foreach ( var extension in extensions )
            {
                var extensionWithoutPeriod = extension.TrimStart( '.' );
                if ( fileExtensionWithoutPeriod.Equals( extensionWithoutPeriod, StringComparison.InvariantCultureIgnoreCase ) )
                    return true;
            }

            return false;
        }
    }
}
