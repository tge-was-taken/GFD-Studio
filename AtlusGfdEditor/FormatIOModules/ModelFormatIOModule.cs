using System;
using System.IO;
using AtlusGfdLib;

namespace AtlusGfdEditor.FormatIOModules
{
    public class ModelFormatIOModule : FormatIOModule<Model>
    {
        public override string Name =>
            "Atlus Gfd Model Format";

        public override string Description =>
            "Format containing texture, material, scene and animation data";

        public override string[] Extensions =>
            new[] { "gfs", "gmd", "gap" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportInternal( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportInternal( Model obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override Model ImportInternal( Stream stream, string filename = null )
        {
            return Resource.Load<Model>( stream );
        }
    }
}
