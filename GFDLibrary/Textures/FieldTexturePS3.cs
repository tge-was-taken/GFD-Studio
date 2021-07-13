using System;
using System.Drawing;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;
using GFDLibrary.Textures.DDS;

namespace GFDLibrary.Textures
{
    public class FieldTexturePS3
    {
        public int Field00 { get; set; }

        public int DataLength => Data.Length;

        public int Field08 { get; set; }

        public int Field0C { get; set; }

        // Data offset

        // Data length again

        public FieldTextureFlags Flags { get; set; }

        public byte MipMapCount { get; set; }

        public byte Field1A { get; set; }

        public byte Field1B { get; set; }

        public int Field1C { get; set; }

        public short Width { get; set; }
        
        public short Height { get; set; }

        public short Field24 { get; set; }

        public byte[] Data { get; set; }

        public FieldTexturePS3( TexturePixelFormat format, byte mipMapCount, short width, short height, byte[] data )
        {
            Field00 = 0x020200FF;
            Field08 = 0x00000001;
            Field0C = 0x00000000;
            if (format != TexturePixelFormat.BC2 && format != TexturePixelFormat.BC3)
            {
                Flags = FieldTextureFlags.Flag2 | FieldTextureFlags.Flag4 | FieldTextureFlags.Flag80;
            }

            if ( format == TexturePixelFormat.BC2 )
            {
                Flags |= FieldTextureFlags.DXT3 | FieldTextureFlags.Flag80;
            }
            else if ( format == TexturePixelFormat.BC3 )
            {
                Flags |= FieldTextureFlags.DXT5 | FieldTextureFlags.Flag80;
            }

            MipMapCount = mipMapCount;
            Field1A = 2;
            Field1B = 0;
            Field1C = 0xAAE4;
            Width = width;
            Height = height;
            Field24 = 1;
            Data = data;
        }

        public FieldTexturePS3( Bitmap bitmap )
        {
            Field00 = 0x020200FF;
            Field08 = 0x00000001;
            Field0C = 0x00000000;
            Flags   = FieldTextureFlags.Flag2 | FieldTextureFlags.Flag4 | FieldTextureFlags.Flag80;

            var ddsFormat = DDSCodec.DetermineBestCompressedFormat( bitmap );
            var data = DDSCodec.CompressPixelData( bitmap, ddsFormat );

            if ( ddsFormat == DDSPixelFormatFourCC.DXT3 )
            {
                Flags |= FieldTextureFlags.DXT3;
            }
            else if ( ddsFormat == DDSPixelFormatFourCC.DXT5 )
            {
                Flags |= FieldTextureFlags.DXT5;
            }

            MipMapCount = ( byte )1;
            Field1A     = 2;
            Field1B     = 0;
            Field1C     = 0xAAE4;
            Width       = ( short )bitmap.Width;
            Height      = ( short )bitmap.Height;
            Field24     = 1;
            Data        = data;
        }

        public FieldTexturePS3( byte[] ddsData )
        {
            Field00 = 0x020200FF;
            Field08 = 0x00000001;
            Field0C = 0x00000000;
            Flags   = FieldTextureFlags.Flag2 | FieldTextureFlags.Flag4 | FieldTextureFlags.Flag80;
            var ddsHeader = new DDSHeader( ddsData );
            var data = new byte[ddsData.Length - ddsHeader.Size - 4];
            Array.Copy( ddsData, ddsHeader.Size + 4, data, 0, data.Length );

            if ( ddsHeader.PixelFormat.FourCC == DDSPixelFormatFourCC.DXT3 )
            {
                Flags |= FieldTextureFlags.DXT3;
            }
            else if ( ddsHeader.PixelFormat.FourCC == DDSPixelFormatFourCC.DXT5 )
            {
                Flags |= FieldTextureFlags.DXT5;
            }

            MipMapCount = ( byte )ddsHeader.MipMapCount;
            Field1A     = 2;
            Field1B     = 0;
            Field1C     = 0xAAE4;
            Width       = ( short )ddsHeader.Width;
            Height      = ( short )ddsHeader.Height;
            Field24     = 1;
            Data        = data;
        }

        public FieldTexturePS3( Stream stream )
        {
            Read( stream, true );
        }

        public FieldTexturePS3( string filepath )
        {
            using ( var fileStream = File.OpenRead( filepath ) )
            {
                Read( fileStream, false );
            }
        }

        public void Save( Stream stream, bool leaveOpen = true )
        {
            Write( stream, true );
        }

        public void Save( string filepath )
        {
            using ( var fileStream = FileUtils.Create( filepath ) )
            {
                Write( fileStream, false );
            }
        }

        private void Read( Stream stream, bool leaveOpen )
        {
            using ( var reader = new EndianBinaryReader( stream, Encoding.Default, leaveOpen, Endianness.BigEndian ) )
            {
                long startPosition = stream.Position;

                Field00 = reader.ReadInt32();
                int dataLength = reader.ReadInt32();
                Field08 = reader.ReadInt32();
                Field0C = reader.ReadInt32();
                int dataOffset = reader.ReadInt32();
                int dataLength2 = reader.ReadInt32();
                Flags = ( FieldTextureFlags )reader.ReadByte();
                MipMapCount = reader.ReadByte();
                Field1A = reader.ReadByte();
                Field1B = reader.ReadByte();
                Field1C = reader.ReadInt32();
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                Field24 = reader.ReadInt16();

                reader.Seek( startPosition + dataOffset, SeekOrigin.Begin );
                Data = reader.ReadBytes( dataLength );
            }
        }

        private void Write( Stream stream, bool leaveOpen )
        {
            using ( var writer = new EndianBinaryWriter( stream, Encoding.Default, leaveOpen, Endianness.BigEndian ) )
            {
                long startPosition = stream.Position;
                const int dataOffset = 0x80;

                writer.Write( Field00 );
                writer.Write( Data.Length );
                writer.Write( Field08 );
                writer.Write( Field0C );
                writer.Write( dataOffset );
                writer.Write( Data.Length );
                writer.Write( (byte)Flags );
                writer.Write( MipMapCount );
                writer.Write( Field1A );
                writer.Write( Field1B );
                writer.Write( Field1C );
                writer.Write( Width );
                writer.Write( Height );
                writer.Write( Field24 );

                writer.Seek( startPosition + dataOffset, SeekOrigin.Begin );
                writer.Write( Data );
            }
        }
    }

    [Flags]
    public enum FieldTextureFlags
    {
        DXT3   = 1 << 0,
        Flag2  = 1 << 1,
        Flag4  = 1 << 2,
        DXT5   = 1 << 3,
        Flag10 = 1 << 4,
        Flag20 = 1 << 5,
        Flag40 = 1 << 6,
        Flag80 = 1 << 7,
    }
}
