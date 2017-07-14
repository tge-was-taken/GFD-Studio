using CSharpImageLibrary.DDS;
using CSharpImageLibrary.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UsefulThings;

namespace CSharpImageLibrary
{
    /// <summary>
    /// Represents an image. Can use Windows codecs if available.
    /// </summary>
    public class ImageEngineImage : IDisposable
    {
        /// <summary>
        /// Original file data used to create this image.
        /// </summary>
        public byte[] OriginalData = null;

        #region Properties
        /// <summary>
        /// Image header.
        /// </summary>
        public AbstractHeader Header { get; set; }

        /// <summary>
        /// Width of image.
        /// </summary>
        public int Width
        {
            get
            {
                return MipMaps[0].Width;
            }
        }

        /// <summary>
        /// Height of image.
        /// </summary>
        public int Height
        {
            get
            {
                return MipMaps[0].Height;
            }
        }

        /// <summary>
        /// Number of mipmaps present.
        /// </summary>
        public int NumMipMaps
        {
            get
            {
                return MipMaps.Count;
            }
        }

        /// <summary>
        /// Format of image.
        /// </summary>
        public ImageEngineFormat Format { get { return FormatDetails.Format; } }


        /// <summary>
        /// Contains details of the image format.
        /// </summary>
        public ImageFormats.ImageEngineFormatDetails FormatDetails { get; private set; }

        
        /// <summary>
        /// List of mipmaps. Single level images only have one mipmap.
        /// </summary>
        public List<MipMap> MipMaps { get; private set; }

        /// <summary>
        /// Path to file. Null if no file e.g. thumbnail from memory.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Size of Image when compressed (Essentially file size)
        /// </summary>
        public int CompressedSize { get; set; }


        /// <summary>
        /// Uncompressed size of main image (no mipmaps)
        /// </summary>
        public int UncompressedSize
        {
            get
            {
                return ImageFormats.GetUncompressedSize(Width, Height, NumberOfChannels, false);
            }
        }


        /// <summary>
        /// Number of channels in image. 
        /// NOTE: Still stored in memory as BGRA regardless.
        /// </summary>
        public int NumberOfChannels
        {
            get
            {
                return FormatDetails.MaxNumberOfChannels;
            }
        }

        /// <summary>
        /// Number of bits per colour.
        /// </summary>
        public int BitCount =>  FormatDetails.BitCount; 

        /// <summary>
        /// Number of bytes per colour. i.e. 1 byte, 4 bytes (int), etc
        /// </summary>
        public int ComponentSize => FormatDetails.ComponentSize; 

        /// <summary>
        /// True = Image is a block compressed DDS.
        /// </summary>
        public bool IsBlockCompressed => FormatDetails.IsBlockCompressed; 

        /// <summary>
        /// Size of compressed blocks. Affected by component size. Default is 1 for normal images (jpg, etc)
        /// </summary>
        public int BlockSize => FormatDetails.BlockSize;

        /// <summary>
        /// True = Format is able to contain mipmaps.
        /// </summary>
        public bool IsMippable => FormatDetails.IsMippable;

        /// <summary>
        /// File Extension of selected format.
        /// </summary>
        public string FileExtension => FormatDetails.Extension;
        #endregion Properties

        /// <summary>
        /// Creates an image supporting many formats including DDS.
        /// </summary>
        /// <param name="stream">Stream containing image.</param>
        /// <param name="maxDimension">Max dimension of created image. Useful for mipmapped images, otherwise resized.</param>
        public ImageEngineImage(MemoryStream stream, int maxDimension = 0)
        {
            Load(stream, maxDimension);
        }


        /// <summary>
        /// Creates an image supporting many formats including DDS.
        /// </summary>
        /// <param name="path">Path to image.</param>
        /// <param name="maxDimension">Max dimension of created image. Useful for mipmapped images, otherwise resized.</param>
        public ImageEngineImage(string path, int maxDimension = 0) : this(File.ReadAllBytes(path), maxDimension)
        {
            FilePath = path;
        }

