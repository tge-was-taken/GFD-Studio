using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.Modules
{
    public class StreamModule : Module<Stream>
    {
        public override string Name 
            => "Generic stream";

        public override string[] Extensions
            => new string[] { "*" };

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
