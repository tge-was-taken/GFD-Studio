using System.IO;
using AtlusGfdLibrary;

namespace AtlusGfdEditor.FormatModules
{
    public class TextureDictionaryFormatModule : FormatModule<TextureDictionary>
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
