using System.Collections.Generic;
using System.IO;
using System.Linq;
using GFDStudio.DataManagement;

namespace GFDStudio.GUI
{
    /// <summary>
    /// The files opened recently, newest last, kept in a file beside the application.
    /// </summary>
    public class FileHistory
    {
        private readonly List<string> mFiles = new();
        private readonly string mPath;

        public FileHistory( string fileName, int maxFileCount )
        {
            mPath = DataStore.GetPath( fileName );
            MaxFileCount = maxFileCount;

            if ( File.Exists( mPath ) )
            {
                foreach ( var line in File.ReadAllLines( mPath ) )
                    Add( line );
            }
        }

        public int MaxFileCount { get; }

        /// <summary>
        /// Newest first, which is the order a menu wants them in.
        /// </summary>
        public IEnumerable<string> Files => Enumerable.Reverse( mFiles );

        public int Count => mFiles.Count;

        public void Add( string path )
        {
            if ( string.IsNullOrWhiteSpace( path ) )
                return;

            mFiles.Remove( path );
            mFiles.Add( path );

            while ( mFiles.Count > MaxFileCount )
                mFiles.RemoveAt( 0 );
        }

        public void Save()
        {
            try
            {
                File.WriteAllLines( mPath, mFiles );
            }
            catch ( IOException )
            {
                // Losing the history is not worth failing a shutdown over.
            }
        }
    }
}
