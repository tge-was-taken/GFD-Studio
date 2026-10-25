using System;
using System.Diagnostics;
using System.IO;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.Native;
using GFDLibrary.Textures.GNF;
using GFDLibrary.Textures.Swizzle;
using GFDLibrary.Graphics;

namespace GFDLibrary.Textures
{
    public static class TextureDecoder
    {
        public static Image Decode( Texture texture )
        {
            return Decode( texture.Data, texture.Format );
        }

        public static Image Decode( FieldTexturePS3 texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            return DecodeDDS( ddsBytes );
        }

        public static Image Decode( GNFTexture texture )
        {
            var ddsBytes = DecodeToDDS( texture );
            return DecodeDDS( ddsBytes );
        }

        public static byte[] DecodeToDDS( FieldTexturePS3 texture )
        {
            var format = DDSPixelFormatFourCC.DXT1;
            if ( texture.Flags.HasFlag( FieldTextureFlags.DXT3 ) )
                format = DDSPixelFormatFourCC.DXT3;
            else if ( texture.Flags.HasFlag( FieldTextureFlags.DXT5 ) )
                format = DDSPixelFormatFourCC.DXT5;

            var header = new DDSHeader( texture.Width, texture.Height, format )
            {
                MipMapCount = texture.MipMapCount,
            };
            if ( texture.MipMapCount > 1 )
            {
                header.Flags |= DDSHeaderFlags.MipMapCount;
                header.Caps |= DDSHeaderCaps.Complex | DDSHeaderCaps.MipMap;
            }

            var ddsBytes = new byte[DDSHeader.SIZE + texture.DataLength];
            using ( var stream = new MemoryStream( ddsBytes ) )
                header.Save( stream );

            Array.Copy( texture.Data, 0, ddsBytes, DDSHeader.SIZE, texture.DataLength );

            return ddsBytes;
        }

        public static byte[] DecodeToDDS( GNFTexture texture )
        {
            var format = DDSPixelFormatFourCC.DXT5;
            var dxgiFormat = DDSDxgiFormat.UNKNOWN;

            switch ( texture.SurfaceFormat )
            {
                case GNF.SurfaceFormat.BC1:
                    format = DDSPixelFormatFourCC.DXT1;
                    break;
                case GNF.SurfaceFormat.BC2:
                    format = DDSPixelFormatFourCC.DXT2;
                    break;
                case GNF.SurfaceFormat.BC3:
                    format = DDSPixelFormatFourCC.DXT5;
                    break;
                case GNF.SurfaceFormat.BC4:
                    format = DDSPixelFormatFourCC.ATI1;
                    break;
                case GNF.SurfaceFormat.BC5:
                    format = DDSPixelFormatFourCC.ATI2N_3Dc;
                    break;
                case GNF.SurfaceFormat.BC6:
                    format = DDSPixelFormatFourCC.DX10;
                    dxgiFormat = DDSDxgiFormat.BC6H_UF16;
                    break;
                case GNF.SurfaceFormat.BC7:
                    format = DDSPixelFormatFourCC.DX10;
                    dxgiFormat = texture.ChannelType == ChannelType.Srgb
                        ? DDSDxgiFormat.BC7_UNORM_SRGB
                        : DDSDxgiFormat.BC7_UNORM;
                    break;
            }

            var header = new DDSHeader( texture.Width, texture.Height, format )
            {
                MipMapCount = texture.LastMipLevel,
                Depth = texture.Depth,
            };

            var headerSize = DDSHeader.SIZE;
            if ( dxgiFormat != DDSDxgiFormat.UNKNOWN )
            {
                header.DxgiFormat = dxgiFormat;
                header.D3D10ResourceDimension = DDSD3D10ResourceDimension.TEXTURE2D;
                header.ArraySize = 1;
                headerSize += sizeof( uint ) * 5;
                header.Size = headerSize - sizeof( int );
            }

            var ddsBytes = new byte[headerSize + texture.Data.Length];
            using ( var stream = new MemoryStream( ddsBytes ) )
                header.Save( stream );

            var data = Swizzler.UnSwizzle( texture.Data, texture.Width, texture.Height,
                format == DDSPixelFormatFourCC.DXT1 ? 8 : 16, SwizzleType.PS4 );

            Array.Copy( data, 0, ddsBytes, headerSize, texture.Data.Length );

            return ddsBytes;
        }

        public static Image Decode( byte[] data, TextureFormat format )
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

        private static Image DecodeDDS( byte[] data )
        {
            try
            {
                using var image = DirectXTex.ScratchImage.FromDDS( data );
                return DDSCodec.ToImage( image );
            }
            catch ( Exception )
            {
                Trace.WriteLine( "Failed to decode DDS texture" );
                return new Image( 32, 32 );
            }
        }

        private static Image DecodeTMX( byte[] data )
        {
            var tmx = new Scarlet.IO.ImageFormats.TMX();
            tmx.Open( new MemoryStream( data ), Scarlet.IO.Endian.LittleEndian );
            return FromScarlet( tmx.GetPixels() );
        }
        private static Image DecodeTGA( byte[] data )
        {
            using var image = DirectXTex.ScratchImage.FromTGA( data );
            return DDSCodec.ToImage( image );
        }
        private static Image DecodeGXT( byte[] data )
        {
            var gxt = new Scarlet.IO.ImageFormats.GXT();
            gxt.Open( new MemoryStream( data ), Scarlet.IO.Endian.LittleEndian );
            return FromScarlet( gxt.GetPixels() );
        }
        private static Image DecodeGNF(byte[] data)
        {
            var gnf = new Scarlet.IO.ImageFormats.GNF();
            gnf.Open(new MemoryStream(data), Scarlet.IO.Endian.LittleEndian);
            return FromScarlet( gnf.GetPixels() );
        }
    
        private static Image FromScarlet( Scarlet.Drawing.ImagePixels pixels )
        {
            return Image.FromBgra( pixels.Data, pixels.Width, pixels.Height );
        }
    }
}
