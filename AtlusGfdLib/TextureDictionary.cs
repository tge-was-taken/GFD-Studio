using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AtlusGfdLib
{
    public sealed class TextureDictionary : Resource, IDictionary<string, Texture>
    {
        private Dictionary<string, Texture> mDictionary;

        public TextureDictionary( uint version )
            : base( ResourceType.TextureDictionary, version )
        {
            mDictionary = new Dictionary<string, Texture>();
        }

        public Texture this[string name]
        {
            get
            {
                return mDictionary[name];
            }
            set
            {
                mDictionary[name] = value;
            }
        }

        public ICollection<Texture> Textures => mDictionary.Values;

        public void Add( Texture texture ) => mDictionary[texture.Name] = texture;

        public int Count => mDictionary.Count;

        public ICollection<string> Keys => ( ( IDictionary<string, Texture> )mDictionary ).Keys;

        public ICollection<Texture> Values => ( ( IDictionary<string, Texture> )mDictionary ).Values;

        public bool IsReadOnly => ( ( IDictionary<string, Texture> )mDictionary ).IsReadOnly;

        public void Clear() => mDictionary.Clear();

        public bool ContainsTexture( string name ) => mDictionary.ContainsKey( name );

        public bool ContainsTexture( Texture material ) => mDictionary.ContainsValue( material );

        public bool Remove( string name ) => mDictionary.Remove( name );

        public bool Remove( Texture texture ) => mDictionary.Remove( texture.Name );

        public bool TryGetTexture( string name, out Texture texture )
        {
            return mDictionary.TryGetValue( name, out texture );
        }

        public bool ContainsKey( string key )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).ContainsKey( key );
        }

        public void Add( string key, Texture value )
        {
            ( ( IDictionary<string, Texture> )mDictionary ).Add( key, value );
        }

        public bool TryGetValue( string key, out Texture value )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).TryGetValue( key, out value );
        }

        public void Add( KeyValuePair<string, Texture> item )
        {
            ( ( IDictionary<string, Texture> )mDictionary ).Add( item );
        }

        public bool Contains( KeyValuePair<string, Texture> item )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).Contains( item );
        }

        public void CopyTo( KeyValuePair<string, Texture>[] array, int arrayIndex )
        {
            ( ( IDictionary<string, Texture> )mDictionary ).CopyTo( array, arrayIndex );
        }

        public bool Remove( KeyValuePair<string, Texture> item )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).Remove( item );
        }

        public IEnumerator<KeyValuePair<string, Texture>> GetEnumerator()
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).GetEnumerator();
        }
    }
}
