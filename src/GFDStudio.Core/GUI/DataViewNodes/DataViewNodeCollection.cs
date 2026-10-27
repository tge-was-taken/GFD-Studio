using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// The child nodes of a <see cref="DataViewNode"/>. The surface mirrors the WinForms
    /// TreeNodeCollection this replaced, so the view nodes read the same as they did.
    /// </summary>
    public sealed class DataViewNodeCollection : IList<DataViewNode>, IList, INotifyCollectionChanged
    {
        private readonly List<DataViewNode> mNodes = new();
        private readonly DataViewNode mOwner;

        internal DataViewNodeCollection( DataViewNode owner ) => mOwner = owner;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => mNodes.Count;

        public bool IsReadOnly => false;

        public DataViewNode this[int index]
        {
            get => mNodes[index];
            set
            {
                var old = mNodes[index];
                Detach( old );
                mNodes[index] = Attach( value );
                OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, value, old, index ) );
            }
        }

        public void Add( DataViewNode node )
        {
            mNodes.Add( Attach( node ) );
            OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, node, mNodes.Count - 1 ) );
        }

        public void AddRange( IEnumerable<DataViewNode> nodes )
        {
            foreach ( var node in nodes )
                mNodes.Add( Attach( node ) );

            OnChanged( Reset );
        }

        public void Insert( int index, DataViewNode node )
        {
            mNodes.Insert( index, Attach( node ) );
            OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, node, index ) );
        }

        public bool Remove( DataViewNode node )
        {
            var index = mNodes.IndexOf( node );
            if ( index < 0 )
                return false;

            mNodes.RemoveAt( index );
            Detach( node );
            OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, node, index ) );
            return true;
        }

        public void RemoveAt( int index )
        {
            var node = mNodes[index];
            Detach( node );
            mNodes.RemoveAt( index );
            OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, node, index ) );
        }

        public void Clear()
        {
            foreach ( var node in mNodes )
                Detach( node );

            mNodes.Clear();
            OnChanged( Reset );
        }

        public bool Contains( DataViewNode node ) => mNodes.Contains( node );

        public int IndexOf( DataViewNode node ) => mNodes.IndexOf( node );

        public void CopyTo( DataViewNode[] array, int arrayIndex ) => mNodes.CopyTo( array, arrayIndex );

        public List<DataViewNode>.Enumerator GetEnumerator() => mNodes.GetEnumerator();

        IEnumerator<DataViewNode> IEnumerable<DataViewNode>.GetEnumerator() => mNodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => mNodes.GetEnumerator();

        //
        // The non-generic IList, which a binding to an INotifyCollectionChanged source needs.
        //

        bool IList.IsFixedSize => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (DataViewNode)value;
        }

        int IList.Add( object value )
        {
            Add( (DataViewNode)value );
            return Count - 1;
        }

        bool IList.Contains( object value ) => value is DataViewNode node && Contains( node );

        int IList.IndexOf( object value ) => value is DataViewNode node ? IndexOf( node ) : -1;

        void IList.Insert( int index, object value ) => Insert( index, (DataViewNode)value );

        void IList.Remove( object value )
        {
            if ( value is DataViewNode node )
                Remove( node );
        }

        void ICollection.CopyTo( Array array, int index ) => ( (ICollection)mNodes ).CopyTo( array, index );

        private DataViewNode Attach( DataViewNode node )
        {
            if ( node == null )
                throw new ArgumentNullException( nameof( node ) );

            node.SetParent( mOwner );
            return node;
        }

        private static void Detach( DataViewNode node ) => node?.SetParent( null );

        private static readonly NotifyCollectionChangedEventArgs Reset =
            new( NotifyCollectionChangedAction.Reset );

        private void OnChanged( NotifyCollectionChangedEventArgs args )
        {
            CollectionChanged?.Invoke( this, args );
            mOwner.NotifyChildrenChanged();
        }
    }
}
