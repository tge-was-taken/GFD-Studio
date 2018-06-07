using System.Collections;
using System.Collections.Generic;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public class MorphTargetList : Resource, IList<MorphTarget>
    {
        private readonly List<MorphTarget> mList;

        public override ResourceType ResourceType => ResourceType.MorphTargetList;

        public MorphTargetList()
        {
            mList = new List<MorphTarget>();
        }


        public MorphTargetList(uint version) :base(version)
        {
            mList = new List<MorphTarget>();
        }

        internal override void Read( ResourceReader reader )
        {
            Flags = reader.ReadInt32();
            int morphCount = reader.ReadInt32();

            for ( int i = 0; i < morphCount; i++ )
            {
                var morphTarget = reader.Read<MorphTarget>( Version );
                Add( morphTarget );
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( Flags );
            writer.WriteInt32( Count );

            foreach ( var target in this )
                target.Write( writer );
        }

        #region IList
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
        #endregion
    }
}