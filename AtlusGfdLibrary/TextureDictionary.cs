using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AtlusGfdLib
{
    public sealed class TextureDictionary : Resource
    {
        private Dictionary<int, Texture> mDictionary;

        internal TextureDictionary( uint version )
            : base( ResourceType.TextureDictionary, version )
        {
            mDictionary = new Dictionary<int, Texture>();
        }

        public Texture this[int key]
        {
            get
            {
                return mDictionary[key];
            }

            set
            {
                mDictionary[key] = value;
            }
        }

        public Texture this[string key]
        {
            get
            {
                return mDictionary[StringHasher.GenerateStringHash( key )];
            }
            set
            {
                mDictionary[StringHasher.GenerateStringHash( key )] = value;
            }
        }

        public ICollection<Texture> Materials => mDictionary.Values;

        public void Add( Texture texture ) => mDictionary[StringHasher.GenerateStringHash( texture.Name )] = texture;

        public int Count => mDictionary.Count;

        public void Clear() => mDictionary.Clear();

        public bool ContainsKey( int key ) => mDictionary.ContainsKey( key );

        public bool ContainsTexture( Texture texture ) => mDictionary.ContainsValue( texture );

        public bool Remove( int key ) => mDictionary.Remove( key );

        public bool Remove( Texture texture ) => mDictionary.Remove( StringHasher.GenerateStringHash( texture.Name ) );

        public bool TryGetValue( int key, out Texture value )
        {
            return mDictionary.TryGetValue( key, out value );
        }
    }
}
