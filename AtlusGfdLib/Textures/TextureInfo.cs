namespace AtlusGfdLib
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
    }
}