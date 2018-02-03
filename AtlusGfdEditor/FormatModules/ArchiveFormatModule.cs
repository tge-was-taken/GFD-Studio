using System.IO;
using AtlusGfdLibrary;

namespace AtlusGfdEditor.FormatModules
{
    public class ArchiveFormatModule : FormatModule<Archive>
    {
        public override string Name
            => "Archive";

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
