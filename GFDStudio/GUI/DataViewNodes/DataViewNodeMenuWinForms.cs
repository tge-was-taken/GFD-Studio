using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// Builds a WinForms context menu from the framework neutral description the view nodes
    /// produce. This is the only place that knows how those items map onto WinForms.
    /// </summary>
    internal static class DataViewNodeMenuWinForms
    {
        public static ContextMenuStrip ToContextMenuStrip( this IEnumerable<DataViewNodeMenuItem> items,
                                                           Func<ContextMenuStrip> owner )
        {
            var strip = new ContextMenuStrip();
            foreach ( var item in items )
                strip.Items.Add( Convert( item, owner ) );

            return strip;
        }

        private static ToolStripItem Convert( DataViewNodeMenuItem item, Func<ContextMenuStrip> owner )
        {
            if ( item.IsSeparator )
                return new ToolStripSeparator();

            var converted = new ToolStripMenuItem( item.Text ) { Name = item.Name };

            if ( item.Action != null )
            {
                converted.ShortcutKeys = ToKeys( item.Shortcut );
                converted.Click += ( s, e ) =>
                {
                    // The context menu stays visible when a dialog opens while a drop down is
                    // showing, so it is hidden explicitly first.
                    var strip = owner();
                    if ( strip != null )
                        strip.Visible = false;

                    item.Action();
                };
            }

            foreach ( var child in item.Items )
                converted.DropDownItems.Add( Convert( child, owner ) );

            return converted;
        }

        private static Keys ToKeys( DataViewNodeMenuShortcut shortcut )
        {
            if ( shortcut.IsNone )
                return Keys.None;

            var keys = shortcut.Key switch
            {
                DataViewNodeMenuKey.A => Keys.A,
                DataViewNodeMenuKey.E => Keys.E,
                DataViewNodeMenuKey.N => Keys.N,
                DataViewNodeMenuKey.R => Keys.R,
                DataViewNodeMenuKey.Delete => Keys.Delete,
                DataViewNodeMenuKey.Up => Keys.Up,
                DataViewNodeMenuKey.Down => Keys.Down,
                _ => Keys.None,
            };

            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Control ) ) keys |= Keys.Control;
            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Shift ) ) keys |= Keys.Shift;
            if ( shortcut.Modifiers.HasFlag( DataViewNodeMenuModifiers.Alt ) ) keys |= Keys.Alt;

            return keys;
        }
    }
}
