using System;
using GFDLibrary.Textures.DDS;
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

            return new Texture( name, format, DDSCodec.CompressImage( image ), field1C, field1D, field1E, field1F );
        }
    }
}
