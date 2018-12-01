using System;
using System.Diagnostics;
using GFDLibrary.IO;

namespace GFDLibrary.Textures
{
    public sealed class Texture : Resource
    {
        private static readonly byte[] sDummyTextureData =
        {
            0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x00, 0x00,
            0x08, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0x78, 0xFD, 0x89, 0x25, 0x05, 0x05, 0x50, 0x50,
            0x00, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x78, 0xFD, 0x89, 0x25,
            0x05, 0x05, 0x50, 0x50, 0x00, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0x78, 0xFD, 0x89, 0x25, 0x05, 0x05, 0x50, 0x50, 0x00, 0x05, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0x78, 0xFD, 0x89, 0x25, 0x05, 0x05, 0x50, 0x50
        };

        public override ResourceType ResourceType => ResourceType.Texture;

        public string Name { get; set; }

        public TextureFormat Format { get; set; }

        public byte[] Data { get; set; }

        // 1C
        public byte Field1C { get; set; }

        // 1D
        public byte Field1D { get; set; }

        // 1E
        public byte Field1E { get; set; }

        // 1F
        public byte Field1F { get; set; }

        public bool IsDefaultTexture { get; private set; }

        public Texture()
        {          
        }

        public Texture( uint version ) : base( version )
        {
        }

        public Texture( string name, TextureFormat format, byte[] data ) : this()
        {
            Name = name;
            Format = format;
            Data = data;
            Field1C = 1;
            Field1D = 1;
            Field1E = 0;
            Field1F = 0;
        }

        public Texture( string name, TextureFormat format, byte[] data, byte field1c, byte field1d, byte field1e, byte field1f ) : this()
        {
            Name = name;
            Format = format;
            Data = data;
            Field1C = field1c;
            Field1D = field1d;
            Field1E = field1e;
            Field1F = field1f;
        }

        public static Texture CreateDefaultTexture(string name)
        {
            return new Texture( name, TextureFormat.DDS, sDummyTextureData ) { IsDefaultTexture = true };
        }

        internal override void Read( ResourceReader reader )
        {
            Name = reader.ReadString();
            Format = ( TextureFormat )reader.ReadInt16();
            Trace.Assert( Enum.IsDefined( typeof( TextureFormat ), Format ), "Unknown texture format" );
            int size = reader.ReadInt32();
            Data = reader.ReadBytes( size );
            Field1C = reader.ReadByte();
            Field1D = reader.ReadByte();
            Field1E = reader.ReadByte();
            Field1F = reader.ReadByte();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteString( Name );
            writer.WriteInt16( ( short ) Format );
            writer.WriteInt32( Data.Length );
            writer.WriteBytes( Data );
            writer.WriteByte( Field1C );
            writer.WriteByte( Field1D );
            writer.WriteByte( Field1E );
            writer.WriteByte( Field1F );
        }

        public override string ToString() => Name;
    }

    public enum TextureFormat : ushort
    {
        Invalid = 0,
        DDS = 1,
    }
}