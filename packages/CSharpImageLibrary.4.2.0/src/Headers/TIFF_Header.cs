using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;

namespace CSharpImageLibrary.Headers
{
    /// <summary>
    /// Information about a TIFF image.
    /// </summary>
    public class TIFF_Header : AbstractHeader
    {
        /// <summary>
        /// Format of image.
        /// </summary>
        public override ImageEngineFormat Format
        {
            get
            {
                return ImageEngineFormat.TIF;
            }
        }

        // Header size isn't really knowable. Changes depending on what tags/data is included. 
        // Also since mipmaps are present, each has a header throughout the file.

        /// <summary>
        /// Identifier indicating Motorola byte ordering (Big Endian). * is the version number (42), but never changes.
        /// </summary>
        public const string MotorolaIdentifier = "MM*";

        /// <summary>
        /// Identifier indicating Intel byte ordering (Little Endian). * is the version number (42), but never changes.
        /// </summary>
        public const string IntelIdentifier = "II*";

        MyBitConverter.Endianness endianness = MyBitConverter.Endianness.LittleEndian;

        #region Sub-Headers
        /// <summary>
        /// Local header of image page. 
        /// Essentially a mipmap header, except these "mipmaps" have no restrictions i.e. They don't have to be related to the first image at all e.g. scanned pages from a scanner.
        /// </summary>
        public struct ImageFileDirectory
        {
            /// <summary>
            /// Essentially the description of a property the image page possesses e.g. Width, colour space, etc
            /// </summary>
            [DebuggerDisplay("{FieldTag} - {FieldType} - {FieldLength}")]
            public struct FieldDescriptor
            {
                /// <summary>
                /// Type of data this field contains.
                /// </summary>
                public enum FieldTypes
                {
                    /// <summary>
                    /// Single byte value.
                    /// </summary>
                    BYTE = 1,

                    /// <summary>
                    /// String, often with trailing null. Either way, length provided is length of string.
                    /// </summary>
                    ASCII_STRING = 2,

                    /// <summary>
                    /// UInt16 value.
                    /// </summary>
                    WORD = 3,

                    /// <summary>
                    /// Double word. UInt32.
                    /// </summary>
                    DWORD_UWORD = 4,

                    /// <summary>
                    /// Fraction, presented by a numerator and a denominator (both UInt32).
                    /// </summary>
                    RATIONAL = 5,
                }

                /// <summary>
                /// Field name, indicating what this field represents.
                /// </summary>
                public enum FieldTags
                {
                    /// <summary>
                    /// Number of bits per sample.
                    /// </summary>
                    BitsPerSample = 0x102,

                    /// <summary>
                    /// RGB colour map for palette colour iamges.
                    /// </summary>
                    ColourMap = 0x140,

                    /// <summary>
                    /// Fancy curves to do with colours...NTSC something? I dunno.
                    /// </summary>
                    ColourResponseCurves = 0x12D,

                    /// <summary>
                    /// Method of compression. A few different methods: None, CCITT (huffman), fax CCITT, fax CCITT Group 4, LZW, packbits.
                    /// </summary>
                    Compression = 0x103,

                    /// <summary>
                    /// Another fancy curve thing.
                    /// </summary>
                    GrayResponseCurve = 0x123,

                    /// <summary>
                    /// Unit of measurement for gray response curve.
                    /// </summary>
                    GrayResponseUnit = 0x122,

                    /// <summary>
                    /// Height of image in pixels.
                    /// </summary>
                    ImageLength = 0x101,

                    /// <summary>
                    /// Width of image in pixels.
                    /// </summary>
                    ImageWidth = 0x100,

                    /// <summary>
                    /// General indication of the kind of data contained in this subfile.
                    /// </summary>
                    NewSubfileType = 0xFE,

                    /// <summary>
                    /// This name sounds really cool. Seems to be some kond of pixel format.
                    /// </summary>
                    PhotometricInterpretation = 0x106,

                    /// <summary>
                    /// Indicates how pixels are stored. Can be contiguously (like normal images) or in planes (like passengers...?)
                    /// </summary>
                    PlanarConfiguration = 0x11C,

                    /// <summary>
                    /// Used when compressing with LZW.
                    /// </summary>
                    Predictor = 0x13D,

                    /// <summary>
                    /// Used with XRes and YRes. Inches, centimeters, none (arbitrary).
                    /// </summary>
                    ResolutionUnit = 0x128,

                    /// <summary>
                    /// Strips can be used for fast access even when compressed. This indicates how many pixel rows are in each strip.
                    /// </summary>
                    RowsPerStrip = 0x116,