        /// <summary>
        /// Creates an image supporting many formats including DDS.
        /// </summary>
        /// <param name="imageData">Fully formatted image data, not just pixels.</param>
        /// <param name="maxDimension">Max dimension of created image. Useful for mipmapped images, otherwise resized.</param>
        public ImageEngineImage(byte[] imageData, int maxDimension = 0)
        {
            using (MemoryStream ms = new MemoryStream(imageData, 0, imageData.Length, false, true))  // Need to be able to access underlying byte[] using <Stream>.GetBuffer()
                Load(ms, maxDimension);
        }

        /// <summary>
        /// Gets string representation of ImageEngineImage.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"File Path: {this.FilePath}");
            sb.AppendLine($"Format: {this.Format.ToString()}");
            sb.AppendLine($"Width x Height: {this.Width}x{this.Height}");
            sb.AppendLine($"Num Mips: {this.NumMipMaps}");
            sb.AppendLine($"Header: {this.Header.ToString()}");

            return sb.ToString();
        }

        void Load(Stream stream, int maxDimension)
        {
            CompressedSize = (int)stream.Length;
            Header = ImageEngine.LoadHeader(stream);

            // DX10
            var DX10Format = DDS_Header.DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
            if (Header is Headers.DDS_Header)
                DX10Format = ((Headers.DDS_Header)Header).DX10_DXGI_AdditionalHeader.dxgiFormat;

            FormatDetails = new ImageFormats.ImageEngineFormatDetails(Header.Format, DX10Format);
            MipMaps = ImageEngine.LoadImage(stream, Header, maxDimension, 0, FormatDetails);

            // Read original data
            OriginalData = new byte[CompressedSize];
            stream.Position = 0;
            stream.Read(OriginalData, 0, CompressedSize);
        }

        #region Savers
        /// <summary>
        /// Saves image in specified format to file. If file exists, it will be overwritten.
        /// </summary>
        /// <param name="destination">File to save to.</param>
        /// <param name="destFormatDetails">Details of destination format.</param>
        /// <param name="GenerateMips">Determines how mipmaps are handled during saving.</param>
        /// <param name="desiredMaxDimension">Maximum size for saved image. Resizes if required, but uses mipmaps if available.</param>
        /// <param name="removeAlpha">True = Alpha removed. False = Uses threshold value and alpha values to mask RGB FOR DXT1 ONLY, otherwise removes completely.</param>
        /// <param name="mipToSave">Index of mipmap to save as single image.</param>
        public async Task Save(string destination, ImageFormats.ImageEngineFormatDetails destFormatDetails, MipHandling GenerateMips, int desiredMaxDimension = 0, int mipToSave = 0, bool removeAlpha = true)
        {
            var data = Save(destFormatDetails, GenerateMips, desiredMaxDimension, mipToSave, removeAlpha);

            using (FileStream fs = new FileStream(destination, FileMode.Create))
                await fs.WriteAsync(data, 0, data.Length);
        }


        /// <summary>
        /// Saves each channel separately incl Alpha.
        /// </summary>
        /// <param name="savePath">General save path. Appends channel name too.</param>
        public void SplitChannels(string savePath)
        {
            ImageEngine.SplitChannels(MipMaps[0], savePath);
        }

        /// <summary>
        /// Saves image in specified format to stream.
        /// Stream position not reset before or after.
        /// </summary>
        /// <param name="destination">Stream to write to at current position.</param>
        /// <param name="destFormatDetails">Details of destination format</param>
        /// <param name="GenerateMips">Determines how mipmaps are handled during saving.</param>
        /// <param name="desiredMaxDimension">Maximum dimension of saved image. Keeps aspect.</param>
        /// <param name="mipToSave">Specifies a mipmap to save within the whole.</param>
        /// <param name="removeAlpha">True = removes alpha. False = Uses threshold value and alpha values to mask RGB FOR DXT1 ONLY, otherwise removes completely.</param>
        public void Save(Stream destination, ImageFormats.ImageEngineFormatDetails destFormatDetails, MipHandling GenerateMips, int desiredMaxDimension = 0, int mipToSave = 0, bool removeAlpha = true)
        {
            var data = Save(destFormatDetails, GenerateMips, desiredMaxDimension, mipToSave, removeAlpha);
            destination.Write(data, 0, data.Length);
        }


