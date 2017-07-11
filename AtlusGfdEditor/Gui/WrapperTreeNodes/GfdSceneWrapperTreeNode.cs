using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdFramework;

namespace AtlusGfdEditor.Gui.WrapperTreeNodes
{
    class GfdSceneWrapperTreeNode : GfdResourceWrapperTreeNode
    {
        internal GfdSceneWrapperTreeNode(GfdScene scene)
            : base(ContextMenuStripFlags.Delete | ContextMenuStripFlags.Export | ContextMenuStripFlags.Move | ContextMenuStripFlags.Replace, scene)
        {

        }
    }
}