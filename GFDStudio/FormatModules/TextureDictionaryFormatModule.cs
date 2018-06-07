using System.IO;
using GFDLibrary;

namespace GFDStudio.FormatModules
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
            return Resource.GetResourceType( stream ) == ResourceType.TextureDictionary;
        }

        protected override void ExportCore( TextureDictionary obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override TextureDictionary ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load< TextureDictionary >( stream );
        }
    }
}
