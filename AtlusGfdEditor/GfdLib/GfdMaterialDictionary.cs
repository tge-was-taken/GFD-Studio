using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdMaterialDictionary : GfdResource, IDictionary<uint, GfdMaterial>
    {
        private Dictionary<uint, GfdMaterial> m_Dictionary;

        public GfdMaterialDictionary(uint version)
            : base(GfdResourceType.MaterialDictionary, version)
        {
            m_Dictionary = new Dictionary<uint, GfdMaterial>();
        }

        #region IDictionary implementation

        public GfdMaterial this[uint key]
        {
            get
            {
                return ((IDictionary<uint, GfdMaterial>)m_Dictionary)[key];
            }

            set
            {
                ((IDictionary<uint, GfdMaterial>)m_Dictionary)[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<uint, GfdMaterial>)m_Dictionary).IsReadOnly;
            }
        }

        public ICollection<uint> Keys
        {
            get
            {
                return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Keys;
            }
        }

        public ICollection<GfdMaterial> Values
        {
            get
            {
                return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Values;
            }
        }

        public void Add(KeyValuePair<uint, GfdMaterial> item)
        {
            ((IDictionary<uint, GfdMaterial>)m_Dictionary).Add(item);
        }

        public void Add(uint key, GfdMaterial value)
        {
            ((IDictionary<uint, GfdMaterial>)m_Dictionary).Add(key, value);
        }

        public void Clear()
        {
            ((IDictionary<uint, GfdMaterial>)m_Dictionary).Clear();
        }

        public bool Contains(KeyValuePair<uint, GfdMaterial> item)
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Contains(item);
        }

        public bool ContainsKey(uint key)
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<uint, GfdMaterial>[] array, int arrayIndex)
        {
            ((IDictionary<uint, GfdMaterial>)m_Dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<uint, GfdMaterial>> GetEnumerator()
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).GetEnumerator();
        }

        public bool Remove(KeyValuePair<uint, GfdMaterial> item)
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Remove(item);
        }

        public bool Remove(uint key)
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).Remove(key);
        }

        public bool TryGetValue(uint key, out GfdMaterial value)
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<uint, GfdMaterial>)m_Dictionary).GetEnumerator();
        }

        #endregion
    }
}