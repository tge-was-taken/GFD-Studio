using System.IO;
using Assimp;
using GFDLibrary.Models.Conversion;
using GFDStudio.IO;

namespace GFDStudio.FormatModules
{
    public class AssimpSceneFormatModule : FormatModule<Scene>
    {
        public override string Name
            => "Assimp Model";

        public override string[] Extensions
            => new[] { "dae", "obj", "fbx" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return PathUtilities.MatchesAnyExtension( filename, Extensions );
        }

        protected override Scene ImportCore( Stream stream, string filename = null )
        {
            return AssimpSceneImporter.ImportFile( filename );
        }

        protected override void ExportCore( Scene obj, Stream stream, string filename = null )
        {
            ModelPackExporter.ExportFile( obj, filename );
        }
    }
}
