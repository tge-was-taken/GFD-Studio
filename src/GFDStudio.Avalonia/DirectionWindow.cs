using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks for a direction, with the axes offered as presets since that is almost always what
    /// is wanted.
    /// </summary>
    public sealed class DirectionWindow : Window
    {
        private readonly NumericUpDown[] mAxes;

        private DirectionWindow( string title, string prompt, Vector3 initial )
        {
            Title = title;
            Width = 480;
            SizeToContent = SizeToContent.Height;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            mAxes = new[]
            {
                Box( (decimal)initial.X ), Box( (decimal)initial.Y ), Box( (decimal)initial.Z ),
            };

            var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            foreach ( var (label, box) in new[] { ("X", mAxes[0]), ("Y", mAxes[1]), ("Z", mAxes[2]) } )
            {
                row.Children.Add( new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center } );
                row.Children.Add( box );
            }

            var presets = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            foreach ( var (name, value) in new[]
                     {
                         ("+X", Vector3.UnitX), ("-X", -Vector3.UnitX),
                         ("+Y", Vector3.UnitY), ("-Y", -Vector3.UnitY),
                         ("+Z", Vector3.UnitZ), ("-Z", -Vector3.UnitZ),
                     } )
            {
                var button = new Button { Content = name, MinWidth = 46 };
                var captured = value;
                button.Click += ( s, e ) => Set( captured );
                presets.Children.Add( button );
            }

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( (Vector3?)Current );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 14,
                Children =
                {
                    new TextBlock { Text = prompt, TextWrapping = TextWrapping.Wrap, Opacity = 0.8 },
                    row,
                    presets,
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

        private Vector3 Current => new( Value( mAxes[0] ), Value( mAxes[1] ), Value( mAxes[2] ) );

        public static Task<Vector3?> Ask( Window owner, string title, string prompt, Vector3 initial )
            => new DirectionWindow( title, prompt, initial ).ShowDialog<Vector3?>( owner );

        private void Set( Vector3 value )
        {
            mAxes[0].Value = (decimal)value.X;
            mAxes[1].Value = (decimal)value.Y;
            mAxes[2].Value = (decimal)value.Z;
        }

        private static NumericUpDown Box( decimal value )
            => new() { Value = value, Increment = 0.1m, FormatString = "0.####", Width = 110 };

        private static float Value( NumericUpDown box ) => (float)( box.Value ?? 0m );
    }
}
