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
    /// Provides information given by GIF file header.
    /// Mostly from http://giflib.sourceforge.net/whatsinagif/bits_and_bytes.html
    /// </summary>
    public class GIF_Header : AbstractHeader
    {
        const int GlobalHeaderSize = 6 + 7 + 768 + 19 + 8; // Header block, Logical Screen Descriptor, Max size Global Colour Table, Application Extension (animation), Graphics Control Extension (Transparency)

        /// <summary>
        /// Characters beginning the file marking it as a GIF image.
        /// </summary>
        public static string Identifier = "GIF";

        #region Properties

        /// <summary>
        /// Version of bitmap spec. Not really used.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Usually ignored. Original use was for mozaic.
        /// </summary>
        public int CanvasHeight { get; private set; }

        /// <summary>
        /// Usually ignored. Original use was for mozaic.
        /// </summary>
        public int CanvasWidth { get; private set; }

        /// <summary>
        /// If true, global colour table present.
        /// </summary>
        public bool HasGlobalColourTable { get; private set; }

        /// <summary>
        /// Bits per pixel in global colour table.
        /// Only valid when Global Colour Table is present.
        /// </summary>
        public int ColourResolution_BPP { get; private set; }

        /// <summary>
        /// Indicates whether Global Colour Table is sorted in order of decreasing importance.
        /// Not used, and only valid when Global Colour Table is present.
        /// </summary>
        public bool ColourSortFlag { get; private set; }

        /// <summary>
        /// Size of Global Colour Table in bytes.
        /// Potentially not filled.
        /// </summary>
        public int GlobalColourTableSize { get; private set; }
        
        /// <summary>
        /// Determines background colour of canvas.
        /// Not generally used since canvas not used.
        /// </summary>
        public int BackgroundColourIndex { get; private set; }

        /// <summary>
        /// Not used by any modern viewers.
        /// </summary>
        public int PixelAspectRatio { get; private set; }

        /// <summary>
        /// Represents colour table for image. Indexed colours.
        /// Not required to be present. Format = RGB, 0-255 each.
        /// </summary>
        public byte[] GlobalColourTable { get; private set; }

        /// <summary>
        /// Frame disposal method for animation.
        /// </summary>
        public int DisposalMethod { get; private set; }

        /// <summary>
        /// Indicates whether there is a transparent colour.
        /// </summary>
        public bool TransparentColourFlag { get; private set; }

        /// <summary>
        /// Frame delay time. i.e. 'speed' of animation.
        /// </summary>
        public int AnimationDelayTime { get; private set; }

        /// <summary>
        /// Index in global colour table of transparent colour.
        /// Only valid if TransparentColourFlag is true.
        /// </summary>
        public int TransparentColourIndex { get; private set; }

        /// <summary>
        /// Number of times animation loops.
        /// </summary>
        public int AnimationLoopCount { get; private set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public bool UserInputFlag { get; private set; }

        /// <summary>
        /// Offset from canvas left.
        /// Usually ignored.
        /// </summary>
        public int ImageLeft { get; private set; }

        /// <summary>
        /// Offset from canvas top. 
        /// Usually ignored.
        /// </summary>
        public int ImageTop { get; private set; }

        /// <summary>
        /// Indicates whether image has a local colour table, overriding the global table (if exists)
        /// </summary>
        public bool HasLocalColourTables { get; private set; }


        /// <summary>
        /// Indicates whether image is interlaced.
        /// Interlacing is showing every other line, then filling in the blanks later so user gets an idea what it looks like first.
        /// </summary>
        public bool IsInterlaced { get; private set; }

        /// <summary>
        /// Same as global sort. Indicates whether local table is ordered in descending order of importance.
        /// </summary>
        public bool LocalSortFlag { get; private set; }

        /// <summary>
        /// Size of local Colour Table in bytes.
        /// </summary>
        public int LocalColourTableSize { get; private set; }
        #endregion Properties

        /// <summary>
        /// Image format.
        /// </summary>
        public override ImageEngineFormat Format
        {
            get
            {
                return ImageEngineFormat.GIF;
            }
        }

        /// <summary>
        /// Reads header from GIF image.
        /// </summary>
        /// <param name="stream">Fully formatted GIF image.</param>
        public GIF_Header(Stream stream)
        {
            Load(stream);
        }

        internal static bool CheckIdentifier(byte[] IDBlock)
        {
            for (int i = 0; i < Identifier.Length; i++)
                if (IDBlock[i] != Identifier[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Read header from stream.
        /// </summary>
        /// <param name="stream">Stream containing header.</param>
        /// <returns>Length of header.</returns>
        protected override long Load(Stream stream)
        {
            base.Load(stream);

            // Read partial header to determine full header size
            byte[] temp = new byte[19];  // for later
            stream.Read(temp, 0, 13); // Partial header only 13 bytes long

            if (!CheckIdentifier(temp))
                throw new FormatException("Stream is not a GIF Image");

            // Read version (not used)
            Version = "8";
            for(int i = 4; i < 6; i++)
                Version += (char)temp[i];

            if (Version != "89a" && Version != "87a")
                Console.WriteLine($"Header version ({Version}) is incorrect. Must be 89a or 87a.");

            // Canvas Width
            CanvasWidth = BitConverter.ToInt16(temp, 6);

            // Canvas Height
            CanvasHeight = BitConverter.ToInt16(temp, 8);

            // Packed Field
            byte tempByte = temp[9];
            HasGlobalColourTable = (tempByte & 0x80) == 0x80;
            ColourResolution_BPP = tempByte & 0x70;
            ColourSortFlag = (tempByte & 0x08) == 0x08;
            GlobalColourTableSize = tempByte & 0x07;

            // Background colour index
            BackgroundColourIndex = temp[10];
            PixelAspectRatio = temp[11];

            // Read colour table
            if (HasGlobalColourTable)
            {
                int estimatedSize = (int)Math.Pow(2, ColourResolution_BPP + 1);
                if (GlobalColourTableSize != estimatedSize)
                    Console.WriteLine($"Global table size incorrect in header. Header = {GlobalColourTableSize}, should be {estimatedSize}");

                GlobalColourTable = stream.ReadBytes(estimatedSize);
            }

            while (stream.Position < stream.Length)
            {
                stream.Read(temp, 0, 19);

                // Need to search for some reason
                bool found = false;
                for (int i = 0; i < 19; i++)
                {
                    if (temp[i] == 0x21 || temp[i] == 0x2C)
                    {
                        stream.Position -= (19-i);
                        found = true;
                        break;
                    }
                }

                // Nothing of interest found, start again from new position
                if (!found)
                    continue;

                // Refresh from new position
                stream.Read(temp, 0, 19);

                if (temp[0] == 0x21)  // Extension indicator
                {
                    if (temp[1] == 0xFF)  // Animated properties indicator
                    {
                        // Ignore bytes 3-16 (1 based)
                        AnimationLoopCount = BitConverter.ToInt16(temp, 16);
                    }
                    else if (temp[1] == 0xF9)
                    {
                        // Skip byte size (why is it even there?)
                        DisposalMethod = temp[3] & 0x1C;
                        UserInputFlag = (temp[3] & 0x02) == 1;
                        TransparentColourFlag = (temp[3] & 0x01) == 1;
                        AnimationDelayTime = BitConverter.ToInt16(temp, 4);
                        TransparentColourIndex = temp[6];
                    }
                    else
                        stream.Seek(-18, SeekOrigin.Current); // Skip ignored optionals ideally, BUT this could also be false match i.e. not an extension block, so we can only skip that identifier and keep looking.
                }
                else
                    break;
            }

            // Read first image descriptor block
            if (temp[0] != 0x2C)
                throw new InvalidDataException($"Image Descriptor incorrect. Got: {temp[0]}, expected: {0x2C}.");

            ImageLeft = BitConverter.ToInt16(temp, 1);
            ImageTop = BitConverter.ToInt16(temp, 3);
            Width = BitConverter.ToInt16(temp, 5);
            Height = BitConverter.ToInt16(temp, 7);

            // Packed byte
            tempByte = temp[9];
            HasLocalColourTables = (tempByte & 0x80) == 1;
            IsInterlaced = (tempByte & 0x70) == 1;
            LocalSortFlag = (tempByte & 0x40) == 1;
            LocalColourTableSize = tempByte & 0x07;

            return stream.Position;
        }
    }
}
