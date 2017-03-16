using System;

namespace AtlusGfdEditor.Framework.Gui.TreeView
{
    [Flags]
    enum ContextMenuStripFlags
    {
        Export      = 1 << 0,
        Replace     = 1 << 1,
        Add         = 1 << 2,
        Move        = 1 << 3,
        Rename      = 1 << 4,
        Delete      = 1 << 5
    }
}
