using System.Collections.Generic;

namespace GFDStudio.GUI.DataViewNodes
{
    public delegate string ListItemNameProvider<in T>( T item, int index );

    public class ListViewNode<T> : DataViewNode<List<T>>
    {
        private readonly ListItemNameProvider<T> mItemNameProvider;
        private readonly IList<string> mItemNames;

        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Add | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
        {
            get
            {
                if ( Data == null )
                    return DataViewNodeFlags.Leaf;

                return Data.Count > 0 ? DataViewNodeFlags.Branch : DataViewNodeFlags.Leaf;
            }
        }

        public ListViewNode( string text, List<T> data, ListItemNameProvider<T> nameProvider ) : base( text, data )
        {
            mItemNameProvider = nameProvider;
        }

        public ListViewNode( string text, List<T> data, IList<string> itemNames ) : base( text, data )
        {
            mItemNames = itemNames;
        }

        protected override void InitializeCore()
        {
            RegisterModelUpdateHandler( () =>
            {
                var list = new List< T >();

                foreach ( DataViewNode viewModel in Nodes )
                    list.Add( (T)viewModel.Data );

                return list;
            });
        }

        protected override void InitializeViewCore()
        {
            for ( int i = 0; i < Data.Count; i++ )
            {
                string itemName = mItemNameProvider != null ? mItemNameProvider( Data[i], i ) : mItemNames[i];
                Nodes.Add( DataViewNodeFactory.Create( itemName, Data[i] ) );
            }
        }
    }
}
