using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load( this );

        public override void OnFrameworkInitializationCompleted()
        {
            if ( ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop )
            {
                UserOptions.Current = new UserOptionsAvalonia();

                // The dialogs need a window to sit over, so they are registered once there is one.
                var window = new MainWindow();
                UserDialogs.Current = new UserDialogsAvalonia( window );

                if ( desktop.Args is { Length: > 0 } args && System.IO.File.Exists( args[0] ) )
                    window.OpenFile( args[0] );

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
