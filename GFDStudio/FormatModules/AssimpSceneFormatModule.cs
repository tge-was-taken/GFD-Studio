using System;
using System.IO;
using GFDStudio.IO;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Proxy class for file formats that do not have a direct type representation.
    /// </summary>
    public class FileFormatProxy
    {
        public FileFormatProxy(Stream stream, string filename = null)
        {
            Stream = stream;
            Filename = filename;
        }

        public Stream Stream { get; }
        public string Filename { get; }
    }

    public class AssimpScene : FileFormatProxy
    {
        public AssimpScene(Stream stream, string filename) : base(stream, filename ) { }
    }

    public class AssimpSceneFormatModule : FormatModule<AssimpScene>
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

        protected override AssimpScene ImportCore( Stream stream, string filename = null )
        {
            return new AssimpScene( stream, filename );
        }

        protected override void ExportCore( AssimpScene obj, Stream stream, string filename = null )
        {
            throw new NotImplementedException();
            // ModelPackExporter.ExportFile( obj, filename );
        }
    }
}
