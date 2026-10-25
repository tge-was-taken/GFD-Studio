using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Shows how far a long running job has got, and lets the user ask it to stop.
    /// </summary>
    public sealed class ProgressWindow : Window, IProgressReport
    {
        private readonly ProgressBar mBar = new() { Minimum = 0, Maximum = 100, Height = 20 };
        private readonly TextBlock mMessage = new() { Text = "Working" };

        private ProgressWindow( string title )
        {
            Title = title;
            Width = 480;
            SizeToContent = SizeToContent.Height;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var cancel = new Button { Content = "Cancel", MinWidth = 80, HorizontalAlignment = HorizontalAlignment.Right };
            cancel.Click += ( s, e ) =>
            {
                CancellationPending = true;
                cancel.IsEnabled = false;
            };

            Content = new StackPanel
            {
                Margin = new Thickness( 16 ),
                Spacing = 12,
                Children = { mMessage, mBar, cancel },
            };

            // The job owns when this closes, so the title bar button only cancels.
            Closing += ( s, e ) =>
            {
                if ( !mFinished )
                {
                    CancellationPending = true;
                    e.Cancel = true;
                }
            };
        }

        private bool mFinished;

        public bool CancellationPending { get; private set; }

        public void Report( int percentage, string message )
            => Dispatcher.UIThread.Post( () =>
            {
                mBar.Value = percentage;
                mMessage.Text = message;
            } );

        /// <summary>
        /// Shows the window, runs the job, then closes it again.
        /// </summary>
        public static Task Run( Window owner, string title, Func<IProgressReport, Task> work )
        {
            var window = new ProgressWindow( title );
            var shown = window.ShowDialog( owner );

            return RunCore();

            async Task RunCore()
            {
                try
                {
                    await work( window );
                }
                finally
                {
                    window.mFinished = true;
                    window.Close();
                    await shown;
                }
            }
        }
    }
}
