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

        public GLTexture( Texture texture )
        {
            Id = GL.GenTexture();
            GL.BindTexture( TextureTarget.Texture2D, Id );

            var ddsHeader = new DDSHeader( new MemoryStream( texture.Data ) );

            // todo: identify and retrieve values from texture
            // todo: disable mipmaps for now, they often break and show up as black ( eg after replacing a texture )
            int mipMapCount = ddsHeader.MipMapCount;
            if ( mipMapCount > 0 )
                --mipMapCount;
            else
                mipMapCount = 1;

            SetTextureParameters( TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureMagFilter.Linear, TextureMinFilter.Linear, mipMapCount );

            var format = GetPixelInternalFormat( ddsHeader.PixelFormat.FourCC );

            UploadDDSTextureData( ddsHeader.Width, ddsHeader.Height, format, mipMapCount, texture.Data, 0x80 );
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

            for ( int mipLevel = 0; mipLevel < mipMapCount; mipLevel++ )
            {
                int mipSize = ( ( mipWidth * mipHeight ) / 16 ) * blockSize;

                if ( mipSize > blockSize )
                    GL.CompressedTexImage2D( TextureTarget.Texture2D, mipLevel, format, mipWidth, mipHeight, 0, mipSize, data + mipOffset );

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
