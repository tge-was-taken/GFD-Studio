using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GFDLibrary.Processing.Models
{
    public static class AssimpConverterCommon
    {
        public static readonly Regex MeshAttachmentNameRegex = new Regex( "_Mesh([0-9]+)", RegexOptions.Compiled );

        public static string UnescapeName( string name )
        {
            return name.Replace( "___", " " );
        }

        public static string EscapeName( string name )
        {
            return name.Replace( " ", "___" );
        }
    }
}
