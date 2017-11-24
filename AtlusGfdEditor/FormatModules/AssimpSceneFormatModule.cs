using System.IO;
using Assimp;
using AtlusGfdLib.Assimp;
using AtlusGfdEditor.IO;

namespace AtlusGfdEditor.FormatModules
{
    public class AssimpSceneFormatModule : FormatModule<Scene>
    {
        public override string Name
            => "Assimp Scene";

        public override string[] Extensions
            => new[] { "dae", "obj", "fbx" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return PathUtillities.MatchesAnyExtension( filename, Extensions );
        }

        protected override Scene ImportCore( Stream stream, string filename = null )
        {
            return AssimpImporter.ImportFile( filename );
        }

        protected override void ExportCore( Scene obj, Stream stream, string filename = null )
        {
            AssimpExporter.ExportFile( obj, filename );
        }
    }
}
