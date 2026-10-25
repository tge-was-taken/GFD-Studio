using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Turns the framework neutral menu description into Avalonia menu items.
    /// </summary>
    public static class DataViewNodeMenuAvalonia
    {
        public static ContextMenu ToContextMenu( this List<DataViewNodeMenuItem> items )
        {
            if ( items == null || items.Count == 0 )
                return null;

            var menu = new ContextMenu();
            menu.ItemsSource = items.Select( ToMenuItem ).ToList();
            return menu;
        }

        private static Control ToMenuItem( DataViewNodeMenuItem item )
        {
            if ( item.IsSeparator )
                return new Separator();

            var menuItem = new MenuItem { Header = ToHeader( item.Text ) };

            if ( item.Items.Count > 0 )
                menuItem.ItemsSource = item.Items.Select( ToMenuItem ).ToList();

            if ( item.Action != null )
                menuItem.Click += ( s, e ) => item.Action();

            if ( !item.Shortcut.IsNone )
                menuItem.InputGesture = ToGesture( item.Shortcut );

            return menuItem;
        }

        /// <summary>
        /// WinForms marks the accelerator with an ampersand and Avalonia with an underscore.
        /// </summary>
        private static string ToHeader( string text )
            => text?.Replace( "_", "__" ).Replace( "&&", "\u0001" ).Replace( "&", "_" ).Replace( "\u0001", "&" );

        private static KeyGesture ToGesture( DataViewNodeMenuShortcut shortcut )
        {
            var modifiers = KeyModifiers.None;

            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Control ) )
                modifiers |= KeyModifiers.Control;

            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Shift ) )
                modifiers |= KeyModifiers.Shift;

            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Alt ) )
                modifiers |= KeyModifiers.Alt;

            return new KeyGesture( ToKey( shortcut.Key ), modifiers );
        }

        private static Key ToKey( DataViewNodeMenuKey key ) => key switch
        {
            DataViewNodeMenuKey.A => Key.A,
            DataViewNodeMenuKey.E => Key.E,
            DataViewNodeMenuKey.N => Key.N,
            DataViewNodeMenuKey.R => Key.R,
            DataViewNodeMenuKey.Delete => Key.Delete,
            DataViewNodeMenuKey.Up => Key.Up,
            DataViewNodeMenuKey.Down => Key.Down,
            _ => Key.None,
        };
    }
}
