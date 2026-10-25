using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GFDLibrary.Graphics;

namespace GFDStudio.Avalonia
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts the library's images for Avalonia, which wants premultiplied BGRA.
        /// </summary>
        public static Bitmap ToAvaloniaBitmap( this Image image )
        {
            var pixels = image.ToBgra();
            var handle = System.Runtime.InteropServices.GCHandle.Alloc( pixels, System.Runtime.InteropServices.GCHandleType.Pinned );

            try
            {
                return new Bitmap( PixelFormat.Bgra8888, AlphaFormat.Unpremul,
                                   handle.AddrOfPinnedObject(),
                                   new PixelSize( image.Width, image.Height ),
                                   new Vector( 96, 96 ),
                                   image.Width * 4 );
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
