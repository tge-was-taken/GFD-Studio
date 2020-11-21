using DirectXTexNet;
using GFDLibrary.Textures.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;

namespace GFDLibrary.Textures.DDS
{
    /// <summary>
    /// DDS codec for decompressing and compressing bitmap data to DDS formats.
    /// </summary>
    public static class DDSCodec
    {
        /// <summary>
        /// Decompress a DDS image file and output an RGBA bitmap.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Bitmap Decompress( string filepath )
        {
            return Decompress( File.ReadAllBytes( filepath ) );
        }

        /// <summary>
        /// Decompress a DDS image stream and output an RGBA bitmap.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Bitmap Decompress( Stream stream )
        {
            // TODO: not make a copy
            var bytes = new byte[stream.Length];
            stream.Read( bytes, 0, ( int )stream.Length );
            return Decompress( bytes );
        }

        /// <summary>
        /// Decompress a DDS image and output an RGBA bitmap.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap Decompress( byte[] image )
        {
            var header = new DDSHeader( image );
            var metadata = DecompressInternal( image );
            return metadata.ConvertToBitmap();
        }

        /// <summary>
        /// Compress the bitmap into a DDS image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static TextureMetadata Compress( Bitmap image, DXGIFormat format )
        {
            return CompressInternal( image, format );
        }

        /// <summary>
        /// Determine the best compressed DDS pixel format for a given bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static DXGIFormat DetermineBestDXGIFormat( Bitmap bitmap )
        {
            var ddsFormat = DXGIFormat.BC1_UNORM;
            if ( BitmapHelper.HasTransparency( bitmap ) )
            {
                ddsFormat = DXGIFormat.BC3_UNORM;
            }

            return ddsFormat;
        }

        private static unsafe TextureMetadata DecompressInternal( byte[] data )
        {
            fixed ( byte* pData = data )
            {
                var guid = TexHelper.Instance.GetWICCodec( WICCodecs.BMP );
                using var dds = TexHelper.Instance.LoadFromDDSMemory( (IntPtr)pData, data.Length, DDS_FLAGS.NONE );
                var ddsMeta = dds.GetMetadata();
                using var dec = TexHelper.Instance.IsCompressed( ddsMeta.Format ) ?
                    dds.Decompress( 0, DXGI_FORMAT.B8G8R8A8_UNORM ) :
                    dds.Convert( 0, DXGI_FORMAT.B8G8R8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0.5f );
                var decMeta = dec.GetMetadata();

                var pixels = new byte[ decMeta.Width * decMeta.Height * 4 ];
                Trace.Assert( dec.GetPixelsSize() == pixels.Length );

                fixed ( byte* pPixels = pixels )
                    Unsafe.CopyBlockUnaligned( (void*)pPixels, (void*)dec.GetPixels(), (uint)pixels.Length );

                return new TextureMetadata()
                {
                    PixelsOffset = 0,
                    Width = decMeta.Width,
                    Height = decMeta.Height,
                    Format = (DXGIFormat)decMeta.Format,
                    Data = pixels,
                };
            }
        }

        private static unsafe TextureMetadata CompressInternal( Bitmap bitmap, DXGIFormat format )
        {
            if ( format == DXGIFormat.UNKNOWN )
                format = DetermineBestDXGIFormat( bitmap );

            // TODO mipmaps
            var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
            using var scratch = TexHelper.Instance.Initialize2D( DXGI_FORMAT.B8G8R8A8_UNORM, bitmap.Width, bitmap.Height, 1, 1, CP_FLAGS.NONE );
            Unsafe.CopyBlock( (void*)scratch.GetPixels(), (void*)bitmapData.Scan0, (uint)( scratch.GetPixelsSize() ) );
            bitmap.UnlockBits( bitmapData );

            var compressFlags = TexHelper.Instance.IsCompressed( ( DXGI_FORMAT)format ) ?
                TEX_COMPRESS_FLAGS.DITHER | TEX_COMPRESS_FLAGS.PARALLEL :
                TEX_COMPRESS_FLAGS.DEFAULT;
            using var ddsImage = scratch.Compress( (DXGI_FORMAT)format, compressFlags, 0.5f );
            var ddsMeta = ddsImage.GetMetadata();
            var ddsStream = ddsImage.SaveToDDSMemory( (DDS_FLAGS)0x40000 );

            var decData = new byte[ ddsStream.Length ];
            fixed ( byte* pDecData = decData )
                Unsafe.CopyBlockUnaligned( (void*)pDecData, (void*)ddsStream.PositionPointer, (uint)ddsStream.Length );

            return new TextureMetadata()
            {
                PixelsOffset = DDSHeader.SIZE,
                Width = ddsMeta.Width,
                Height = ddsMeta.Height,
                Format = (DXGIFormat)ddsMeta.Format,
                Data = decData,
            };
        }

        private static int GetBlockSize( DXGIFormat format )
        {
            switch ( format )
            {
                case DXGIFormat.BC1_TYPELESS:
                case DXGIFormat.BC1_UNORM:
                case DXGIFormat.BC1_UNORM_SRGB:
                case DXGIFormat.BC4_SNORM:
                case DXGIFormat.BC4_TYPELESS:
                case DXGIFormat.BC4_UNORM:
                    return 8;

                case DXGIFormat.BC2_TYPELESS:
                case DXGIFormat.BC2_UNORM:
                case DXGIFormat.BC2_UNORM_SRGB:
                case DXGIFormat.BC3_TYPELESS:
                case DXGIFormat.BC3_UNORM:
                case DXGIFormat.BC3_UNORM_SRGB:
                case DXGIFormat.BC5_SNORM:
                case DXGIFormat.BC5_TYPELESS:
                case DXGIFormat.BC5_UNORM:
                    return 16;
            }

            return 0;
        }
    }

    public class TextureMetadata
    {
        public int PixelsOffset { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public DXGIFormat Format { get; init; }
        public byte[] Data { get; init; }
        public Span<byte> Pixels => Data.AsSpan().Slice( PixelsOffset );

        public Bitmap ConvertToBitmap()
        {
            var bitmap = new Bitmap( Width, Height, PixelFormat.Format32bppArgb );
            var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

            unsafe
            {
                fixed ( byte* pixels = Pixels )
                    Unsafe.CopyBlock( (void*)bitmapData.Scan0.ToPointer(), (void*)pixels, (uint)( Pixels.Length ) );
            }

            bitmap.UnlockBits( bitmapData );

            return bitmap;
        }
    }
}
