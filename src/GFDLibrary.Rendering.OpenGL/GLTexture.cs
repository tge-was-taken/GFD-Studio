using System;
using System.IO;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using OpenTK.Graphics.OpenGL4;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLTexture : IDisposable
    {
        public int Id { get; }

        public GLTexture( Texture texture )
        {
            Id = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, Id );

            if ( texture.Format == TextureFormat.DDS && TryUploadDDS( texture.Data ) )
                return;

            UploadDecoded( texture );
        }

        /// <summary>
        /// Hands the compressed blocks straight to the driver. Returns false for a format the
        /// driver cannot take, which is then decoded on the CPU instead.
        /// </summary>
        private static bool TryUploadDDS( byte[] data )
        {
            var header = new DDSHeader( new MemoryStream( data ) );

            if ( !TryGetPixelInternalFormat( header, out var format ) )
                return false;

            // todo: identify and retrieve values from texture
            // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
            var mipMapCount = header.MipMapCount > 0 ? header.MipMapCount - 1 : 1;

            SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, mipMapCount );

            var dataOffset = 0x4 + 0x7C; // 4 + 124, INFO + HEADER

            // A DX10 four character code means the extended header sits between the header and
            // the pixel data, whatever format it names.
            if ( header.PixelFormat.FourCC == DDSPixelFormatFourCC.DX10 )
                dataOffset += 0x14;

            UploadDDSTextureData( header.Width, header.Height, format, 1, data, dataOffset );
            return true;
        }

        private static unsafe void UploadDecoded( Texture texture )
        {
            // rip hardware acceleration
            var bitmap = TextureDecoder.Decode( texture );
            SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, 1 );

            var pixels = bitmap.Pixels;

            fixed ( byte* pPixels = &pixels[ 0 ] )
            {
                GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba,
                               PixelType.UnsignedByte, ( IntPtr )pPixels );
            }
        }

        public GLTexture( FieldTexturePS3 texture )
        {
            Id = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, Id );

            // todo: identify and retrieve values from texture
            // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
            SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, texture.MipMapCount - 1 );

            var format = GetPixelInternalFormat( texture.Flags );

            UploadDDSTextureData( texture.Width, texture.Height, format, texture.MipMapCount, texture.Data );
        }

        public void Bind()
        {
            GL.BindTexture( TextureTarget.Texture2D, Id );
        }

        private static void SetTextureParameters( TextureWrapMode wrapS, TextureWrapMode wrapT, TextureMagFilter magFilter, TextureMinFilter minFilter, int maxMipLevel )
        {
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )wrapS );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )wrapT );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )magFilter );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )minFilter );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, maxMipLevel );
        }

        private static bool TryGetPixelInternalFormat( DDSHeader header, out PixelInternalFormat format )
        {
            var pixelFormat = header.PixelFormat;

            if ( pixelFormat.FourCC == 0 &&
                 pixelFormat.Flags.HasFlag( DDSPixelFormatFlags.RGB ) &&
                 pixelFormat.RGBBitCount == 32 )
            {
                format = pixelFormat.Flags.HasFlag( DDSPixelFormatFlags.AlphaPixels )
                    ? PixelInternalFormat.Rgba
                    : PixelInternalFormat.Rgb;

                return true;
            }

            switch ( pixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.DXT1:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    return true;

                // DXT2 and DXT4 are DXT3 and DXT5 with the colour premultiplied by the alpha,
                // which the games do not use, so the blocks decode the same way.
                case DDSPixelFormatFourCC.DXT2:
                case DDSPixelFormatFourCC.DXT3:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    return true;

                case DDSPixelFormatFourCC.DXT4:
                case DDSPixelFormatFourCC.DXT5:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    return true;

                case DDSPixelFormatFourCC.ATI1:
                    format = PixelInternalFormat.CompressedRedRgtc1;
                    return true;

                case DDSPixelFormatFourCC.ATI2N_3Dc:
                    format = PixelInternalFormat.CompressedRgRgtc2;
                    return true;

                case DDSPixelFormatFourCC.A8R8G8B8:
                    format = PixelInternalFormat.Rgba8;
                    return true;

                case DDSPixelFormatFourCC.DX10:
                    return TryGetPixelInternalFormat( header.DxgiFormat, out format );
            }

            format = 0;
            return false;
        }

        private static bool TryGetPixelInternalFormat( DDSDxgiFormat dxgiFormat, out PixelInternalFormat format )
        {
            switch ( dxgiFormat )
            {
                case DDSDxgiFormat.BC1_TYPELESS:
                case DDSDxgiFormat.BC1_UNORM:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    return true;

                case DDSDxgiFormat.BC1_UNORM_SRGB:
                    format = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext;
                    return true;

                case DDSDxgiFormat.BC2_TYPELESS:
                case DDSDxgiFormat.BC2_UNORM:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    return true;

                case DDSDxgiFormat.BC2_UNORM_SRGB:
                    format = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                    return true;

                case DDSDxgiFormat.BC3_TYPELESS:
                case DDSDxgiFormat.BC3_UNORM:
                    format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    return true;

                case DDSDxgiFormat.BC3_UNORM_SRGB:
                    format = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
                    return true;

                case DDSDxgiFormat.BC4_TYPELESS:
                case DDSDxgiFormat.BC4_UNORM:
                    format = PixelInternalFormat.CompressedRedRgtc1;
                    return true;

                case DDSDxgiFormat.BC4_SNORM:
                    format = PixelInternalFormat.CompressedSignedRedRgtc1;
                    return true;

                case DDSDxgiFormat.BC5_TYPELESS:
                case DDSDxgiFormat.BC5_UNORM:
                    format = PixelInternalFormat.CompressedRgRgtc2;
                    return true;

                case DDSDxgiFormat.BC5_SNORM:
                    format = PixelInternalFormat.CompressedSignedRgRgtc2;
                    return true;

                case DDSDxgiFormat.BC6H_TYPELESS:
                case DDSDxgiFormat.BC6H_UF16:
                    format = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                    return true;

                case DDSDxgiFormat.BC6H_SF16:
                    format = PixelInternalFormat.CompressedRgbBptcSignedFloat;
                    return true;

                case DDSDxgiFormat.BC7_TYPELESS:
                case DDSDxgiFormat.BC7_UNORM:
                    format = PixelInternalFormat.CompressedRgbaBptcUnorm;
                    return true;

                case DDSDxgiFormat.BC7_UNORM_SRGB:
                    format = PixelInternalFormat.CompressedSrgbAlphaBptcUnorm;
                    return true;
            }

            format = 0;
            return false;
        }

        private static PixelInternalFormat GetPixelInternalFormat( FieldTextureFlags flags )
        {
            PixelInternalFormat format = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
            if ( flags.HasFlag( FieldTextureFlags.DXT3 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
            }
            else if ( flags.HasFlag( FieldTextureFlags.DXT5 ) )
            {
                format = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
            }

            return format;
        }

        private static unsafe void UploadDDSTextureData( int width, int height, PixelInternalFormat format, int mipMapCount, byte[] data, int dataOffset = 0 )
        {
            fixed ( byte* pData = &data[ 0 ] )
                UploadDDSTextureData( width, height, format, mipMapCount, ( IntPtr ) ( pData + dataOffset ) );
        }

        private static void UploadDDSTextureData( int width, int height, PixelInternalFormat format, int mipMapCount, IntPtr data )
        {
            var uncompressed = format == PixelInternalFormat.Rgb || format == PixelInternalFormat.Rgba;
            var blockSize = GetBlockSize( format );
            var mipWidth  = width;
            var mipHeight = height;
            var mipOffset = 0;

            for ( int mipLevel = 0; mipLevel < mipMapCount && mipWidth > 0 && mipHeight > 0; mipLevel++ )
            {
                if ( uncompressed )
                {
                    GL.TexImage2D( TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data + mipOffset );
                    mipOffset += mipWidth * mipHeight * 4;
                }
                else
                {
                    // A level is a whole number of 4x4 blocks, so one smaller than that still
                    // takes a full block and the last levels are not all the same size.
                    var mipSize = Math.Max( 1, ( mipWidth + 3 ) / 4 ) * Math.Max( 1, ( mipHeight + 3 ) / 4 ) * blockSize;
                    GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, (InternalFormat)format, mipWidth, mipHeight, 0, mipSize, data + mipOffset );
                    mipOffset += mipSize;
                }

                mipWidth  /= 2;
                mipHeight /= 2;
            }
        }

        private static int GetBlockSize( PixelInternalFormat format )
        {
            switch ( format )
            {
                case PixelInternalFormat.CompressedRgbS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedSrgbS3tcDxt1Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRedRgtc1:
                case PixelInternalFormat.CompressedSignedRedRgtc1:
                    return 8;

                default:
                    return 16;
            }
        }

        #region IDisposable Support
        private bool mDisposed = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    GL.DeleteTexture( Id );
                }

                mDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose( true );
        }
        #endregion
    }
}
