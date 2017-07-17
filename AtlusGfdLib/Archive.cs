using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AtlusGfdLib.IO;

namespace AtlusGfdLib
{
    public class Archive : IDisposable, IEnumerable<string>
    {
        private long mStreamStartPosition;
        private Stream mStream;
        private Dictionary<string, ArchiveEntry> mEntryMap;
        private bool mOwnsStream = true;

        public Archive(string filepath)
        {
            mStream = File.OpenRead( filepath );
            ReadEntryHeaders();
        }

        public Archive(Stream stream, bool ownsStream = true)
        {
            mStreamStartPosition = stream.Position;
            mStream = stream;
            mOwnsStream = ownsStream;

            ReadEntryHeaders();
        }

        public Archive(byte[] data)
        {
            mStream = new MemoryStream( data );
            ReadEntryHeaders();
        }

        public SubStream this[string fileName] => OpenFile( fileName );

        public IEnumerable<string> EnumerateFiles() => mEntryMap.Select(x => x.Key);

        public SubStream OpenFile( string fileName )
        {
            var entry = mEntryMap[fileName];
            return new SubStream( mStream, entry.DataPosition, entry.Length );
        }

        public void Dispose()
        {
            mStream.Dispose();
        }

        public void SaveToFile( string filePath )
        {
            using ( var fileStream = File.Create( filePath ) )
            {
                mStream.Position = 0;
                mStream.CopyTo( fileStream );
            }
        }

        public void SaveToStream( Stream stream )
        {
            mStream.Position = 0;
            mStream.CopyTo( stream );
        }

        private void ReadEntryHeaders()
        {
            mEntryMap = new Dictionary<string, ArchiveEntry>();
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
            mReader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian );
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

            if ( fileName.Length == 0 && length == 0 )
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
                mReader.Position = AlignmentHelper.Align( mReader.Position + entry.Length, 64 );
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
            mWriter.Position = AlignmentHelper.Align( mWriter.Position, 64 );
        }

        private void WriteTerminatorEntry()
        {
            mWriter.Position += 255;
            mWriter.Write( ( byte )0 );
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
            WriteArchive( File.Create( filePath ), false );
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

            stream.Position = 0;
        }
    }
}
