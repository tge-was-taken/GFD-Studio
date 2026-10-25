using System;
using System.IO;
using GFDLibrary.Graphics;
using GFDLibrary.Textures.Native;

namespace GFDLibrary.Textures.DDS
{
    /// <summary>
    /// Block compression, DDS and PNG handling, over the DirectXTex codecs.
    /// </summary>
    public static class DDSCodec
    {
        public static Image DecompressImage( Stream stream )
        {
            if ( stream.CanSeek )
                stream.Position = 0;

            using var buffer = new MemoryStream();
            stream.CopyTo( buffer );
            return DecompressImage( buffer.ToArray() );
        }

        public static Image DecompressImage( string filepath )
        {
            return DecompressImage( File.ReadAllBytes( filepath ) );
        }

        public static Image DecompressImage( byte[] dds )
        {
            using var image = DirectXTex.ScratchImage.FromDDS( dds );
            return ToImage( image );
        }

        public static byte[] CompressImage( Image image )
        {
            return CompressImage( image, DetermineBestCompressedFormat( image ) );
        }

        public static byte[] CompressImage( Image image, DDSPixelFormatFourCC format )
        {
            using var source = FromImage( image );
            using var mipped = source.GenerateMipMaps() ?? source;
            using var compressed = mipped.Compress( ToDxgiFormat( format ) );
            return compressed.SaveToDDS();
        }

        /// <summary>Compresses to raw blocks, with no DDS header and no mipmaps.</summary>
        public static byte[] CompressPixelData( Image image, DDSPixelFormatFourCC format )
        {
            using var source = FromImage( image );
            using var compressed = source.Compress( ToDxgiFormat( format ) );
            return compressed.GetBlocks();
        }

        public static Image DecompressPixelData( byte[] data, int width, int height, DDSPixelFormatFourCC format )
        {
            using var blocks = DirectXTex.ScratchImage.FromBlocks( data, width, height, ToDxgiFormat( format ) );
            using var decompressed = blocks.Decompress( DDSDxgiFormat.R8G8B8A8_UNORM );
            return ToImage( decompressed );
        }

        public static DDSPixelFormatFourCC DetermineBestCompressedFormat( Image image )
        {
            return image.HasTransparency() ? DDSPixelFormatFourCC.DXT5 : DDSPixelFormatFourCC.DXT1;
        }

        internal static DDSDxgiFormat ToDxgiFormat( DDSPixelFormatFourCC format )
        {
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1: return DDSDxgiFormat.BC1_UNORM;
                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3: return DDSDxgiFormat.BC2_UNORM;
                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5: return DDSDxgiFormat.BC3_UNORM;
                case DDSPixelFormatFourCC.ATI1: return DDSDxgiFormat.BC4_UNORM;
                case DDSPixelFormatFourCC.ATI2N_3Dc: return DDSDxgiFormat.BC5_UNORM;
                default:
                    throw new NotSupportedException( $"No DXGI format for {format}" );
            }
        }

        /// <summary>Decompresses or converts whatever was loaded, then hands back an image.</summary>
        internal static Image ToImage( DirectXTex.ScratchImage image )
        {
            var format = ( DDSDxgiFormat )image.Metadata.Format;

            if ( IsCompressed( format ) )
            {
                using var decompressed = image.Decompress( DDSDxgiFormat.R8G8B8A8_UNORM );
                return ToImage( decompressed );
            }

            // A PNG arrives tagged sRGB. The bytes are already what we want, so retag rather
            // than let Convert apply a colour space transform to them.
            if ( format == DDSDxgiFormat.R8G8B8A8_UNORM_SRGB )
            {
                image.OverrideFormat( DDSDxgiFormat.R8G8B8A8_UNORM );
                format = DDSDxgiFormat.R8G8B8A8_UNORM;
            }

            if ( format != DDSDxgiFormat.R8G8B8A8_UNORM )
            {
                using var converted = image.Convert( DDSDxgiFormat.R8G8B8A8_UNORM );
                return ToImage( converted );
            }

            return new Image( image.Width, image.Height, image.GetPixels() );
        }

        internal static DirectXTex.ScratchImage FromImage( Image image )
        {
            return DirectXTex.ScratchImage.FromPixels( image.Pixels, image.Width, image.Height,
                                                       DDSDxgiFormat.R8G8B8A8_UNORM );
        }

        /// <summary>The BC1 to BC5 and BC6H to BC7 ranges of the DXGI format list.</summary>
        internal static bool IsCompressed( DDSDxgiFormat format )
        {
            return ( format >= DDSDxgiFormat.BC1_TYPELESS && format <= DDSDxgiFormat.BC5_SNORM )
                || ( format >= DDSDxgiFormat.BC6H_TYPELESS && format <= DDSDxgiFormat.BC7_UNORM_SRGB );
        }
    }
}
