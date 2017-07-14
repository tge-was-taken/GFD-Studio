using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpImageLibrary.ImageFormats;

namespace CSharpImageLibrary.Headers
{
    /// <summary>
    /// Base header class for image headers.
    /// </summary>
    public abstract class AbstractHeader
    {
        /// <summary>
        /// Format of image as seen by header.
        /// </summary>
        public abstract ImageEngineFormat Format { get; }

        /// <summary>
        /// Width of image.
        /// </summary>
        public virtual int Width { get; protected set; }

        /// <summary>
        /// Height of image.
        /// </summary>
        public virtual int Height { get; protected set; }

        /// <summary>
        /// Loads header from stream.
        /// </summary>
        /// <param name="stream">Stream to load header from.</param>
        /// <returns>Length of header.</returns>
        protected virtual long Load(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return 0;
        }

        /// <summary>
        /// Provides string representation of header.
        /// </summary>
        /// <returns>String of header properties.</returns>
        public override string ToString()
        {
            // Add some spacing for readability.
            return UsefulThings.General.StringifyObject(this);
        }
    }
}
