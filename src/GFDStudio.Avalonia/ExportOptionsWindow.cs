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
    /// Asks where to write a node and in which format. The format is chosen here rather than in
    /// the file picker, because a picker cannot say which of its filters the user settled on.
    /// </summary>
    public sealed class ExportOptionsWindow : Window
    {
        private readonly ExportOptionsRequest mRequest;
        private readonly ComboBox mFormat;
        private readonly TextBox mPath;

        private readonly TextBlock mDescription = new()
        {
            Opacity = 0.75,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 34,
        };

        private ExportOptionsWindow( ExportOptionsRequest request )
        {
            mRequest = request;

            Title = "Export";
            Width = 620;
            SizeToContent = SizeToContent.Height;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            mFormat = new ComboBox
            {
                ItemsSource = request.Formats,
                SelectedIndex = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                // Two lines in the list so the choice can be made at a glance, one in the closed
                // box, where the description would crowd out the name.
                ItemTemplate = new FuncDataTemplate<FileFormat>( ( format, scope ) => Entry( format ), true ),
                SelectionBoxItemTemplate = new FuncDataTemplate<FileFormat>(
                    ( format, scope ) => new TextBlock { Text = $"{format.Name}  (*.{format.Extension})" }, true ),
            };

            mPath = new TextBox { HorizontalAlignment = HorizontalAlignment.Stretch };

            var browse = new Button { Content = "Browse...", MinWidth = 90 };
            browse.Click += async ( s, e ) => await Browse();

            mFormat.SelectionChanged += ( s, e ) =>
            {
                mPath.Text = WithExtension( mPath.Text );
                mDescription.Text = Selected.Description;
            };
            mPath.Text = WithExtension( Path.Combine( LastDirectory, Sanitize( request.SuggestedName ) ) );
            mDescription.Text = Selected.Description;

            var export = new Button { Content = "Export", MinWidth = 90, IsDefault = true };
            export.Click += ( s, e ) =>
            {
                if ( !string.IsNullOrWhiteSpace( mPath.Text ) )
                    Close( new ExportOptionsResult( mPath.Text, Selected ) );
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
            Place( grid, Label( "Save to" ), 1, 0 );
            Place( grid, mPath, 1, 1 );
            Place( grid, browse, 1, 2 );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 16,
                Children =
                {
                    grid,
                    mDescription,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { export, cancel },
                    },
                },
            };
        }

        /// <summary>
        /// Where the last export went, so a run of exports does not start from scratch each time.
        /// </summary>
        private static string LastDirectory { get; set; } = string.Empty;

        private FileFormat Selected => mFormat.SelectedItem as FileFormat ?? mRequest.Formats[0];

        public static async Task<ExportOptionsResult> Ask( Window owner, ExportOptionsRequest request )
        {
            var result = await new ExportOptionsWindow( request ).ShowDialog<ExportOptionsResult>( owner );

            if ( result != null )
                LastDirectory = Path.GetDirectoryName( result.FilePath ) ?? string.Empty;

            return result;
        }

        private async Task Browse()
        {
            var format = Selected;

            var file = await StorageProvider.SaveFilePickerAsync( new FilePickerSaveOptions
            {
                Title = "Select a file to export to",
                SuggestedFileName = Path.GetFileName( mPath.Text ),
                DefaultExtension = format.Extension,
                ShowOverwritePrompt = true,
            } );

            var path = file?.TryGetLocalPath();
            if ( path != null )
                mPath.Text = WithExtension( path );
        }

        /// <summary>
        /// Puts the chosen format's extension on the path, replacing whatever was there. A
        /// compound extension such as ascii.fbx counts as one.
        /// </summary>
        private string WithExtension( string path )
        {
            if ( string.IsNullOrWhiteSpace( path ) )
                return path;

            var directory = Path.GetDirectoryName( path ) ?? string.Empty;
            var name = Path.GetFileName( path );

            foreach ( var known in mRequest.Formats.Select( x => "." + x.Extension )
                                           .OrderByDescending( x => x.Length ) )
            {
                if ( name.EndsWith( known, StringComparison.OrdinalIgnoreCase ) )
                {
                    name = name[..^known.Length];
                    break;
                }
            }

            return Path.Combine( directory, $"{name}.{Selected.Extension}" );
        }

        private static string Sanitize( string name )
            => string.Concat( ( name ?? "export" ).Split( Path.GetInvalidFileNameChars() ) );

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
