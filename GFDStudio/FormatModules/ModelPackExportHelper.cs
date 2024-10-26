using System.IO;
using GFDLibrary;
using GFDLibrary.Conversion.AssimpNet;
using GFDLibrary.Conversion.FbxSdk;

namespace GFDStudio.FormatModules
{
    public static class ModelPackExportHelper
    {
        public static void ExportFile( ModelPack modelPack, string path )
        {
            var ext = Path.GetExtension( path );
            if ( ext.Equals( ".fbx", System.StringComparison.OrdinalIgnoreCase ) )
            {
                FbxSdkModelPackExporter.ExportFile( modelPack, path, new FbxSdkModelPackExporterConfig() );
            }
            else
            {
                AssimpNetModelPackExporter.ExportFile( modelPack, path );
            }
        }
    }
}
