using System.IO;
using GFDStudio.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Bitmap = System.Drawing.Bitmap;

namespace GFDStudio.FormatModules
{
    public class BitmapFormatModule : FormatModule<Image<Rgba32>>
    {
        public override string Name =>
            "Bitmap";

        public override string[] Extensions =>
            new[] { "png", "bmp" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Bitmap;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return PathUtilities.MatchesAnyExtension( filename, Extensions );
        }

        protected override void ExportCore( Image<Rgba32> obj, Stream stream, string filename = null )
        {
            // The encoder comes from the file extension, with PNG for an unknown or absent one.
            var manager = Configuration.Default.ImageFormatsManager;
            var format = filename != null ? manager.FindFormatByFileExtension( Path.GetExtension( filename ) ) : null;

            obj.Save( stream, format != null ? manager.FindEncoder( format ) : new PngEncoder() );
        }

        protected override Image<Rgba32> ImportCore( Stream stream, string filename = null )
        {
            return Image.Load<Rgba32>( stream );
        }

        protected override Bitmap GetBitmapCore( Image<Rgba32> obj )
        {
            return obj.ToBitmap();
        }
    }
}
