using System;
using System.IO;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLTexture : IDisposable
    {
        public int Id { get; }

        public unsafe GLTexture( Texture texture )
        {
            Id = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, Id );

            switch ( texture.Format )
            {
                case TextureFormat.DDS:
                    {
                        var ddsHeader = new DDSHeader( new MemoryStream( texture.Data ) );
                        var dataOffset = 0x4 + 0x7C; // 4 + 124, INFO + HEADER

                        // todo: identify and retrieve values from texture
                        // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
                        int mipMapCount = ddsHeader.MipMapCount;
                        if ( mipMapCount > 0 )
                            --mipMapCount;
                        else
                            mipMapCount = 1;

                        SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, mipMapCount );

                        var format = (PixelInternalFormat)0;

                        if ( ddsHeader.PixelFormat.FourCC != 0 ||
                            !ddsHeader.PixelFormat.Flags.HasFlag( DDSPixelFormatFlags.RGB ) ||
                            ddsHeader.PixelFormat.RGBBitCount != 32 )
                            format = GetPixelInternalFormat( ddsHeader.PixelFormat.FourCC );
                        else if ( ddsHeader.PixelFormat.Flags.HasFlag( DDSPixelFormatFlags.AlphaPixels ) )
                            format = PixelInternalFormat.Rgba;
                        else
                            format = PixelInternalFormat.Rgb;

                        if ( format == PixelInternalFormat.CompressedRgbaBptcUnorm ) dataOffset += 0x14; //DX10 have an additional 20 bytes of header

                        UploadDDSTextureData( ddsHeader.Width, ddsHeader.Height, format, 1, texture.Data, dataOffset );
                    }
                    break;

                default:
                    {
                        // rip hardware acceleration
                        var bitmap = TextureDecoder.Decode( texture );
                        SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, 1 );

                        var bitmapData = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, bitmap.Width, bitmap.Height ),
                                         System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

                        var pixels = new byte[bitmap.Width * bitmap.Height * 4];
                        fixed ( byte* pPixels = &pixels[ 0 ] )
                        {
                            var pSrc = ( byte* ) bitmapData.Scan0;
                            var pDest = pPixels;
                            for ( int y = 0; y < bitmap.Height; y++ )
                            {
                                for ( int x = 0; x < bitmap.Width; x++ )
                                {
                                    *pDest++ = pSrc[ 2 ];
                                    *pDest++ = pSrc[ 1 ];
                                    *pDest++ = pSrc[ 0 ];
                                    *pDest++ = pSrc[ 3 ];
                                    pSrc += 4;
                                }
                            }

                            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba,
                                           PixelType.UnsignedByte, ( IntPtr )pPixels );
                        }

                        bitmap.UnlockBits( bitmapData );
                    }
                    break;

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

        private static PixelInternalFormat GetPixelInternalFormat( DDSPixelFormatFourCC format )
        {
            switch ( format )
            {
                case DDSPixelFormatFourCC.DXT1:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;

                case DDSPixelFormatFourCC.DXT3:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;

                case DDSPixelFormatFourCC.DXT5:
                    return PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;

                case DDSPixelFormatFourCC.A8R8G8B8:
                    return PixelInternalFormat.Rgba8;

                case DDSPixelFormatFourCC.DX10:
                    return PixelInternalFormat.CompressedRgbaBptcUnorm;

                default:
                    throw new NotImplementedException( format.ToString() );
            }
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
            int mipWidth  = width;
            int mipHeight = height;
            int blockSize = ( format == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext ) ? 8 : 16;
            int mipOffset = 0;

            if (format == PixelInternalFormat.Rgb || format == PixelInternalFormat.Rgba)
                blockSize = 8;

            for ( int mipLevel = 0; mipLevel < mipMapCount; mipLevel++ )
            {
                int mipSize = ( ( mipWidth * mipHeight ) / 16 ) * blockSize;

                if ( mipSize > blockSize)
                {
                    switch ( format )
                    {
                        case PixelInternalFormat.Rgb:
                        //    GL.TexImage2D(TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data + mipOffset);
                        //    break;
                        case PixelInternalFormat.Rgba:
                            GL.TexImage2D(TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data + mipOffset);
                            break;
                        default:
                            GL.CompressedTexImage2D(TextureTarget.Texture2D, mipLevel, (InternalFormat)format, mipWidth, mipHeight, 0, mipSize, data + mipOffset);
                            break;
                    }
                }

                mipOffset += mipSize;
                mipWidth  /= 2;
                mipHeight /= 2;
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
