using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GFDLibrary.Textures.Utilities;
using DirectXTexNet;
using System.Runtime.CompilerServices;
using GFDLibrary.Textures.DDS;

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
                data = DDSCodec.Compress( bitmap, DXGIFormat.UNKNOWN ).Data;
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Texture( name, format, data, field1C, field1D, field1E, field1F );
        }
    }
}
