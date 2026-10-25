using System;
using System.Collections;
using System.Collections.Generic;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// The child nodes of a <see cref="DataViewNode"/>. The surface mirrors the WinForms
    /// TreeNodeCollection this replaced, so the view nodes read the same as they did.
    /// </summary>
    public sealed class DataViewNodeCollection : IList<DataViewNode>
    {
        private readonly List<DataViewNode> mNodes = new();
        private readonly DataViewNode mOwner;

        internal DataViewNodeCollection( DataViewNode owner ) => mOwner = owner;

        public int Count => mNodes.Count;

        public bool IsReadOnly => false;

        public DataViewNode this[int index]
        {
            get => mNodes[index];
            set
            {
                Detach( mNodes[index] );
                mNodes[index] = Attach( value );
                OnChanged();
            }
        }

        public void Add( DataViewNode node )
        {
            mNodes.Add( Attach( node ) );
            OnChanged();
        }

        public void AddRange( IEnumerable<DataViewNode> nodes )
        {
            foreach ( var node in nodes )
                mNodes.Add( Attach( node ) );

            OnChanged();
        }

        public void Insert( int index, DataViewNode node )
        {
            mNodes.Insert( index, Attach( node ) );
            OnChanged();
        }

        public bool Remove( DataViewNode node )
        {
            if ( !mNodes.Remove( node ) )
                return false;

            Detach( node );
            OnChanged();
            return true;
        }

        public void RemoveAt( int index )
        {
            Detach( mNodes[index] );
            mNodes.RemoveAt( index );
            OnChanged();
        }

        public void Clear()
        {
            foreach ( var node in mNodes )
                Detach( node );

            mNodes.Clear();
            OnChanged();
        }

        public bool Contains( DataViewNode node ) => mNodes.Contains( node );

        public int IndexOf( DataViewNode node ) => mNodes.IndexOf( node );

        public void CopyTo( DataViewNode[] array, int arrayIndex ) => mNodes.CopyTo( array, arrayIndex );

        public List<DataViewNode>.Enumerator GetEnumerator() => mNodes.GetEnumerator();

        IEnumerator<DataViewNode> IEnumerable<DataViewNode>.GetEnumerator() => mNodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => mNodes.GetEnumerator();

        private DataViewNode Attach( DataViewNode node )
        {
            if ( node == null )
                throw new ArgumentNullException( nameof( node ) );

            node.SetParent( mOwner );
            return node;
        }

        private static void Detach( DataViewNode node ) => node?.SetParent( null );

        private void OnChanged() => mOwner.NotifyChildrenChanged();
    }
}
