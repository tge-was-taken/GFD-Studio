using System.IO;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.FormatModules
{
    public class MeshFormatModule : FormatModule<Mesh>
    {
        public override string Name
            => "Mesh";

        public override string[] Extensions
            => new[] { "gmsh" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Mesh;
        }

        protected override void ExportCore( Mesh obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Mesh ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Mesh>( stream );
        }
    }
}