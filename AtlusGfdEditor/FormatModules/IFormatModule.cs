using System;
using System.Drawing;
using System.IO;

namespace AtlusGfdEditor.FormatModules
{
    public interface IFormatModule
    {

        /// <summary>
        /// Gets the name of the file format.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the extensions of the file format.
        /// </summary>
        string[] Extensions { get; }

        /// <summary>
        /// Gets the usage flags.
        /// </summary>
        FormatModuleUsageFlags UsageFlags { get; }

        /// <summary>
        /// Gets the object type associated with the format module.
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Checks if the stream is in the correct format and can be imported.
        /// </summary>
        /// <param name="stream">The stream containing data that might or might not be in the expected format.</param>
        /// <returns>If the stream can be imported.</returns>
        bool CanImport( Stream stream, string filename = null );

        /// <summary>
        /// Checks if the file is in the correct format and can be imported.
        /// </summary>
        /// <param name="filepath">The file containing data that might or might not be in the expected format.</param>
        /// <returns>If the file can be imported.</returns>
        bool CanImport( string filepath );

        /// <summary>
        /// Checks if the data is in the correct format and can be imported.
        /// </summary>
        /// <param name="data">The data that might or might not be in the expected format.</param>
        /// <returns>If the data can be imported.</returns>
        bool CanImport( byte[] data, string filename = null );

        /// <summary>
        /// Imports the data from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        object Import( Stream stream, string filename = null );

        /// <summary>
        /// Imports the data from the given file.
        /// </summary>
        /// <param name="filepath">The file containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        object Import( string filepath );

        /// <summary>
        /// Imports the data from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        object Import( byte[] data, string filename = null );

        /// <summary>
        /// Exports the format object to a given stream.
        /// </summary>
        /// <param name="obj">The format object.</param>
        /// <param name="stream">The destination stream.</param>
        void Export( object obj, Stream stream, string filename = null );

        /// <summary>
        /// Exports the format object to a given file path.
        /// </summary>
        /// <param name="obj">The format object.</param>
        /// <param name="filepath">The destination file path.</param>
        void Export( object obj, string filepath );

        /// <summary>
        /// Exports the format object to byte array.
        /// </summary>
        /// <param name="obj">The format object.</param>
        void Export( object obj, out byte[] data, string filename = null );

        /// <summary>
        /// Gets image data for the format object.
        /// </summary>
        /// <param name="obj">The format object.</param>
        /// <returns>Image data.</returns>
        Bitmap GetBitmap( object obj );
    }

    [Flags]
    public enum FormatModuleUsageFlags
    {
        Import           = 0b0001,
        Export           = 0b0010,
        ImportForEditing = 0b0100, 
        Bitmap           = 0b1000,
    }
}