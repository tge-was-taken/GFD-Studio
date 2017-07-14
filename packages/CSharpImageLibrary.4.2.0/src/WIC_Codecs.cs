using CSharpImageLibrary.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UsefulThings;
using System.Windows.Media;
using System.Windows;

namespace CSharpImageLibrary
{
    /// <summary>
    /// Provides native Windows codec functionality for Windows 8.1+.
    /// </summary>
    public static class WIC_Codecs
    {
        /// <summary>
        /// JPG Quality setting. Higher values = higher quality and image size.
        /// </summary>
        public static int JPGCompressionSetting = 90;

        /// <summary>
        /// Tests whether Windows WIC Codecs are present.
        /// </summary>
        /// <returns>True if WIC Codecs available</returns>
        internal static bool WindowsCodecsPresent()
        {
            byte[] testData = Resources.DXT1_CodecTest;  // KFreon: Tiny test image in resources

            try
            {
                BitmapImage bmp = AttemptUsingWindowsCodecs(testData, 0, 0);
                bmp.Freeze();
                if (bmp == null)
                {
                    ImageEngine.WindowsWICCodecsAvailable = false;
                    return false;  // KFreon: Decoding failed. PROBABLY due to no decoding available
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                ImageEngine.WindowsWICCodecsAvailable = false;
                return false;  // KFreon: Non decoding related error - Who knows...
            }

            return true;
        }


        #region Loading
        /// <summary>
        /// Loads useful information from an image file.
        /// </summary>
        /// <param name="imageFile">Path to image file.</param>
        /// <param name="decodeWidth">Width to decode to. Aspect unchanged if decodeHeight = 0.</param>
        /// <param name="decodeHeight">Height to decode to. Aspect unchanged if decodeWidth = 0.</param>
        /// <param name="scale">DOMINANT. decodeWidth and decodeHeight ignored if this is > 0. Amount to scale by. Range 0-1.</param>
        /// <param name="formatDetails">Details about the format being loaded.</param>
        /// <param name="isDDS">True = Image is a DDS.</param>
        /// <returns>BGRA Pixel Data as stream.</returns>
        internal static List<MipMap> LoadWithCodecs(string imageFile, int decodeWidth, int decodeHeight, double scale, bool isDDS, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            if (isDDS && !ImageEngine.WindowsWICCodecsAvailable)
                return null;

            using (FileStream fs = new FileStream(imageFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LoadWithCodecs(fs, decodeWidth, decodeHeight, scale, isDDS, formatDetails);
        }


        /// <summary>
        /// Loads useful information from image stream using Windows 8.1+ codecs.
        /// </summary>
        /// <param name="stream">Stream containing entire file. NOT just pixels.</param>
        /// <param name="decodeWidth">Width to decode as. Aspect ratio unchanged if decodeHeight = 0.</param>
        /// <param name="decodeHeight">Height to decode as. Aspect ratio unchanged if decodeWidth = 0.</param>
        /// <param name="isDDS">True = image is a DDS.</param>
        /// <param name="scale">DOMINANT. DecodeWidth and DecodeHeight ignored if this is > 0. Amount to scale by. Range 0-1.</param>
        /// <param name="formatDetails">Details about the format being loaded.</param>
        /// <returns>BGRA Pixel Data as stream.</returns>
        internal static List<MipMap> LoadWithCodecs(Stream stream, int decodeWidth, int decodeHeight, double scale, bool isDDS, ImageFormats.ImageEngineFormatDetails formatDetails)
        {
            if (isDDS && !ImageEngine.WindowsWICCodecsAvailable)
                return null;

            bool alternateDecodeDimensions = decodeHeight != 0 || decodeWidth != 0 || scale != 0;
            int alternateWidth = decodeWidth;
            int alternateHeight = decodeHeight;

            List<MipMap> mipmaps = new List<MipMap>();

            if (isDDS)
            {
                // KFreon: Attempt to load any mipmaps
                stream.Seek(0, SeekOrigin.Begin);
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnDemand);

                // Setup alternateDimensions if required
                if (scale != 0)
                {
                    alternateHeight = (int)(decoder.Frames[0].Height * scale);
                    alternateWidth = (int)(decoder.Frames[0].Width * scale);
                }

                foreach (var mipmap in decoder.Frames)
                {
                    // KFreon: Skip mipmaps that are too big if asked to load a smaller image
                    if (alternateDecodeDimensions)
                    {
                        if ((alternateWidth != 0 && mipmap.Width > alternateWidth) || 
                            (alternateHeight != 0 && mipmap.Height > alternateHeight))
                            continue;
                    }

                    mipmaps.Add(new MipMap(mipmap.GetPixelsAsBGRA32(), mipmap.PixelWidth, mipmap.PixelHeight, formatDetails));
                }

                if (mipmaps.Count == 0)
                {
                    // KFreon: Image has no mips, so resize largest
                    var frame = decoder.Frames[0];
                    var mip = new MipMap(frame.GetPixelsAsBGRA32(), frame.PixelWidth, frame.PixelHeight, formatDetails);

                    // Calculate scale if required
                    if (scale == 0)
                    {
                        double xScale = alternateWidth * 1.0 / frame.PixelWidth;
                        double yScale = alternateHeight * 1.0 / frame.PixelHeight;

                        scale = xScale == 0 ? yScale : xScale;
                    }

                    mip = ImageEngine.Resize(mip, scale);
                    mipmaps.Add(mip);
                }
            }
            else
            {
                // KFreon: No Mipmaps
                BitmapImage bmp = AttemptUsingWindowsCodecs(stream, alternateWidth, alternateHeight);
                if (bmp == null)
                    return null;
                bmp.Freeze();

                mipmaps.Add(new MipMap(bmp.GetPixelsAsBGRA32(), bmp.PixelWidth, bmp.PixelHeight, formatDetails));
            }

            return mipmaps;
        }


        /// <summary>
        /// Attempts to read image using WIC Codecs.
        /// Returns null if unable to.
        /// </summary>
        /// <param name="imagePath">Path to image file.</param>
        /// <param name="decodeHeight">Height to decode to. 0 = no scaling.</param>
        /// <param name="decodeWidth">Width to decode to. 0 = no scaling.</param>
        /// <returns>Loaded Image</returns>
        private static BitmapImage AttemptUsingWindowsCodecs(string imagePath, int decodeWidth, int decodeHeight)
        {
            BitmapImage img = null;
            try
            {
                img = UsefulThings.WPF.Images.CreateWPFBitmap(imagePath, decodeWidth, decodeHeight);
            }
            catch (FileFormatException fileformatexception)
            {
                Debug.WriteLine(fileformatexception);
            }
            catch (NotSupportedException notsupportedexception)
            {
                Debug.WriteLine(notsupportedexception);
            }

            img.Freeze();
            return img;
        }


        /// <summary>
        /// Attempts to read image using WIC Codecs.
        /// Returns null if unable to.
        /// </summary>
        /// <param name="ImageFileData">Entire image file. NOT raw pixel data.</param>
        /// <param name="decodeWidth">Width to decode to. 0 = no scaling.</param>
        /// <param name="decodeHeight">Height to decode to. 0 = no scaling.</param>
        /// <returns>Loaded image.</returns>
        private static BitmapImage AttemptUsingWindowsCodecs(byte[] ImageFileData, int decodeWidth, int decodeHeight)
        {
            BitmapImage img = null;
            try
            {
                img = UsefulThings.WPF.Images.CreateWPFBitmap(ImageFileData, decodeWidth, decodeHeight);
            }
            catch (FileFormatException fileformatexception)
            {
                Debug.WriteLine(fileformatexception);
            }
            catch (NotSupportedException notsupportedexception)
            {
                Debug.WriteLine(notsupportedexception);
            }

            img.Freeze();
            return img;
        }


        /// <summary>
        /// Attempts to read image using WIC Codecs.
        /// Returns null if unable to.
        /// </summary>
        /// <param name="stream">Stream containing entire file. NOT raw pixels.</param>
        /// <param name="decodeWidth">Width to decode to. Aspect unchanged if decodeHeight = 0.</param>
        /// <param name="decodeHeight">Height to decode to. Aspect unchanged if decodeWidth = 0.</param>
        /// <returns>BitmapImage of stream.</returns>
        private static BitmapImage AttemptUsingWindowsCodecs(Stream stream, int decodeWidth, int decodeHeight)
        {
            BitmapImage img = null;
            try
            {
                img = UsefulThings.WPF.Images.CreateWPFBitmap(stream, decodeWidth, decodeHeight);
            }
            catch (FileFormatException fileformatexception)
            {
                Debug.WriteLine(fileformatexception);
            }
            catch (NotSupportedException notsupportedexception)
            {
                Debug.WriteLine(notsupportedexception);
            }

            img.Freeze();
            return img;
        }
        #endregion Loading


        internal static byte[] SaveWithCodecs(byte[] imageData, ImageEngineFormat format, int width, int height, AlphaSettings alphaSetting)
        {
            var image = UsefulThings.WPF.Images.CreateWriteableBitmap(imageData, width, height);
            image.Freeze();
            BitmapFrame frame = null;

            if (alphaSetting == AlphaSettings.RemoveAlphaChannel)
                frame = BitmapFrame.Create(new FormatConvertedBitmap(image, PixelFormats.Bgr32, image.Palette, 0));
            else if (format == ImageEngineFormat.BMP || format == ImageEngineFormat.PNG)
            {
                // Check if there's any alpha.
                bool anyAlpha = false;
                for (int i = 3; i < imageData.Length; i += 4)
                    if (imageData[i] != 255)
                    {
                        anyAlpha = true;
                        break;
                    }

                if (!anyAlpha)
                    frame = BitmapFrame.Create(new FormatConvertedBitmap(image, PixelFormats.Bgr32, image.Palette, 0));
            }
            else
                frame = BitmapFrame.Create(image);

            frame.Freeze();

            // KFreon: Choose encoder based on desired format.
            BitmapEncoder encoder = null;
            int estimatedImageSize = 0;

            int estimateHeaderSize = 1024;
            switch (format)
            {
                case ImageEngineFormat.BMP:
                    encoder = new BmpBitmapEncoder();
                    estimatedImageSize = estimateHeaderSize + width * height * 4;  // Fairly good estimation
                    break;
                case ImageEngineFormat.JPG:
                    encoder = new JpegBitmapEncoder();
                    ((JpegBitmapEncoder)encoder).QualityLevel = JPGCompressionSetting;
                    estimatedImageSize = estimateHeaderSize + width * height / 6;  // Estimation
                    break;
                case ImageEngineFormat.PNG:
                    encoder = new PngBitmapEncoder();
                    estimatedImageSize = estimateHeaderSize + width * height / 2;  // Estimation
                    break;
                case ImageEngineFormat.GIF:
                    encoder = new GifBitmapEncoder();
                    estimatedImageSize = estimateHeaderSize + width * height / 5;  // Estimation
                    break;
                case ImageEngineFormat.TIF:
                    encoder = new TiffBitmapEncoder();
                    estimatedImageSize = estimateHeaderSize + width * height; // Esimation
                    break;
                default:
                    throw new InvalidOperationException($"Unable to encode format: {format} using Windows 8.1 Codecs.");
            }

            encoder.Frames.Add(frame);
            using (MemoryStream ms = new MemoryStream(estimatedImageSize))  // Big enough to reduce memory copying.
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
                
        }
    }
}
