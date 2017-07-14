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
    /// Contains information about a JPG image.
    /// </summary>
    public class JPG_Header : AbstractHeader
    {
        /// No printable characters mark a jpg. See <see cref="CheckIdentifier(byte[])"/>.

        const int HeaderSize = 20;
        #region Properties
        /// <summary>
        /// Length of data section (APP0/JFIF), includes thumbnail etc.
        /// </summary>
        public int DataSectionLength { get; private set; }

        /// <summary>
        /// Identifier as a JPEG image. Should always be JFIF.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Version of JFIF file was created with.
        /// </summary>
        public string Version { get; private set; }

        public enum UnitsType
        {
            None = 0,
            DotsPerInch = 1,
            DotsPerCentimeter = 2,
        }

        /// <summary>
        /// Units of resolution. Dunno what means what though.
        /// </summary>
        public UnitsType ResolutionUnits { get; private set; }

        /// <summary>
        /// Horizontal resolution in unit specified by <see cref="ResolutionUnits"/>.
        /// </summary>
        public int HorizontalResolution { get; private set; }

        /// <summary>
        /// Vertical resolution in unit specified by <see cref="ResolutionUnits"/>.
        /// </summary>
        public int VerticalResolution { get; private set; }

        /// <summary>
        /// Horizontal pixel count of Thumbnail stored in image.
        /// If no thumbnail, set to 0.
        /// </summary>
        public int XThumbnailPixelCount { get; private set; }

        /// <summary>
        /// Vertical pixel count of Thumbnail stored in image.
        /// If no thumbnail, set to 0.
        /// </summary>
        public int YThumbnailPixelCount { get; private set; }
        #endregion Properties

        /// <summary>
        /// Image format.
        /// </summary>
        public override ImageEngineFormat Format
        {
            get
            {
                return ImageEngineFormat.JPG;
            }
        }

        internal static bool CheckIdentifier(byte[] IDBlock)
        {
            return IDBlock[0] == 0xFF && IDBlock[1] == 0xD8 && IDBlock[2] == 0xFF;
        }

        /// <summary>
        /// Read header of JPG image.
        /// </summary>
        /// <param name="stream">Fully formatted JPG image.</param>
        /// <returns>Length of header.</returns>
        protected override long Load(Stream stream)
        {
            base.Load(stream);
            byte[] temp = stream.ReadBytes(HeaderSize);

            if (!CheckIdentifier(temp))
                throw new FormatException("Stream is not a BMP Image");

            DataSectionLength = BitConverter.ToInt16(temp, 4);
            Identifier = BitConverter.ToString(temp, 6, 5);
            Version = temp[11] + ".0" + temp[12];
            ResolutionUnits = (UnitsType)temp[13];
            HorizontalResolution = MyBitConverter.ToInt16(temp, 14, MyBitConverter.Endianness.BigEndian);
            VerticalResolution = MyBitConverter.ToInt16(temp, 16, MyBitConverter.Endianness.BigEndian);
            XThumbnailPixelCount = temp[18];
            YThumbnailPixelCount = temp[19];

            return HeaderSize;
        }

        /// <summary>
        /// Loads a JPG header.
        /// </summary>
        /// <param name="stream"></param>
        public JPG_Header(Stream stream)
        {
            Load(stream);
        }
    }
}
