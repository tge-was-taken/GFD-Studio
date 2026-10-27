using System;
using System.IO;

namespace GFDLibrary.IO.Common
{
    public static class FileUtils
    {
        private static void MaybeCreateDirectory( string path )
        {
            var directory = Path.GetDirectoryName( path );
            if ( !string.IsNullOrWhiteSpace( directory ) )
                Directory.CreateDirectory( directory );
        }

        public static FileStream Create( string path )
        {
            MaybeCreateDirectory( path );
            return File.Create( path );
        }

        public static FileStream Create( string path, string originalPath )
        {
            var fullPath = Path.GetFullPath( path );
            var fullOriginalPath = originalPath != null ? Path.GetFullPath( originalPath ) : string.Empty;

            if ( fullPath.Equals( fullOriginalPath, StringComparison.CurrentCulture ) )
            {
                var effectivePath = fullPath;
                while ( File.Exists( effectivePath ) )
                    effectivePath += "_";

                return new ReplacingOriginalFileWorkaroundFileStream( effectivePath, fullPath );
            }
            else
            {
                return Create( fullPath );
            }
        }
    }

    public class ReplacingOriginalFileWorkaroundFileStream : FileStream
    {
        public string DestinationPath { get; }

        public ReplacingOriginalFileWorkaroundFileStream( string path, string destinationPath ) : base( path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose )
        {
            DestinationPath = destinationPath;
        }

        protected override void Dispose( bool disposing )
        {
            if ( File.Exists( Name ) )
                File.Copy( Name, DestinationPath, true );

            base.Dispose( disposing );

            if ( File.Exists( Name ) )
                File.Delete( Name );
        }
    }
}