                    /// <summary>
                    /// RGB = 3, palette and grayscale = 1. Seems to be number of channels in a way.
                    /// </summary>
                    SamplesPerPixel = 0x115,

                    /// <summary>
                    /// Number of bytes in a strip. Saves having to figure this out blindly.
                    /// </summary>
                    StripByteCounts = 0x117,

                    /// <summary>
                    /// Byte offsets of each strip w.r.t. start of file.
                    /// </summary>
                    StripOffsets = 0x111,

                    /// <summary>
                    /// Number of pixels per ResolutionUnit in X Direction.
                    /// </summary>
                    XResolution = 0x11A,

                    /// <summary>
                    /// Number of pixels per ResolutionUnit in Y Direction.
                    /// </summary>
                    YResolution = 0x11B,

                    /// <summary>
                    /// Name of artist.
                    /// </summary>
                    Artist = 0x13B,

                    /// <summary>
                    /// Date and time of image creation. YYYY:MM:DD HH:MM:SS in 24 hour time.
                    /// </summary>
                    DateTime = 0x132,

                    /// <summary>
                    /// "ENIAC or whatever" says the documentation. So who knows?
                    /// </summary>
                    HostComputer = 0x13C,

                    /// <summary>
                    /// User description of image.
                    /// </summary>
                    ImageDescription = 0x10E,

                    /// <summary>
                    /// Manufacturer of camera, scanner, etc
                    /// </summary>
                    Make = 0x10F,

                    /// <summary>
                    /// Model name/number of camera, scanner, etc
                    /// </summary>
                    Model = 0x110,

                    /// <summary>
                    /// Name and relase number of software package that created the image.
                    /// </summary>
                    Software = 0x131,

                    /// <summary>
                    /// Options for fax compressed images.
                    /// </summary>
                    Group3Options = 0x124,

                    /// <summary>
                    /// Options  for fax group 4 compressed images.
                    /// </summary>
                    Group4Options = 0x125,

                    /// <summary>
                    /// Name of document image that was scanned.
                    /// </summary>
                    DocumentName = 0x10D,

                    /// <summary>
                    /// Name of page scanned.
                    /// </summary>
                    PageName = 0x11D,

                    /// <summary>
                    /// Number of page scanned.
                    /// </summary>
                    PageNumber = 0x129,

                    /// <summary>
                    /// Offset from left. Like canvas positioning.
                    /// </summary>
                    XPosition = 0x11E,

                    /// <summary>
                    /// Offset from Top. Like canvas positioning.
                    /// </summary>
                    YPosition = 0x11F,

                    /// <summary>
                    /// Seems to be some maths/curves thing indicating where "white" is.
                    /// </summary>
                    WhitePoint = 0x13E,

                    /// <summary>
                    /// Another cool name. Some more maths/curve thing indicating where each primary colour is.
                    /// </summary>
                    PrimaryChromaticities = 0x13F,

                    /// <summary>
                    /// General indication of what kind of data this subfile is. Full res data, reduced res data, single page of multi page image.
                    /// </summary>
                    SubFileType = 0xFF,

                    /// <summary>
                    /// Somehow represents the orientation of the image, so it can be rotated without actually changing pixel data.
                    /// Clever girl...
                    /// </summary>
                    Orientation = 0x112,

                    /// <summary>
                    /// How image is smoothed. Line art, dithered, error diffused.
                    /// </summary>
                    Thresholding = 0x107,

                    /// <summary>
                    /// Indicates type of colour style. Continous tone (natural image), synthetic image (greatly restricted colour range, see ColourList)
                    /// </summary>
                    ColourImageType = 0x13E,

                    /// <summary>
                    /// List of colours used in image. Only practical if significantly restricted colour spectrum.
                    /// </summary>
                    ColourList = 0x13F,
                }

                #region Properties
                /// <summary>
                /// "Name" of field.
                /// </summary>
                public FieldTags FieldTag { get; private set; }

                /// <summary>
                /// Type of data field contains e.g. byte, int32, etc
                /// </summary>
                public FieldTypes FieldType { get; private set; }

                /// <summary>
                /// Length of field data.
                /// </summary>
                public int FieldLength { get; private set; }

                /// <summary>
                /// Offset of field data.
                /// </summary>
                public int DataOffset { get; private set; }

                /// <summary>
                /// Data indicated by field.
                /// </summary>
                public byte[] Data { get; private set; }
                #endregion Properties


