using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib;

namespace AtlusGfdEditor.Modules
{
    public class TextureModule : Module<Texture>
    {
        public override string Name
            => "Atlus Gfd Texture";

        public override string[] Extensions
            => new[] { "gtx" };

        public override FormatModuleUsageFlags UsageFlags
            => FormatModuleUsageFlags.Bitmap;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override void ExportCore( Texture obj, Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override Texture ImportCore( Stream stream, string filename = null )
        {
            throw new NotImplementedException();
        }

        protected override Bitmap GetBitmapCore( Texture obj )
        {
            return TextureDecoder.Decode( obj );
        }
    }
}
