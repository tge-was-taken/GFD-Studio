using System;
using System.Drawing;
using System.IO;
using System.Text;
using GFDLibrary.IO.Common;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.Swizzle;

namespace GFDLibrary.Textures.GNF
{
    /// <summary>
    /// PS4 Texture Format
    /// </summary>
    public class GNFTexture
    {
        public const int MAGIC = 0x20464E47; // GNF\0x20

        private uint mWord0;
        private uint mWord1;
        private uint mWord2;
        private uint mWord3;
        private uint mWord4;
        private uint mWord5;
        private uint mWord6;

        public byte Version { get; set; }

        public byte Alignment { get; set; }

        public int MinLodClamp
        {
            get => ( int )( mWord1 & 0x000fff00 ) >> 8;
            set => mWord1 = ( ( uint )value & 0x000fff00 ) << 8;
        }

        public SurfaceFormat SurfaceFormat
        {
            get => ( SurfaceFormat ) ( ( int ) ( mWord1 & 0x03f00000 ) >> 20 );
            set => mWord1 = ( ( uint )value & 0x03f00000 ) << 20;
        }

        public ChannelType ChannelType
        {
            get => ( ChannelType ) ( ( int ) ( mWord1 & 0x3c000000 ) >> 26 );
            set => mWord1 = ( ( uint )value & 0x3c000000 ) << 26;
        }

        public int Width
        {
            get => ( ( int )( mWord2 & 0x00003fff ) ) + 1;
            set => mWord2 = ( ( uint ) ( value - 1 ) & 0x00003fff );
        }

        public int Height
        {
            get => ( ( int )( mWord2 & 0x0fffc000 ) >> 14 ) + 1;
            set => mWord2 = ( ( uint )( value - 1 ) & 0x0fffc000 ) << 14;
        }

        public SamplerModulationFactor SamplerModulationFactor
        {
            get => ( SamplerModulationFactor ) ( ( int ) ( mWord2 & 0x70000000 ) >> 28 );
            set => mWord2 = ( ( uint )value & 0x70000000 ) << 28;
        }

        public TextureChannel ChannelOrderX
        {
            get => ( TextureChannel ) ( ( int ) ( mWord3 & 0x00000007 ) );
            set => mWord3 = ( ( uint )value & 0x00000007 );
        }

        public TextureChannel ChannelOrderY
        {
            get => ( TextureChannel ) ( ( int ) ( mWord3 & 0x00000038 ) >> 3 );
            set => mWord3 = ( ( uint )value & 0x00000038 ) << 3;
        }

        public TextureChannel ChannelOrderZ
        {
            get => ( TextureChannel ) ( ( int ) ( mWord3 & 0x000001c0 ) >> 6 );
            set => mWord3 = ( ( uint )value & 0x000001c0 ) << 6;
        }

        public TextureChannel ChannelOrderW
        {
            get => ( TextureChannel ) ( ( int ) ( mWord3 & 0x00000e00 ) >> 9 );
            set => mWord3 = ( ( uint )value & 0x00000e00 ) << 9;
        }

        public int BaseMipLevel
        {
            get => ( int )( mWord3 & 0x0000f000 ) >> 12;
            set => mWord3 = ( ( uint )value & 0x0000f000 ) << 12;
        }

        public int LastMipLevel
        {
            get => ( int )( mWord3 & 0x000f0000 ) >> 16;
            set => mWord3 = ( ( uint )value & 0x000f0000 ) << 16;
        }

        public TileMode TileMode
        {
            get => ( TileMode ) ( ( int ) ( mWord3 & 0x01f00000 ) >> 20 );
            set => mWord3 = ( ( uint )value & 0x01f00000 ) << 20;
        }

        public bool IsPaddedToPow2
        {
            get => ( mWord3 & 0x02000000 ) >> 25 != 0;
            set => mWord3 = ( value ? 1u : 0u & 0x02000000 ) << 25;
        }

        public TextureType TextureType
        {
            get => ( TextureType ) ( ( mWord3 & 0xf0000000 ) >> 28 );
            set => mWord3 = ( ( uint )value & 0xf0000000 ) << 28;
        }

        public int Depth
        {
            get => ( ( int ) ( mWord4 & 0x00001fff ) + 1 );
            set => mWord4 = ( ( uint ) ( value - 1 ) & 0x00001fff );
        }

        public int Pitch
        {
            get => ( ( int )( mWord4 & 0x07ffe000 ) >> 13 ) + 1;
            set => mWord4 = ( ( uint )( value - 1 ) & 0x07ffe000 ) << 13;
        }

