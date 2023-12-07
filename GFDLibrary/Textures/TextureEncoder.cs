using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BCnEncoder.Encoder;
using CSharpImageLibrary;
using GFDLibrary.Textures.Utilities;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace GFDLibrary.Textures
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
                BcEncoder encoder = new BcEncoder
                {
                    OutputOptions =
                    {
                        GenerateMipMaps = true,
                        Quality = CompressionQuality.BestQuality,
                        Format = BitmapHelper.HasTransparency(bitmap) ? CompressionFormat.Bc3 : CompressionFormat.Bc1,
                        FileFormat = OutputFileFormat.Dds
                    }
                };

                using MemoryStream pngstream = new MemoryStream();
                bitmap.Save( pngstream, ImageFormat.Png );
                pngstream.Seek( 0, SeekOrigin.Begin );

                using Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>( pngstream );
                using MemoryStream datastream = new MemoryStream();
                encoder.EncodeToStream( image, datastream );
                data = datastream.ToArray();
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
            if ( BitmapHelper.HasTransparency( bitmap ) )
            {
                ddsFormat = ImageEngineFormat.DDS_DXT5;
            }

            return ddsFormat;
        }
    }
}
