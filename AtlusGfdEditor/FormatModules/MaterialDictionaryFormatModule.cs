using System.IO;
using AtlusGfdLib;

namespace AtlusGfdEditor.FormatModules
{
    public class MaterialDictionaryFormatModule : FormatModule<MaterialDictionary>
    {
        public override string Name
            => "Material dictionary";

        public override string[] Extensions
            => new[] { "gmtd" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportCore( MaterialDictionary obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override MaterialDictionary ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<MaterialDictionary>( stream );
        }
    }
}
