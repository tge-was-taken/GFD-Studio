using System.IO;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.FormatModules
{
    public class MorphFormatModule : FormatModule<Morph>
    {
        public override string Name
            => "Morph";

        public override string[] Extensions
            => new[] { "gmrp" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.GetResourceType( stream ) == ResourceType.Light;
        }

        protected override void ExportCore( Morph obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Morph ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Morph>( stream );
        }
    }
}