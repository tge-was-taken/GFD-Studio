using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using Microsoft.IO;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace CSharpImageLibrary
{
    /// <summary>
    /// Represents a mipmap of an image.
    /// </summary>
    public class MipMap
    {
        /// <summary>
        /// Pixels in bitmap image.
        /// </summary>
        public byte[] Pixels
        {
            get; set;
        }

        /// <summary>
        /// Size of mipmap in memory.
        /// </summary>
        public int UncompressedSize { get; private set; }

        /// <summary>
        /// Details of the format that this mipmap was created from.
        /// </summary>
        public ImageFormats.ImageEngineFormatDetails LoadedFormatDetails { get; private set; }

        /// <summary>
        /// Mipmap width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Mipmap height.
        /// </summary>
        public int Height { get; set; }


        /// <summary>
        /// Creates a Mipmap object from a WPF image.
        /// </summary>
        public MipMap(byte[] pixels, int width, int height, ImageFormats.ImageEngineFormatDetails details)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
            LoadedFormatDetails = details;

            UncompressedSize = ImageFormats.GetUncompressedSize(width, height, details.MaxNumberOfChannels, false);
        }


        /// <summary>
        /// Creates a WPF image from this mipmap.
        /// </summary>
        /// <returns>WriteableBitmap of image.</returns>
        public BitmapSource ToImage()
        {
            var tempPixels = ImageEngine.GetPixelsAsBGRA32(Width, Height, Pixels, LoadedFormatDetails);
            var bmp = UsefulThings.WPF.Images.CreateWriteableBitmap(tempPixels, Width, Height);
            bmp.Freeze();
            return bmp;
        }
    }
}
