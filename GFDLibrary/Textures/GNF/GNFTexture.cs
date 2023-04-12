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
        private uint mWord1 = 0x00000008;
        private uint mWord2;
        private uint mWord3 = 0x04000000;
        private uint mWord4;
        private uint mWord5;
        private uint mWord6;

        public byte Version { get; set; }

        public byte Alignment { get; set; }

        public int MinLodClamp
        {
            get => (int)( mWord1 & 0x000fff00 ) >> 8;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x000fff00;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 8 ) & 0x000fff00;
                mWord1 = ( mWord1 & mask ) | newVal;
            }
        }

        public SurfaceFormat SurfaceFormat
        {
            get => (SurfaceFormat)( (int)( mWord1 & 0x03f00000 ) >> 20 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x03f00000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 20 ) & 0x03f00000;
                mWord1 = ( mWord1 & mask ) | newVal;
            }
        }

        public ChannelType ChannelType
        {
            get => (ChannelType)( (int)( mWord1 & 0x3c000000 ) >> 26 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x3c000000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 26 ) & 0x3c000000;
                mWord1 = ( mWord1 & mask ) | newVal;
            }
        }

        public int Width
        {
            get => ( (int)( mWord2 & 0x00003fff ) ) + 1;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00003fff;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = (uint)( value - 1 ) & 0x00003fff;
                mWord2 = ( mWord2 & mask ) | newVal;
            }
        }

        public int Height
        {
            get => ( (int)( mWord2 & 0x0fffc000 ) >> 14 ) + 1;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x0fffc000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)( value - 1 ) << 14 ) & 0x0fffc000;
                mWord2 = ( mWord2 & mask ) | newVal;
            }
        }

        public SamplerModulationFactor SamplerModulationFactor
        {
            get => (SamplerModulationFactor)( (int)( mWord2 & 0x70000000 ) >> 28 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x70000000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 28 ) & 0x70000000;
                mWord2 = ( mWord2 & mask ) | newVal;
            }
        }

        public TextureChannel ChannelOrderX
        {
            get => (TextureChannel)( (int)( mWord3 & 0x00000007 ) );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00000007;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value & 0x00000007 );
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public TextureChannel ChannelOrderY
        {
            get => (TextureChannel)( (int)( mWord3 & 0x00000038 ) >> 3 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00000038;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 3 ) & 0x00000038;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public TextureChannel ChannelOrderZ
        {
            get => (TextureChannel)( (int)( mWord3 & 0x000001c0 ) >> 6 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x000001c0;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 6 ) & 0x000001c0;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public TextureChannel ChannelOrderW
        {
            get => (TextureChannel)( (int)( mWord3 & 0x00000e00 ) >> 9 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00000e00;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 9 ) & 0x00000e00;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public int BaseMipLevel
        {
            get => (int)( mWord3 & 0x0000f000 ) >> 12;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x0000f000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 12 ) & 0x0000f000;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public int LastMipLevel
        {
            get => (int)( mWord3 & 0x000f0000 ) >> 16;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x000f0000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 16 ) & 0x000f0000;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public TileMode TileMode
        {
            get => (TileMode)( (int)( mWord3 & 0x01f00000 ) >> 20 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x01f00000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 20 ) & 0x01f00000;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public bool IsPaddedToPow2
        {
            get => ( mWord3 & 0x02000000 ) >> 25 != 0;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x02000000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( ( value ? 1u : 0u ) << 25 ) & 0x02000000;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public TextureType TextureType
        {
            get => (TextureType)( ( mWord3 & 0xf0000000 ) >> 28 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0xf0000000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 28 ) & 0xf0000000;
                mWord3 = ( mWord3 & mask ) | newVal;
            }
        }

        public int Depth
        {
            get => ( (int)( mWord4 & 0x00001fff ) + 1 );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00001fff;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = (uint)( value - 1 ) & 0x00001fff;
                mWord4 = ( mWord4 & mask ) | newVal;
            }
        }

        public int Pitch
        {
            get => ( (int)( mWord4 & 0x07ffe000 ) >> 13 ) + 1;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x07ffe000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)( value - 1 ) << 13 ) & 0x07ffe000;
                mWord4 = ( mWord4 & mask ) | newVal;
            }
        }

        public int BaseArraySliceIndex
        {
            get => (int)( mWord5 & 0x00001fff );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00001fff;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = (uint)value & 0x00001fff;
                mWord5 = ( mWord5 & mask ) | newVal;
            }
        }

        public int LastArraySliceIndex
        {
            get => (int)( mWord5 & 0x03ffe000 ) >> 13;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x03ffe000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 13 ) & 0x03ffe000;
                mWord5 = ( mWord5 & mask ) | newVal;
            }
        }

        public int MinLodWarning
        {
            get => (int)( mWord6 & 0x00000fff );
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00000fff;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = (uint)value & 0x00000fff;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public int MipStatsCounterIndex
        {
            get => (int)( mWord6 & 0x000ff000 ) >> 12;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x000ff000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 12 ) & 0x000ff000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public bool MipStatsEnabled
        {
            get => ( (int)( mWord6 & 0x00100000 ) >> 20 ) != 0;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00100000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( ( value ? 1u : 0u ) << 20 ) & 0x00100000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public bool MetadataCompressionEnabled
        {
            get => ( (int)( mWord6 & 0x00200000 ) >> 21 ) != 0;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00200000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( ( value ? 1u : 0u ) << 21 ) & 0x00200000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public bool DccAlphaOnMsb
        {
            get => ( (int)( mWord6 & 0x00400000 ) >> 22 ) != 0;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00400000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( ( value ? 1u : 0u ) << 22 ) & 0x00400000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public int DccColorTransform
        {
            get => (int)( mWord6 & 0x00800000 ) >> 23;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x00800000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( (uint)value << 23 ) & 0x00800000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
        }

        public bool UseAltTileMode
        {
            get => ( (int)( mWord6 & 0x01000000 ) >> 24 ) != 0;
            set
            {
                // Create a bitmask.
                uint mask = 0xffffffff ^ 0x01000000;
                // Clear the bit range for this modification using the bitmask, and set the new value using bitwise OR.
                uint newVal = ( ( value ? 1u : 0u ) << 24 ) & 0x01000000;
                mWord6 = ( mWord6 & mask ) | newVal;
            }
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
            var ddsFormat = DDSCodec.DetermineBestCompressedFormat( bitmap );
            var dds = DDSCodec.CompressPixelData( bitmap, ddsFormat );
            var surfaceFormat = GetSurfaceFormat( ddsFormat );
            Init( surfaceFormat, 1, ( short )bitmap.Width, ( short )bitmap.Height, dds, false );
        }

        private static SurfaceFormat GetSurfaceFormat( DDSPixelFormatFourCC ddsFormat )
        {
            SurfaceFormat surfaceFormat;
            switch ( ddsFormat )
            {
                case DDSPixelFormatFourCC.DXT1:
                    surfaceFormat = SurfaceFormat.BC1;
                    break;
                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3:
                    surfaceFormat = SurfaceFormat.BC2;
                    break;

                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5:
                    surfaceFormat = SurfaceFormat.BC3;
                    break;

                case DDSPixelFormatFourCC.ATI1:
                    surfaceFormat = SurfaceFormat.BC4;
                    break;

                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    surfaceFormat = SurfaceFormat.BC5;
                    break;

                case DDSPixelFormatFourCC.DX10:
                    surfaceFormat = SurfaceFormat.BC7;
                    break;

                default:
                    throw new NotSupportedException( ddsFormat.ToString() );
            }

            return surfaceFormat;
        }

        public GNFTexture( DDSStream dds )
        {
            var surfaceFormat = GetSurfaceFormat( dds.PixelFormat.FourCC );
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
            SurfaceFormat              = format;
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