        /// <summary>
        /// Saves fully formatted image in specified format to byte array.
        /// </summary>
        /// <param name="destFormatDetails">Details about destination format.</param>
        /// <param name="GenerateMips">Determines how mipmaps are handled during saving.</param>
        /// <param name="desiredMaxDimension">Maximum size for saved image. Resizes if required, but uses mipmaps if available.</param>
        /// <param name="mipToSave">Index of mipmap to save directly.</param>
        /// <param name="removeAlpha">True = Alpha removed. False = Uses threshold value and alpha values to mask RGB FOR DXT1, otherwise completely removed.</param>
        /// <returns></returns>
        public byte[] Save(ImageFormats.ImageEngineFormatDetails destFormatDetails, MipHandling GenerateMips, int desiredMaxDimension = 0, int mipToSave = 0, bool removeAlpha = true)
        {
            if (destFormatDetails.Format == ImageEngineFormat.Unknown)
                throw new InvalidOperationException("Save format cannot be 'Unknown'");

            AlphaSettings alphaSetting = AlphaSettings.KeepAlpha;
            if (removeAlpha)
                alphaSetting = AlphaSettings.RemoveAlphaChannel;
            else if (destFormatDetails.Format == ImageEngineFormat.DDS_DXT2 || destFormatDetails.Format == ImageEngineFormat.DDS_DXT4)
                alphaSetting = AlphaSettings.Premultiply;

            // If same format and stuff, can just return original data, or chunks of it.
            if (destFormatDetails.Format == Format)
                return AttemptSaveUsingOriginalData(destFormatDetails, GenerateMips, desiredMaxDimension, mipToSave, alphaSetting);
            else
                return ImageEngine.Save(MipMaps, destFormatDetails, GenerateMips, alphaSetting, desiredMaxDimension, mipToSave);
        }

