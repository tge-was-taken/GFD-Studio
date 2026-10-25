using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GFDLibrary.Textures.Utilities
{
    public static class ImageHelper
    {
        public static bool HasTransparency( Image<Rgba32> image )
        {
            var transparent = false;

            image.ProcessPixelRows( accessor =>
            {
                for ( var y = 0; y < accessor.Height; y++ )
                {
                    var row = accessor.GetRowSpan( y );
                    for ( var x = 0; x < row.Length; x++ )
                    {
                        if ( row[x].A != 255 )
                        {
                            transparent = true;
                            return;
                        }
                    }
                }
            } );

            return transparent;
        }

        public static Image<Rgba32> Create( Graphics.Color[] colors, int width, int height )
        {
            var image = new Image<Rgba32>( width, height );

            image.ProcessPixelRows( accessor =>
            {
                for ( var y = 0; y < height; y++ )
                {
                    var row = accessor.GetRowSpan( y );
                    for ( var x = 0; x < width; x++ )
                    {
                        var color = colors[x + y * width];
                        row[x] = new Rgba32( color.R, color.G, color.B, color.A );
                    }
                }
            } );

            return image;
        }

        /// <summary>
        /// Loads an image from 4 bytes per pixel in B, G, R, A order with no row padding, the
        /// layout the DDS and Scarlet decoders produce.
        /// </summary>
        public static Image<Rgba32> FromBgra( byte[] data, int width, int height )
        {
            var image = new Image<Rgba32>( width, height );

            image.ProcessPixelRows( accessor =>
            {
                for ( var y = 0; y < height; y++ )
                {
                    var row = accessor.GetRowSpan( y );
                    var offset = y * width * 4;
                    for ( var x = 0; x < width; x++ )
                    {
                        var i = offset + ( x * 4 );
                        row[x] = new Rgba32( data[i + 2], data[i + 1], data[i], data[i + 3] );
                    }
                }
            } );

            return image;
        }

        /// <summary>
        /// The inverse of <see cref="FromBgra"/>.
        /// </summary>
        public static byte[] ToBgra( Image<Rgba32> image )
        {
            var data = new byte[image.Width * image.Height * 4];

            image.ProcessPixelRows( accessor =>
            {
                for ( var y = 0; y < accessor.Height; y++ )
                {
                    var row = accessor.GetRowSpan( y );
                    var offset = y * accessor.Width * 4;
                    for ( var x = 0; x < row.Length; x++ )
                    {
                        var i = offset + ( x * 4 );
                        data[i] = row[x].B;
                        data[i + 1] = row[x].G;
                        data[i + 2] = row[x].R;
                        data[i + 3] = row[x].A;
                    }
                }
            } );

            return data;
        }
    }
}
