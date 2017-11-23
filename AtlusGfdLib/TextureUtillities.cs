using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;

namespace AtlusGfdLib
{
    public static class TextureUtillities
    {
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

        public static byte[] GetRawPixelData( Texture texture )
        {
            var ddsPixelData = new byte[texture.Data.Length - 0x80];
            Array.Copy( texture.Data, 0x80, ddsPixelData, 0, ddsPixelData.Length );

            return ddsPixelData;
        }
    }
}
