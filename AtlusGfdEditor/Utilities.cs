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
        public static bool MatchExtension( string filename, params string[] extensions )
        {
            var extension = Path.GetExtension( filename );
            if ( extension == null || !extensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) )
                return false;

            return true;
        }
    }
}
