using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
{
    public class SceneFormatModule : FormatModule<Scene>
    {
        public override string Name
            => "Scene";

        public override string[] Extensions
            => new[] { "gsn" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Scene;
        }

        protected override void ExportCore( Scene obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Scene ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Scene>( stream );
        }
    }
}