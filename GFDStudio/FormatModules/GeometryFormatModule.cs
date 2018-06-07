using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class GeometryFormatModule : FormatModule<Geometry>
    {
        public override string Name
            => "Geometry";

        public override string[] Extensions
            => new[] { "ggeo" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Geometry;
        }

        protected override void ExportCore( Geometry obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Geometry ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Geometry>( stream );
        }
    }
}