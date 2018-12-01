using System;

namespace GFDLibrary.Textures
{
    public static class TextureUtilities
    {
        public static byte[] GetPixelData( Texture texture )
        {
            var ddsPixelData = new byte[texture.Data.Length - 0x80];
            Array.Copy( texture.Data, 0x80, ddsPixelData, 0, ddsPixelData.Length );

            return ddsPixelData;
        }
    }
}
