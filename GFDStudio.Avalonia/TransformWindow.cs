using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks for a scale, position and rotation to apply to a batch of animation packs.
    /// </summary>
    public sealed class TransformWindow : Window
    {
        private readonly NumericUpDown[] mScale = Row( 1 );
        private readonly NumericUpDown[] mPosition = Row( 0 );
        private readonly NumericUpDown[] mRotation = Row( 0, 0, 0, 1 );

        private TransformWindow( string title )
        {
            Title = title;
            Width = 460;
            SizeToContent = SizeToContent.Height;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( new TransformRequest(
                new Vector3( Value( mScale[0] ), Value( mScale[1] ), Value( mScale[2] ) ),
                new Vector3( Value( mPosition[0] ), Value( mPosition[1] ), Value( mPosition[2] ) ),
                new Quaternion( Value( mRotation[0] ), Value( mRotation[1] ), Value( mRotation[2] ), Value( mRotation[3] ) ) ) );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 12,
                Children =
                {
                    Group( "Scale", mScale, "X", "Y", "Z" ),
                    Group( "Position", mPosition, "X", "Y", "Z" ),
                    Group( "Rotation", mRotation, "X", "Y", "Z", "W" ),
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { ok, cancel },
                    },
                },
            };
        }

        public static Task<TransformRequest> Ask( Window owner, string title )
            => new TransformWindow( title ).ShowDialog<TransformRequest>( owner );

        private static NumericUpDown[] Row( params decimal[] defaults )
        {
            if ( defaults.Length == 1 )
                defaults = new[] { defaults[0], defaults[0], defaults[0] };

            var boxes = new NumericUpDown[defaults.Length];
            for ( var i = 0; i < defaults.Length; i++ )
                boxes[i] = new NumericUpDown { Value = defaults[i], Increment = 0.1m, FormatString = "0.####" };

            return boxes;
        }

        private static Control Group( string name, NumericUpDown[] boxes, params string[] labels )
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            row.Children.Add( new TextBlock { Text = name, Width = 70, VerticalAlignment = VerticalAlignment.Center } );

            for ( var i = 0; i < boxes.Length; i++ )
            {
                row.Children.Add( new TextBlock { Text = labels[i], VerticalAlignment = VerticalAlignment.Center } );
                boxes[i].Width = 84;
                row.Children.Add( boxes[i] );
            }

            return row;
        }

        private static float Value( NumericUpDown box )
            => (float)( box.Value ?? 0m );
    }
}
