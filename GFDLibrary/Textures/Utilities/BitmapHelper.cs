using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GFDLibrary.Textures.Utilities
{
    public static class BitmapHelper
    {   
        // https://stackoverflow.com/questions/3064854/determine-if-alpha-channel-is-used-in-an-image/39013496#39013496
        public static Boolean HasTransparency( Bitmap bitmap )
        {
            // not an alpha-capable color format.
            if ( ( bitmap.Flags & ( Int32 )ImageFlags.HasAlpha ) == 0 )
                return false;
            // Indexed formats. Special case because one index on their palette is configured as THE transparent color.
            if ( bitmap.PixelFormat == PixelFormat.Format8bppIndexed || bitmap.PixelFormat == PixelFormat.Format4bppIndexed )
            {
                ColorPalette pal = bitmap.Palette;
                // Find the transparent index on the palette.
                Int32 transCol = -1;
                for ( int i = 0; i < pal.Entries.Length; i++ )
                {
                    Color col = pal.Entries[i];
                    if ( col.A != 255 )
                    {
                        // Color palettes should only have one index acting as transparency. Not sure if there's a better way of getting it...
                        transCol = i;
                        break;
                    }
                }
                // none of the entries in the palette have transparency information.
                if ( transCol == -1 )
                    return false;
                // Check pixels for existence of the transparent index.
                Int32 colDepth = Image.GetPixelFormatSize( bitmap.PixelFormat );
                BitmapData data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, bitmap.PixelFormat );
                Int32 stride = data.Stride;
                Byte[] bytes = new Byte[bitmap.Height * stride];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                if ( colDepth == 8 )
                {
                    // Last line index.
                    Int32 lineMax = bitmap.Width - 1;
                    for ( Int32 i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        Byte b = bytes[i];
                        if ( b == transCol )
                            return true;
                    }
                }
                else if ( colDepth == 4 )
                {
                    // line size in bytes. 1-indexed for the moment.
                    Int32 lineMax = bitmap.Width / 2;
                    // Check if end of line ends on half a byte.
                    Boolean halfByte = bitmap.Width % 2 != 0;
                    // If it ends on half a byte, one more needs to be processed.
                    // We subtract in the other case instead, to make it 0-indexed right away.
                    if ( !halfByte )
                        lineMax--;
                    for ( Int32 i = 0; i < bytes.Length; i++ )
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if ( linepos > lineMax )
                            continue;
                        Byte b = bytes[i];
                        if ( ( b & 0x0F ) == transCol )
                            return true;
                        if ( halfByte && linepos == lineMax ) // reached last byte of the line. If only half a byte to check on that, abort and go on with loop.
                            continue;
                        if ( ( ( b & 0xF0 ) >> 4 ) == transCol )
                            return true;
                    }
                }
                return false;
            }
            if ( bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb )
            {
                BitmapData data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, bitmap.PixelFormat );
                Byte[] bytes = new Byte[bitmap.Height * data.Stride];
                Marshal.Copy( data.Scan0, bytes, 0, bytes.Length );
                bitmap.UnlockBits( data );
                for ( Int32 p = 3; p < bytes.Length; p += 4 )
                {
                    if ( bytes[p] != 255 )
                        return true;
                }
                return false;
            }
            // Final "screw it all" method. This is pretty slow, but it won't ever be used, unless you
            // encounter some really esoteric types not handled above, like 16bppArgb1555 and 64bppArgb.
            for ( Int32 i = 0; i < bitmap.Width; i++ )
            {
                for ( Int32 j = 0; j < bitmap.Height; j++ )
                {
                    if ( bitmap.GetPixel( i, j ).A != 255 )
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a new <see cref="Bitmap"/> instance using an array of <see cref="Color"/> pixels and the image width and height.
        /// </summary>
        /// <param name="colors"><see cref="GFDLibrary.Graphics.Color"/> array containing the color of each pixel in the image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>A new <see cref="Bitmap"/> instance created using the data provided.</returns>
        public static Bitmap Create( Graphics.Color[] colors, int width, int height )
        {
            var bitmap = new Bitmap( width, height, PixelFormat.Format32bppArgb );
            var bitmapData = bitmap.LockBits
                (
                 new Rectangle( 0, 0, width, height ),
                 ImageLockMode.ReadWrite,
                 bitmap.PixelFormat
                );

            unsafe
            {
                byte* p = ( byte* )bitmapData.Scan0;
                Parallel.For( 0, height, y =>
                {
                    for ( int x = 0; x < width; x++ )
                    {
                        int   offset = ( x * 4 ) + y * bitmapData.Stride;
                        var color  = colors[x + y * width];
                        p[offset]     = color.B;
                        p[offset + 1] = color.G;
                        p[offset + 2] = color.R;
                        p[offset + 3] = color.A;
                    }
                } );
            }

            bitmap.UnlockBits( bitmapData );

            return bitmap;
        }
    }
}
