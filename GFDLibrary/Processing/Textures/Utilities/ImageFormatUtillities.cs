using System;
using System.Drawing.Imaging;
using System.IO;

namespace GFDLibrary.IO.Utilities
{
    public static class ImageFormatHelper
    {
        public static ImageFormat GetImageFormatFromPath( string path )
        {
            var extension = Path.GetExtension( path ).ToLowerInvariant();

            switch ( extension )
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
