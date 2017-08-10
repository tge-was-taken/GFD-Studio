using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.Modules
{
    public class ArchiveModule : Module<Archive>
    {
        public override string Name
            => "Archive file";

        public override string[] Extensions
            => new[] { "bin" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Archive.IsValidArchive( stream );
        }

        protected override void ExportCore( Archive obj, Stream stream, string filename = null )
        {
            obj.Save( stream );
        }

        protected override Archive ImportCore( Stream stream, string filename = null )
        {
            return new Archive( stream );
        }
    }
}
