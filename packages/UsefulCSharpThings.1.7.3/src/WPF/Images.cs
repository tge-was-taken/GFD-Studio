using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides functions to work with WPF Images.
    /// </summary>
    public static class Images
    {
        #region Bitmaps
        /// <summary>
        /// Overlays one image on top of another.
        /// Both images MUST be the same size.
        /// </summary>
        /// <param name="source">Base image i.e. the "bottom" one.</param>
        /// <param name="overlay">Overlaying image i.e. the "top" one.</param>
        /// <returns>New image with overlay on top of source.</returns>
        public static BitmapSource Overlay(BitmapSource source, BitmapSource overlay)
        {
            if (source.Width != overlay.Width || source.Height != overlay.Width)
                throw new InvalidDataException("Source and overlay must be the same dimensions.");

            var drawing = new DrawingVisual();
            var context = drawing.RenderOpen();
            context.DrawImage(source, new System.Windows.Rect(0, 0, source.Width, source.Height));
            context.DrawImage(overlay, new System.Windows.Rect(0, 0, overlay.Width, overlay.Height));

            context.Close();
            var overlayed = new RenderTargetBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format);
            overlayed.Render(drawing);

            return overlayed;
        }


        #region Creation
        /// <summary>
        /// Creates a WriteableBitmap from an array of pixels.
        /// </summary>
        /// <param name="pixels">Pixel data</param>
        /// <param name="width">Width of image</param>
        /// <param name="height">Height of image</param>
        /// <param name="format">Defines how pixels are layed out.</param>
        /// <returns>WriteableBitmap containing pixels</returns>
        public static WriteableBitmap CreateWriteableBitmap(Array pixels, int width, int height, PixelFormat format)
        {
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, format, BitmapPalettes.Halftone256Transparent);
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, wb.BackBufferStride, 0);
            return wb;
        }

        /// <summary>
        /// Creates a WriteableBitmap from an array of pixels with the default BGRA32 pixel format.
        /// </summary>
        /// <param name="pixels">Pixel data</param>
        /// <param name="width">Width of image</param>
        /// <param name="height">Height of image</param>
        /// <returns>WriteableBitmap containing pixels</returns>
        public static WriteableBitmap CreateWriteableBitmap(Array pixels, int width, int height)
        {
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, wb.BackBufferStride, 0);
            return wb;
        }


        /// <summary>
        /// Creates a WPF style Bitmap (i.e. not using the System.Drawing.Bitmap)
        /// </summary>
        /// <param name="source">Stream containing bitmap data. NOTE fully formatted bitmap file, not just data.</param>
        /// <param name="cacheOption">Determines how/when image data is cached. Default is "Cache to memory on load."</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="DisposeStream">True = dispose of parent stream.</param>
        /// <returns>Bitmap from stream.</returns>
        public static BitmapImage CreateWPFBitmap(Stream source, int decodeWidth = 0, int decodeHeight = 0, BitmapCacheOption cacheOption = BitmapCacheOption.OnLoad, bool DisposeStream = false)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.DecodePixelHeight = decodeHeight;

            // KFreon: Rewind stream to start
            source.Seek(0, SeekOrigin.Begin);
            bmp.StreamSource = source;
            bmp.CacheOption = cacheOption;
            bmp.EndInit();
            bmp.Freeze();  // Allows use across threads somehow (seals memory I'd guess)

            if (DisposeStream)
                source.Dispose();

            return bmp;
        }


        /// <summary>
        /// Creates WPF Bitmap from byte array.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in byte[]</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage object.</returns>
        public static BitmapImage CreateWPFBitmap(byte[] source, int decodeWidth = 0, int decodeHeight = 0)
        {
            using (MemoryStream ms = new MemoryStream(source))  
                return CreateWPFBitmap(ms, decodeWidth, decodeHeight);
        }


        /// <summary>
        /// Creates WPF Bitmap from List of bytes.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in List of bytes.</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage of source data.</returns>
        public static BitmapImage CreateWPFBitmap(List<byte> source, int decodeWidth = 0, int decodeHeight = 0)
        {
            byte[] newsource = source.ToArray(source.Count);
            return CreateWPFBitmap(newsource, decodeWidth, decodeHeight);
        }


        /// <summary>
        /// Creates WPF Bitmap from a file.
        /// </summary>
        /// <param name="Filename">Path to file.</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage based on file.</returns>
        public static BitmapImage CreateWPFBitmap(string Filename, int decodeWidth = 0, int decodeHeight = 0)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.DecodePixelHeight = decodeHeight;
            bmp.UriSource = new Uri(Filename);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }


        /// <summary>
        /// Creates WPF BitmapSource from a GDI Bitmap.
        /// Bitmap MUST STAY ALIVE for the life of this BitmapSource.
        /// </summary>
        /// <param name="GDIBitmap">Bitmap to convert.</param>
        /// <returns>BitmapSource of GDIBitmap</returns>
        public static BitmapSource CreateWPFBitmap(System.Drawing.Bitmap GDIBitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(GDIBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }


        /// <summary>
        /// Creates a WPF bitmap from another BitmapSource.
        /// </summary>
        /// <param name="source">Image source to create from.</param>
        /// <param name="decodeWidth">Width to decode to.</param>
        /// <param name="decodeHeight">Height to decode to.</param>
        /// <returns>BitmapImage of source</returns>
        public static BitmapImage CreateWPFBitmap(BitmapSource source, int decodeWidth = 0, int decodeHeight = 0)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(ms);

            return CreateWPFBitmap(ms, decodeWidth, decodeHeight, DisposeStream: true);
        }
        #endregion

        /// <summary>
        /// Witchcraft I got off the internet that makes manual resizing work.
        /// I apologise to whomever I got this off, I've forgotten who and where.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static double BiCubicKernel(double x)
        {
            if (x < 0)
            {
                x = -x;
            }

            double bicubicCoef = 0;

            if (x <= 1)
            {
                bicubicCoef = (1.5 * x - 2.5) * x * x + 1;
            }
            else if (x < 2)
            {
                bicubicCoef = ((-0.5 * x + 2.5) * x - 4) * x + 2;
            }

            return bicubicCoef;
        }


        // NOT MINE. Got it from a website I can't remember. Similar to the WriteableBitmapEx code.
        [Obsolete("Shouldn't be required. Just load into BitmapImage with decodeWidth/Height parameters.")]
        static BitmapSource ManualResize(WriteableBitmap source, int width, int height)
        {
            int sourceWidth = source.PixelWidth;
            int sourceHeight = source.PixelHeight;

            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);



            byte[] destination = new byte[width * height * 4];
            double heightFactor = sourceWidth / (double)width;
            double widthFactor = sourceHeight / (double)height;

            // Coordinates of source points
            double ox, oy, dx, dy, k1, k2;
            int ox1, oy1, ox2, oy2;

            // Width and height decreased by 1
            int maxHeight = sourceHeight - 1;
            int maxWidth = sourceWidth - 1;

            for (int y = 0; y < height; y++)
            {
                // Y coordinates
                oy = (y * widthFactor) - 0.5;

                oy1 = (int)oy;
                dy = oy - oy1;

                for (int x = 0; x < width; x++)
                {
                    // X coordinates
                    ox = (x * heightFactor) - 0.5f;
                    ox1 = (int)ox;
                    dx = ox - ox1;

                    // Destination color components
                    double r = 0;
                    double g = 0;
                    double b = 0;
                    double a = 0;

                    for (int n = -1; n < 3; n++)
                    {
                        // Get Y cooefficient
                        k1 = BiCubicKernel(dy - n);

                        oy2 = oy1 + n;
                        if (oy2 < 0)
                        {
                            oy2 = 0;
                        }

                        if (oy2 > maxHeight)
                        {
                            oy2 = maxHeight;
                        }

                        for (int m = -1; m < 3; m++)
                        {
                            // Get X cooefficient
                            k2 = k1 * BiCubicKernel(m - dx);

                            ox2 = ox1 + m;
                            if (ox2 < 0)
                            {
                                ox2 = 0;
                            }

                            if (ox2 > maxWidth)
                            {
                                ox2 = maxWidth;
                            }

                            int index = oy2 * stride + 4 * ox2;
                            Color color = Color.FromArgb(pixels[index + 3], pixels[index], pixels[index + 1], pixels[index + 2]);

                            r += k2 * color.R;
                            g += k2 * color.G;
                            b += k2 * color.B;
                            a += k2 * color.A;
                        }
                    }

                    int destIndex = y * 4 * width + 4 * x;
                    destination[destIndex + 3] = ToByte(a);
                    destination[destIndex] = ToByte(r);
                    destination[destIndex + 1] = ToByte(g);
                    destination[destIndex + 2] = ToByte(b);
                }
            }

            WriteableBitmap resized = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            resized.WritePixels(new Int32Rect(0, 0, width, height), destination, width * 4, 0);
            return resized;
        }

        internal static byte ToByte(double value)
        {
            return Convert.ToByte(Clamp(value, 0, 255));
        }

        internal static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }

            if (value.CompareTo(max) > 0)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Saves WPF bitmap to disk as a JPG.
        /// </summary>
        /// <param name="img">Image to save.</param>
        /// <param name="Destination">Path to save to.</param>
        public static void SaveWPFBitmapToDiskAsJPG(BitmapImage img, string Destination)
        {
            using (FileStream fs = new FileStream(Destination, FileMode.CreateNew))
                SaveWPFBitmapToStreamAsJPG(img, fs);
        }


        /// <summary>
        /// Saves image as JPG to stream.
        /// </summary>
        /// <param name="img">Image to save.</param>
        /// <param name="stream">Destination stream.</param>
        public static void SaveWPFBitmapToStreamAsJPG(BitmapImage img, Stream stream)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 96; // KFreon: Chosen arbitrarily.
            encoder.Frames.Add(BitmapFrame.Create(img, null, null, null));
            encoder.Save(stream);
        }
        #endregion


        /// <summary>
        /// Converts an old GDI Palette to the new WPF BitmapPalette
        /// </summary>
        /// <param name="GDIPalette">Old palette to convert.</param>
        /// <returns>New WPF Palette.</returns>
        public static BitmapPalette ConvertGDIPaletteToWPF(System.Drawing.Imaging.ColorPalette GDIPalette)
        {
            List<Color> Colours = new List<Color>();
            foreach(var colour in GDIPalette.Entries)
                Colours.Add(Color.FromArgb(colour.A, colour.R, colour.G, colour.B));

            return Colours.Count > 0 ? new BitmapPalette(Colours) : BitmapPalettes.Halftone256Transparent;
        }
    }
}
