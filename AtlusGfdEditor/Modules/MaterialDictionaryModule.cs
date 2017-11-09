using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.Modules
{
    public class MaterialDictionaryModule : Module<MaterialDictionary>
    {
        public override string Name
            => "Material dictionary";

        public override string[] Extensions
            => new[] { "gmtd" };

        public override FormatModuleUsageFlags UsageFlags
            => 0;

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
            var resource = Resource.Load<Model>( stream );
            return resource.MaterialDictionary;
        }
    }
}
