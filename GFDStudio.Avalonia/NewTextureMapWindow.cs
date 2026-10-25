using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks which slot a new texture map goes in and what to call it.
    /// </summary>
    public sealed class NewTextureMapWindow : Window
    {
        private static readonly string[] Types =
        {
            "Diffuse Map", "Normal Map", "Specular Map", "Reflection Map", "Highlight Map",
            "Glow Map", "Night Map", "Detail Map", "Shadow Map",
        };

        private readonly TextBox mName = new();
        private readonly ComboBox mType = new() { ItemsSource = Types, SelectedIndex = 0 };

        private NewTextureMapWindow()
        {
            Title = "New texture map";
            SizeToContent = SizeToContent.Height;
            Width = 380;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( new NewTextureMap( mName.Text ?? string.Empty, mType.SelectedIndex ) );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions( "Auto,*" ),
                RowDefinitions = new RowDefinitions( "Auto,Auto" ),
                ColumnSpacing = 12,
                RowSpacing = 8,
            };

            grid.Children.Add( Label( "Type", 0 ) );
            grid.Children.Add( Cell( mType, 0 ) );
            grid.Children.Add( Label( "Name", 1 ) );
            grid.Children.Add( Cell( mName, 1 ) );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 16,
                Children =
                {
                    grid,
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

        public static Task<NewTextureMap> Show( Window owner )
            => new NewTextureMapWindow().ShowDialog<NewTextureMap>( owner );

        private static Control Label( string text, int row )
        {
            var block = new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow( block, row );
            Grid.SetColumn( block, 0 );
            return block;
        }

        private static Control Cell( Control control, int row )
        {
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow( control, row );
            Grid.SetColumn( control, 1 );
            return control;
        }
    }
}
