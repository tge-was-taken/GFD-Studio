using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public class MorphTargetList : IList<MorphTarget>
    {
        private readonly List<MorphTarget> mList;

        public MorphTargetList()
        {
            mList = new List<MorphTarget>();
        }

        public MorphTarget this[int index] { get => mList[index]; set => mList[index] = value; }

        public int Flags { get; set; }

        public int Count => mList.Count;

        public bool IsReadOnly => ( ( IList<MorphTarget> )mList ).IsReadOnly;

        public void Add( MorphTarget item )
        {
            mList.Add( item );
        }

        public void Clear()
        {
            mList.Clear();
        }

        public bool Contains( MorphTarget item )
        {
            return mList.Contains( item );
        }

        public void CopyTo( MorphTarget[] array, int arrayIndex )
        {
            mList.CopyTo( array, arrayIndex );
        }

        public IEnumerator<MorphTarget> GetEnumerator()
        {
            return ( ( IList<MorphTarget> )mList ).GetEnumerator();
        }

        public int IndexOf( MorphTarget item )
        {
            return mList.IndexOf( item );
        }

        public void Insert( int index, MorphTarget item )
        {
            mList.Insert( index, item );
        }

        public bool Remove( MorphTarget item )
        {
            return mList.Remove( item );
        }

        public void RemoveAt( int index )
        {
            mList.RemoveAt( index );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IList<MorphTarget> )mList ).GetEnumerator();
        }
    }
}