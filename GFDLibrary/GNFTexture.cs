using System.IO;
using System.Text;
using GFDLibrary.IO.Common;

namespace GFDLibrary
{
    /// <summary>
    /// PS4 Texture Format
    /// </summary>
    public class GNFTexture
    {
        private const int MAGIC = 0x20464E47; // GNF\0x20

        public int Field04 { get; set; }
        public byte Field08 { get; set; }
        public byte Field09 { get; set; }
        public byte Field0A { get; set; }
        public byte Field0B { get; set; }
        public int Field10 { get; set; }
        public short Field14 { get; set; }
        public byte PixelFormat { get; set; } 
        public byte Field17 { get; set; }
        public int Field18 { get; set; }
        public uint Field1C { get; set; }
        public int Field20 { get; set; }
        public int Field24 { get; set; }
        public int Field28 { get; set; }
        public byte[] Data { get; set; }

        public short Width
        {
            get => ( short )( ( Field18 & 0x3FFF ) + 1 );
            set
            {
                Field18 = ( Field18 & ~0x3FFF ) | ( value - 1 ) & 0x3FFF;
                Field20 = ( Field20 & ~( 0x3FFF << 13 ) | ( ( value - 1 ) & 0x3FFF ) << 13 );
            }
        }

        public short Height
        {
            get => ( short ) ( ( ( Field18 >> 14 ) & 0x3FFF ) + 1 );
            set => Field18 = Field18 & ~( 0x3FFF << 14 ) | ( ( value - 1 ) & 0x3FFF ) << 14;
        }

        public GNFTexture( TexturePixelFormat format, byte mipMapCount, short width, short height, byte[] data )
        {
            Field04 = 0xF8;
            Field08 = 0x02;
            Field09 = 0x01;
            Field0A = 0x08;
            Field0B = 0x00;
            Field10 = 0;
            Field14 = 8;
            PixelFormat = ( byte ) ( format == TexturePixelFormat.DXT1 ? 0x30 : 0x50 );
            Field17 = 0x02;
            Field18 = 0x707FC1FF;
            Field1C = 0x96D20FAC;
            Field24 = 0;
            Field28 = 0;
            Data = data;
            Width = width;
            Height = height;
        }

        public GNFTexture( string filePath )
        {
            using ( var reader = new EndianBinaryReader( File.OpenRead( filePath ), Endianness.LittleEndian ) )
                Read( reader );
        }

        public GNFTexture( Stream stream, bool leaveOpen )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, Endianness.LittleEndian ) )
                Read( reader );
        }

        public void Save( string filePath )
        {
            using ( var writer = new EndianBinaryWriter( File.Create( filePath ), Endianness.LittleEndian ) )
                Write( writer );
        }

        public void Save( Stream stream )
        {
            using ( var writer = new EndianBinaryWriter( stream, Encoding.Default, true, Endianness.LittleEndian ) )
                Write( writer );
        }

        internal void Read( EndianBinaryReader reader )
        {
            var magic = reader.ReadInt32();
            if ( magic != MAGIC )
                throw new InvalidDataException();

            Field04 = reader.ReadInt32();
            Field08 = reader.ReadByte();
            Field09 = reader.ReadByte();
            Field0A = reader.ReadByte();
            Field0B = reader.ReadByte();
            var size = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field14 = reader.ReadInt16();
            PixelFormat = reader.ReadByte();
            Field17 = reader.ReadByte();
            Field18 = reader.ReadInt32();
            Field1C = reader.ReadUInt32();
            Field20 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            Field28 = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            reader.SeekCurrent( 0xD0 );
            Data = reader.ReadBytes( dataSize );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            long startPosition = writer.BaseStream.Position;

            writer.Write( MAGIC );
            writer.Write( Field04 );
            writer.Write( Field08 );
            writer.Write( Field09 );
            writer.Write( Field0A );
            writer.Write( Field0B );
            var fileSizePosition = writer.BaseStream.Position; writer.Write( 0 );
            writer.Write( Field10 );
            writer.Write( Field14 );
            writer.Write( PixelFormat );
            writer.Write( Field17 );
            writer.Write( Field18 );
            writer.Write( Field1C );
            writer.Write( Field20 );
            writer.Write( Field24 );
            writer.Write( Field28 );
            writer.Write( Data.Length );
            writer.SeekCurrent( 0xD0 );
            writer.Write( Data );

            var endPosition = writer.Position;
            var fileSize = writer.Position - startPosition;
            writer.SeekBegin( fileSizePosition );
            writer.Write( ( int ) fileSize );
            writer.SeekBegin( endPosition );
        }
    }
}