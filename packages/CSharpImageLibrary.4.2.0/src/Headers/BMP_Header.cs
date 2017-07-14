using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;

namespace CSharpImageLibrary.Headers
{
    /// <summary>
    /// Provides information given by BMP headers.
    /// </summary>
    public class BMP_Header : AbstractHeader
    {
        /// <summary>
        /// File header for BMP file.
        /// Contains general file information such as size and data offset.
        /// </summary>
        public struct BMPFileHeader
        {
            /// <summary>
            /// Size of File header.
            /// </summary>
            public const int HeaderSize = 14;

            // Skipping ID. 2 bytes
            /// <summary>
            /// Size of entire BMP file incl headers.
            /// </summary>
            public int FileSize; // 4 bytes

            /// <summary>
            /// Reserved for something.
            /// </summary>
            public int Reserved1; // 2 bytes

            /// <summary>
            /// Reserved for something 2.
            /// </summary>
            public int Reserved2; // 2 bytes

            /// <summary>
            /// Offset in file of pixel data block.
            /// </summary>
            public int DataOffset; // 4 bytes

            /// <summary>
            /// Read File header from BMP header.
            /// </summary>
            /// <param name="headerBlock">Header block containing BMP File Header.</param>
            public BMPFileHeader(byte[] headerBlock)
            {
                FileSize = headerBlock[2];
                for (int i = 3; i < 7; i++)
                    FileSize |= headerBlock[i];

                Reserved1 = headerBlock[7];
                Reserved2 = headerBlock[8];

                DataOffset = headerBlock[10];
                for (int i = 11; i < 14; i++)
                    DataOffset |= headerBlock[i];
            }

            /// <summary>
            /// Show string representation of header.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return UsefulThings.General.StringifyObject(this, true);
            }
        }

        /// <summary>
        /// Detailed image header.
        /// </summary>
        public struct BMPDIBHeader
        {
            /// <summary>
            /// Size of DIB header.
            /// </summary>
            public const int HeaderSize = 40;

            // Core header
            /// <summary>
            /// Width of image.
            /// </summary>
            public int Width;

            /// <summary>
            /// Height of image.
            /// </summary>
            public int Height;

            /// <summary>
            /// Colour panes. Dunno what this is.
            /// </summary>
            public const int ColourPlanes = 1;

            /// <summary>
            /// Bits per pixel.
            /// </summary>
            public int BPP;

            // Info header

            /// <summary>
            /// Method of compression.
            /// </summary>
            public int CompressionMethod;

            /// <summary>
            /// Size of data block.
            /// </summary>
            public int RawImageSize;

            /// <summary>
            /// Horizontal resolution. Probably 96 pixels per inch. Filthy Imperial measurements.
            /// </summary>
            public int HorizontalResolution;

            /// <summary>
            /// Vertical resolution. Probably 96 pixels per inch. Filthy Imperial measurements.
            /// </summary>
            public int VerticalResolution;

            /// <summary>
            /// Number of colours in indexed palette.
            /// </summary>
            public int NumColoursInPalette;

            /// <summary>
            /// Number of important colours in palette.
            /// Usually ignored cos why put them in there then...
            /// </summary>
            public int NumImportantColours;

            /// <summary>
            /// Reads the detailed DIB header from a full file-DIB header block.
            /// </summary>
            /// <param name="headerBlock"></param>
            public BMPDIBHeader(byte[] headerBlock)
            {
                // Core Header
                Width = BitConverter.ToInt32(headerBlock, 18);
                Height = BitConverter.ToInt32(headerBlock, 22);
                BPP = BitConverter.ToInt16(headerBlock, 28);

                // Info Header (standard for Windows)

                CompressionMethod = BitConverter.ToInt32(headerBlock, 30);
                RawImageSize = BitConverter.ToInt32(headerBlock, 34);
                HorizontalResolution = BitConverter.ToInt32(headerBlock, 38);
                VerticalResolution = BitConverter.ToInt32(headerBlock, 42);
                NumColoursInPalette = BitConverter.ToInt32(headerBlock, 46);
                NumImportantColours = BitConverter.ToInt32(headerBlock, 50);
            }
        }

        const int HeaderSize = 14 + 40; // File header, standard DIB header (BITMAPINFOHEADER)

        /// <summary>
        /// Characters beginning the file marking it as a BMP image.
        /// </summary>
        public const string Identifier = "BM";

        #region Properties
        /// <summary>
        /// General File size information.
        /// </summary>
        public BMPFileHeader FileHeader { get; private set; }

        /// <summary>
        /// Detailed image information.
        /// </summary>
        public BMPDIBHeader DIBHeader { get; private set; }
        #endregion Properties

        /// <summary>
        /// Reads header from BMP image.
        /// </summary>
        /// <param name="stream">Fully formatted BMP image.</param>
        public BMP_Header(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Image Format.
        /// </summary>
        public override ImageEngineFormat Format
        {
            get
            {
                return ImageEngineFormat.BMP;
            }
        }

        internal static bool CheckIdentifier(byte[] IDBlock)
        {
            for (int i = 0; i < Identifier.Length; i++)
                if (IDBlock[i] != Identifier[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Loads BMP header from stream.
        /// </summary>
        /// <param name="stream">Fully formatted BMP image.</param>
        /// <returns>Length of header.</returns>
        protected override long Load(Stream stream)
        {
            base.Load(stream);
            byte[] temp = stream.ReadBytes(HeaderSize);

            if (!CheckIdentifier(temp))
                throw new FormatException("Stream is not a BMP Image");

            FileHeader = new BMPFileHeader(temp);
            DIBHeader = new BMPDIBHeader(temp);

            Width = DIBHeader.Width;
            Height = DIBHeader.Height;

            return HeaderSize;
        }
    }
}
