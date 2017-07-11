using AtlusGfdFramework;
using AtlusGfdEditor.Framework.Gui.TreeView;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdTextureDictionaryWrapperTreeNode : GfdResourceWrapperTreeNode
    {
        internal GfdTextureDictionaryWrapperTreeNode(GfdTextureDictionary textureDictionary)
            : base(ContextMenuStripFlags.Add | ContextMenuStripFlags.Delete | ContextMenuStripFlags.Move, textureDictionary)
        {
        }
    }
}