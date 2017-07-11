using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class MaterialDictionary : Resource
    {
        private Dictionary<int, Material> mDictionary;

        public MaterialDictionary(uint version)
            : base(ResourceType.MaterialDictionary, version)
        {
            mDictionary = new Dictionary<int, Material>();
        }

        public Material this[int key]
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

        public Material this[string key]
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

        public ICollection<Material> Materials => mDictionary.Values;

        public void Add( Material material ) => mDictionary[StringHasher.GenerateStringHash( material.Name )] = material;

        public int Count => mDictionary.Count;

        public bool IsReadOnly => false;

        public void Clear() => mDictionary.Clear();

        public bool ContainsKey( int key ) => mDictionary.ContainsKey( key );

        public bool ContainsMaterial( Material material ) => mDictionary.ContainsValue( material );

        public bool Remove( int key ) => mDictionary.Remove( key );

        public bool Remove( Material material ) => mDictionary.Remove( StringHasher.GenerateStringHash( material.Name ) );

        public bool TryGetValue( int key, out Material value)
        {
            return mDictionary.TryGetValue(key, out value);
        }
    }
}