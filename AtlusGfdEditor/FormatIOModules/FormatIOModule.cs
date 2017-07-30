using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.FormatIOModules
{
    public abstract class FormatIOModule<T> : IFormatIOModule
    {
        /// <summary>
        /// Gets the name of the file format.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the file format.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the extensions of the file format.
        /// </summary>
        public abstract string[] Extensions { get; }

        /// <summary>
        /// Gets the object type associated with the format module.
        /// </summary>
        public Type ObjectType => typeof( T );

        /// <summary>
        /// Gets the usage flags.
        /// </summary>
        public abstract FormatModuleUsageFlags UsageFlags { get; }

        /// <summary>
        /// Checks if the stream is in the correct format and can be imported.
        /// </summary>
        /// <param name="stream">The stream containing data that might or might not be in the expected format.</param>
        /// <returns>If the stream can be imported.</returns>
        public bool CanImport( Stream stream, string filename = null )
        {
            // check if the module supports importing
            if ( !UsageFlags.HasFlag( FormatModuleUsageFlags.Import ) )
                return false;

            // if a filename was given, match the extension of it with the known extensions first
            if ( filename != null && !Utilities.MatchExtension( filename, Extensions ) )
                return false;

            return CanImportInternal( stream, filename );
        }

        /// <summary>
        /// Checks if the file is in the correct format and can be imported.
        /// </summary>
        /// <param name="filepath">The file containing data that might or might not be in the expected format.</param>
        /// <returns>If the file can be imported.</returns>
        public bool CanImport( string filepath )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return CanImport( stream, Path.GetFileName(filepath) );
            }
        }

        /// <summary>
        /// Checks if the data is in the correct format and can be imported.
        /// </summary>
        /// <param name="data">The data that might or might not be in the expected format.</param>
        /// <returns>If the data can be imported.</returns>
        public bool CanImport( byte[] bytes, string filename = null )
        {
            using ( var stream = new MemoryStream( bytes ) )
            {
                return CanImport( stream, filename );
            }
        }


        /// <summary>
        /// Imports the data from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        public T Import( Stream stream, string filename = null )
        {
            // check if the module supports importing
            if ( !UsageFlags.HasFlag( FormatModuleUsageFlags.Import ) )
                throw new NotSupportedException( "Module does not provide import capabilities" );

            return ImportInternal( stream, filename );
        }

        /// <summary>
        /// Imports the data from the given file.
        /// </summary>
        /// <param name="filepath">The file containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        public T Import( string filepath )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return Import( stream, Path.GetFileName( filepath ) );
            }
        }

        /// <summary>
        /// Imports the data from the given stream.
        /// </summary>
        /// <param name="stream">The stream containing the format data.</param>
        /// <returns>An object whose type is associated with this format module.</returns>
        public T Import( byte[] bytes, string filename = null )
        {
            using ( var stream = new MemoryStream( bytes ) )
            {
                return Import( stream, filename );
            }
        }

        /// <summary>
        /// Exports the format object to a given stream.
        /// </summary>
        /// <param name="obj">The format object.</param>
        /// <param name="stream">The destination stream.</param>
        public void Export( T obj, Stream stream, string filename = null )
        {
            // check if the module supports exporting
            if ( !UsageFlags.HasFlag( FormatModuleUsageFlags.Export ) )
                throw new NotSupportedException( "Module does not provide export capabilities" );

            ExportInternal( obj, stream, filename );
        }

        /// <summary>
        /// Exports the format object to a given file path.
        /// </summary>
        /// <param name="obj">The format object.</param>
        /// <param name="filepath">The destination file path.</param>
        public void Export( T obj, string filepath )
        {
            using ( var stream = File.Create( filepath ) )
            {
                Export( obj, stream, Path.GetFileName( filepath ) );
            }
        }

        /// <summary>
        /// Exports the format object to byte array.
        /// </summary>
        /// <param name="obj">The format object.</param>
        public void Export( T obj, out byte[] data, string filename = null )
        {
            using ( var stream = new MemoryStream() )
            {
                Export( obj, stream, filename );

                data = stream.ToArray();
            }
        }

        protected abstract bool CanImportInternal( Stream stream, string filename = null );

        protected abstract T ImportInternal( Stream stream, string filename = null );

        protected abstract void ExportInternal( T obj, Stream stream, string filename = null );

        //
        // IFormatModule implementation
        //

        object IFormatIOModule.Import( Stream stream, string filename = null ) 
            => Import( stream, filename );

        object IFormatIOModule.Import( string filepath ) 
            => Import( filepath );

        object IFormatIOModule.Import( byte[] bytes, string filename = null ) 
            => Import( bytes, filename );

        void IFormatIOModule.Export( object obj, Stream stream, string filename = null ) 
            => Export( ( T )obj, stream, filename );

        void IFormatIOModule.Export( object obj, string filepath ) 
            => Export( ( T )obj, filepath );

        void IFormatIOModule.Export( object obj, out byte[] data, string filename = null ) 
            => Export( ( T )obj, out data, filename );
    }
}
