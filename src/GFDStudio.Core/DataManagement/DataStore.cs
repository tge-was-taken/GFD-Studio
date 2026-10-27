using System;
using System.IO;

namespace GFDStudio.DataManagement
{
    public static class DataStore
    {
        private static readonly string sBaseDirectoryPath = Path.GetFullPath( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "app_data" ) );

        static DataStore()
        {
            if ( !Directory.Exists( sBaseDirectoryPath ) )
                Directory.CreateDirectory( sBaseDirectoryPath );
        }

        public static string GetPath( string relativePath ) => Path.Combine( sBaseDirectoryPath, relativePath );
    }
}
