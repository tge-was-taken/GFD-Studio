using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.GfdLib;

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