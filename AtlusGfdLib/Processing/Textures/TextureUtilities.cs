using System;

namespace AtlusGfdLibrary
{
    public static class TextureUtilities
    {
        public static byte[] GetRawPixelData( Texture texture )
        {
            var ddsPixelData = new byte[texture.Data.Length - 0x80];
            Array.Copy( texture.Data, 0x80, ddsPixelData, 0, ddsPixelData.Length );

            return ddsPixelData;
        }
    }
}