        public int BaseArraySliceIndex
        {
            get => ( int )( mWord5 & 0x00001fff );
            set => mWord5 = ( ( uint )value & 0x00001fff );
        }

        public int LastArraySliceIndex
        {
            get => ( int )( mWord5 & 0x03ffe000 ) >> 13;
            set => mWord5 = ( ( uint )value & 0x03ffe000 ) << 13;
        }

        public int MinLodWarning
        {
            get => ( int )( mWord6 & 0x00000fff );
            set => mWord6 = ( ( uint )value & 0x00000fff );
        }

        public int MipStatsCounterIndex
        {
            get => ( int )( mWord6 & 0x000ff000 ) >> 12;
            set => mWord6 = ( ( uint )value & 0x000ff000 ) << 12;
        }

        public bool MipStatsEnabled
        {
            get => ( ( int ) ( mWord6 & 0x00100000 ) >> 20 ) != 0;
            set => mWord6 = ( ( value ? 1u : 0u ) & 0x00100000 ) << 20;
        }

        public bool MetadataCompressionEnabled
        {
            get => ( ( int ) ( mWord6 & 0x00200000 ) >> 21 ) != 0;
            set => mWord6 = ( value ? 1u : 0u & 0x00200000 ) << 21;
        }

        public bool DccAlphaOnMsb
        {
            get => ( ( int ) ( mWord6 & 0x00400000 ) >> 22 ) != 0;
            set => mWord6 = ( value ? 1u : 0u & 0x00400000 ) << 22;
        }

        public int DccColorTransform
        {
            get => ( int )( mWord6 & 0x00800000 ) >> 23;
            set => mWord6 = ( ( uint )value & 0x00800000 ) << 23;
        }

        public bool UseAltTileMode
        {
            get => ( ( int ) ( mWord6 & 0x01000000 ) >> 24 ) != 0;
            set => mWord6 = ( (value ? 1u : 0u) & 0x01000000 ) << 24;
        }

        public byte[] Data { get; set; }

        public GNFTexture( TexturePixelFormat format, byte mipMapCount, short width, short height, byte[] data, bool isSwizzled )
        {
            SurfaceFormat surfaceFormat;
            switch ( format )
            {
                case TexturePixelFormat.BC1:
                    surfaceFormat = SurfaceFormat.BC1;
                    break;
                case TexturePixelFormat.BC2:
                    surfaceFormat = SurfaceFormat.BC2;
                    break;
                case TexturePixelFormat.BC3:
                    surfaceFormat = SurfaceFormat.BC3;
                    break;
                case TexturePixelFormat.BC7:
                    surfaceFormat = SurfaceFormat.BC7;
                    break;
                case TexturePixelFormat.ARGB:
                    surfaceFormat = SurfaceFormat.Format8_8_8_8;
                    break;
                default:
                    throw new NotImplementedException( format.ToString() );
            }

            Init( surfaceFormat, mipMapCount, width, height, data, isSwizzled );
        }

        public GNFTexture( Bitmap bitmap )
        {
            var texture = DDSCodec.Compress( bitmap, DXGIFormat.UNKNOWN );
            var surfaceFormat = GetSurfaceFormat( texture.Format );
            Init( surfaceFormat, 1, (short)bitmap.Width, (short)bitmap.Height, texture.Pixels.ToArray(), false );
        }

        private static SurfaceFormat GetSurfaceFormat( DXGIFormat ddsFormat )
        {
            SurfaceFormat surfaceFormat;
            switch ( ddsFormat )
            {
                case DXGIFormat.BC1_TYPELESS:
                case DXGIFormat.BC1_UNORM:
                case DXGIFormat.BC1_UNORM_SRGB:
                    surfaceFormat = SurfaceFormat.BC1;
                    break;
                case DXGIFormat.BC2_TYPELESS:
                case DXGIFormat.BC2_UNORM:
                case DXGIFormat.BC2_UNORM_SRGB:
                    surfaceFormat = SurfaceFormat.BC2;
                    break;
                case DXGIFormat.BC3_TYPELESS:
                case DXGIFormat.BC3_UNORM:
                case DXGIFormat.BC3_UNORM_SRGB:
                    surfaceFormat = SurfaceFormat.BC3;
                    break;
                case DXGIFormat.BC4_SNORM:
                case DXGIFormat.BC4_TYPELESS:
                case DXGIFormat.BC4_UNORM:
                    surfaceFormat = SurfaceFormat.BC4;
                    break;
                case DXGIFormat.BC5_SNORM:
                case DXGIFormat.BC5_TYPELESS:
                case DXGIFormat.BC5_UNORM:
                    surfaceFormat = SurfaceFormat.BC5;
                    break;

                default:
                    throw new NotSupportedException( ddsFormat.ToString() );
            }

            return surfaceFormat;
        }

