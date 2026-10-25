using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform.Storage;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// One entry of a format module filter: a description and the patterns it matches.
    /// </summary>
    public sealed record FileFilter( string Description, string[] Patterns )
    {
        /// <summary>
        /// Splits the pipe separated filter the format modules generate.
        /// </summary>
        public static List<FileFilter> Parse( string filter )
        {
            var filters = new List<FileFilter>();
            if ( string.IsNullOrEmpty( filter ) )
                return filters;

            var parts = filter.Split( '|' );
            for ( var i = 0; i + 1 < parts.Length; i += 2 )
                filters.Add( new FileFilter( parts[i], parts[i + 1].Split( ';', StringSplitOptions.RemoveEmptyEntries ) ) );

            return filters;
        }

        public FilePickerFileType ToPickerType()
            => new( Description ) { Patterns = Patterns };

        public bool Matches( string path )
        {
            var extension = Path.GetExtension( path );
            return Patterns.Any( x => x == "*.*" ||
                                      string.Equals( Path.GetExtension( x ), extension, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary>
        /// Works out which filter the user settled on, counted from one. The picker does not say
        /// which one they chose, so the file extension decides, and the first filter stands in
        /// when nothing matches.
        /// </summary>
        public static int IndexOf( List<FileFilter> filters, string path )
        {
            var index = filters.FindIndex( x => x.Matches( path ) );
            return index < 0 ? 1 : index + 1;
        }
    }
}
