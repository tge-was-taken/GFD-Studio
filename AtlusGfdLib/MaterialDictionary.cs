using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AtlusGfdLib
{
    public sealed class MaterialDictionary : Resource
    {
        private Dictionary<string, Material> mDictionary;

        public MaterialDictionary(uint version)
            : base(ResourceType.MaterialDictionary, version)
        {
            mDictionary = new Dictionary<string, Material>();
        }

        public Material this[string name]
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

        public IList<Material> Materials => mDictionary.Values.ToList();

        public void Add( Material material ) => mDictionary[material.Name] = material;

        public int Count => mDictionary.Count;

        public void Clear() => mDictionary.Clear();

        public bool ContainsMaterial( string name ) => mDictionary.ContainsKey( name );

        public bool ContainsMaterial( Material material ) => mDictionary.ContainsValue( material );

        public bool Remove( string name ) => mDictionary.Remove( name );

        public bool Remove( Material material ) => mDictionary.Remove( material.Name );

        public bool TryGetMaterial( string name, out Material material)
        {
            return mDictionary.TryGetValue( name, out material);
        }
    }
}