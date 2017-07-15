using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AtlusGfdLib
{
    public sealed class TextureDictionary : Resource
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

        public void Clear() => mDictionary.Clear();

        public bool ContainsTexture( string name ) => mDictionary.ContainsKey( name );

        public bool ContainsTexture( Texture material ) => mDictionary.ContainsValue( material );

        public bool Remove( string name ) => mDictionary.Remove( name );

        public bool Remove( Texture texture ) => mDictionary.Remove( texture.Name );

        public bool TryGetTexture( string name, out Texture texture )
        {
            return mDictionary.TryGetValue( name, out texture );
        }
    }
}
