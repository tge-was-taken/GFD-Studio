using System.IO;
using GFDLibrary;
using GFDLibrary.Cameras;

namespace GFDStudio.FormatModules
{
    public class CameraFormatModule : FormatModule<Camera>
    {
        public override string Name
            => "Camera";

        public override string[] Extensions
            => new[] { "gcm" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Camera;
        }

        protected override void ExportCore( Camera obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Camera ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Camera>( stream );
        }
    }
}