using System.IO;
using AtlusGfdLibrary;

namespace AtlusGfdEditor.FormatModules
{
    public class MaterialFormatModule : FormatModule<Material>
    {
        public override string Name
            => "Material";

        public override string[] Extensions
            => new[] { "gmt" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportCore( Material obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override Material ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load< Material >( stream );
        }
    }
}
