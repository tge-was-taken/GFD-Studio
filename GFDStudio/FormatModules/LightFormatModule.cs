using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class LightFormatModule : FormatModule<Light>
    {
        public override string Name
            => "Light";

        public override string[] Extensions
            => new[] { "glt" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Light;
        }

        protected override void ExportCore( Light obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Light ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Light>( stream );
        }
    }
}