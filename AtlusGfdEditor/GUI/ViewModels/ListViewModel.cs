using System.Collections.Generic;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public delegate string ListItemNameProvider<in T>( T item, int index );

    public class ListViewModel<T> : TreeNodeViewModel<List<T>>
    {
        private readonly ListItemNameProvider<T> mItemNameProvider;
        private readonly IList<string> mItemNames;

        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Add | TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename;

        public override TreeNodeViewModelFlags NodeFlags
        {
            get
            {
                if ( Model == null )
                    return TreeNodeViewModelFlags.Leaf;

                return Model.Count > 0 ? TreeNodeViewModelFlags.Branch : TreeNodeViewModelFlags.Leaf;
            }
        }

        public ListViewModel( string text, List<T> resource, ListItemNameProvider<T> nameProvider ) : base( text, resource )
        {
            mItemNameProvider = nameProvider;
        }

        public ListViewModel( string text, List<T> resource, IList<string> itemNames ) : base( text, resource )
        {
            mItemNames = itemNames;
        }

        protected override void InitializeCore()
        {
            RegisterModelUpdateHandler( () =>
            {
                var list = new List< T >();

                foreach ( TreeNodeViewModel viewModel in Nodes )
                    list.Add( (T)viewModel.Model );

                return list;
            });
        }

        protected override void InitializeViewCore()
        {
            for ( int i = 0; i < Model.Count; i++ )
            {
                string itemName = mItemNameProvider != null ? mItemNameProvider( Model[i], i ) : mItemNames[i];
                Nodes.Add( TreeNodeViewModelFactory.Create( itemName, Model[i] ) );
            }
        }
    }
}
