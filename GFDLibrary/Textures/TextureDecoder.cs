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
            var ddsHeader = new DDS_Header( 1, texture.Height, texture.Width, imageFormat, dx10ImageFormat );
            ddsHeader.WriteToArray( ddsBytes, 0 );        

            // unswizzle
            var data = Swizzler.UnSwizzle( texture.Data, texture.Width, texture.Height, imageFormat == ImageEngineFormat.DDS_DXT1 ? 8 : 16, SwizzleType.PS4 );

            // write pixel data
            Array.Copy( data, 0, ddsBytes, ddsHeaderSize, texture.Data.Length );

            return ddsBytes;
        }

        public static Bitmap Decode( byte[] data, TextureFormat format )
        {
            switch ( format )
            {
                case TextureFormat.DDS:
                    return DecodeDDS( data );
                case TextureFormat.TMX:
                    return DecodeTMX( data );
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
                // Prefer DDSCodec -- handles alpha properly but doesn't handle non pow 2 textures & DX10+ formats
                return DDSCodec.DecompressImage( data );
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
