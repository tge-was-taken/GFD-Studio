using System.Windows.Forms;

namespace GFDStudio.GUI.DataViewNodes
{
    public delegate void DataTreeNodeViewModelPropertyChangedEventHandler( object sender, DataViewNodePropertyChangedEventArgs args );

    public class DataTreeView : TreeView
    {
        public event DataTreeNodeViewModelPropertyChangedEventHandler UserPropertyChanged;

        public new DataViewNode TopNode
        {
            get
            {
                if ( !DesignMode )
                {
                    return Nodes.Count > 0 ? ( DataViewNode ) Nodes[ 0 ] : null;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ( Nodes.Count > 0 )
                    Nodes[ 0 ] = value;
                else if ( value != null )
                    Nodes.Add( value );
            }
        }

        /// <summary>
        /// Sets the first visible node in the tree. Clears all nodes if not empty.
        /// </summary>
        /// <param name="node"></param>
        public void SetTopNode( DataViewNode node )
        {
            // clear nodes if not empty
            if ( Nodes.Count != 0 )
            {
                Nodes.Clear();
            }

            // initialize its view as it will be the first visible node
            node.InitializeView();

            // add top node
            Nodes.Add( node );
        }

        public void RefreshSelection()
        {
            OnAfterSelect( new TreeViewEventArgs( SelectedNode ) );
        }

        public void ExpandNode( DataViewNode viewModel )
        {
            // check if the first child node is a dummy node
            if ( viewModel.Nodes.Count > 0 && viewModel.Nodes[0].Text.Length == 0 )
            {
                // initialize the view so the user doesn't get to see the dummy node
                viewModel.InitializeView(true);
            }

            foreach ( DataViewNode childNode in viewModel.Nodes )
            {
                if ( childNode.Nodes.Count == 0 && childNode.NodeFlags.HasFlag( DataViewNodeFlags.Branch ) )
                {
                    // HACK: add a dummy node for each branch
                    // so the expand icon shows up even when a node hasn't initialized yet
                    childNode.AddChildNode( new TreeNode() );
                }
            }
        }

        internal void InvokeUserPropertyChanged( DataViewNode viewModel, string propertyName )
        {
            UserPropertyChanged?.Invoke( this, new DataViewNodePropertyChangedEventArgs( viewModel, propertyName ) );
        }

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            // initialize view for selected node
            var adapter = ( DataViewNode )e.Node;
            adapter.InitializeView();

            base.OnAfterSelect( e );
        }

        protected override void OnNodeMouseClick( TreeNodeMouseClickEventArgs e )
        {
            if ( e.Button == MouseButtons.Right )
                SelectedNode = e.Node;
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            ExpandNode( ( DataViewNode )e.Node );

            base.OnAfterExpand( e );
        }
    }
}
