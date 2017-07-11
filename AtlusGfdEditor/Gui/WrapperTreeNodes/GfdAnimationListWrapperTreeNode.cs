using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdFramework;

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