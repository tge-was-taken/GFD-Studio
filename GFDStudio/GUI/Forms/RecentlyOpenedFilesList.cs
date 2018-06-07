using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public class RecentlyOpenedFilesList
    {
        private readonly List<string> mFiles = new List<string>();
        private readonly ToolStripItemCollection mCollection;
        private readonly EventHandler mClickHandler;

        public IEnumerable<string> Files => mFiles;

        public int MaxFileCount { get; set; }

        public int Count => mFiles.Count;

        public string Last => mFiles[mFiles.Count - 1];

        public RecentlyOpenedFilesList( string path, int maxFileCount, ToolStripItemCollection collection, EventHandler clickHandler )
        {
            MaxFileCount = maxFileCount;
            mCollection = collection;
            mClickHandler = clickHandler;

            if ( File.Exists( path ) )
            {
                foreach ( string line in File.ReadAllLines( path ) )
                    Add( line );
            }
        }

        public void Add( string path )
        {
            // Don't add the file if it is already on top of the stack, prevents duplicates if you
            // open the same file multiple times consecutively.
            if ( Count != 0 && Last == path )
                return;

            mFiles.Add( path );

            // Pop entries if we have too many files
            if ( mFiles.Count > MaxFileCount )
            {
                mFiles.RemoveRange( 0, ( mFiles.Count - MaxFileCount ) );
            }

            // Add menu item to the collection
            var item = new ToolStripMenuItem( path );
            item.Click += mClickHandler;
            mCollection.Insert( 0, item );
        }

        public void Save( string path )
        {
            using ( var writer = File.CreateText( path ) )
            {
                // Write list of files backwards from least recent to most recent
                foreach ( var filePath in mFiles )
                    writer.WriteLine( filePath );
            }
        }
    }
}