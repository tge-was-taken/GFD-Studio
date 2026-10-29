using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.PropertyGrid.Services;
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
                CellEditFactoryService.Default.AddFactory( new DrawingColorCellEditFactory() );
                CellEditFactoryService.Default.AddFactory( new TypeConverterCellEditFactory() );
                CellEditFactoryService.Default.AddFactory( new EnumCellEditFactory() );
                CellEditFactoryService.Default.AddFactory( new FlagsCellEditFactory() );
                CellEditFactoryService.Default.AddFactory( new Vector4ColorCellEditFactory() );

                UserOptions.Current = new UserOptionsAvalonia();

                // The dialogs need a window to sit over, so they are registered once there is one.
                var window = new MainWindow();
                UserDialogs.Current = new UserDialogsAvalonia( window );

                // A file named on the command line is opened once the window is up and has had a
                // chance to lay out, because a dialog over a window that is not showing throws.
                if ( desktop.Args is { Length: > 0 } args && System.IO.File.Exists( args[0] ) )
                    window.Opened += ( s, e ) => Dispatcher.UIThread.Post( () => window.OpenFile( args[0] ) );

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
