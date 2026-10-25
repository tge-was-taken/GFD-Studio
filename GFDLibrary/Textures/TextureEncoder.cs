using System;
using System.IO;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using GFDLibrary.Textures.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GFDLibrary.Textures
{
    public static class TextureEncoder
    {
        public static Texture Encode( string name, TextureFormat format, Image<Rgba32> image )
        {
            return Encode( name, format, 1, 1, 0, 0, image );
        }

        public static Texture Encode( string name, TextureFormat format, byte field1C, byte field1D, byte field1E, byte field1F, Image<Rgba32> image )
        {
            if ( format != TextureFormat.DDS )
                throw new NotImplementedException();

            var encoder = new BcEncoder
            {
                OutputOptions =
                {
                    GenerateMipMaps = true,
                    Quality = CompressionQuality.BestQuality,
                    Format = ImageHelper.HasTransparency( image ) ? CompressionFormat.Bc3 : CompressionFormat.Bc1,
                    FileFormat = OutputFileFormat.Dds
                }
            };

            using var stream = new MemoryStream();
            encoder.EncodeToStream( image, stream );

            return new Texture( name, format, stream.ToArray(), field1C, field1D, field1E, field1F );
        }
    }
}
