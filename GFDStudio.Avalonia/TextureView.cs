using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Image = GFDLibrary.Graphics.Image;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Which parts of a texture to draw. The channels can be viewed on their own, because a
    /// mask packed into one of them is invisible in the combined image.
    /// </summary>
    [Flags]
    public enum TextureChannels
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Alpha = 8,
        Colour = Red | Green | Blue,
        All = Colour | Alpha,
    }

    /// <summary>
    /// Draws a texture over a checker, at its own size unless panned or zoomed, and without
    /// smoothing so that single pixels stay readable.
    /// </summary>
    public sealed class TextureView : Control
    {
        private static readonly IBrush CheckerDark = new SolidColorBrush( Color.FromRgb( 0x50, 0x50, 0x50 ) );
        private static readonly IBrush CheckerLight = new SolidColorBrush( Color.FromRgb( 0x70, 0x70, 0x70 ) );
        private const int CheckerSize = 8;

        private Image mImage;
        private Bitmap mBitmap;
        private TextureChannels mChannels = TextureChannels.All;
        private double mScale = 1;
        private Point mOffset;
        private Point mLastPointer;

        public TextureView() => ClipToBounds = true;

        public int PixelWidth => mImage?.Width ?? 0;

        public int PixelHeight => mImage?.Height ?? 0;

        public double Scale
        {
            get => mScale;
            set
            {
                mScale = Math.Clamp( value, 0.05, 32 );
                InvalidateVisual();
            }
        }

        public TextureChannels Channels
        {
            get => mChannels;
            set
            {
                mChannels = value;
                Rebuild();
            }
        }

        public void Show( Image image )
        {
            mImage = image;
            mScale = 1;
            mOffset = default;
            Rebuild();
        }

        /// <summary>
        /// Puts the texture back at its own size, centred.
        /// </summary>
        public void ResetView()
        {
            mScale = 1;
            mOffset = default;
            InvalidateVisual();
        }

        public override void Render( DrawingContext context )
        {
            DrawChecker( context );

            if ( mBitmap == null )
                return;

            var width = PixelWidth * mScale;
            var height = PixelHeight * mScale;
            var origin = new Point( ( Bounds.Width - width ) / 2 + mOffset.X,
                                    ( Bounds.Height - height ) / 2 + mOffset.Y );

            using ( context.PushRenderOptions( new RenderOptions { BitmapInterpolationMode = BitmapInterpolationMode.None } ) )
                context.DrawImage( mBitmap, new Rect( origin, new Size( width, height ) ) );
        }

        protected override void OnPointerPressed( PointerPressedEventArgs e )
        {
            base.OnPointerPressed( e );
            mLastPointer = e.GetPosition( this );
        }

        protected override void OnPointerMoved( PointerEventArgs e )
        {
            base.OnPointerMoved( e );

            var position = e.GetPosition( this );
            var point = e.GetCurrentPoint( this );

            if ( point.Properties.IsLeftButtonPressed || point.Properties.IsRightButtonPressed )
            {
                mOffset += position - mLastPointer;
                InvalidateVisual();
            }

            mLastPointer = position;
        }

        protected override void OnPointerWheelChanged( PointerWheelEventArgs e )
        {
            base.OnPointerWheelChanged( e );
            Scale *= e.Delta.Y > 0 ? 1.25 : 0.8;
        }

        private void DrawChecker( DrawingContext context )
        {
            context.FillRectangle( CheckerDark, new Rect( Bounds.Size ) );

            for ( var y = 0; y < Bounds.Height; y += CheckerSize )
            {
                for ( var x = ( y / CheckerSize % 2 == 0 ? 0 : CheckerSize ); x < Bounds.Width; x += CheckerSize * 2 )
                    context.FillRectangle( CheckerLight, new Rect( x, y, CheckerSize, CheckerSize ) );
            }
        }

        private void Rebuild()
        {
            mBitmap = mImage == null ? null : Filtered( mImage, mChannels ).ToAvaloniaBitmap();
            InvalidateVisual();
        }

        /// <summary>
        /// Keeps only the chosen channels. A single channel is drawn as grey so it reads as a
        /// mask rather than a tint, and dropping alpha makes the image fully opaque.
        /// </summary>
        private static Image Filtered( Image image, TextureChannels channels )
        {
            if ( channels == TextureChannels.All )
                return image;

            var pixels = (byte[])image.ToBgra().Clone();
            var single = channels is TextureChannels.Red or TextureChannels.Green or TextureChannels.Blue or TextureChannels.Alpha;

            for ( var i = 0; i < pixels.Length; i += 4 )
            {
                var b = pixels[i];
                var g = pixels[i + 1];
                var r = pixels[i + 2];
                var a = pixels[i + 3];

                if ( single )
                {
                    var value = channels switch
                    {
                        TextureChannels.Red => r,
                        TextureChannels.Green => g,
                        TextureChannels.Blue => b,
                        _ => a,
                    };

                    pixels[i] = pixels[i + 1] = pixels[i + 2] = value;
                    pixels[i + 3] = 255;
                    continue;
                }

                pixels[i] = channels.HasFlag( TextureChannels.Blue ) ? b : (byte)0;
                pixels[i + 1] = channels.HasFlag( TextureChannels.Green ) ? g : (byte)0;
                pixels[i + 2] = channels.HasFlag( TextureChannels.Red ) ? r : (byte)0;
                pixels[i + 3] = channels.HasFlag( TextureChannels.Alpha ) ? a : (byte)255;
            }

            return new Image( image.Width, image.Height, FromBgra( pixels ) );
        }

        private static byte[] FromBgra( byte[] bgra )
        {
            var rgba = new byte[bgra.Length];

            for ( var i = 0; i < bgra.Length; i += 4 )
            {
                rgba[i] = bgra[i + 2];
                rgba[i + 1] = bgra[i + 1];
                rgba[i + 2] = bgra[i];
                rgba[i + 3] = bgra[i + 3];
            }

            return rgba;
        }
    }
}
