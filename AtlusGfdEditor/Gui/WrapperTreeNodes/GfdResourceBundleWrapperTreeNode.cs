using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.GfdLib;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdResourceBundleWrapperTreeNode : GfdResourceWrapperTreeNode
    {
        internal GfdResourceBundleWrapperTreeNode(GfdResourceBundle resourceBundle)
            : base(ContextMenuStripFlags.Add | ContextMenuStripFlags.Export, resourceBundle)
        {
        }
    }
}
