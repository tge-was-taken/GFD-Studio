using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.FormatIOModules
{
    public class ArchiveFormatIOModule : FormatIOModule<Archive>
    {
        public override string Name
            => "Archive file";

        public override string Description
            => "Generic archive file used for storing data";

        public override string[] Extensions
            => new[] { "bin" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportInternal( Stream stream, string filename = null )
        {
            return Archive.IsValidArchive( stream );
        }

        protected override void ExportInternal( Archive obj, Stream stream, string filename = null )
        {
            obj.SaveToStream( stream );
        }

        protected override Archive ImportInternal( Stream stream, string filename = null )
        {
            return new Archive( stream );
        }
    }
}
