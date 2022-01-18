using System.IO;
using GFDLibrary;
using GFDLibrary.Effects;

namespace GFDStudio.FormatModules
{
    public class EplFormatModule : FormatModule<Epl>
    {
        public override string Name
            => "Epl";

        public override string[] Extensions
            => new[] { "epl" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return true;
        }

        protected override void ExportCore( Epl obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override Epl ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<Epl>( stream );
        }
    }
}