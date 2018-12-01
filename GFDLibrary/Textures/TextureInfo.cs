using System;
using GFDLibrary.Textures.DDS;

namespace GFDLibrary.Textures
{
    public class TextureInfo
    {
        public Texture Texture { get; }

        public string Name { get; }

        public int Width { get; }

        public int Height { get; }

        public TexturePixelFormat PixelFormat { get; }

        public int MipMapCount { get; }

        internal TextureInfo( Texture texture, int width, int height, int mipMapCount, TexturePixelFormat format )
        {
            Texture = texture;
            Name = texture.Name;
            Width = width;
            Height = height;
            PixelFormat = format;
            MipMapCount = mipMapCount;
        }

        public static TextureInfo GetTextureInfo( Texture texture )
        {
            // Load DDS header so we can extract some info from it
            var ddsHeader = new DDSHeader( texture.Data );

            TexturePixelFormat format;
            switch ( ddsHeader.PixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.DXT1:
                    format = TexturePixelFormat.BC1;
                    break;
                case DDSPixelFormatFourCC.DXT3:
                    format = TexturePixelFormat.BC2;
                    break;
                case DDSPixelFormatFourCC.DXT5:
                    format = TexturePixelFormat.BC3;
                    break;
                case DDSPixelFormatFourCC.A8R8G8B8:
                    format = TexturePixelFormat.ARGB;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new TextureInfo( texture, ddsHeader.Width, ddsHeader.Height, ddsHeader.MipMapCount, format );
        }
    }
}