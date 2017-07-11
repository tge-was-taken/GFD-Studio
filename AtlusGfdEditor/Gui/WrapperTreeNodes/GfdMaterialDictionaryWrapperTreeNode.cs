using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdFramework;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdMaterialDictionaryWrapperTreeNode : GfdResourceWrapperTreeNode
    {
        internal GfdMaterialDictionaryWrapperTreeNode(GfdMaterialDictionary materialDictionary)
            : base(ContextMenuStripFlags.Add | ContextMenuStripFlags.Delete | ContextMenuStripFlags.Move, materialDictionary)
        {
        }
    }
}