using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GFDLibrary.IO;
using GFDLibrary.Models.Conversion;

namespace GFDLibrary.Materials
{
    public sealed class MaterialDictionary : Resource, IDictionary<string, Material>
    {
        private readonly Dictionary<string, Material> mDictionary;

        public override ResourceType ResourceType => ResourceType.MaterialDictionary;

        public MaterialDictionary()
        {
            mDictionary = new Dictionary<string, Material>();
        }

        public MaterialDictionary( uint version )
            : base( version )
        {
            mDictionary = new Dictionary<string, Material>();
        }

        public Material this[string name]
        {
            get => mDictionary[name];
            set => mDictionary[name] = value;
        }

        public static MaterialDictionary ConvertAllToMaterialPreset( MaterialDictionary materialDictionary, ModelPackConverterOptions options )
        {
            Material newMaterial = null;
            var newMaterialDictionary = new MaterialDictionary(options.Version);

            foreach (var material in materialDictionary.Materials)
            {
                var materialName = material.Name;
                var diffuseTexture = material.DiffuseMap;
                var shadowTexture = material.ShadowMap;
                if (shadowTexture == null)
                    shadowTexture = material.DiffuseMap;
                var specularTexture = material.SpecularMap;
                if (specularTexture == null)
                    specularTexture = material.DiffuseMap;

                if (diffuseTexture == null) newMaterial = material;

                else newMaterial = MaterialFactory.CreateMaterial( materialName, diffuseTexture.Name, options );
                
                newMaterialDictionary.Add(newMaterial);
            }

            return newMaterialDictionary;
        }

        public void ReplaceWith(MaterialDictionary other)
        {
            foreach (var material in other)
            {
                // Don't replace the material if we're replacing a normal material with a preset one.
                if (!ContainsKey(material.Key) || !material.Value.IsPresetMaterial || this[material.Key].IsPresetMaterial)
                    this[material.Key] = material.Value;
            }

            var toRemove = new List<string>();
            foreach (var material in this)
            {
                if (!other.TryGetMaterial(material.Key, out _))
                    toRemove.Add(material.Key);
            }

            foreach (string s in toRemove)
                Remove(s);
        }

        protected override void ReadCore( ResourceReader reader )
        {
            var count = reader.ReadInt32();
            for ( int i = 0; i < count; i++ )
            {
                var material = reader.ReadResource<Material>( Version );
                Add( material );
            }
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( Count );
            foreach ( var material in Materials )
                writer.WriteResource( material );
        }

        #region IDictionary
        public IList<Material> Materials
            => mDictionary.Values.ToList();

        public void Add( Material material )
            => mDictionary[material.Name] = material;

        public int Count => mDictionary.Count;

        public ICollection<string> Keys
            => ( ( IDictionary<string, Material> )mDictionary ).Keys;

        public ICollection<Material> Values
            => ( ( IDictionary<string, Material> )mDictionary ).Values;

        public bool IsReadOnly
            => ( ( IDictionary<string, Material> )mDictionary ).IsReadOnly;

        public void Clear() => mDictionary.Clear();

        public bool ContainsMaterial( string name )
            => mDictionary.ContainsKey( name );

        public bool ContainsMaterial( Material material )
            => mDictionary.ContainsValue( material );

        public bool Remove( string name )
            => mDictionary.Remove( name );

        public bool Remove( Material material )
            => mDictionary.Remove( material.Name );

        public bool TryGetMaterial( string name, out Material material )
        {
            return mDictionary.TryGetValue( name, out material );
        }

        public bool ContainsKey( string key )
        {
            return mDictionary.ContainsKey( key );
        }

        public void Add( string key, Material value )
        {
            mDictionary.Add( key, value );
        }

        public bool TryGetValue( string key, out Material value )
        {
            return mDictionary.TryGetValue( key, out value );
        }

        public void Add( KeyValuePair<string, Material> item )
        {
            ( ( IDictionary<string, Material> )mDictionary ).Add( item );
        }

        public bool Contains( KeyValuePair<string, Material> item )
        {
            return ( ( IDictionary<string, Material> )mDictionary ).Contains( item );
        }

        public void CopyTo( KeyValuePair<string, Material>[] array, int arrayIndex )
        {
            ( ( IDictionary<string, Material> )mDictionary ).CopyTo( array, arrayIndex );
        }

        public bool Remove( KeyValuePair<string, Material> item )
        {
            return ( ( IDictionary<string, Material> )mDictionary ).Remove( item );
        }

        public IEnumerator<KeyValuePair<string, Material>> GetEnumerator()
        {
            return ( ( IDictionary<string, Material> )mDictionary ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IDictionary<string, Material> )mDictionary ).GetEnumerator();
        }
        #endregion
    }
}
