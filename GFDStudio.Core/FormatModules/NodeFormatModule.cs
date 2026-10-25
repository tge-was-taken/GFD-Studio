using System.IO;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.FormatModules
{
    public class NodeFormatModule : FormatModule<Node>
    {
        public override string Name
            => "Node";

        public override string[] Extensions
            => new[] { "gnd" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Node;
        }

        protected override void ExportCore( Node obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Node ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Node>( stream );
        }
    }
}