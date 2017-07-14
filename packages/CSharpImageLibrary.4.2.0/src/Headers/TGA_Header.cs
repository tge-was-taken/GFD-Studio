using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpImageLibrary.Headers
{
    /// <summary>
    /// Reads the header of a Targa (TGA) image.
    /// </summary>
    public class TGA_Header : AbstractHeader
    {
        #region Properties
        /// <summary>
        /// Base TGA header.
        /// </summary>
        internal TargaHeader header { get; } = new TargaHeader();

        /// <summary>
        /// Image format.
        /// </summary>
        public override ImageEngineFormat Format
        {
            get
            {
                return ImageEngineFormat.TGA;
            }
        }
        #endregion Properties

        /// <summary>
        /// Reads a TGA header from stream.
        /// </summary>
        /// <param name="stream"></param>
        public TGA_Header(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Reads the header of a TGA image.
        /// </summary>
        /// <param name="stream">Fully formatted TGA image.</param>
        /// <returns>Length of header.</returns>
        protected override long Load(Stream stream)
        {
            base.Load(stream);

            // Class already written. Complex and I don't want to rewrite it.
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                TargaImage.LoadTGAHeaderInfo(br, header);

            Width = header.Width;
            Height = header.Height;
            return stream.Position;
        }
    }
}
