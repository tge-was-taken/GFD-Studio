using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;

namespace GFDLibrary
{
    public static class TextureDecoder
    {
        public static Bitmap Decode( Texture texture )
        {
            return Decode( texture.Data, texture.Format );
        }

        public static Bitmap Decode( FieldTexture texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            var ddsImage = new ImageEngineImage( ddsBytes );
            return ImageEngineImageToBitmap( ddsImage );
        }

        public static byte[] DecodeToDDS( FieldTexture texture )
        {
            var surfaceFormat = ImageEngineFormat.DDS_DXT1;
            if ( texture.Flags.HasFlag( FieldTextureFlags.DXT3 ) )
            {
                surfaceFormat = ImageEngineFormat.DDS_DXT3;
            }
            else if ( texture.Flags.HasFlag( FieldTextureFlags.DXT5 ) )
            {
                surfaceFormat = ImageEngineFormat.DDS_DXT5;
            }

            var ddsBytes = new byte[0x80 + texture.DataLength];

            // create & write header
            var ddsHeader = new DDS_Header( texture.MipMapCount, texture.Height, texture.Width, surfaceFormat );
            ddsHeader.WriteToArray( ddsBytes, 0 );

            // write pixel data
            Array.Copy( texture.Data, 0, ddsBytes, 0x80, texture.DataLength );

            return ddsBytes;
        }

        public static Bitmap Decode( byte[] data, TextureFormat format )
        {
            Bitmap bitmap;

            if ( format == TextureFormat.DDS )
            {
                try
                {
                    // load image from dds data
                    var image = new ImageEngineImage( data );
                    bitmap = ImageEngineImageToBitmap( image );
                }
                catch ( Exception )
                {
                    // Bug: ImagineEngine randomly crashes? Seems to happen with P3D/P5D files only.
                    // Seems like it doesn't support some configuration
                    bitmap = new Bitmap( 32, 32, PixelFormat.Format32bppArgb );
                    Trace.WriteLine( "ImageEngine failed to decode DDS texture" );
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            return bitmap;
        }

        private static Bitmap ImageEngineImageToBitmap( ImageEngineImage image )
        {
            // save the image to bmp
            var bitmapStream = new MemoryStream();
            image.Save( bitmapStream, new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.BMP ), MipHandling.KeepTopOnly );

            // load the saved bmp into a new bitmap
            return new Bitmap( bitmapStream );
        }
    }
}
