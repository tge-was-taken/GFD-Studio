using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class ModelFormatModule : FormatModule<Model>
    {
        public override string Name =>
            "Model";

        public override string[] Extensions =>
            new[] { "gfs", "gmd" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Model;
        }

        protected override void ExportCore( Model obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Model ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Model>( stream );
        }
    }
}
