using System;
using System.IO;
using System.Windows.Markup;
using GFDStudio.IO;

namespace GFDStudio.FormatModules
{
    public class AssimpScene : FileFormatProxy
    {
        public AssimpScene(Stream stream, string filename) : base(stream, filename ) { }
    }

    public class AssimpSceneFormatModule : FormatModule<AssimpScene>
    {
        public override string Name
            => "3D Model";

        public override string[] Extensions
            => new[] { "dae", "obj", "fbx", "ascii.fbx" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return PathUtilities.MatchesAnyExtension( filename, Extensions );
        }

        protected override AssimpScene ImportCore( Stream stream, string filename = null )
        {
            return new AssimpScene( stream, filename );
        }

        protected override void ExportCore( AssimpScene obj, Stream stream, string filename = null )
        {
            throw new NotSupportedException();
        }
    }
}
