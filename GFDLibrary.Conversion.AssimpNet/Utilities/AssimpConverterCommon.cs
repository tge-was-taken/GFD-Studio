using System.Text.RegularExpressions;

namespace GFDLibrary.Conversion.AssimpNet.Utilities
{
    internal static class AssimpConverterCommon
    {
        // Based on Maxscript importer
        public static readonly Regex LegacyMeshAttachmentNameRegex = new Regex( "_Mesh([0-9]+)", RegexOptions.Compiled );
        public static readonly Regex MeshAttachmentNameRegex = new Regex( "_gfdMesh_([0-9]+)", RegexOptions.Compiled );

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
