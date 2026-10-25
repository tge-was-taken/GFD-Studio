using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;

namespace GFDStudio.FormatModules
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
            return Resource.GetResourceType( stream ) == ResourceType.MaterialDictionary;
        }

        protected override void ExportCore( MaterialDictionary obj, Stream stream, string filename = null )
        {
            obj.Save( stream, true );
        }

        protected override MaterialDictionary ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load<MaterialDictionary>( stream );
        }
    }
}
