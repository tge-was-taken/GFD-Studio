using System.IO;
using AtlusGfdLib;

namespace AtlusGfdEditor.FormatModules
{
    public class ModelFormatModule : FormatModule<Model>
    {
        public override string Name =>
            "Model";

        public override string[] Extensions =>
            new[] { "gfs", "gmd", "gap" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportCore( Model obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override Model ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Model>( stream );
        }
    }
}
