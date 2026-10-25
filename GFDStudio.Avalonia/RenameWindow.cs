using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks for a node's new name. Avalonia's tree has no label editing of its own.
    /// </summary>
    public sealed class RenameWindow : Window
    {
        private readonly TextBox mTextBox;

        private RenameWindow( string text )
        {
            Title = "Rename";
            SizeToContent = SizeToContent.Height;
            Width = 360;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            mTextBox = new TextBox { Text = text };
            mTextBox.KeyDown += ( s, e ) =>
            {
                if ( e.Key == Key.Enter )
                    Close( mTextBox.Text );
                else if ( e.Key == Key.Escape )
                    Close( null );
            };

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( mTextBox.Text );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 16,
                Children =
                {
                    mTextBox,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { ok, cancel },
                    },
                },
            };

            Opened += ( s, e ) =>
            {
                mTextBox.SelectAll();
                mTextBox.Focus();
            };
        }

        public static async void Show( Visual owner, DataViewNode node )
        {
            if ( TopLevel.GetTopLevel( owner ) is not Window window )
                return;

            var name = await new RenameWindow( node.Text ).ShowDialog<string>( window );
            if ( name != null )
                node.CommitRename( name );
        }
    }
}
