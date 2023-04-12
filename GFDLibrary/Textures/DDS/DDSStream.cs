using System.IO;

namespace GFDLibrary.Textures.DDS
{
#pragma warning disable S101 // Types should be named in PascalCase
    public sealed class DDSStream : Stream
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private readonly Stream mStream;
        private readonly DDSHeader mHeader;

        public DDSHeaderFlags Flags
        {
            get => mHeader.Flags;
            set => mHeader.Flags = value;
        }

        public int Height
        {
            get => mHeader.Height;
            set => mHeader.Height = value;
        }

        public int Width
        {
            get => mHeader.Width;
            set => mHeader.Width = value;
        }

        public int PitchOrLinearSize
        {
            get => mHeader.PitchOrLinearSize;
            set => mHeader.PitchOrLinearSize = value;
        }

        public int Depth
        {
            get => mHeader.Depth;
            set => mHeader.Depth = value;
        }

        public int MipMapCount
        {
            get => mHeader.MipMapCount;
            set => mHeader.MipMapCount = value;
        }

        public int[] Reserved => mHeader.Reserved;

        public DDSPixelFormat PixelFormat => mHeader.PixelFormat;

        public DDSHeaderCaps Caps
        {
            get => mHeader.Caps;
            set => mHeader.Caps = value;
        }

        public int Caps2
        {
            get => mHeader.Caps2;
            set => mHeader.Caps2 = value;
        }

        public int Caps3
        {
            get => mHeader.Caps3;
            set => mHeader.Caps3 = value;
        }

        public int Caps4
        {
            get => mHeader.Caps4;
            set => mHeader.Caps4 = value;
        }

        public int Reserved2
        {
            get => mHeader.Reserved2;
            set => mHeader.Reserved2 = value;
        }

        public DDSStream( Stream stream )
        {
            mStream = stream;
            mHeader = new DDSHeader( stream );
        }

        public DDSStream( string filePath ) : this( File.OpenRead( filePath ) )
        {
        }

        public override void Flush() => mStream.Flush();

        public override long Seek( long offset, SeekOrigin origin ) 
            => mStream.Seek( offset, origin );

        public override void SetLength( long value ) 
            => mStream.SetLength( value );

        public override int Read( byte[] buffer, int offset, int count ) 
            => mStream.Read( buffer, offset, count );

        public override void Write( byte[] buffer, int offset, int count ) 
            => mStream.Write( buffer, offset, count );

        public override bool CanRead => mStream.CanRead;

        public override bool CanSeek => mStream.CanSeek;

        public override bool CanWrite => mStream.CanWrite;

        public override long Length => mStream.Length;

        public override long Position
        {
            get => mStream.Position;
            set => mStream.Position = value;
        }

        public MemoryStream GetPixelDataStream()
        {
            var savedPosition = mStream.Position;
            //The Size read by DDSHeader does not include the length of MAGIC (32 bits).
            //In order to correctly obtain PixelData in subsequent steps,
            //the Size here needs to be increased by the length of MAGIC (32 bits), which is 4.
            mStream.Position = mHeader.Size += sizeof( int );
            var dataStream = new MemoryStream();
            mStream.CopyTo( dataStream );
            mStream.Position = savedPosition;
            return dataStream;
        }

        public byte[] GetPixelData()
        {       
            return GetPixelDataStream().ToArray();
        }

        public void Save( string filePath )
        {
            using ( var fileStream = File.Create( filePath ) )
                Save( fileStream );
        }

        public void Save( Stream fileStream )
        {
            var savedPosition = mStream.Position;
            mStream.Position = 0;
            mStream.CopyTo( fileStream );
            mStream.Position = savedPosition;
        }
    }
}
