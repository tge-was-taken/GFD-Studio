using System.IO;
using GFDLibrary;
using GFDLibrary.Conversion.AssimpNet;
#if FBXSDK
using GFDLibrary.Conversion.FbxSdk;
#endif

namespace GFDStudio.FormatModules
{
    public static class ModelPackExportHelper
    {
        public static void ExportFile( ModelPack modelPack, string path )
        {
#if FBXSDK
            if ( Path.GetExtension( path ).Equals( ".fbx", System.StringComparison.OrdinalIgnoreCase ) )
            {
                FbxSdkModelPackExporter.ExportFile( modelPack, path, new FbxSdkModelPackExporterConfig() );
                return;
            }
#endif
            AssimpNetModelPackExporter.ExportFile( modelPack, path );
        }
    }
}
