namespace GFDLibrary.Textures.DDS
{
    /// <summary>
    /// Old method of identifying Compressed textures.
    /// DX10 indicates new texture, the DX10 Additional header will contain the true format. See <see cref="DXGIFormat"/>.
    /// </summary>
    public enum DDSPixelFormatFourCC
    {
        /// <summary>
        /// Used when FourCC is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// (BC1) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Simple Non Alpha.
        /// </summary>
        DXT1 = 0x31545844,  // 1TXD i.e. DXT1 backwards

        /// <summary>
        /// (BC2) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Sharp Alpha. Premultiplied alpha. 
        /// </summary>
        DXT2 = 0x32545844,

        /// <summary>
        /// (BC2) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Sharp Alpha. 
        /// </summary>
        DXT3 = 0x33545844,

        /// <summary>
        /// (BC3) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Gradient Alpha. Premultiplied alpha.
        /// </summary>
        DXT4 = 0x34545844,

        /// <summary>
        /// (BC3) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Gradient Alpha. 
        /// </summary>
        DXT5 = 0x35545844,

        /// <summary>
        /// Fancy new DirectX 10+ format indicator. DX10 Header will contain true format.
        /// </summary>
        DX10 = 0x30315844,

        /// <summary>
        /// (BC4) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Normal (bump) Maps. 8 bit single channel with alpha.
        /// </summary>
        ATI1 = 0x31495441,

        /// <summary>
        /// (BC5) Block Compressed Texture. Compresses 4x4 texels.
        /// Used for Normal (bump) Maps. Pair of 8 bit channels.
        /// </summary>
        ATI2N_3Dc = 0x32495441,

        R8G8B8 = 20,
        A8R8G8B8,
        X8R8G8B8,
        R5G6B5,
        X1R5G5B5,
        A1R5G5B5,
        A4R4G4B4,
        R3G3B2,
        A8,
        A8R3G3B2,
        X4R4G4B4,
        A2B10G10R10,
        A8B8G8R8,
        X8B8G8R8,
        G16R16,
        A2R10G10B10,
        A16B16G16R16,

        A8P8 = 40,
        P8,

        L8 = 50,
        A8L8,
        A4L4,

        V8U8 = 60,
        L6V5U5,
        X8L8V8U8,
        Q8W8V8U8,
        V16U16,
        A2W10V10U10,

        UYVY = 0x59565955,
        R8G8_B8G8 = 0x47424752,
        YUY2 = 0x32595559,
        G8R8_G8B8 = 0x42475247,

        D16_LOCKABLE = 70,
        D32,
        D15S1,
        D24S8,
        D24X8,
        D24X4S4,
        D16,

        D32F_LOCKABLE = 82,
        D24FS8,

        L16 = 81,

        Q16Q16V16U16 = 110,
        R16F,
        G16R16F,
        A16B16G16R16F,
        R32F,
        G32R32F,
        A32B32G32R32F,
        CxV8U8,
    }

