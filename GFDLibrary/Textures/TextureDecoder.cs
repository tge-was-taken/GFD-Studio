using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CSharpImageLibrary;
using CSharpImageLibrary.Headers;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.GNF;
using GFDLibrary.Textures.Swizzle;
using static CSharpImageLibrary.Headers.DDS_Header;
using BCnEncoder.Decoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using Microsoft.Toolkit.HighPerformance;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace GFDLibrary.Textures
{
    public static class TextureDecoder
    {
        public static Bitmap Decode( Texture texture )
        {
            return Decode( texture.Data, texture.Format );
        }

        public static Bitmap Decode( FieldTexturePS3 texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            return DecodeDDS( ddsBytes );
        }

        public static Bitmap Decode( GNFTexture texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            return DecodeDDS( ddsBytes );
        }

        public static byte[] DecodeToDDS( FieldTexturePS3 texture )
        {
            var surfaceFormat = ImageEngineFormat.DDS_DXT1;
            if ( texture.Flags.HasFlag( FieldTextureFlags.DXT3 ) )
            {
                surfaceFormat = ImageEngineFormat.DDS_DXT3;
            }
            else if ( texture.Flags.HasFlag( FieldTextureFlags.DXT5 ) )
            {
                surfaceFormat = ImageEngineFormat.DDS_DXT5;
            }

            var ddsBytes = new byte[0x80 + texture.DataLength];

            // create & write header
            var ddsHeader = new DDS_Header( texture.MipMapCount, texture.Height, texture.Width, surfaceFormat );
            ddsHeader.WriteToArray( ddsBytes, 0 );

            // write pixel data
            Array.Copy( texture.Data, 0, ddsBytes, 0x80, texture.DataLength );

            return ddsBytes;
        }

        public static byte[] DecodeToDDS( GNFTexture texture )
        {
            var imageFormat = ImageEngineFormat.DDS_DXT5;
            var dx10ImageFormat = DDS_Header.DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
            switch ( texture.SurfaceFormat )
            {
                case GNF.SurfaceFormat.BC1:
                    imageFormat = ImageEngineFormat.DDS_DXT1;
                    break;
                case GNF.SurfaceFormat.BC2:
                    imageFormat = ImageEngineFormat.DDS_DXT2;
                    break;
                case GNF.SurfaceFormat.BC3:
                    imageFormat = ImageEngineFormat.DDS_DXT5;
                    break;
                case GNF.SurfaceFormat.BC4:
                    imageFormat = ImageEngineFormat.DDS_ATI1;
                    break;
                case GNF.SurfaceFormat.BC5:
                    imageFormat = ImageEngineFormat.DDS_ATI2_3Dc;
                    break;
                case GNF.SurfaceFormat.BC6:
                    imageFormat = ImageEngineFormat.DDS_DX10;
                    dx10ImageFormat = DDS_Header.DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16;
                    break;
                case GNF.SurfaceFormat.BC7:
                    imageFormat = ImageEngineFormat.DDS_DX10;

                    switch ( texture.ChannelType )
                    {
                        case ChannelType.Srgb:
                            dx10ImageFormat = DDS_Header.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB;
                            break;
                        default:
                            dx10ImageFormat = DDS_Header.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                            break;
                    }
                    break;
            }

            var ddsHeaderSize = 0x80;
            if ( dx10ImageFormat != DDS_Header.DXGI_FORMAT.DXGI_FORMAT_UNKNOWN )
                ddsHeaderSize += 20;

            var ddsBytes = new byte[ddsHeaderSize + texture.Data.Length];

            // create & write header
            var ddsHeader = new DDS_Header( texture.LastMipLevel, texture.Height, texture.Width, imageFormat, dx10ImageFormat ); //Mips:1 => texture.LastMipLevel
            ddsHeader.dwPitchOrLinearSize = DDSFormatDetails.CalculatePitchOrLinearSize( texture.Width, texture.Height, ImageEngineFormat2FourCC(ddsHeader.Format), out DDSHeaderFlags additionalFlags );
            ddsHeader.dwFlags |= (DDSdwFlags)additionalFlags;
            ddsHeader.dwDepth = texture.Depth;
            ddsHeader.WriteToArray( ddsBytes, 0 );        

            // unswizzle
            var data = Swizzler.UnSwizzle( texture.Data, texture.Width, texture.Height, imageFormat == ImageEngineFormat.DDS_DXT1 ? 8 : 16, SwizzleType.PS4 );

            // write pixel data
            Array.Copy( data, 0, ddsBytes, ddsHeaderSize, texture.Data.Length );

            return ddsBytes;
        }

        /// <summary>
        /// Cast ImageEngineFormat to DDSPixelFormatFourCC for easier calculation of PitchOrLinearSize.
        /// </summary>
        public static DDSPixelFormatFourCC ImageEngineFormat2FourCC( ImageEngineFormat format )
        {
            switch ( format )
            {
                case ImageEngineFormat.Unknown:
                case ImageEngineFormat.JPG:
                case ImageEngineFormat.PNG:
                case ImageEngineFormat.BMP:
                case ImageEngineFormat.TGA:
                case ImageEngineFormat.GIF:
                case ImageEngineFormat.TIF:
                case ImageEngineFormat.DDS_CUSTOM:
                    return DDSPixelFormatFourCC.Unknown;
                case ImageEngineFormat.DDS_DXT1:
                    return DDSPixelFormatFourCC.DXT1;
                case ImageEngineFormat.DDS_DXT2:
                    return DDSPixelFormatFourCC.DXT2;
                case ImageEngineFormat.DDS_DXT3:
                    return DDSPixelFormatFourCC.DXT3;
                case ImageEngineFormat.DDS_DXT4:
                    return DDSPixelFormatFourCC.DXT4;
                case ImageEngineFormat.DDS_DXT5:
                    return DDSPixelFormatFourCC.DXT5;
                case ImageEngineFormat.DDS_DX10:
                    return DDSPixelFormatFourCC.DX10;
                case ImageEngineFormat.DDS_ABGR_8:
                    return DDSPixelFormatFourCC.A8B8G8R8;
                case ImageEngineFormat.DDS_ATI1:
                    return DDSPixelFormatFourCC.ATI1;
                case ImageEngineFormat.DDS_V8U8:
                    return DDSPixelFormatFourCC.V8U8;
                case ImageEngineFormat.DDS_G8_L8:
                    return DDSPixelFormatFourCC.L8;
                case ImageEngineFormat.DDS_A8L8:
                    return DDSPixelFormatFourCC.A8L8;
                case ImageEngineFormat.DDS_RGB_8:
                    return DDSPixelFormatFourCC.R8G8B8;
                case ImageEngineFormat.DDS_ATI2_3Dc:
                    return DDSPixelFormatFourCC.ATI2N_3Dc;
                case ImageEngineFormat.DDS_ARGB_8:
                    return DDSPixelFormatFourCC.A8R8G8B8;
                case ImageEngineFormat.DDS_R5G6B5:
                    return DDSPixelFormatFourCC.R5G6B5;
                case ImageEngineFormat.DDS_ARGB_4:
                    return DDSPixelFormatFourCC.A4R4G4B4;
                case ImageEngineFormat.DDS_A8:
                    return DDSPixelFormatFourCC.A8;
                case ImageEngineFormat.DDS_G16_R16:
                    return DDSPixelFormatFourCC.G16R16;
                case ImageEngineFormat.DDS_ARGB_32F:
                    return DDSPixelFormatFourCC.A32B32G32R32F;
            }

            return 0;
        }

        public static Bitmap Decode( byte[] data, TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.DDS:
                case TextureFormat.EPT:
                    return DecodeDDS( data );
                case TextureFormat.TMX:
                    return DecodeTMX( data );
                case TextureFormat.TGA:
                    return DecodeTGA( data );
                case TextureFormat.GXT:
                    return DecodeGXT( data );
                case TextureFormat.GNF:
                    return DecodeGNF(data);
                default:
                    throw new NotSupportedException();
            }
        }

        private static Bitmap DecodeDDS( byte[] data )
        {
            try
            {
                BcDecoder decoder = new BcDecoder();
                MemoryStream texturestream = new MemoryStream( data );
                Image<Rgba32> rgba32image = decoder.DecodeToImageRgba32( texturestream );
                // this is so cringe
                var bitmap = new Bitmap( rgba32image.Width, rgba32image.Height, PixelFormat.Format32bppArgb );
                for ( int y = 0; y < rgba32image.Height; y++ )
                {
                    for ( int x = 0; x < rgba32image.Width; x++ )
                    {
                        var pixel = rgba32image[x, y];
                        var color = System.Drawing.Color.FromArgb( pixel.A, pixel.R, pixel.G, pixel.B );
                        bitmap.SetPixel( x, y, color );
                    }
                }
                return bitmap;
            }
            catch ( Exception )
            {
            }

            try
            {
                // Image engine SUCKS at alpha handling, but its better than nothing
                return DecodeDDSWithImageEngine( data );
            }
            catch ( Exception )
            {
            }

            // RIP
            Trace.WriteLine( "Failed to decode DDS texture" );
            return new Bitmap( 32, 32, PixelFormat.Format32bppArgb );
        }

        private static Bitmap DecodeDDSWithImageEngine( byte[] data )
        {
            var ddsImage = new ImageEngineImage( data );
            return ImageEngineImageToBitmap( ddsImage );
        }

        private static Bitmap ImageEngineImageToBitmap( ImageEngineImage image )
        {
            // save the image to bmp
            var bitmapStream = new MemoryStream();

            try
            {
                image.Save( bitmapStream, new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.PNG ), MipHandling.KeepTopOnly, 0, 0, false );
            }
            catch ( NullReferenceException /* good library */ )
            {
                image.Save( bitmapStream, new ImageFormats.ImageEngineFormatDetails( ImageEngineFormat.PNG ), MipHandling.KeepTopOnly );
            }

            // load the saved bmp into a new bitmap
            return new Bitmap( bitmapStream );
        }

        private static Bitmap DecodeTMX( byte[] data )
        {
            var tmx = new Scarlet.IO.ImageFormats.TMX();
            tmx.Open( new MemoryStream( data ), Scarlet.IO.Endian.LittleEndian );
            return tmx.GetBitmap();
        }
        private static Bitmap DecodeTGA( byte[] data )
        {
            return TgaDecoderTest.TgaDecoder.FromBinary( data );
        }
        private static Bitmap DecodeGXT( byte[] data )
        {
            var gxt = new Scarlet.IO.ImageFormats.GXT();
            gxt.Open( new MemoryStream( data ), Scarlet.IO.Endian.LittleEndian );
            return gxt.GetBitmap();
        }
        private static Bitmap DecodeGNF(byte[] data)
        {
            var gnf = new Scarlet.IO.ImageFormats.GNF();
            gnf.Open(new MemoryStream(data), Scarlet.IO.Endian.LittleEndian);
            return gnf.GetBitmap();
        }
    }
}
