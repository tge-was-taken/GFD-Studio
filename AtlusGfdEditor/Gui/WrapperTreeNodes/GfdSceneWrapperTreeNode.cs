using AtlusGfdEditor.Framework.Gui.TreeView;
using AtlusGfdEditor.GfdLib;

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