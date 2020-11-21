using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.GNF;
using GFDLibrary.Textures.Swizzle;
using DirectXTexNet;

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
            return DDSCodec.Decompress( ddsBytes );
        }

        public static Bitmap Decode( GNFTexture texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            return DDSCodec.Decompress( ddsBytes );
        }

        public static byte[] DecodeToDDS( FieldTexturePS3 texture )
        {
            var surfaceFormat = DDSPixelFormatFourCC.DXT1;
            if ( texture.Flags.HasFlag( FieldTextureFlags.BC2 ) )
            {
                surfaceFormat = DDSPixelFormatFourCC.DXT3;
            }
            else if ( texture.Flags.HasFlag( FieldTextureFlags.BC3 ) )
            {
                surfaceFormat = DDSPixelFormatFourCC.DXT5;
            }

            var ddsBytes = new byte[0x80 + texture.DataLength];
            var ddsStream = new MemoryStream( ddsBytes );

            // create & write header
            var ddsHeader = new DDSHeader( texture.MipMapCount, texture.Height, texture.Width, surfaceFormat );
            ddsHeader.Save( ddsStream );

            // write pixel data
            Array.Copy( texture.Data, 0, ddsBytes, 0x80, texture.DataLength );

            return ddsBytes;
        }

        public static byte[] DecodeToDDS( GNFTexture texture )
        {
            var imageFormat = DDSPixelFormatFourCC.DXT5;
            var dx10ImageFormat = DXGIFormat.UNKNOWN;
            switch ( texture.SurfaceFormat )
            {
                case SurfaceFormat.BC1:
                    imageFormat = DDSPixelFormatFourCC.DXT1;
                    break;
                case SurfaceFormat.BC2:
                    imageFormat = DDSPixelFormatFourCC.DXT2;
                    break;
                case SurfaceFormat.BC3:
                    imageFormat = DDSPixelFormatFourCC.DXT5;
                    break;
                case SurfaceFormat.BC4:
                    imageFormat = DDSPixelFormatFourCC.ATI1;
                    break;
                case SurfaceFormat.BC5:
                    imageFormat = DDSPixelFormatFourCC.ATI2N_3Dc;
                    break;
                case SurfaceFormat.BC6:
                    imageFormat = DDSPixelFormatFourCC.DX10;
                    dx10ImageFormat = DXGIFormat.BC6H_UF16;
                    break;
                case SurfaceFormat.BC7:
                    imageFormat = DDSPixelFormatFourCC.DX10;

                    switch ( texture.ChannelType )
                    {
                        case ChannelType.Srgb:
                            dx10ImageFormat = DXGIFormat.BC7_UNORM_SRGB;
                            break;
                        default:
                            dx10ImageFormat = DXGIFormat.BC7_UNORM;
                            break;
                    }
                    break;
            }

            var ddsHeaderSize = 0x80;
            if ( dx10ImageFormat != DXGIFormat.UNKNOWN )
                ddsHeaderSize += 20;

            var ddsBytes = new byte[ddsHeaderSize + texture.Data.Length];

            // create & write header
            var ddsHeader = new DDSHeader( 1, texture.Height, texture.Width, imageFormat, dx10ImageFormat );
            ddsHeader.Save( new MemoryStream( ddsBytes ) );        

            // unswizzle
            var data = Swizzler.UnSwizzle( texture.Data, texture.Width, texture.Height, imageFormat == DDSPixelFormatFourCC.DXT1 ? 8 : 16, SwizzleType.PS4 );

            // write pixel data
            Array.Copy( data, 0, ddsBytes, ddsHeaderSize, texture.Data.Length );

            return ddsBytes;
        }

        public static Bitmap Decode( byte[] data, TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.DDS:
                    return DDSCodec.Decompress( data );
                case TextureFormat.GXT:
                    return DecodeGXT( data );
                default:
                    throw new NotSupportedException();
            }
        }

        private static Bitmap DecodeGXT( byte[] data )
        {
            var gxt = new Scarlet.IO.ImageFormats.GXT();
            gxt.Open( new MemoryStream( data ), Scarlet.IO.Endian.LittleEndian );
            return gxt.GetBitmap();
        }
    }
}
