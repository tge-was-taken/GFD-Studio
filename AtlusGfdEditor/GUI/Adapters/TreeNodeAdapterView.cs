using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class TreeNodeAdapterView : TreeView
    {
        /// <summary>
        /// Sets the first visible node in the tree. Clears all nodes if not empty.
        /// </summary>
        /// <param name="node"></param>
        public void SetTopNode( TreeNodeAdapter node )
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

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            // initialize view for selected node
            var adapter = ( TreeNodeAdapter )e.Node;
            adapter.InitializeView();

            base.OnAfterSelect( e );
        }

        protected override void OnAfterExpand( TreeViewEventArgs e )
        {
            // check if the first child node is a dummy node
            if ( e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text == string.Empty )
            {
                // initialize the view so the user doesn't get to see the dummy node
                ( ( TreeNodeAdapter )e.Node ).InitializeView();
            }

            foreach ( TreeNodeAdapter childNode in e.Node.Nodes )
            {
                if ( childNode.Nodes.Count == 0 && childNode.NodeFlags.HasFlag(TreeNodeAdapter.Flags.Branch) )
                {
                    // HACK: add a dummy node for each branch
                    // so the expand icon shows up even when a node hasn't initialized yet
                    childNode.Nodes.Add( string.Empty );
                }
            }

            base.OnAfterExpand( e );
        }
    }
}
