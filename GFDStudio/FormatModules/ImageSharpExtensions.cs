using GFDLibrary.Textures.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Bitmap = System.Drawing.Bitmap;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace GFDStudio.FormatModules
{
    /// <summary>
    /// Bridges the library's ImageSharp images to the WinForms controls, which need a Bitmap.
    /// </summary>
    public static class ImageSharpExtensions
    {
        public static Bitmap ToBitmap( this Image<Rgba32> image )
        {
            var bitmap = new Bitmap( image.Width, image.Height, PixelFormat.Format32bppArgb );
            var bitmapData = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, image.Width, image.Height ),
                                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

            var pixels = ImageHelper.ToBgra( image );
            System.Runtime.InteropServices.Marshal.Copy( pixels, 0, bitmapData.Scan0, pixels.Length );

            bitmap.UnlockBits( bitmapData );

            return bitmap;
        }

        public static Image<Rgba32> LoadImage( string path )
        {
            return Image.Load<Rgba32>( path );
        }

        /// <summary>
        /// Saves to a path, letting ImageSharp pick the encoder from the extension.
        /// </summary>
        public static void SaveImage( this Image<Rgba32> image, string path )
        {
            image.Save( path );
        }
    }
}
