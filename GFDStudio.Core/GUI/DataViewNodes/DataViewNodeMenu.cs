using System;
using System.Collections.Generic;

namespace GFDStudio.GUI.DataViewNodes
{
    [Flags]
    public enum DataViewNodeMenuModifiers
    {
        None = 0,
        Control = 1,
        Shift = 2,
        Alt = 4,
    }

    /// <summary>
    /// The keys the context menus bind to, rather than every key a keyboard has.
    /// </summary>
    public enum DataViewNodeMenuKey
    {
        None,
        A,
        E,
        N,
        R,
        Delete,
        Up,
        Down,
    }

    public readonly record struct DataViewNodeMenuShortcut( DataViewNodeMenuModifiers Modifiers, DataViewNodeMenuKey Key )
    {
        public static DataViewNodeMenuShortcut Control( DataViewNodeMenuKey key )
            => new( DataViewNodeMenuModifiers.Control, key );

        public bool IsNone => Key == DataViewNodeMenuKey.None;
    }

    /// <summary>
    /// One entry in a node's context menu, described without reference to a UI framework so the
    /// same menu can be built for WinForms or Avalonia.
    /// </summary>
    public sealed class DataViewNodeMenuItem
    {
        public static readonly DataViewNodeMenuItem Separator = new() { IsSeparator = true };

        /// <summary>
        /// Identifies the item when nesting submenus, which are addressed by a slash separated path.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The text to show. An ampersand marks the accelerator, as WinForms writes it.
        /// </summary>
        public string Text { get; init; }

        /// <summary>
        /// What the item does, or null when it only holds a submenu.
        /// </summary>
        public Action Action { get; internal set; }

        public DataViewNodeMenuShortcut Shortcut { get; init; }

        public bool IsSeparator { get; private init; }

        public List<DataViewNodeMenuItem> Items { get; } = new();

        public DataViewNodeMenuItem FindChild( string name )
            => Items.Find( x => x.Name == name );
    }
}
