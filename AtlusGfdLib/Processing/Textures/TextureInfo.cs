using System;
using System.IO;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;

namespace AtlusGfdLibrary
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
            var ddsHeader = new DDS_Header( new MemoryStream( texture.Data ) );

            TexturePixelFormat format;
            switch ( ddsHeader.Format )
            {
                case ImageEngineFormat.DDS_DXT1:
                    format = TexturePixelFormat.DXT1;
                    break;
                case ImageEngineFormat.DDS_DXT3:
                    format = TexturePixelFormat.DXT3;
                    break;
                case ImageEngineFormat.DDS_DXT5:
                    format = TexturePixelFormat.DXT5;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new TextureInfo( texture, ddsHeader.Width, ddsHeader.Height, ddsHeader.dwMipMapCount, format );
        }
    }
}