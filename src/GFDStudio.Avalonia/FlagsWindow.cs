using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Ticks the members of a flags enum, in a window rather than inline where the list would
    /// crowd out every other property.
    /// </summary>
    public sealed class FlagsWindow : Window
    {
        private readonly List<(Enum Value, CheckBox Box)> mEntries = new();

        private FlagsWindow( string title, Type enumType, Enum current )
        {
            Title = title;
            Width = 420;
            Height = 560;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var list = new StackPanel { Spacing = 2, Margin = new Thickness( 4 ) };

            foreach ( var member in EnumNames.Members( enumType ) )
            {
                // A zero member means "none", which is the absence of the others rather than a tick.
                if ( EnumNames.ToNumber( member ) == 0 )
                    continue;

                var box = new CheckBox
                {
                    Content = EnumNames.Describe( member ),
                    IsChecked = current != null && current.HasFlag( member ),
                };

                mEntries.Add( (member, box) );
                list.Children.Add( box );
            }

            var all = new Button { Content = "All", MinWidth = 70 };
            all.Click += ( s, e ) => mEntries.ForEach( x => x.Box.IsChecked = true );

            var none = new Button { Content = "None", MinWidth = 70 };
            none.Click += ( s, e ) => mEntries.ForEach( x => x.Box.IsChecked = false );

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( Combine( enumType ) );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            var buttons = new Grid { ColumnDefinitions = new ColumnDefinitions( "Auto,Auto,*,Auto,Auto" ), ColumnSpacing = 8, Margin = new Thickness( 12 ) };
            Place( buttons, all, 0 );
            Place( buttons, none, 1 );
            Place( buttons, ok, 3 );
            Place( buttons, cancel, 4 );

            var root = new DockPanel();
            DockPanel.SetDock( buttons, Dock.Bottom );
            root.Children.Add( buttons );
            root.Children.Add( new ScrollViewer { Content = list, Margin = new Thickness( 12, 12, 12, 0 ) } );
            Content = root;
        }

        public static Task<Enum> Ask( Visual owner, string title, Type enumType, Enum current )
        {
            if ( TopLevel.GetTopLevel( owner ) is not Window window )
                return Task.FromResult<Enum>( null );

            return new FlagsWindow( title, enumType, current ).ShowDialog<Enum>( window );
        }

        private Enum Combine( Type enumType )
        {
            long result = 0;

            foreach ( var (value, box) in mEntries.Where( x => x.Box.IsChecked == true ) )
                result |= EnumNames.ToNumber( value );

            return (Enum)Enum.ToObject( enumType, result );
        }

        private static void Place( Grid grid, Control control, int column )
        {
            Grid.SetColumn( control, column );
            grid.Children.Add( control );
        }
    }
}
