using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.Modules
{
    public class MaterialModule : Module<Material>
    {
        public override string Name
            => "Material";

        public override string[] Extensions
            => new[] { "gmt" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Import;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Resource.IsValidResource( stream );
        }

        protected override void ExportCore( Material obj, Stream stream, string filename = null )
        {
            Resource.Save( obj, stream );
        }

        protected override Material ImportCore( Stream stream, string filename = null )
        {
            return Resource.Load< Material >( stream );
        }
    }
}
