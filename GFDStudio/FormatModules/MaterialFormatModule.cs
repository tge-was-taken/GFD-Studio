using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
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
            return Resource.GetResourceType( stream ) == ResourceType.Material;
        }

        protected override void ExportCore( Material obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Material ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load< Material >( stream );
        }
    }
}
