using System.Collections;
using System.Collections.Generic;

namespace GFDLibrary
{
    public class UserPropertyCollection : IDictionary<string, UserProperty>
    {
        private readonly Dictionary<string, UserProperty> mDictionary;

        public UserPropertyCollection()
        {
            mDictionary = new Dictionary< string, UserProperty >();
        }

        public UserPropertyCollection( int capacity )
        {
            mDictionary = new Dictionary< string, UserProperty >( capacity );
        }

        public void Add( UserProperty property )
        {
            mDictionary[property.Name] = property;
        }

        public void Remove( UserProperty property )
        {
            Remove( property.Name );
        }

        #region IDictionary implementation

        public UserProperty this[string key] { get => mDictionary[key]; set => mDictionary[key] = value; }

        public ICollection<string> Keys => ( ( IDictionary<string, UserProperty> )mDictionary ).Keys;

        public ICollection<UserProperty> Values => ( ( IDictionary<string, UserProperty> )mDictionary ).Values;

        public int Count => mDictionary.Count;

        public bool IsReadOnly => ( ( IDictionary<string, UserProperty> )mDictionary ).IsReadOnly;

        public void Add( string key, UserProperty value )
        {
            mDictionary.Add( key, value );
        }

        public void Add( KeyValuePair<string, UserProperty> item )
        {
            ( ( IDictionary<string, UserProperty> )mDictionary ).Add( item );
        }

        public void Clear()
        {
            mDictionary.Clear();
        }

        public bool Contains( KeyValuePair<string, UserProperty> item )
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).Contains( item );
        }

        public bool ContainsKey( string key )
        {
            return mDictionary.ContainsKey( key );
        }

        public void CopyTo( KeyValuePair<string, UserProperty>[] array, int arrayIndex )
        {
            ( ( IDictionary<string, UserProperty> )mDictionary ).CopyTo( array, arrayIndex );
        }

        public IEnumerator<KeyValuePair<string, UserProperty>> GetEnumerator()
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).GetEnumerator();
        }

        public bool Remove( string key )
        {
            return mDictionary.Remove( key );
        }

        public bool Remove( KeyValuePair<string, UserProperty> item )
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).Remove( item );
        }

        public bool TryGetValue( string key, out UserProperty value )
        {
            return mDictionary.TryGetValue( key, out value );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).GetEnumerator();
        }

        #endregion
    }
}
