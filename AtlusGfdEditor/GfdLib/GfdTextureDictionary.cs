using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdTextureDictionary : GfdResource, IDictionary<uint, GfdTexture>
    {
        private Dictionary<uint, GfdTexture> m_Dictionary;

        internal GfdTextureDictionary(uint version)
            : base(GfdResourceType.TextureDictionary, version)
        {
            m_Dictionary = new Dictionary<uint, GfdTexture>();
        }

        #region IDictionary implementation 

        public GfdTexture this[uint key]
        {
            get
            {
                return m_Dictionary[key];
            }

            set
            {
                m_Dictionary[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<uint> Keys
        {
            get
            {
                return m_Dictionary.Keys;
            }
        }

        public ICollection<GfdTexture> Values
        {
            get
            {
                return m_Dictionary.Values;
            }
        }

        public void Add(KeyValuePair<uint, GfdTexture> item)
        {
            m_Dictionary.Add(item.Key, item.Value);
        }

        public void Add(uint key, GfdTexture value)
        {
            if (!m_Dictionary.ContainsKey(key))
                m_Dictionary.Add(key, value);
        }

        public void Clear()
        {
            m_Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<uint, GfdTexture> item)
        {
            if (m_Dictionary.ContainsKey(item.Key) && m_Dictionary[item.Key] == item.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(uint key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<uint, GfdTexture>[] array, int arrayIndex)
        {
            var keys = new uint[m_Dictionary.Keys.Count];
            m_Dictionary.Keys.CopyTo(keys, 0);

            for (int i = 0; i < keys.Length; i++)
                array[arrayIndex + i] = new KeyValuePair<uint, GfdTexture>(keys[i], m_Dictionary[keys[i]]);
        }

        public IEnumerator<KeyValuePair<uint, GfdTexture>> GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        public bool Remove(KeyValuePair<uint, GfdTexture> item)
        {
            return m_Dictionary.Remove(item.Key);
        }

        public bool Remove(uint key)
        {
            return m_Dictionary.Remove(key);
        }

        public bool TryGetValue(uint key, out GfdTexture value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        #endregion
    }
}
