using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using CSharpImageLibrary;

namespace AtlusGfdLib
{
    public static class TextureEncoder
    {
        public static Texture Encode( string name, TextureFormat format, Bitmap bitmap )
        {
            return Encode( name, format, 1, 1, 0, 0, bitmap );
        }

        public static Texture Encode( string name, TextureFormat format, byte field1c, byte field1d, byte field1e, byte field1f, Bitmap bitmap )
        {
            byte[] data = null;

            if ( format == TextureFormat.DDS )
            {
                var image = GetImageEngineImageFromBitmap( bitmap );
                var ddsFormat = DetermineBestDDSFormat( bitmap );
                data = image.Save( new ImageFormats.ImageEngineFormatDetails( ddsFormat ), MipHandling.GenerateNew );
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Texture( name, format, data, field1c, field1d, field1e, field1f );
        }

        private static ImageEngineImage GetImageEngineImageFromBitmap( Bitmap bitmap )
        {
            // save bitmap to stream
            var bitmapStream = new MemoryStream();
            bitmap.Save( bitmapStream, ImageFormat.Bmp );

            // create bitmap image
            return new ImageEngineImage( bitmapStream );
        }

        private static ImageEngineFormat DetermineBestDDSFormat( Bitmap bitmap )
        {
            var ddsFormat = ImageEngineFormat.DDS_DXT1;
            if ( HasTransparency( bitmap ) )
            {
                ddsFormat = ImageEngineFormat.DDS_DXT5;
            }

            return ddsFormat;
        }

        // https://stackoverflow.com/questions/3064854/determine-if-alpha-channel-is-used-in-an-image/39013496#39013496
        private static Boolean HasTransparency( Bitmap bitmap )
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
    }
}
