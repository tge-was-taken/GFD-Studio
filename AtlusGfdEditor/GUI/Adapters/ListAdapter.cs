using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public delegate string ItemNameProvider< in T>( T item, int index );

    public class ListAdapter<T> : TreeNodeAdapter<List<T>>
    {
        private readonly ItemNameProvider<T> mItemNameProvider;
        private readonly IList<string> mItemNames;

        public override MenuFlags ContextMenuFlags
            => MenuFlags.Add | MenuFlags.Delete | MenuFlags.Move | MenuFlags.Rename;

        public override Flags NodeFlags
        {
            get
            {
                if ( Resource == null )
                    return Flags.Leaf;

                return Resource.Count > 0 ? Flags.Branch : Flags.Leaf;
            }
        }

        public ListAdapter( string text, List<T> resource, ItemNameProvider<T> nameProvider ) : base( text, resource )
        {
            mItemNameProvider = nameProvider;
        }

        public ListAdapter( string text, List<T> resource, IList<string> itemNames ) : base( text, resource )
        {
            mItemNames = itemNames;
        }

        protected override void InitializeCore()
        {
            RegisterRebuildAction( () => ( from TreeNodeAdapter<T> adapter in Nodes select adapter.Resource ).ToList() );
        }

        protected override void InitializeViewCore()
        {
            for ( int i = 0; i < Resource.Count; i++ )
            {
                string itemName = mItemNameProvider != null ? mItemNameProvider( Resource[i], i ) : mItemNames[i];
                Nodes.Add( TreeNodeAdapterFactory.Create( itemName, Resource[i] ) );
            }
        }
    }
}
