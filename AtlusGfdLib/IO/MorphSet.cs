using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public class MorphTargetList : IList<MorphTarget>
    {
        private List<MorphTarget> mList;

        public MorphTargetList()
        {
            mList = new List<MorphTarget>();
        }

        public MorphTarget this[int index] { get => ( ( IList<MorphTarget> )mList )[index]; set => ( ( IList<MorphTarget> )mList )[index] = value; }

        public int Flags { get; set; }

        public int Count => ( ( IList<MorphTarget> )mList ).Count;

        public bool IsReadOnly => ( ( IList<MorphTarget> )mList ).IsReadOnly;

        public void Add( MorphTarget item )
        {
            ( ( IList<MorphTarget> )mList ).Add( item );
        }

        public void Clear()
        {
            ( ( IList<MorphTarget> )mList ).Clear();
        }

        public bool Contains( MorphTarget item )
        {
            return ( ( IList<MorphTarget> )mList ).Contains( item );
        }

        public void CopyTo( MorphTarget[] array, int arrayIndex )
        {
            ( ( IList<MorphTarget> )mList ).CopyTo( array, arrayIndex );
        }

        public IEnumerator<MorphTarget> GetEnumerator()
        {
            return ( ( IList<MorphTarget> )mList ).GetEnumerator();
        }

        public int IndexOf( MorphTarget item )
        {
            return ( ( IList<MorphTarget> )mList ).IndexOf( item );
        }

        public void Insert( int index, MorphTarget item )
        {
            ( ( IList<MorphTarget> )mList ).Insert( index, item );
        }

        public bool Remove( MorphTarget item )
        {
            return ( ( IList<MorphTarget> )mList ).Remove( item );
        }

        public void RemoveAt( int index )
        {
            ( ( IList<MorphTarget> )mList ).RemoveAt( index );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IList<MorphTarget> )mList ).GetEnumerator();
        }
    }
}