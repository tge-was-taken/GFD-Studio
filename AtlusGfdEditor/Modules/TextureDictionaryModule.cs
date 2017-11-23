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
            => "Texture dictionary";

        public override string[] Extensions
            => new[] { "gtxd" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportCore( TextureDictionary obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override TextureDictionary ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load< TextureDictionary >( stream );
        }
    }
}
