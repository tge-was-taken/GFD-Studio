using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public abstract partial class TreeNodeViewModel : TreeNode
    {
        // Override all these to hide them from the property grid
        [Browsable( false )]
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        [Browsable( false )]
        public override ContextMenu ContextMenu
        {
            get => base.ContextMenu;
            set => base.ContextMenu = value;
        }

        [Browsable( false )]
        public override ContextMenuStrip ContextMenuStrip
        {
            get => base.ContextMenuStrip;
            set => base.ContextMenuStrip = value;
        }

        [Browsable( false )]
        public new Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Browsable( false )]
        public new Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        [Browsable( false )]
        public new Font NodeFont
        {
            get => base.NodeFont;
            set => base.NodeFont = value;
        }

        [Browsable( false )]
        public new string ToolTipText
        {
            get => base.ToolTipText;
            set => base.ToolTipText = value;
        }

        [Browsable( false )]
        public new bool Checked
        {
            get => base.Checked;
            set => base.Checked = value;
        }

        [Browsable( false )]
        public new int ImageIndex
        {
            get => base.ImageIndex;
            set => base.ImageIndex = value;
        }

        [Browsable( false )]
        public new string ImageKey
        {
            get => base.ImageKey;
            set => base.ImageKey = value;
        }

        [Browsable( false )]
        public new int Index => base.Index;

        [Browsable( false )]
        public new int SelectedImageIndex
        {
            get => base.SelectedImageIndex;
            set => base.SelectedImageIndex = value;
        }

        [Browsable( false )]
        public new string SelectedImageKey
        {
            get => base.SelectedImageKey;
            set => base.SelectedImageKey = value;
        }

        [Browsable( false )]
        public new int StateImageIndex
        {
            get => base.StateImageIndex;
            set => base.StateImageIndex = value;
        }

        [Browsable( false )]
        public new string StateImageKey
        {
            get => base.StateImageKey;
            set => base.StateImageKey = value;
        }

        [Browsable( false )]
        public new object Tag
        {
            get => base.Tag;
            set => base.Tag = value;
        }
    }
}
