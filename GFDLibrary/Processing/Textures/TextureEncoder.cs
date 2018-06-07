using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CSharpImageLibrary;
using GFDLibrary.IO.Utilities;

namespace GFDLibrary
{
    public static class TextureEncoder
    {
        public static Texture Encode( string name, TextureFormat format, Bitmap bitmap )
        {
            return Encode( name, format, 1, 1, 0, 0, bitmap );
        }

        public static Texture Encode( string name, TextureFormat format, byte field1C, byte field1D, byte field1E, byte field1F, Bitmap bitmap )
        {
            byte[] data = null;

            if ( format == TextureFormat.DDS )
            {
                var image = GetImageEngineImageFromBitmap( bitmap );
                var ddsFormat = DetermineBestDDSFormat( bitmap );
                data = image.Save( new ImageFormats.ImageEngineFormatDetails( ddsFormat ), MipHandling.GenerateNew, 0, 0, false );
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Texture( name, format, data, field1C, field1D, field1E, field1F );
        }

        private static ImageEngineImage GetImageEngineImageFromBitmap( Bitmap bitmap )
        {
            // save bitmap to stream
            var bitmapStream = new MemoryStream();
            bitmap.Save( bitmapStream, ImageFormat.Png );

            // create bitmap image
            return new ImageEngineImage( bitmapStream );
        }

        private static ImageEngineFormat DetermineBestDDSFormat( Bitmap bitmap )
        {
            var ddsFormat = ImageEngineFormat.DDS_DXT1;
            if ( BitmapUtilities.HasTransparency( bitmap ) )
            {
                ddsFormat = ImageEngineFormat.DDS_DXT3;
            }

            return ddsFormat;
        }
    }
}
