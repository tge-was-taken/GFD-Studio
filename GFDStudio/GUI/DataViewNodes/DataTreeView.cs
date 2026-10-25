using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GFDStudio.GUI.Forms;

namespace GFDStudio.GUI.DataViewNodes
{
    public delegate void DataTreeNodeViewModelPropertyChangedEventHandler( object sender, DataViewNodePropertyChangedEventArgs args );

    /// <summary>
    /// Mirrors a tree of <see cref="DataViewNode"/> into WinForms tree nodes. The view nodes know
    /// nothing about this; they describe themselves and ask for view operations through
    /// <see cref="IDataViewNodeView"/>.
    /// </summary>
    public class DataTreeView : TreeView, IDataViewNodeView
    {
        public event DataTreeNodeViewModelPropertyChangedEventHandler UserPropertyChanged;

        private readonly Dictionary<DataViewNode, TreeNode> mTreeNodes = new();
        private DataViewNode mTopNode;
        private DataViewNode mRenamingNode;

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public new DataViewNode TopNode
        {
            get => DesignMode ? null : mTopNode;
            set => SetTopNode( value );
        }

        public DataTreeView()
        {
            ImageList = new ImageList();
        }

        /// <summary>
        /// Shows the given node as the only node in the tree.
        /// </summary>
        public void SetTopNode( DataViewNode node )
        {
            Nodes.Clear();
            mTreeNodes.Clear();
            mTopNode = node;

            if ( node == null )
                return;

            node.AttachView( this );
            node.InitializeView();
            Nodes.Add( Build( node ) );
        }

        /// <summary>
        /// Gets the view node a WinForms tree node stands for.
        /// </summary>
        public static DataViewNode GetViewNode( TreeNode treeNode ) => treeNode?.Tag as DataViewNode;

        public void RefreshSelection() => OnAfterSelect( new TreeViewEventArgs( SelectedNode ) );

        public void Select( DataViewNode node )
        {
            if ( mTreeNodes.TryGetValue( node, out var treeNode ) )
                SelectedNode = treeNode;
        }

        public void EnsureVisible( DataViewNode node )
        {
            if ( mTreeNodes.TryGetValue( node, out var treeNode ) )
                treeNode.EnsureVisible();
        }

        public void BeginRename( DataViewNode node )
        {
            if ( !mTreeNodes.TryGetValue( node, out var treeNode ) )
                return;

            mRenamingNode = node;
            LabelEdit = true;
            treeNode.BeginEdit();
        }

        public void NotifyUserPropertyChanged( DataViewNode node, string propertyName )
            => UserPropertyChanged?.Invoke( this, new DataViewNodePropertyChangedEventArgs( node, propertyName ) );

        public void OnChildrenChanged( DataViewNode node )
        {
            if ( mTreeNodes.TryGetValue( node, out var treeNode ) )
                SyncChildren( node, treeNode );
        }

        private TreeNode Build( DataViewNode node )
        {
            var treeNode = new TreeNode( node.Text ) { Tag = node, Name = node.Name };
            mTreeNodes[node] = treeNode;

            ApplyIcon( node, treeNode );
            treeNode.ContextMenuStrip = node.MenuItems.ToContextMenuStrip( () => treeNode.ContextMenuStrip );
            if ( treeNode.ContextMenuStrip != null )
                treeNode.ContextMenuStrip.VisibleChanged += ( s, e ) => Theme.RecursivelySetColor( treeNode.ContextMenuStrip );

            SyncChildren( node, treeNode );
            return treeNode;
        }

        private void ApplyIcon( DataViewNode node, TreeNode treeNode )
        {
            if ( string.IsNullOrWhiteSpace( node.ImageKey ) )
                return;

            if ( !ImageList.Images.ContainsKey( node.ImageKey ) )
                ImageList.Images.Add( node.ImageKey, new Bitmap( node.ImageKey ) );

            treeNode.ImageKey = treeNode.SelectedImageKey = node.ImageKey;
        }

        private void SyncChildren( DataViewNode node, TreeNode treeNode )
        {
            treeNode.Nodes.Clear();

            foreach ( var child in node.Nodes )
            {
                var childTreeNode = Build( child );
                treeNode.Nodes.Add( childTreeNode );

                // A branch that has not built its view yet gets a placeholder, so the expander
                // shows up before the user ever selects it.
                if ( child.Nodes.Count == 0 && child.NodeFlags.HasFlag( DataViewNodeFlags.Branch ) )
                    childTreeNode.Nodes.Add( new TreeNode() );
            }
        }

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            GetViewNode( e?.Node )?.InitializeView();
            base.OnAfterSelect( e );
        }

        protected override void OnNodeMouseClick( TreeNodeMouseClickEventArgs e )
        {
            if ( e.Button == MouseButtons.Right )
                SelectedNode = e.Node;
        }

        protected override void OnBeforeExpand( TreeViewCancelEventArgs e )
        {
            GetViewNode( e.Node )?.InitializeView();
            base.OnBeforeExpand( e );
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            var node = GetViewNode( e.Node );
            if ( node != null )
                node.IsExpanded = true;

            base.OnAfterExpand( e );
        }

        protected override void OnAfterCollapse( TreeViewEventArgs e )
        {
            var node = GetViewNode( e.Node );
            if ( node != null )
                node.IsExpanded = false;

            base.OnAfterCollapse( e );
        }

        protected override void OnAfterLabelEdit( NodeLabelEditEventArgs e )
        {
            var node = mRenamingNode ?? GetViewNode( e.Node );
            mRenamingNode = null;
            LabelEdit = false;

            if ( node != null && !e.CancelEdit )
                node.CommitRename( e.Label );

            base.OnAfterLabelEdit( e );
        }
    }
}
