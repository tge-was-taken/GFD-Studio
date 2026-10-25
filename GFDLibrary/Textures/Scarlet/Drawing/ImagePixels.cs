using System;

namespace Scarlet.Drawing
{
    /// <summary>
    /// Decoded image pixels, 4 bytes per pixel in B, G, R, A order and no row padding.
    /// </summary>
    public class ImagePixels
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] Data { get; }

        public ImagePixels(int width, int height, byte[] data)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < width * height * 4) throw new ArgumentException("Pixel data is shorter than the image", nameof(data));

            Width = width;
            Height = height;
            Data = data;
        }
    }
}
