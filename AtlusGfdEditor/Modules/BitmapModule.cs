using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib.IO;

namespace AtlusGfdEditor.Modules
{
    public class BitmapModule : Module<Bitmap>
    {
        public override string Name =>
            "Bitmap";

        public override string[] Extensions =>
            new[] { "png", "bmp" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.Import | FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Bitmap;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return Utilities.MatchesAnyExtension( filename, Extensions );
        }

        protected override void ExportCore( Bitmap obj, Stream stream, string filename = null )
        {
            obj.Save( stream, filename != null ? ImageFormatHelper.GetImageFormatFromPath(filename) : ImageFormat.Png );
        }

        protected override Bitmap ImportCore( Stream stream, string filename = null )
        {
            return new Bitmap( stream );
        }

        protected override Bitmap GetBitmapCore( Bitmap obj )
        {
            return obj;
        }
    }
}
