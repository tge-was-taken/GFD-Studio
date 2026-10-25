using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks which file to replace a node from, in which format, and what of the original to
    /// keep. The keep toggles only show for a material, which is the only thing they affect.
    /// </summary>
    public sealed class ReplaceOptionsWindow : Window
    {
        private readonly ReplaceOptionsRequest mRequest;
        private readonly ComboBox mFormat;
        private readonly TextBox mPath;
        private readonly CheckBox mRetainNames;
        private readonly CheckBox mRetainColors;

        private readonly TextBlock mDescription = new()
        {
            Opacity = 0.75,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 18,
        };

        private ReplaceOptionsWindow( ReplaceOptionsRequest request )
        {
            mRequest = request;

            Title = "Replace";
            Width = 620;
            SizeToContent = SizeToContent.Height;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            mFormat = new ComboBox
            {
                ItemsSource = request.Formats,
                SelectedIndex = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ItemTemplate = new FuncDataTemplate<FileFormat>( ( format, scope ) => Entry( format ), true ),
                SelectionBoxItemTemplate = new FuncDataTemplate<FileFormat>(
                    ( format, scope ) => new TextBlock { Text = $"{format.Name}  (*.{format.Extension})" }, true ),
            };

            mPath = new TextBox { HorizontalAlignment = HorizontalAlignment.Stretch };

            var browse = new Button { Content = "Browse...", MinWidth = 90 };
            browse.Click += async ( s, e ) => await Browse();

            mFormat.SelectionChanged += ( s, e ) => mDescription.Text = Selected.Description;
            mDescription.Text = Selected.Description;

            mRetainNames = new CheckBox
            {
                Content = "Keep the original material's name and texture names",
                IsChecked = request.RetainTextureNames,
                IsVisible = request.ShowMaterialOptions,
            };

            mRetainColors = new CheckBox
            {
                Content = "Keep the original material's colour values",
                IsChecked = request.RetainColorValues,
                IsVisible = request.ShowMaterialOptions,
            };

            var replace = new Button { Content = "Replace", MinWidth = 90, IsDefault = true };
            replace.Click += ( s, e ) =>
            {
                if ( !string.IsNullOrWhiteSpace( mPath.Text ) && File.Exists( mPath.Text ) )
                    Close( new ReplaceOptionsResult( mPath.Text, Selected,
                                                     mRetainNames.IsChecked == true,
                                                     mRetainColors.IsChecked == true ) );
            };

            var cancel = new Button { Content = "Cancel", MinWidth = 90, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions( "Auto,*,Auto" ),
                RowDefinitions = new RowDefinitions( "Auto,Auto" ),
                ColumnSpacing = 10,
                RowSpacing = 10,
            };

            Place( grid, Label( "Format" ), 0, 0 );
            Place( grid, mFormat, 0, 1, 2 );
            Place( grid, Label( "Read from" ), 1, 0 );
            Place( grid, mPath, 1, 1 );
            Place( grid, browse, 1, 2 );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 14,
                Children =
                {
                    grid,
                    mDescription,
                    mRetainNames,
                    mRetainColors,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { replace, cancel },
                    },
                },
            };
        }

        private FileFormat Selected => mFormat.SelectedItem as FileFormat ?? mRequest.Formats[0];

        public static Task<ReplaceOptionsResult> Ask( Window owner, ReplaceOptionsRequest request )
            => new ReplaceOptionsWindow( request ).ShowDialog<ReplaceOptionsResult>( owner );

        private async Task Browse()
        {
            var files = await StorageProvider.OpenFilePickerAsync( new FilePickerOpenOptions
            {
                Title = "Select a replacement file",
                AllowMultiple = false,
                FileTypeFilter = mRequest.Formats
                                         .Select( x => new FileFilter( x.Name, new[] { "*." + x.Extension } ).ToPickerType() )
                                         .Append( FilePickerFileTypes.All )
                                         .ToList(),
            } );

            var path = files.FirstOrDefault()?.TryGetLocalPath();
            if ( path == null )
                return;

            mPath.Text = path;
            MatchFormatTo( path );
        }

        /// <summary>
        /// Picks the format whose extension the chosen file carries, so browsing sets both.
        /// </summary>
        private void MatchFormatTo( string path )
        {
            var match = mRequest.Formats
                                .OrderByDescending( x => x.Extension.Length )
                                .FirstOrDefault( x => path.EndsWith( "." + x.Extension, StringComparison.OrdinalIgnoreCase ) );

            if ( match != null )
                mFormat.SelectedItem = match;
        }

        private static Control Entry( FileFormat format )
        {
            var entry = new StackPanel { Spacing = 1 };
            entry.Children.Add( new TextBlock { Text = $"{format.Name}  (*.{format.Extension})" } );

            if ( !string.IsNullOrEmpty( format.Description ) )
            {
                entry.Children.Add( new TextBlock
                {
                    Text = format.Description,
                    Opacity = 0.7,
                    FontSize = 11,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 520,
                } );
            }

            return entry;
        }

        private static TextBlock Label( string text )
            => new() { Text = text, VerticalAlignment = VerticalAlignment.Center };

        private static void Place( Grid grid, Control control, int row, int column, int span = 1 )
        {
            Grid.SetRow( control, row );
            Grid.SetColumn( control, column );
            Grid.SetColumnSpan( control, span );
            grid.Children.Add( control );
        }
    }
}
