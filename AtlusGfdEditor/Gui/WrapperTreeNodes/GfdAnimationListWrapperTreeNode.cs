using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.GfdLib;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdAnimationListWrapperTreeNode : GfdResourceWrapperTreeNode
    {
        public GfdAnimationListWrapperTreeNode(GfdAnimationList animationList)
            : base(ContextMenuStripFlags.Add | ContextMenuStripFlags.Delete | ContextMenuStripFlags.Move, animationList)
        {
        }
    }
}