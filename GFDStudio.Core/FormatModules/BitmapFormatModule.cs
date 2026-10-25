using System.IO;
using GFDLibrary.Graphics;
using GFDLibrary.Textures;
using GFDStudio.IO;

namespace GFDStudio.FormatModules
{
    public class BitmapFormatModule : FormatModule<Image>
    {
        public override string Name =>
            "Bitmap";

        public override string[] Extensions =>
            new[] { "png" };

        public override FormatModuleUsageFlags UsageFlags =>
             FormatModuleUsageFlags.ImportForEditing | FormatModuleUsageFlags.Export | FormatModuleUsageFlags.Image;

        protected override bool CanImportCore( Stream stream, string filename = null )
        {
            return PathUtilities.MatchesAnyExtension( filename, Extensions );
        }

        protected override void ExportCore( Image obj, Stream stream, string filename = null )
        {
            PngCodec.Save( obj, stream );
        }

        protected override Image ImportCore( Stream stream, string filename = null )
        {
            return PngCodec.Load( stream );
        }

        protected override Image GetImageCore( Image obj )
        {
            return obj;
        }
    }
}
