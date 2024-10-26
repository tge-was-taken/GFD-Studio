using System.Text.RegularExpressions;

namespace GFDLibrary.Conversion.AssimpNet.Utilities
{
    internal static class AssimpConverterCommon
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