        byte[] AttemptSaveUsingOriginalData(ImageFormats.ImageEngineFormatDetails destFormatDetails, MipHandling GenerateMips, int desiredMaxDimension, int mipToSave, AlphaSettings alphaSetting)
        {
            int start = 0;
            int destStart = 0;
            int length = OriginalData.Length;
            int newWidth = Width;
            int newHeight = Height;
            DDS_Header tempHeader = null;
            byte[] data = null;
            byte[] tempOriginalData = OriginalData;

            if (destFormatDetails.IsDDS)
            {
                destStart = destFormatDetails.HeaderSize;
                start = destStart;

                int mipCount = 0;

                if (mipToSave != 0)
                {
                    mipCount = 1;
                    newWidth = MipMaps[mipToSave].Width;
                    newHeight = MipMaps[mipToSave].Height;

                    start = ImageFormats.GetCompressedSize(mipToSave, destFormatDetails, Width, Height);
                    length = ImageFormats.GetCompressedSize(1, destFormatDetails, newWidth, newHeight);
                }
                else if (desiredMaxDimension != 0 && desiredMaxDimension < Width && desiredMaxDimension < Height)
                {
                    int index = MipMaps.FindIndex(t => t.Width < desiredMaxDimension && t.Height < desiredMaxDimension);

                    // If none found, do a proper save and see what happens.
                    if (index == -1)
                        return ImageEngine.Save(MipMaps, destFormatDetails, GenerateMips, alphaSetting, desiredMaxDimension, mipToSave);

                    mipCount -= index;
                    newWidth = MipMaps[index].Width;
                    newHeight = MipMaps[index].Height;

                    start = ImageFormats.GetCompressedSize(index, destFormatDetails, Width, Height);
                    length = ImageFormats.GetCompressedSize(mipCount, destFormatDetails, newWidth, newHeight);
                }
                else
                {
                    if (alphaSetting == AlphaSettings.RemoveAlphaChannel)
                    {
                        // Can't edit alpha directly in premultiplied formats. Not easily anyway.
                        if (destFormatDetails.IsPremultipliedFormat)
                            return ImageEngine.Save(MipMaps, destFormatDetails, GenerateMips, alphaSetting, desiredMaxDimension, mipToSave);


                        // DDS Formats only
                        switch (destFormatDetails.Format)
                        {
                            // Excluded cos they have no true alpha
                            case ImageEngineFormat.DDS_A8:
                            case ImageEngineFormat.DDS_A8L8:
                            case ImageEngineFormat.DDS_ATI1:
                            case ImageEngineFormat.DDS_ATI2_3Dc:
                            case ImageEngineFormat.DDS_V8U8:
                            case ImageEngineFormat.DDS_G16_R16:
                            case ImageEngineFormat.DDS_G8_L8:
                            case ImageEngineFormat.DDS_R5G6B5:
                            case ImageEngineFormat.DDS_RGB_8:
                            case ImageEngineFormat.DDS_DXT1:
                                break;

                            // Exluded cos they're alpha isn't easily edited
                            case ImageEngineFormat.DDS_DXT2:
                            case ImageEngineFormat.DDS_DXT4:
                                break;

                            // Excluded cos they're currently unsupported
                            case ImageEngineFormat.DDS_CUSTOM:
                            case ImageEngineFormat.DDS_DX10:
                            case ImageEngineFormat.DDS_ARGB_4:
                                break;

                            case ImageEngineFormat.DDS_ABGR_8:
                            case ImageEngineFormat.DDS_ARGB_32F:
                            case ImageEngineFormat.DDS_ARGB_8:
                            case ImageEngineFormat.DDS_DXT3:
                            case ImageEngineFormat.DDS_DXT5:
                                tempOriginalData = new byte[OriginalData.Length];
                                Array.Copy(OriginalData, tempOriginalData, OriginalData.Length);

                                // Edit alpha values
                                int alphaStart = 128;
                                int alphaJump = 0;
                                byte[] alphaBlock = null;
                                if (destFormatDetails.IsBlockCompressed)
                                {
                                    alphaJump = 16;
                                    alphaBlock = new byte[8];
                                    for (int i = 0; i < 8; i++)
                                        alphaBlock[i] = 255;
                                }
                                else
                                {
                                    alphaJump = destFormatDetails.ComponentSize * 4;
                                    alphaBlock = new byte[destFormatDetails.ComponentSize];

                                    switch (destFormatDetails.ComponentSize)
                                    {
                                        case 1:
                                            alphaBlock[0] = 255;
                                            break;
                                        case 2:
                                            alphaBlock = BitConverter.GetBytes(ushort.MaxValue);
                                            break;
                                        case 4:
                                            alphaBlock = BitConverter.GetBytes(1f);
                                            break;
                                    }
                                }

                                for (int i = alphaStart; i < OriginalData.Length; i += alphaJump)
                                    Array.Copy(alphaBlock, 0, tempOriginalData, i, alphaBlock.Length);

                                break;
                        }
                    }


                    switch (GenerateMips)
                    {
                        case MipHandling.KeepExisting:
                            mipCount = NumMipMaps;
                            break;
                        case MipHandling.Default:
                            if (NumMipMaps > 1)
                                mipCount = NumMipMaps;
                            else
                                goto case MipHandling.GenerateNew;  // Eww goto...
                            break;
                        case MipHandling.GenerateNew:
                            ImageEngine.DestroyMipMaps(MipMaps);
                            ImageEngine.TestDDSMipSize(MipMaps, destFormatDetails, Width, Height, out double fixXScale, out double fixYScale, GenerateMips);

                            // Wrong sizing, so can't use original data anyway.
                            if (fixXScale != 0 || fixYScale != 0)
                                return ImageEngine.Save(MipMaps, destFormatDetails, GenerateMips, alphaSetting, desiredMaxDimension, mipToSave);


                            mipCount = DDSGeneral.BuildMipMaps(MipMaps);

                            // Compress mipmaps excl top
                            byte[] formattedMips = DDSGeneral.Save(MipMaps.GetRange(1, MipMaps.Count - 1), destFormatDetails, alphaSetting);
                            if (formattedMips == null)
                                return null;

                            // Get top mip size and create destination array 
                            length = ImageFormats.GetCompressedSize(0, destFormatDetails, newWidth, newHeight); // Should be the length of the top mipmap.
                            data = new byte[formattedMips.Length + length];

                            // Copy smaller mips to destination
                            Array.Copy(formattedMips, destFormatDetails.HeaderSize, data, length, formattedMips.Length - destFormatDetails.HeaderSize);
                            break;
                        case MipHandling.KeepTopOnly:
                            mipCount = 1;
                            length = ImageFormats.GetCompressedSize(1, destFormatDetails, newWidth, newHeight);
                            break;
                    }
                }

                // Header
                tempHeader = new DDS_Header(mipCount, newWidth, newHeight, destFormatDetails.Format, destFormatDetails.DX10Format);
            }
            
            // Use existing array, otherwise create one.
            data = data ?? new byte[length];
            Array.Copy(tempOriginalData, start, data, destStart, length - destStart);

            // Write header if existing (DDS Only)
            if (tempHeader != null)
                tempHeader.WriteToArray(data, 0);

            return data;
        }
        #endregion Savers

