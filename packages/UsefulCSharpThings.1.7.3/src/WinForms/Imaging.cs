using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WinForms
{
    /// <summary>
    /// Collection of Image functions
    /// </summary>
    public static class Imaging
    {
        /// <summary>
        /// Pads a non-square image by adding whitespace where necessary.
        /// </summary>
        /// <param name="filename">Image location.</param>
        /// <param name="maxDimension">Largest size to display.</param>
        /// <returns>Square bitmap.</returns>
        public static Bitmap PadImageToSquare(string filename, int maxDimension)
        {
            using (Image bmp = Image.FromFile(filename))
                return PadImageToSquare(bmp, maxDimension);
        }


        /// <summary>
        /// Pads a non-square image by adding whitespace where necessary.
        /// </summary>
        /// <param name="image">Image to make square.</param>
        /// <param name="maxDimension">Largest size to display.</param>
        /// <returns>Square bitmap.</returns>
        public static Bitmap PadImageToSquare(Image image, int maxDimension)
        {
            int tw, th, tx, ty;
            int w = image.Width;
            int h = image.Height;
            double whRatio = (double)w / h;
            if (image.Width >= image.Height)
            {
                tw = maxDimension;
                th = (int)(tw / whRatio);
            }
            else
            {
                th = maxDimension;
                tw = (int)(th * whRatio);
            }
            tx = (maxDimension - tw) / 2;
            ty = (maxDimension - th) / 2;
            Bitmap thumb = new Bitmap(maxDimension, maxDimension, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(thumb);
            g.Clear(Color.White);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, new Rectangle(tx, ty, tw, th), new Rectangle(0, 0, w, h), GraphicsUnit.Pixel);
            g.Dispose();
            return thumb;
        }

        /// <summary>
        /// Creates Bitmap from pixels.
        /// </summary>
        /// <param name="pixels">Array of pixels.</param>
        /// <param name="Width">Width of image.</param>
        /// <param name="Height">Height of image.</param>
        /// <returns>Bitmap containing pixels.</returns>
        public static Bitmap CreateBitmap(byte[] pixels, int Width, int Height)
        {
            var rect = new Rectangle(0, 0, Width, Height);
            Bitmap bmp = new Bitmap(Width, Height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            bmp.UnlockBits(data);

            return bmp;
        }


        /// <summary>
        /// Creates a GDI bitmap from a WPF bitmap.
        /// </summary>
        /// <param name="img">WPF bitmap source.</param>
        /// <param name="ignoreAlpha">True = creates a bitmap without alpha.</param>
        /// <returns>GDI bitmap.</returns>
        public static Bitmap CreateBitmap(System.Windows.Media.Imaging.BitmapSource img, bool ignoreAlpha)
        {
            var rect = new Rectangle(0, 0, img.PixelWidth, img.PixelHeight);
            Bitmap bmp = new Bitmap(img.PixelWidth, img.PixelHeight, ignoreAlpha ? PixelFormat.Format32bppRgb : PixelFormat.Format32bppArgb);

            BitmapData data;
            if (ignoreAlpha)
                data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            else
                data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            img.CopyPixels(new System.Windows.Int32Rect(0, 0, img.PixelWidth, img.PixelHeight), data.Scan0, 4 * img.PixelWidth * img.PixelHeight, 4 * img.PixelWidth);
            bmp.UnlockBits(data);

            return bmp;
        }

        /// <summary>
        /// Saves given image to file.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="savepath">Path to save image to.</param>
        /// <returns>True if saved successfully. False if failed or already exists.</returns>
        public static bool SaveImage(Image image, string savepath)
        {
            if (!File.Exists(savepath))
                try
                {
                    image.Save(savepath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("GDI Error in: " + savepath);
                    Debug.WriteLine("ERROR: " + e.Message);
                    return false;
                }

            return true;
        }


        /// <summary>
        /// Salts resize image function. Returns resized image.
        /// </summary>
        /// <param name="imgToResize">Image to resize</param>
        /// <param name="size">Size to shape to</param>
        /// <returns>Resized image as an Image.</returns>
        public static Image resizeImage(Image imgToResize, Size size)
        {
            // KFreon: And so begins the black magic
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            if (destHeight == 0)
                destHeight = 1;

            if (destWidth == 0)
                destWidth = 1;

            Bitmap b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            }
            return (Image)b;
        }


        /// <summary>
        /// Extracts all raw pixels from bitmap.
        /// </summary>
        /// <param name="bmp">Bitmap to extract data from.</param>
        /// <returns>Raw pixels.</returns>
        public static byte[] GetPixelDataFromBitmap(Bitmap bmp)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var length = data.Stride * data.Height;
            byte[] bytes = new byte[length];
            Marshal.Copy(data.Scan0, bytes, 0, length);
            bmp.UnlockBits(data);

            return bytes;
        }
    }
}
