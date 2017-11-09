using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtlusGfdLib.IO;

namespace AtlusGfdLib
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

        public FieldTexture( bool dxt5, byte mipMapCount, short width, short height, byte[] data )
        {
            Field00 = 0x020200FF;
            Field08 = 0x00000001;
            Field0C = 0x00000000;
            Flags = FieldTextureFlags.Flag2 | FieldTextureFlags.Flag4 | FieldTextureFlags.Flag80;
            if ( dxt5 )
                Flags |= FieldTextureFlags.DXT5;
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
        DXT3   = 0b0000_0001,
        Flag2  = 0b0000_0010,
        Flag4  = 0b0000_0100,
        DXT5   = 0b0000_1000,
        Flag10 = 0b0001_0000,
        Flag20 = 0b0010_0000,
        Flag40 = 0b0100_0000,
        Flag80 = 0b1000_0000,
    }
}
