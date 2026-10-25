using System;

namespace GFDLibrary.Graphics
{
    /// <summary>
    /// An RGBA32 image, 4 bytes per pixel with no row padding.
    /// </summary>
    public sealed class Image
    {
        public int Width { get; }

        public int Height { get; }

        /// <summary>Pixels in R, G, B, A order, row by row.</summary>
        public byte[] Pixels { get; }

        public Image( int width, int height )
            : this( width, height, new byte[width * height * 4] )
        {
        }

        public Image( int width, int height, byte[] pixels )
        {
            if ( width <= 0 ) throw new ArgumentOutOfRangeException( nameof( width ) );
            if ( height <= 0 ) throw new ArgumentOutOfRangeException( nameof( height ) );
            if ( pixels == null ) throw new ArgumentNullException( nameof( pixels ) );
            if ( pixels.Length < width * height * 4 )
                throw new ArgumentException( "Pixel data is shorter than the image", nameof( pixels ) );

            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public Color this[int x, int y]
        {
            get
            {
                var i = Offset( x, y );
                return new Color( Pixels[i], Pixels[i + 1], Pixels[i + 2], Pixels[i + 3] );
            }
            set
            {
                var i = Offset( x, y );
                Pixels[i] = value.R;
                Pixels[i + 1] = value.G;
                Pixels[i + 2] = value.B;
                Pixels[i + 3] = value.A;
            }
        }

        private int Offset( int x, int y )
        {
            if ( ( uint )x >= ( uint )Width ) throw new ArgumentOutOfRangeException( nameof( x ) );
            if ( ( uint )y >= ( uint )Height ) throw new ArgumentOutOfRangeException( nameof( y ) );

            return ( ( y * Width ) + x ) * 4;
        }

        public bool HasTransparency()
        {
            for ( var i = 3; i < Pixels.Length; i += 4 )
            {
                if ( Pixels[i] != 255 )
                    return true;
            }

            return false;
        }

        /// <summary>Builds an image from 4 bytes per pixel in B, G, R, A order.</summary>
        public static Image FromBgra( byte[] data, int width, int height )
        {
            var pixels = new byte[width * height * 4];
            for ( var i = 0; i < pixels.Length; i += 4 )
            {
                pixels[i] = data[i + 2];
                pixels[i + 1] = data[i + 1];
                pixels[i + 2] = data[i];
                pixels[i + 3] = data[i + 3];
            }

            return new Image( width, height, pixels );
        }

        /// <summary>The inverse of <see cref="FromBgra"/>.</summary>
        public byte[] ToBgra()
        {
            var data = new byte[Pixels.Length];
            for ( var i = 0; i < data.Length; i += 4 )
            {
                data[i] = Pixels[i + 2];
                data[i + 1] = Pixels[i + 1];
                data[i + 2] = Pixels[i];
                data[i + 3] = Pixels[i + 3];
            }

            return data;
        }

        public static Image FromColors( Color[] colors, int width, int height )
        {
            var image = new Image( width, height );
            for ( var i = 0; i < width * height; i++ )
            {
                var color = colors[i];
                image.Pixels[i * 4] = color.R;
                image.Pixels[( i * 4 ) + 1] = color.G;
                image.Pixels[( i * 4 ) + 2] = color.B;
                image.Pixels[( i * 4 ) + 3] = color.A;
            }

            return image;
        }
    }
}
