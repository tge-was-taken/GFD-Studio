using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;

namespace GFDLibrary
{
    public class FieldTexture
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

        public FieldTexture( TexturePixelFormat format, byte mipMapCount, short width, short height, byte[] data )
        {
            Field00 = 0x020200FF;
            Field08 = 0x00000001;
            Field0C = 0x00000000;
            Flags = FieldTextureFlags.Flag2 | FieldTextureFlags.Flag4 | FieldTextureFlags.Flag80;

            if ( format == TexturePixelFormat.DXT3 )
            {
                Flags |= FieldTextureFlags.DXT3;
            }
            else if ( format == TexturePixelFormat.DXT5 )
            {
                Flags |= FieldTextureFlags.DXT5;
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

        public FieldTexture( Stream stream )
        {
            Read( stream, true );
        }

        public FieldTexture( string filepath )
        {
            using ( var fileStream = File.OpenRead( filepath ) )
            {
                Read( fileStream, false );
            }
        }

        public void Save( Stream stream )
        {
            Write( stream, true );
        }

        public void Save( string filepath )
        {
            using ( var fileStream = File.Create( filepath ) )
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
                Debug.Assert( ( reader.Position - startPosition ) == 0x26 );

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
                Debug.Assert( ( writer.Position - startPosition ) == 0x26 );

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
