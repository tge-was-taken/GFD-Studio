using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AtlusGfdEditor.GUI.Adapters
{
    public abstract partial class TreeNodeAdapter : TreeNode
	{
        // Override all these to hide them from the property grid
        [Browsable( false )]
        public new string Name { get { return base.Name; } set { base.Name = value; } }
        [Browsable( false )]
        public override ContextMenu ContextMenu { get { return base.ContextMenu; } set { base.ContextMenu = value; } }
        [Browsable( false )]
        public override ContextMenuStrip ContextMenuStrip { get { return base.ContextMenuStrip; } set { base.ContextMenuStrip = value; } }
        [Browsable( false )]
        public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        [Browsable( false )]
        public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
        [Browsable( false )]
        public new Font NodeFont { get { return base.NodeFont; } set { base.NodeFont = value; } }
        [Browsable( false )]
        public new string ToolTipText { get { return base.ToolTipText; } set { base.ToolTipText = value; } }
        [Browsable( false )]
        public new bool Checked { get { return base.Checked; } set { base.Checked = value; } }
        [Browsable( false )]
        public new int ImageIndex { get { return base.ImageIndex; } set { base.ImageIndex = value; } }
        [Browsable( false )]
        public new string ImageKey { get { return base.ImageKey; } set { base.ImageKey = value; } }
        [Browsable( false )]
        public new int Index { get { return base.Index; } }
        [Browsable( false )]
        public new int SelectedImageIndex { get { return base.SelectedImageIndex; } set { base.SelectedImageIndex = value; } }
        [Browsable( false )]
        public new string SelectedImageKey { get { return base.SelectedImageKey; } set { base.SelectedImageKey = value; } }
        [Browsable( false )]
        public new int StateImageIndex { get { return base.StateImageIndex; } set { base.StateImageIndex = value; } }
        [Browsable( false )]
        public new string StateImageKey { get { return base.StateImageKey; } set { base.StateImageKey = value; } }
        [Browsable( false )]
        public new object Tag { get { return base.Tag; } set { base.Tag = value; } }
    }
}
