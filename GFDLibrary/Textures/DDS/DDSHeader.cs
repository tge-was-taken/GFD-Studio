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

        public DDSHeaderDXT10 AdditionalHeader { get; } = new DDSHeaderDXT10();

        public DDSHeader()
        {
            Size = SIZE_WITHOUT_MAGIC;
            Flags = DDSHeaderFlags.Caps | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.PixelFormat;
            Caps  = DDSHeaderCaps.Texture;
        }

        public DDSHeader( int mipMapCount, int width, int height, DDSPixelFormatFourCC format, DXGIFormat dx10Format = DXGIFormat.UNKNOWN )
        {
            Size = SIZE_WITHOUT_MAGIC;
            Flags = DDSHeaderFlags.Caps | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.PixelFormat;
            if ( mipMapCount != 1 )
                Flags |= DDSHeaderFlags.MipMapCount;

            Height = height;
            Width = width;
            PitchOrLinearSize = DDSFormatDetails.CalculatePitchOrLinearSize( width, height, format, out var additionalFlags );
            Flags |= additionalFlags;

            PixelFormat.FourCC = format;
            Caps = DDSHeaderCaps.Texture;
            if ( mipMapCount != 1 )
                Caps |= DDSHeaderCaps.Complex | DDSHeaderCaps.MipMap;
            
            if ( format == DDSPixelFormatFourCC.DX10 )
            {
                AdditionalHeader.Format = dx10Format;
                AdditionalHeader.ResourceDimension = D3D10_RESOURCE_DIMENSION.DDS_DIMENSION_TEXTURE2D;
                AdditionalHeader.MiscFlag = D3D10_RESOURCE_MISC_FLAGS.D3D10_RESOURCE_MISC_GENERATE_MIPS;
                AdditionalHeader.MiscFlags2 = DDS_ALPHA_MODE.DDS_ALPHA_MODE_UNKNOWN;
                AdditionalHeader.ArraySize = 1;
            }
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
                AdditionalHeader.Read( reader );
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

            if ( AdditionalHeader.Format != DXGIFormat.UNKNOWN )
                AdditionalHeader.Write( writer );
        }
    }

    /// <summary>
    /// Additional header used by DXGI/DX10 DDS'.
    /// </summary>
    public class DDSHeaderDXT10
    {
        public const int SIZE = 20;

        /// <summary>
        /// Surface format.
        /// </summary>
        public DXGIFormat Format { get; set; }

        /// <summary>
        /// Dimension of texture (1D, 2D, 3D)
        /// </summary>
        public D3D10_RESOURCE_DIMENSION ResourceDimension { get; set; }

        /// <summary>
        /// Identifies less common options. e.g. 0x4 = DDS_RESOURCE_MISC_TEXTURECUBE
        /// </summary>
        public D3D10_RESOURCE_MISC_FLAGS MiscFlag { get; set; }

        /// <summary>
        /// Number of elements in array.
        /// For 2D textures that are cube maps, it's the number of cubes. Can also be random images made with texassemble.exe.
        /// For 3D textures, must be 1.
        /// </summary>
        public uint ArraySize { get; set; }

        /// <summary>
        /// Alpha flags.
        /// </summary>
        public DDS_ALPHA_MODE MiscFlags2 { get; set; }

        internal void Read( BinaryReader reader )
        {
            Format = (DXGIFormat)reader.ReadInt32();
            ResourceDimension = (D3D10_RESOURCE_DIMENSION)reader.ReadInt32();
            MiscFlag = (D3D10_RESOURCE_MISC_FLAGS)reader.ReadInt32();
            ArraySize = reader.ReadUInt32();
            MiscFlags2 = (DDS_ALPHA_MODE)reader.ReadInt32();
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( (int)Format );
            writer.Write( (int)ResourceDimension );
            writer.Write( (int)MiscFlag );
            writer.Write( (int)ArraySize );
            writer.Write( (int)MiscFlags2 );
        }
    }

    public enum D3D10_RESOURCE_MISC_FLAGS
    {
        D3D10_RESOURCE_MISC_GENERATE_MIPS = 0x1,
        D3D10_RESOURCE_MISC_SHARED = 0x2,
        D3D10_RESOURCE_MISC_TEXTURECUBE = 0x4,
        D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX = 0x10,
        D3D10_RESOURCE_MISC_GDI_COMPATIBLE = 0x20
    }

    /// <summary>
    /// Option flags to indicate alpha mode.
    /// </summary>
    public enum DDS_ALPHA_MODE
    {
        /// <summary>
        /// Alpha content unknown. Default for legacy files, assumed to be "Straight".
        /// </summary>
        DDS_ALPHA_MODE_UNKNOWN = 0,

        /// <summary>
        /// Any alpha is "straight". Standard. i.e. RGBA all separate channels. 
        /// </summary>
        DDS_ALPHA_MODE_STRAIGHT = 1,

        /// <summary>
        /// Any alpha channels are premultiplied i.e. RGB (without A) with premultiplied alpha has values that include alpha by way of multiplying the original RGB with the A.
        /// </summary>
        DDS_ALPHA_MODE_PREMULTIPLIED = 2,

        /// <summary>
        /// Alpha is fully opaque.
        /// </summary>
        DDS_ALPHA_MODE_OPAQUE = 3,

        /// <summary>
        /// Alpha channel isn't for transparency.
        /// </summary>
        DDS_ALPHA_MODE_CUSTOM = 4,
    }

    /// <summary>
    /// Indicates type of DXGI/DX10 texture.
    /// </summary>
    public enum D3D10_RESOURCE_DIMENSION
    {
        // TODO: Incomplete?

        /// <summary>
        /// 1D Texture specified by dwWidth of DDS_Header = size of texture. Typically, dwHeight = 1 and DDSD_HEIGHT flag is also set in dwFlags.
        /// </summary>
        DDS_DIMENSION_TEXTURE1D = 2,

        /// <summary>
        /// 2D Texture specified by dwWidth and dwHeight of DDS_Header. Can be cube map if miscFlag and arraySize members are set.
        /// </summary>
        DDS_DIMENSION_TEXTURE2D = 3,

        /// <summary>
        /// 3D Texture specified by dwWidth, dwHeight, and dwDepth. Must have DDSD_DEPTH Flag set in dwFlags.
        /// </summary> 
        DDS_DIMENSION_TEXTURE3D = 4,
    }

    /// <summary>
    /// DXGI/DX10 formats.
    /// </summary>
    public enum DXGIFormat
    {
        UNKNOWN = 0,
        R32G32B32A32_TYPELESS = 1,
        R32G32B32A32_FLOAT = 2,
        R32G32B32A32_UINT = 3,
        R32G32B32A32_SINT = 4,
        R32G32B32_TYPELESS = 5,
        R32G32B32_FLOAT = 6,
        R32G32B32_UINT = 7,
        R32G32B32_SINT = 8,
        R16G16B16A16_TYPELESS = 9,
        R16G16B16A16_FLOAT = 10,
        R16G16B16A16_UNORM = 11,
        R16G16B16A16_UINT = 12,
        R16G16B16A16_SNORM = 13,
        R16G16B16A16_SINT = 14,
        R32G32_TYPELESS = 15,
        R32G32_FLOAT = 16,
        R32G32_UINT = 17,
        R32G32_SINT = 18,
        R32G8X24_TYPELESS = 19,
        D32_FLOAT_S8X24_UINT = 20,
        R32_FLOAT_X8X24_TYPELESS = 21,
        X32_TYPELESS_G8X24_UINT = 22,
        R10G10B10A2_TYPELESS = 23,
        R10G10B10A2_UNORM = 24,
        R10G10B10A2_UINT = 25,
        R11G11B10_FLOAT = 26,
        R8G8B8A8_TYPELESS = 27,
        R8G8B8A8_UNORM = 28,
        R8G8B8A8_UNORM_SRGB = 29,
        R8G8B8A8_UINT = 30,
        R8G8B8A8_SNORM = 31,
        R8G8B8A8_SINT = 32,
        R16G16_TYPELESS = 33,
        R16G16_FLOAT = 34,
        R16G16_UNORM = 35,
        R16G16_UINT = 36,
        R16G16_SNORM = 37,
        R16G16_SINT = 38,
        R32_TYPELESS = 39,
        D32_FLOAT = 40,
        R32_FLOAT = 41,
        R32_UINT = 42,
        R32_SINT = 43,
        R24G8_TYPELESS = 44,
        D24_UNORM_S8_UINT = 45,
        R24_UNORM_X8_TYPELESS = 46,
        X24_TYPELESS_G8_UINT = 47,
        R8G8_TYPELESS = 48,
        R8G8_UNORM = 49,
        R8G8_UINT = 50,
        R8G8_SNORM = 51,
        R8G8_SINT = 52,
        R16_TYPELESS = 53,
        R16_FLOAT = 54,
        D16_UNORM = 55,
        R16_UNORM = 56,
        R16_UINT = 57,
        R16_SNORM = 58,
        R16_SINT = 59,
        R8_TYPELESS = 60,
        R8_UNORM = 61,
        R8_UINT = 62,
        R8_SNORM = 63,
        R8_SINT = 64,
        A8_UNORM = 65,
        R1_UNORM = 66,
        R9G9B9E5_SHAREDEXP = 67,
        R8G8_B8G8_UNORM = 68,
        G8R8_G8B8_UNORM = 69,
        BC1_TYPELESS = 70,
        BC1_UNORM = 71,
        BC1_UNORM_SRGB = 72,
        BC2_TYPELESS = 73,
        BC2_UNORM = 74,
        BC2_UNORM_SRGB = 75,
        BC3_TYPELESS = 76,
        BC3_UNORM = 77,
        BC3_UNORM_SRGB = 78,
        BC4_TYPELESS = 79,
        BC4_UNORM = 80,
        BC4_SNORM = 81,
        BC5_TYPELESS = 82,
        BC5_UNORM = 83,
        BC5_SNORM = 84,
        B5G6R5_UNORM = 85,
        B5G5R5A1_UNORM = 86,
        B8G8R8A8_UNORM = 87,
        B8G8R8X8_UNORM = 88,
        R10G10B10_XR_BIAS_A2_UNORM = 89,
        B8G8R8A8_TYPELESS = 90,
        B8G8R8A8_UNORM_SRGB = 91,
        B8G8R8X8_TYPELESS = 92,
        B8G8R8X8_UNORM_SRGB = 93,
        BC6H_TYPELESS = 94,
        BC6H_UF16 = 95,
        BC6H_SF16 = 96,
        BC7_TYPELESS = 97,
        BC7_UNORM = 98,
        BC7_UNORM_SRGB = 99,
        AYUV = 100,
        Y410 = 101,
        Y416 = 102,
        NV12 = 103,
        P010 = 104,
        P016 = 105,
        OPAQUE_420 = 106,
        YUY2 = 107,
        Y210 = 108,
        Y216 = 109,
        NV11 = 110,
        AI44 = 111,
        IA44 = 112,
        P8 = 113,
        A8P8 = 114,
        B4G4R4A4_UNORM = 115
    }
}