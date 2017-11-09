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
            => 0;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return false;
        }

        protected override void ExportCore( Material obj, Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override Material ImportCore( Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }
    }
}
