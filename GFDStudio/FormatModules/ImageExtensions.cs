using GFDLibrary.Graphics;
using Bitmap = System.Drawing.Bitmap;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Bridges the library's images to the WinForms controls, which need a Bitmap.
    /// </summary>
    public static class ImageExtensions
    {
        public static Bitmap ToBitmap( this Image image )
        {
            var bitmap = new Bitmap( image.Width, image.Height, PixelFormat.Format32bppArgb );
            var bitmapData = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, image.Width, image.Height ),
                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

            // Format32bppArgb is BGRA in memory.
            var pixels = image.ToBgra();
            System.Runtime.InteropServices.Marshal.Copy( pixels, 0, bitmapData.Scan0, pixels.Length );

            bitmap.UnlockBits( bitmapData );

            return bitmap;
        }
    }
}