        /// <summary>
        /// Releases resources used by mipmap MemoryStreams.
        /// </summary>
        public void Dispose()
        {
            // Nothing for now I guess...
        }

        /// <summary>
        /// Creates a WPF Bitmap from largest mipmap.
        /// Does NOT require that image remains alive.
        /// </summary>
        /// <param name="ShowAlpha">True = flattens alpha, directly affecting RGB.</param>
        /// <param name="maxDimension">Resizes image or uses a mipmap if available. Overrides mipIndex if specified.</param>
        /// <param name="mipIndex">Index of mipmap to retrieve. Overridden by maxDimension if it's specified.</param>
        /// <returns>WPF bitmap of largest mipmap.</returns>
        public BitmapSource GetWPFBitmap(int maxDimension = 0, bool ShowAlpha = false, int mipIndex = 0)
        {
            MipMap mip = MipMaps[mipIndex];
            return GetWPFBitmap(mip, maxDimension, ShowAlpha);
        }


        BitmapSource GetWPFBitmap(MipMap mip, int maxDimension, bool ShowAlpha)
        {
            BitmapSource bmp = null;

            if (maxDimension != 0)
            {
                // Choose a mip of the correct size, if available.
                var sizedMip = MipMaps.Where(m => (m.Height <= maxDimension && m.Width <= maxDimension) || (m.Width <= maxDimension && m.Height <= maxDimension));
                if (sizedMip.Any())
                {
                    var mip1 = sizedMip.First();
                    bmp = mip1.ToImage();
                }
                else
                {
                    double scale = (double)maxDimension / (Height > Width ? Height : Width);
                    mip = ImageEngine.Resize(mip, scale);
                    bmp = mip.ToImage();
                }         
            }
            else
                bmp = mip.ToImage();

            if (!ShowAlpha)
                bmp = new FormatConvertedBitmap(bmp, System.Windows.Media.PixelFormats.Bgr32, null, 0);

            bmp.Freeze();
            return bmp;
        }


        /// <summary>
        /// Resizes image.
        /// If single mip, scales to DesiredDimension.
        /// If multiple mips, finds closest mip and scales it (if required). DESTROYS ALL OTHER MIPS.
        /// </summary>
        /// <param name="DesiredDimension">Desired size of images largest dimension.</param>
        public void Resize(int DesiredDimension)
        {
            var top = MipMaps[0];
            var determiningDimension = top.Width > top.Height ? top.Width : top.Height;
            double scale = (double)DesiredDimension / determiningDimension;  
            Resize(scale);
        }


        /// <summary>
        /// Scales top mipmap and DESTROYS ALL OTHERS.
        /// </summary>
        /// <param name="scale">Scaling factor. </param>
        public void Resize(double scale)
        {
            MipMap closestMip = null;
            double newScale = 0;
            double desiredSize = MipMaps[0].Width * scale;

            double min = double.MaxValue;
            foreach (var mip in MipMaps)
            {
                double temp = Math.Abs(mip.Width - desiredSize);
                if (temp < min)
                {
                    closestMip = mip;
                    min = temp;
                }
            }

            newScale = desiredSize / closestMip.Width;

            MipMaps[0] = ImageEngine.Resize(closestMip, newScale);
            MipMaps.RemoveRange(1, NumMipMaps - 1);
        }
    }
}
