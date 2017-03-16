using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AtlusGfdEditor.Framework.Gui.TreeView
{
    abstract class BaseWrapperTreeNode : TreeNode, INotifyPropertyChanged
    {
        private bool m_IsModified;
        private ContextMenuStripFlags m_CtxFlags;

        public event PropertyChangedEventHandler PropertyChanged;
        public event ChildNodePropertyChangedEventHandler ChildNodePropertyChanged;

        public BaseWrapperTreeNode(ContextMenuStripFlags flags, object wrappedObject)
            : base()
        {
            Tag = wrappedObject;
            m_CtxFlags = flags;
            InitializeContextMenu();
            InitializeEvents();
        }

        public BaseWrapperTreeNode(ContextMenuStripFlags flags)
            : this(flags, null)
        {
        }

        // Utility
        public T GetWrappedObject<T>()
        {
            return (T)Tag;
        }

        // Initialization functions
        private void InitializeContextMenu()
        {
            ContextMenuStrip = new ContextMenuStrip();

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Export))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Export", null, ContextMenuStripExportClickedEventHandler, Keys.Control | Keys.E));
            }

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Replace))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Replace", null, ContextMenuStripReplaceClickedEventHandler, Keys.Control | Keys.R));
                if (!m_CtxFlags.HasFlag(ContextMenuStripFlags.Add))
                    ContextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Add))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Add", null, ContextMenuStripAddClickedEventHandler, Keys.Control | Keys.A));
                if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Move))
                    ContextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Move))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Move &Up", null, ContextMenuStripMoveUpClickedEventHandler, Keys.Control | Keys.Up));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Move &Down", null, ContextMenuStripMoveDownClickedEventHandler, Keys.Control | Keys.Down));
            }

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Rename))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Re&name", null, ContextMenuStripRenameClickedEventHandler, Keys.Control | Keys.N));
                if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Delete))
                    ContextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            if (m_CtxFlags.HasFlag(ContextMenuStripFlags.Delete))
            {
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Delete", null, ContextMenuStripDeleteClickedEventHandler, Keys.Control | Keys.Delete));
            }
        }

        private void InitializeEvents()
        {
            PropertyChanged += BaseWrapperTreeNodePropertyChangedEventHandler;
            ChildNodePropertyChanged += BaseWrapperTreeNodeChildNodePropertyChangedEventHandler;
            //TreeView.AfterLabelEdit += TreeViewAfterLabelEditEventHandler;
        }

        // Invoke property changed
        protected void InvokePropertyChangedEvent([CallerMemberName]string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void InvokeParentChildNodePropertyChangedEvent(string propertyName)
        {
            var baseWrapperParent = Parent as BaseWrapperTreeNode;
            if (baseWrapperParent != null)
                baseWrapperParent.ChildNodePropertyChanged.Invoke(this, new ChildNodePropertyChangedEventArgs(propertyName, this));
        }

        // Events
        protected virtual void BaseWrapperTreeNodePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            // Current node is modified
            m_IsModified = true;

            // Make sure the parent is notified
            InvokeParentChildNodePropertyChangedEvent(e.PropertyName);
        }

        protected virtual void BaseWrapperTreeNodeChildNodePropertyChangedEventHandler(object sender, ChildNodePropertyChangedEventArgs e)
        {
            BaseWrapperTreeNodePropertyChangedEventHandler(this, new PropertyChangedEventArgs(nameof(Nodes)));
        }

        protected virtual void TreeViewAfterLabelEditEventHandler(object sender, NodeLabelEditEventArgs e)
        {
            if (!e.CancelEdit && e.Node == this)
            {
                Text = e.Label;
                InvokePropertyChangedEvent(nameof(Text));
            }
        }

        // Context menu handlers
        protected void ContextMenuStripMoveUpClickedEventHandler(object sender, EventArgs e)
        {
            var parent = Parent;

            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(this);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, this);
                }

                InvokePropertyChangedEvent("Index");
            }
            else if (TreeView.Nodes.Contains(this)) //root node
            {
                int index = TreeView.Nodes.IndexOf(this);
                if (index > 0)
                {
                    TreeView.Nodes.RemoveAt(index);
                    TreeView.Nodes.Insert(index - 1, this);
                }
            }

            TreeView.SelectedNode = this;
        }

        protected void ContextMenuStripMoveDownClickedEventHandler(object sender, EventArgs e)
        {
            var parent = Parent;

            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(this);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, this);
                }

                InvokePropertyChangedEvent("Index");
            }
            else if (TreeView != null && TreeView.Nodes.Contains(this)) //root node
            {
                int index = TreeView.Nodes.IndexOf(this);
                if (index < TreeView.Nodes.Count - 1)
                {
                    TreeView.Nodes.RemoveAt(index);
                    TreeView.Nodes.Insert(index + 1, this);
                }
            }

            TreeView.SelectedNode = this;
        }

        protected void ContextMenuStripDeleteClickedEventHandler(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                InvokePropertyChangedEvent("Exists");
            }

            Remove();
        }

        protected void ContextMenuStripRenameClickedEventHandler(object sender, EventArgs e)
        {
            BeginEdit();
        }

        protected virtual void ContextMenuStripExportClickedEventHandler(object sender, EventArgs e) { }

        protected virtual void ContextMenuStripAddClickedEventHandler(object sender, EventArgs e) { }

        protected virtual void ContextMenuStripReplaceClickedEventHandler(object sender, EventArgs e) { }

        // Hide base class properties from property grid
        //Override all these to hide them from the property grid
        [Browsable(false)]
        public new string Name { get { return base.Name; } set { base.Name = value; } }
        [Browsable(false)]
        public override ContextMenu ContextMenu { get { return base.ContextMenu; } set { base.ContextMenu = value; } }
        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip { get { return base.ContextMenuStrip; } set { base.ContextMenuStrip = value; } }
        [Browsable(false)]
        public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        [Browsable(false)]
        public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
        [Browsable(false)]
        public new Font NodeFont { get { return base.NodeFont; } set { base.NodeFont = value; } }
        [Browsable(false)]
        public new string Text { get { return base.Text; } set { base.Text = value; } }
        [Browsable(false)]
        public new string ToolTipText { get { return base.ToolTipText; } set { base.ToolTipText = value; } }
        [Browsable(false)]
        public new bool Checked { get { return base.Checked; } set { base.Checked = value; } }
        [Browsable(false)]
        public new int ImageIndex { get { return base.ImageIndex; } set { base.ImageIndex = value; } }
        [Browsable(false)]
        public new string ImageKey { get { return base.ImageKey; } set { base.ImageKey = value; } }
        [Browsable(false)]
        public new int Index { get { return base.Index; } }
        [Browsable(false)]
        public new int SelectedImageIndex { get { return base.SelectedImageIndex; } set { base.SelectedImageIndex = value; } }
        [Browsable(false)]
        public new string SelectedImageKey { get { return base.SelectedImageKey; } set { base.SelectedImageKey = value; } }
        [Browsable(false)]
        public new int StateImageIndex { get { return base.StateImageIndex; } set { base.StateImageIndex = value; } }
        [Browsable(false)]
        public new string StateImageKey { get { return base.StateImageKey; } set { base.StateImageKey = value; } }
        [Browsable(false)]
        public new object Tag { get { return base.Tag; } set { base.Tag = value; } }
    }
}