        public GNFTexture( DDSStream dds )
        {
            var surfaceFormat = GetSurfaceFormat( dds.PixelFormat.FourCC.ToDXGIFormat() );
            Init( surfaceFormat, ( byte ) dds.MipMapCount, ( short ) dds.Width, ( short ) dds.Height, dds.GetPixelData(), false );
        }

        private void Init( SurfaceFormat format, byte mipMapCount, short width, short height, byte[] data, bool isSwizzled )
        {
            Width  = width;
            Height = height;
            Data   = isSwizzled ? data : Swizzler.Swizzle( data, width, height, format == SurfaceFormat.BC1 ? 8 : 16, SwizzleType.PS4 );
            Alignment                  = 8;
            BaseArraySliceIndex        = 0;
            BaseMipLevel               = 0;
            ChannelOrderX              = TextureChannel.X;
            ChannelOrderY              = TextureChannel.Y;
            ChannelOrderZ              = TextureChannel.Z;
            ChannelOrderW              = TextureChannel.W;
            ChannelType                = ChannelType.UNorm;
            DccAlphaOnMsb              = false;
            DccColorTransform          = 0;
            Depth                      = 1;
            IsPaddedToPow2             = false;
            LastArraySliceIndex        = 0;
            LastMipLevel               = 0;
            MetadataCompressionEnabled = false;
            MinLodClamp                = 0;
            MinLodWarning              = 0;
            MipStatsCounterIndex       = 0;
            MipStatsEnabled            = false;
            Pitch                      = Width;
            SamplerModulationFactor    = SamplerModulationFactor.Factor1_0000;
            TileMode                   = TileMode.Thin_1DThin;
            TextureType                = TextureType.Type2D;
            UseAltTileMode             = false;
            Version                    = 2;
            LastMipLevel               = mipMapCount;
        }

        public GNFTexture( string filePath )
        {
            using ( var reader = new EndianBinaryReader( File.OpenRead( filePath ), Endianness.LittleEndian ) )
                Read( reader );
        }

        public GNFTexture( Stream stream, bool leaveOpen = true )
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
            // Header
            var magic = reader.ReadInt32();
            if ( magic != MAGIC )
                throw new InvalidDataException();

            var contentsSize = reader.ReadInt32();
            var dataOffset = reader.Position + contentsSize;

            // Contents
            Version = reader.ReadByte();

            var textureCount = reader.ReadByte();
            if ( textureCount != 1 )
                throw new NotSupportedException( "GNF textures with more than 1 texture not supported" );

            Alignment = reader.ReadByte();
            var unused = reader.ReadByte();
            var streamSize = reader.ReadInt32();
            var dataSize = streamSize - contentsSize - 8;

            // Texture
            mWord0 = reader.ReadUInt32();
            mWord1 = reader.ReadUInt32();
            mWord2 = reader.ReadUInt32();
            mWord3 = reader.ReadUInt32();
            mWord4 = reader.ReadUInt32();
            mWord5 = reader.ReadUInt32();
            mWord6 = reader.ReadUInt32();
            var metadataOffset = reader.ReadUInt32();
            reader.SeekBegin( dataOffset );
            Data = reader.ReadBytes( dataSize );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            long startPosition = writer.BaseStream.Position;

            writer.Write( MAGIC );
            writer.Write( 0xF8 ); // contents size
            writer.Write( Version );
            writer.Write( ( byte ) 1 ); // texture count
            writer.Write( Alignment );
            writer.Write( ( byte ) 0 ); // unused
            var streamSizePosition = writer.BaseStream.Position; writer.Write( 0 ); // stream size
            writer.Write( mWord0 );
            writer.Write( mWord1 );
            writer.Write( mWord2 );
            writer.Write( mWord3 );
            writer.Write( mWord4 );
            writer.Write( mWord5 );
            writer.Write( mWord6 );
            writer.Write( Data.Length ); // aka metadata address

            writer.SeekBegin( startPosition + 0x100 );
            writer.Write( Data );

            var endPosition = writer.Position;
            var fileSize = writer.Position - startPosition;
            writer.SeekBegin( streamSizePosition );
            writer.Write( ( int ) fileSize );
            writer.SeekBegin( endPosition );
        }
    }
}