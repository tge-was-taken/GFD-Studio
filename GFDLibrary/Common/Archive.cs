using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GFDLibrary.IO.Common;

namespace GFDLibrary.Common
{
    public class Archive : IDisposable, IEnumerable<string>
    {
        public static bool IsValidArchive( string filepath )
        {
            using ( var stream = File.OpenRead( filepath ) )
                return IsValidArchive( stream );
        }

        public static bool IsValidArchive( byte[] data )
        {
            using ( var stream = new MemoryStream( data ) )
                return IsValidArchive( stream );
        }

        public static bool IsValidArchive( Stream stream )
        {
            // Check if the stream is smaller than the size of an entry
            if ( stream.Length < 255 )
                return false;

            using ( var reader = new ArchiveReader( stream, true ))
            {
                // Read first entry, if this succeeds it's probably a valid archive
                return reader.ReadEntryHeader( out var dummy );
            }
        }

        private long mStreamStartPosition;
        private Stream mStream;
        private Dictionary<string, ArchiveEntry> mEntryMap;
        private bool mOwnsStream;

        public Archive(string filepath)
        {
            Initialize( File.OpenRead( filepath ), true );
        }

        public Archive(Stream stream, bool ownsStream = true)
        {
            Initialize( stream, ownsStream );
        }

        public Archive(byte[] data)
        {
            Initialize( new MemoryStream( data ), true );
        }

        public StreamView this[string fileName] => OpenFile( fileName );

        public IEnumerable<string> EnumerateFiles() => mEntryMap.Select(x => x.Key);

        public bool TryOpenFile( string filename, out StreamView stream )
        {
            if ( !mEntryMap.TryGetValue( filename, out var entry ) )
            {
                stream = null;
                return false;
            }

            stream = new StreamView( mStream, entry.DataPosition, entry.Length );
            return true;
        }

        public StreamView OpenFile( string filename )
        {
            if ( !TryOpenFile( filename, out var stream))
            {
                throw new Exception( "File does not exist" );
            }

            return stream;
        }

        public void Dispose()
        {
            mStream.Dispose();
        }

        public void Save( string filePath )
        {
            using ( var fileStream = File.Create( filePath ) )
            {
                mStream.Position = 0;
                mStream.CopyTo( fileStream );
            }
        }

        public void Save( Stream stream )
        {
            mStream.Position = 0;
            mStream.CopyTo( stream );
        }

        private void Initialize( Stream stream, bool ownsStream )
        {
            mStreamStartPosition = stream.Position;
            mStream = stream;
            mOwnsStream = ownsStream;
            mEntryMap = new Dictionary<string, ArchiveEntry>();

            ReadEntryHeaders();
        }

