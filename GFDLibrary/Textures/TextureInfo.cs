using System;
using System.Diagnostics;
using System.IO;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.Utilities;

namespace GFDLibrary.Textures
{
    public class TextureInfo
    {
        public Texture Texture { get; }

        public string Name { get; }

        public int Width { get; }

        public int Height { get; }

        public TexturePixelFormat PixelFormat { get; }

        public int MipMapCount { get; }

        internal TextureInfo( Texture texture, int width, int height, int mipMapCount, TexturePixelFormat format )
        {
            Texture = texture;
            Name = texture.Name;
            Width = width;
            Height = height;
            PixelFormat = format;
            MipMapCount = mipMapCount;
        }

        public static TextureInfo GetTextureInfo( Texture texture )
        {
            // Load DDS header so we can extract some info from it
            var ddsHeader = new DDSHeader( texture.Data );

            TexturePixelFormat format;
            switch ( ddsHeader.PixelFormat.FourCC )
            {
                case DDSPixelFormatFourCC.DXT1:
                    format = TexturePixelFormat.BC1;
                    break;
                case DDSPixelFormatFourCC.DXT3:
                    format = TexturePixelFormat.BC2;
                    break;
                case DDSPixelFormatFourCC.DXT5:
                    format = TexturePixelFormat.BC3;
                    break;
                case DDSPixelFormatFourCC.A8R8G8B8:
                    format = TexturePixelFormat.ARGB;
                    break;
                case DDSPixelFormatFourCC.DX10:
                    switch (ddsHeader.DxgiFormat)
                    {
                        case DDSDxgiFormat.BC7_UNORM:
                            format = TexturePixelFormat.BC7;
                            break;
                        default:
                            throw new NotSupportedException( $"Unsupported DXGI format {ddsHeader.DxgiFormat}" );
                    }
                    break;
                case DDSPixelFormatFourCC.Unknown:
                    // Maybe from a screen ripping tool, or something else
                    Logger.Debug( $"{nameof(GetTextureInfo)}({texture}): DDS FourCC not set" );

                    if ( ddsHeader.PixelFormat.Flags.HasFlag( DDSPixelFormatFlags.RGB ) &&
                         ddsHeader.PixelFormat.Flags.HasFlag( DDSPixelFormatFlags.AlphaPixels ) &&
                         ddsHeader.PixelFormat.RGBBitCount == 32 )
                    {
                        Logger.Debug( "Converting RGBA pixels to DDS" );

                        // Read pixel colors
                        var pixelData = new Graphics.Color[( texture.Data.Length - DDSHeader.SIZE ) / 4];
                        for ( int i = 0; i < pixelData.Length; i++ )
                        {
                            pixelData[ i ] = new Graphics.Color
                            (
                                 texture.Data[ DDSHeader.SIZE + ( i * 4 ) + 2 ], // r
                                 texture.Data[ DDSHeader.SIZE + ( i * 4 ) + 1 ], // g
                                 texture.Data[ DDSHeader.SIZE + ( i * 4 ) + 0 ], // b
                                 texture.Data[ DDSHeader.SIZE + ( i * 4 ) + 3 ]  // a
                            );
                        }
                        
                        // Create RGBA bitmap
                        var bitmap = BitmapHelper.Create( pixelData, ddsHeader.Width, ddsHeader.Height );

                        // Convert bitmap to DDS
                        texture.Data = DDSCodec.CompressImage( bitmap );
                        return GetTextureInfo( texture );
                    }
                    else
                    {
                        goto default;
                    }

                default:
                    throw new NotSupportedException( $"Unsupported DDS pixel format {ddsHeader.PixelFormat.FourCC}" );
            }

            return new TextureInfo( texture, ddsHeader.Width, ddsHeader.Height, ddsHeader.MipMapCount, format );
        }
    }
}