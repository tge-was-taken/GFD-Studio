using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace GFDStudio.Avalonia
{
    public enum MessageWindowButtons
    {
        Ok,
        YesNo,
    }

    /// <summary>
    /// A message box, which Avalonia does not provide one of.
    /// </summary>
    public sealed class MessageWindow : Window
    {
        private bool mResult;

        private MessageWindow( string text, string caption, MessageWindowButtons buttons )
        {
            Title = caption;
            SizeToContent = SizeToContent.WidthAndHeight;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var message = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 460,
            };

            var row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8,
            };

            if ( buttons == MessageWindowButtons.YesNo )
            {
                row.Children.Add( Button( "Yes", true ) );
                row.Children.Add( Button( "No", false ) );
            }
            else
            {
                row.Children.Add( Button( "OK", true ) );
            }

            Content = new StackPanel
            {
                Margin = new Thickness( 20 ),
                Spacing = 20,
                MinWidth = 260,
                Children = { message, row },
            };
        }

        public static Task<bool> Show( Window owner, string text, string caption, MessageWindowButtons buttons )
        {
            var window = new MessageWindow( text, caption, buttons );
            return window.ShowDialog<bool>( owner );
        }

        private Button Button( string text, bool result )
        {
            var button = new Button { Content = text, MinWidth = 80, IsDefault = result };
            button.Click += ( s, e ) =>
            {
                mResult = result;
                Close( mResult );
            };

            return button;
        }
    }
}
