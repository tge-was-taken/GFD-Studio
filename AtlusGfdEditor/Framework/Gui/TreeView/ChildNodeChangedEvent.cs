using System;
using System.Windows.Forms;

namespace AtlusGfdEditor.Framework.Gui.TreeView
{
    public class ChildNodePropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public TreeNode Node { get; }

        public ChildNodePropertyChangedEventArgs(string propertyName, TreeNode node)
        {
            PropertyName = propertyName;
            Node = node;
        }
    }

    public delegate void ChildNodePropertyChangedEventHandler(object sender, ChildNodePropertyChangedEventArgs e);
}