    public static class DDS3PixelFormatFourCCExtensions
    {
        public static DXGIFormat ToDXGIFormat(this DDSPixelFormatFourCC f)
        {
            switch ( f )
            {
                case DDSPixelFormatFourCC.DXT1:
                    return DXGIFormat.BC1_UNORM;
                case DDSPixelFormatFourCC.DXT2:
                    return DXGIFormat.BC2_UNORM;
                case DDSPixelFormatFourCC.DXT3:
                    return DXGIFormat.BC2_UNORM;
                case DDSPixelFormatFourCC.DXT4:
                    return DXGIFormat.BC3_UNORM;
                case DDSPixelFormatFourCC.DXT5:
                    return DXGIFormat.BC3_UNORM;
                case DDSPixelFormatFourCC.ATI1:
                    return DXGIFormat.BC4_UNORM;
                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    return DXGIFormat.BC5_UNORM;
                case DDSPixelFormatFourCC.R8G8B8:
                    return DXGIFormat.B8G8R8X8_UNORM;
                case DDSPixelFormatFourCC.A8R8G8B8:
                    return DXGIFormat.B8G8R8A8_UNORM;
                case DDSPixelFormatFourCC.X8R8G8B8:
                    return DXGIFormat.B8G8R8X8_UNORM;
                case DDSPixelFormatFourCC.R5G6B5:
                    return DXGIFormat.B5G6R5_UNORM;
                case DDSPixelFormatFourCC.X1R5G5B5:
                    return DXGIFormat.B5G5R5A1_UNORM;
                case DDSPixelFormatFourCC.A1R5G5B5:
                    return DXGIFormat.B5G5R5A1_UNORM;
                case DDSPixelFormatFourCC.A4R4G4B4:
                    return DXGIFormat.B4G4R4A4_UNORM;
                case DDSPixelFormatFourCC.R3G3B2:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.A8:
                    return DXGIFormat.A8_UNORM;
                case DDSPixelFormatFourCC.A8R3G3B2:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.X4R4G4B4:
                    return DXGIFormat.B4G4R4A4_UNORM;
                case DDSPixelFormatFourCC.A2B10G10R10:
                    return DXGIFormat.R10G10B10A2_UNORM;
                case DDSPixelFormatFourCC.A8B8G8R8:
                    return DXGIFormat.R8G8B8A8_UNORM;
                case DDSPixelFormatFourCC.X8B8G8R8:
                    return DXGIFormat.R8G8B8A8_UNORM;
                case DDSPixelFormatFourCC.G16R16:
                    return DXGIFormat.R16G16_UNORM;
                case DDSPixelFormatFourCC.A2R10G10B10:
                    return DXGIFormat.R10G10B10A2_UNORM;
                case DDSPixelFormatFourCC.A16B16G16R16:
                    return DXGIFormat.R16G16B16A16_UNORM;
                case DDSPixelFormatFourCC.A8P8:
                    return DXGIFormat.A8P8;
                case DDSPixelFormatFourCC.P8:
                    return DXGIFormat.P8;
                case DDSPixelFormatFourCC.L8:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.A8L8:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.A4L4:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.V8U8:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.L6V5U5:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.X8L8V8U8:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.Q8W8V8U8:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.V16U16:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.A2W10V10U10:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.UYVY:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.R8G8_B8G8:
                    return DXGIFormat.R8G8_B8G8_UNORM;
                case DDSPixelFormatFourCC.YUY2:
                    return DXGIFormat.YUY2;
                case DDSPixelFormatFourCC.G8R8_G8B8:
                    return DXGIFormat.G8R8_G8B8_UNORM;
                case DDSPixelFormatFourCC.D16_LOCKABLE:
                    return DXGIFormat.D16_UNORM;
                case DDSPixelFormatFourCC.D32:
                    return DXGIFormat.D32_FLOAT;
                case DDSPixelFormatFourCC.D15S1:
                    return DXGIFormat.D16_UNORM;
                case DDSPixelFormatFourCC.D24S8:
                    return DXGIFormat.D24_UNORM_S8_UINT;
                case DDSPixelFormatFourCC.D24X8:
                    return DXGIFormat.D24_UNORM_S8_UINT;
                case DDSPixelFormatFourCC.D24X4S4:
                    return DXGIFormat.D24_UNORM_S8_UINT;
                case DDSPixelFormatFourCC.D16:
                    return DXGIFormat.D16_UNORM;
                case DDSPixelFormatFourCC.D32F_LOCKABLE:
                    return DXGIFormat.D32_FLOAT;
                case DDSPixelFormatFourCC.D24FS8:
                    return DXGIFormat.D32_FLOAT_S8X24_UINT;
                case DDSPixelFormatFourCC.L16:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.Q16Q16V16U16:
                    return DXGIFormat.UNKNOWN;
                case DDSPixelFormatFourCC.R16F:
                    return DXGIFormat.R16_FLOAT;
                case DDSPixelFormatFourCC.G16R16F:
                    return DXGIFormat.R16G16_FLOAT;
                case DDSPixelFormatFourCC.A16B16G16R16F:
                    return DXGIFormat.R16G16B16A16_FLOAT;
                case DDSPixelFormatFourCC.R32F:
                    return DXGIFormat.R32_FLOAT;
                case DDSPixelFormatFourCC.G32R32F:
                    return DXGIFormat.R32G32_FLOAT;
                case DDSPixelFormatFourCC.A32B32G32R32F:
                    return DXGIFormat.R32G32B32A32_FLOAT;
                case DDSPixelFormatFourCC.CxV8U8:
                    return DXGIFormat.UNKNOWN;
                default:
                    return DXGIFormat.UNKNOWN;
            }
        }
    }
}
