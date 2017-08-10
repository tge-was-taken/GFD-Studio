using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.Modules
{
    public class TextureDictionaryModule : Module<TextureDictionary>
    {
        public override string Name
            => "Atlus Gfd Texture Dictionary";

        public override string[] Extensions
            => new[] { "gtd" };

        public override FormatModuleUsageFlags UsageFlags
            => 0;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override void ExportCore( TextureDictionary obj, Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override TextureDictionary ImportCore( Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }
    }
}
