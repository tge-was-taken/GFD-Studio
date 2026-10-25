using System;
using GFDLibrary.Textures.Native;
using GFDLibrary.Textures.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GFDLibrary.Textures.DDS
{
    /// <summary>
    /// Block compression and DDS handling, over the DirectXTex codecs.
    /// </summary>
    public static class DDSCodec
    {
        public static Image<Rgba32> DecompressImage( System.IO.Stream stream )
        {
            if ( stream.CanSeek )
                stream.Position = 0;

            using var buffer = new System.IO.MemoryStream();
            stream.CopyTo( buffer );
            return DecompressImage( buffer.ToArray() );
        }

        public static Image<Rgba32> DecompressImage( string filepath )
        {
            return DecompressImage( System.IO.File.ReadAllBytes( filepath ) );
        }

        public static Image<Rgba32> DecompressImage( byte[] dds )
        {
            using var image = DirectXTex.ScratchImage.FromDDS( dds );
            return ToImage( image );
        }

        public static byte[] CompressImage( Image<Rgba32> image )
        {
            return CompressImage( image, DetermineBestCompressedFormat( image ) );
        }

        public static byte[] CompressImage( Image<Rgba32> image, DDSPixelFormatFourCC format )
        {
            using var source = FromImage( image );
            using var mipped = source.GenerateMipMaps() ?? source;
            using var compressed = mipped.Compress( ToDxgiFormat( format ) );
            return compressed.SaveToDDS();
        }

        /// <summary>Compresses to raw blocks, with no DDS header and no mipmaps.</summary>
        public static byte[] CompressPixelData( Image<Rgba32> image, DDSPixelFormatFourCC format )
        {
            using var source = FromImage( image );
            using var compressed = source.Compress( ToDxgiFormat( format ) );
            return compressed.GetBlocks();
        }

        public static Image<Rgba32> DecompressPixelData( byte[] data, int width, int height, DDSPixelFormatFourCC format )
        {
            using var blocks = DirectXTex.ScratchImage.FromBlocks( data, width, height, ToDxgiFormat( format ) );
            using var decompressed = blocks.Decompress( DDSDxgiFormat.R8G8B8A8_UNORM );
            return ToImage( decompressed );
        }

        public static DDSPixelFormatFourCC DetermineBestCompressedFormat( Image<Rgba32> image )
        {
            return ImageHelper.HasTransparency( image ) ? DDSPixelFormatFourCC.DXT5 : DDSPixelFormatFourCC.DXT1;
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
        internal static Image<Rgba32> ToImage( DirectXTex.ScratchImage image )
        {
            var format = ( DDSDxgiFormat )image.Metadata.Format;

            if ( IsCompressed( format ) )
            {
                using var decompressed = image.Decompress( DDSDxgiFormat.R8G8B8A8_UNORM );
                return ToImage( decompressed );
            }

            if ( format != DDSDxgiFormat.R8G8B8A8_UNORM )
            {
                using var converted = image.Convert( DDSDxgiFormat.R8G8B8A8_UNORM );
                return ToImage( converted );
            }

            return Image.LoadPixelData<Rgba32>( image.GetPixels(), image.Width, image.Height );
        }

        internal static DirectXTex.ScratchImage FromImage( Image<Rgba32> image )
        {
            var pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo( pixels );
            return DirectXTex.ScratchImage.FromPixels( pixels, image.Width, image.Height, DDSDxgiFormat.R8G8B8A8_UNORM );
        }

        /// <summary>The BC1 to BC5 and BC6H to BC7 ranges of the DXGI format list.</summary>
        internal static bool IsCompressed( DDSDxgiFormat format )
        {
            return ( format >= DDSDxgiFormat.BC1_TYPELESS && format <= DDSDxgiFormat.BC5_SNORM )
                || ( format >= DDSDxgiFormat.BC6H_TYPELESS && format <= DDSDxgiFormat.BC7_UNORM_SRGB );
        }
    }
}
