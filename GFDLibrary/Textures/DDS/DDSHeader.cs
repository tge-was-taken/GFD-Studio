using System.IO;
using System.Text;
using GFDLibrary.IO.Common;

namespace GFDLibrary.Textures.DDS
{
    public class DDSHeader
    {
        public const int MAGIC = 0x20534444; // 'DDS '
        public const int SIZE = 0x80;
        public const int SIZE_WITHOUT_MAGIC = SIZE - 4;

        public int Size { get; set; }

        public DDSHeaderFlags Flags { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int PitchOrLinearSize { get; set; }

        public int Depth { get; set; }

        public int MipMapCount { get; set; }

        public int[] Reserved { get; } = new int[11];

        public DDSPixelFormat PixelFormat { get; } = new DDSPixelFormat();

        public DDSHeaderCaps Caps { get; set; }

        public int Caps2 { get; set; }

        public int Caps3 { get; set; }

        public int Caps4 { get; set; }

        public int Reserved2 { get; set; }

        /// <summary>Only for FourCC with DX10</summary>
        public DDSDxgiFormat DxgiFormat { get; set; }
        /// <summary>Only for FourCC with DX10</summary>
        public DDSD3D10ResourceDimension D3D10ResourceDimension { get; set; }
        /// <summary>Only for FourCC with DX10</summary>
        public uint MiscFlag { get; set; }
        /// <summary>Only for FourCC with DX10</summary>
        public uint ArraySize { get; set; }
        /// <summary>Only for FourCC with DX10</summary>
        public DDSD3D10ResourceDimension MiscFlags2 { get; set; }

        public DDSHeader()
        {
            Size = SIZE_WITHOUT_MAGIC;
            Flags = DDSHeaderFlags.Caps | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.PixelFormat;
            Caps  = DDSHeaderCaps.Texture;
        }

        public DDSHeader( int width, int height, DDSPixelFormatFourCC format ) : this()
        {
            Height             =  height;
            Width              =  width;
            PitchOrLinearSize  = DDSFormatDetails.CalculatePitchOrLinearSize( width, height, format, out var additionalFlags );
            Flags              |= additionalFlags;
            PixelFormat.FourCC =  format;
        }

        public DDSHeader( byte[] data ) : this( new MemoryStream( data ), false ) { }

        public DDSHeader( Stream stream, bool leaveOpen = true )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian ) )
                Read( reader );
        }

        public DDSHeader( string file ) : this( File.OpenRead( file ), false ) { }

        public void Save( string file )
        {
            Save( File.OpenWrite( file ), false );
        }

        public void Save( Stream stream, bool leaveOpen = true )
        {
            using ( var writer = new EndianBinaryWriter( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian ) )
                Write( writer );
        }

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            Save( stream );
            return stream;
        }

        internal void Read( BinaryReader reader )
        {
            var magic = reader.ReadInt32();
            if ( magic != MAGIC )
                throw new InvalidDataException( "Header magic value did not match the expected value" );

            Size              = reader.ReadInt32();
            Flags             = ( DDSHeaderFlags )reader.ReadInt32();
            Height            = reader.ReadInt32();
            Width             = reader.ReadInt32();
            PitchOrLinearSize = reader.ReadInt32();
            Depth             = reader.ReadInt32();
            MipMapCount       = reader.ReadInt32();

            for ( var i = 0; i < Reserved.Length; i++ )
            {
                Reserved[i] = reader.ReadInt32();
            }

            PixelFormat.Read( reader );
            Caps      = ( DDSHeaderCaps )reader.ReadInt32();
            Caps2     = reader.ReadInt32();
            Caps3     = reader.ReadInt32();
            Caps4     = reader.ReadInt32();
            Reserved2 = reader.ReadInt32();

            if ( PixelFormat.FourCC == DDSPixelFormatFourCC.DX10 )
            {
                Size                  += sizeof(uint) * 5;
                DxgiFormat             = (DDSDxgiFormat)reader.ReadUInt32();
                D3D10ResourceDimension = (DDSD3D10ResourceDimension)reader.ReadUInt32();
                MiscFlag               = reader.ReadUInt32();
                ArraySize              = reader.ReadUInt32();
                MiscFlags2             = (DDSD3D10ResourceDimension)reader.ReadUInt32();
            }
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( MAGIC );
            writer.Write( Size );
            writer.Write( ( int )Flags );
            writer.Write( Height );
            writer.Write( Width );
            writer.Write( PitchOrLinearSize );
            writer.Write( Depth );
            writer.Write( MipMapCount );

            for ( var i = 0; i < Reserved.Length; i++ )
            {
                writer.Write( Reserved[i] );
            }

            PixelFormat.Write( writer );
            writer.Write( ( int )Caps );
            writer.Write( Caps2 );
            writer.Write( Caps3 );
            writer.Write( Caps4 );
            writer.Write( Reserved2 );
        }
    }
}