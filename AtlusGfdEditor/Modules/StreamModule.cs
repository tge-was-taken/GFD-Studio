using System.IO;

namespace AtlusGfdEditor.Modules
{
    public class StreamModule : Module<Stream>
    {
        public override string Name 
            => "File";

        public override string[] Extensions
            => new[] { "*" };

        public override FormatModuleUsageFlags UsageFlags => FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return true;
        }

        protected override void ExportCore( Stream obj, Stream stream, string filename = null )
        {
            obj.CopyTo( stream );
        }

        protected override Stream ImportCore( Stream stream, string filename = null )
        {
            return stream;
        }
    }
}