        private void ReadEntryHeaders()
        {
            var reader = new ArchiveReader( mStream, true );

            while ( reader.ReadEntryHeader( out var entry ) ) 
            {
                mEntryMap[entry.FileName] = entry;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return EnumerateFiles().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal struct ArchiveEntry
    {
        public string FileName;
        public int Length;
        public long DataPosition;
    }

    internal class ArchiveReader : IDisposable
    {
        private EndianBinaryReader mReader;
        private StringBuilder mStringBuilder;

        public ArchiveReader( Stream stream, bool leaveOpen = false )
        {
            mReader = new EndianBinaryReader( stream, Encoding.ASCII, leaveOpen, Endianness.LittleEndian );
            mStringBuilder = new StringBuilder();
        }

        public bool ReadEntryHeader( out ArchiveEntry entry, bool skipData = true )
        {
            long entryStartPosition = mReader.Position;
            if ( entryStartPosition == mReader.BaseStreamLength )
            {
                entry = new ArchiveEntry();
                return false;
            }

            // read entry name
            while ( true )
            {
                byte b = mReader.ReadByte();
                if ( b == 0 )
                    break;

                mStringBuilder.Append( ( char )b );

                // just to be safe
                if ( mStringBuilder.Length == 252 )
                    break;
            }

            string fileName = mStringBuilder.ToString();

            // set position to length field
            mReader.Position = entryStartPosition + 252;

            // read entry length
            int length = mReader.ReadInt32();

            if ( fileName.Length == 0 || length <= 0 || length > 1024 * 1024 * 100 )
            {
                entry = new ArchiveEntry();
                return false;
            }

            // make an entry
            entry.FileName = fileName;
            entry.Length = length;
            entry.DataPosition = mReader.Position;

            // clear string builder for next iteration
            mStringBuilder.Clear();

            if ( skipData )
            {
                mReader.Position = AlignmentUtilities.Align( mReader.Position + entry.Length, 64 );
            }

            return true;
        }

        public void Dispose()
        {
            ( ( IDisposable )mReader ).Dispose();
        }
    }

    internal class ArchiveWriter : IDisposable
    {
        private EndianBinaryWriter mWriter;

        public ArchiveWriter( Stream stream, bool leaveOpen = false )
        {
            mWriter = new EndianBinaryWriter( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian );
        }

        public void WriteEntry( string fileName, Stream stream )
        {
            WriteFileName( fileName );
            WriteFileData( stream );
        }

        public void WriteEntry( string fileName, byte[] data )
        {
            WriteFileName( fileName );
            WriteFileData( data );
        }

        private void WriteFileName( string fileName )
        {
            mWriter.Write( fileName, StringBinaryFormat.FixedLength, 252 );
        }

        private void WriteFileData( Stream stream )
        {
            mWriter.Write( ( int )stream.Length );
            stream.Position = 0;
            stream.CopyTo( mWriter.BaseStream );
            WriteEntryAlignment();
        }

        private void WriteFileData( byte[] data )
        {
            mWriter.Write( data.Length );
            mWriter.Write( data );
            WriteEntryAlignment();
        }

        private void WriteEntryAlignment()
        {
            mWriter.Position = AlignmentUtilities.Align( mWriter.Position, 64 );
        }

        private void WriteTerminatorEntry()
        {
            for ( int i = 0; i < 0x100; i++ )
            {
                mWriter.Write( ( byte )0 );
            }
        }

        public void Dispose()
        {
            WriteTerminatorEntry();
            mWriter.Dispose();
        }
    }

    public class ArchiveBuilder
    {
        private List<(string FilePath, Stream Stream)> mFiles;

        public ArchiveBuilder()
        {
            mFiles = new List<(string FilePath, Stream Stream)>();
        }

        public ArchiveBuilder AddFile( string filePath )
        {
            AddFile( filePath, null );
            return this;
        }

        public ArchiveBuilder AddFile( string fileName, Stream stream )
        {
            mFiles.Add( (fileName, stream) );
            return this;
        }

        public ArchiveBuilder AddDirectory( string directoryPath, bool recurse = true )
        {
            return AddDirectory( directoryPath, "*.*", recurse );
        }

        public ArchiveBuilder AddDirectory( string directoryPath, string searchPattern, bool recurse = true )
        {
            if ( !Directory.Exists( directoryPath ) )
                throw new DirectoryNotFoundException( directoryPath );

            foreach ( var filePath in Directory.EnumerateFiles( directoryPath, searchPattern, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ) )
            {
                AddFile( filePath );
            }

            return this;
        }

        public Archive Build()
        {
            var stream = new MemoryStream();
            WriteArchive( stream, true );

            return new Archive( stream, true );
        }

        public void BuildFile( string filePath )
        {
            WriteArchive( FileUtils.Create( filePath ), false );
        }

        public Stream BuildStream()
        {
            var stream = new MemoryStream();
            WriteArchive( stream, true );

            return stream;
        }

        private void WriteArchive( Stream stream, bool leaveOpen )
        {
            using ( var writer = new ArchiveWriter( stream, leaveOpen ) )
            {
                foreach ( var file in mFiles )
                {
                    var fileName = Path.GetFileName( file.FilePath );

                    if ( file.Stream == null )
                    {
                        using ( var fileStream = File.OpenRead( file.FilePath ) )
                        {
                            writer.WriteEntry( fileName, fileStream );
                        }
                    }
                    else
                    {
                        writer.WriteEntry( fileName, file.Stream );
                    }
                }
            }

            if ( leaveOpen )
                stream.Position = 0;
        }
    }
}