                /// <summary>
                /// Read field descriptor from block.
                /// A "property" of the image.
                /// </summary>
                /// <param name="IDBlock">Block containing descriptor, but NOT it's data.</param>
                /// <param name="endianness">Big or little endianness defined by TIFF header.</param>
                /// <param name="dataStream">Full image stream to read descriptor data from.</param>
                public FieldDescriptor(byte[] IDBlock, MyBitConverter.Endianness endianness, Stream dataStream)
                {
                    FieldTag = (FieldTags)MyBitConverter.ToInt16(IDBlock, 0, endianness);
                    FieldType = (FieldTypes)MyBitConverter.ToInt16(IDBlock, 2, endianness);
                    FieldLength = MyBitConverter.ToInt32(IDBlock, 4, endianness);
                    DataOffset = MyBitConverter.ToInt32(IDBlock, 8, endianness);

                    // Read data indicated by descriptor
                    long oldOffset = dataStream.Position;
                    dataStream.Seek(DataOffset, SeekOrigin.Begin);
                    Data = new byte[FieldLength];
                    dataStream.Read(Data, 0, FieldLength);

                    // Reset stream position for next descriptor
                    dataStream.Seek(oldOffset, SeekOrigin.Begin);
                }
            }

            /// <summary>
            /// Number of field descriptors present for this image.
            /// </summary>
            public ushort NumberOfEntries { get; private set; }

            /// <summary>
            /// Descriptors (properties) for this image.
            /// </summary>
            public List<FieldDescriptor> FieldDescriptors { get; private set; }

            /// <summary>
            /// Offset of next sub image information directory.
            /// </summary>
            public int NextIFDOffset { get; private set; }


            /// <summary>
            /// Read sub image header.
            /// </summary>
            /// <param name="stream">Stream to read local header from.</param>
            /// <param name="endianness">Big or little, as defined by TIFF header.</param>
            public ImageFileDirectory(Stream stream, MyBitConverter.Endianness endianness) : this()
            {
                var bytes = stream.ReadBytes(2);
                NumberOfEntries = MyBitConverter.ToUInt16(bytes, 0, endianness);
                FieldDescriptors = new List<FieldDescriptor>();

                
                for (int i = 0; i < NumberOfEntries; i++)
                {
                    bytes = stream.ReadBytes(12);
                    FieldDescriptors.Add(new FieldDescriptor(bytes, endianness, stream));
                }
            }
        }
        #endregion Sub-Headers

        #region Properties
        /// <summary>
        /// Offset of first mipmap local header.
        /// </summary>
        public uint FirstImageOffset { get; private set; }

        /// <summary>
        /// List of mipmap headers.
        /// </summary>
        public List<ImageFileDirectory> Pages { get; private set; }

        /// <summary>
        /// Number of sub images (mipmaps).
        /// </summary>
        public int NumMipMaps
        {
            get
            {
                return Pages?.Count ?? -1;
            }
        }
        #endregion Properties

        /// <summary>
        /// Creates TIFF Header from stream containing, at least, a TIFF header.
        /// </summary>
        /// <param name="stream"></param>
        public TIFF_Header(Stream stream)
        {
            Load(stream);
        }


        /// <summary>
        /// Loads TIFF header from stream.
        /// </summary>
        /// <param name="stream">Fully formatted TIFF image.</param>
        /// <returns>Length of header.</returns>
        protected override long Load(Stream stream)
        {
            base.Load(stream);
            byte[] temp = stream.ReadBytes(8);

            if (!CheckIdentifier(temp))
                throw new FormatException("Stream is not a recognised TIFF image.");

            // Change byte order if required.
            if (temp[0] == 'M')
                endianness = MyBitConverter.Endianness.BigEndian;

            // Header
            FirstImageOffset = MyBitConverter.ToUInt32(temp, 4, endianness);
            stream.Seek(FirstImageOffset, SeekOrigin.Begin);
            var IFD = new ImageFileDirectory(stream, endianness);
            Pages = new List<ImageFileDirectory>() { IFD };

            // Add mipmaps if they exist
            while (IFD.NextIFDOffset != 0)
            {
                IFD = new ImageFileDirectory(stream, endianness);
                Pages.Add(IFD);
            }

            return 0; // No sensible return value since header is not contiguous (right?)
        }

        internal static bool CheckIdentifier(byte[] IDBlock)
        {
            return (IDBlock[0] == 'I' && IDBlock[1] == 'I' && IDBlock[2] == '*') || 
                (IDBlock[0] == 'M' && IDBlock[1] == 'M' && IDBlock[2] == '*');
        }

    }
}
