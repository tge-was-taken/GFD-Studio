using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class ShaderCache : Resource, IList<Shader>
    {
        private List<Shader> m_Shaders;

        internal ShaderCache(uint version)
            : base(ResourceType.ShaderCache, version)
        {
            m_Shaders = new List<Shader>();
        }

        #region IList implementation

        public Shader this[int index]
        {
            get
            {
                return m_Shaders[index];
            }
        }

        Shader IList<Shader>.this[int index]
        {
            get
            {
                return m_Shaders[index];
            }

            set
            {
                m_Shaders[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Shaders.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Shader item)
        {
            m_Shaders.Add(item);
        }

        public void Clear()
        {
            m_Shaders.Clear();
        }

        public bool Contains(Shader item)
        {
            return m_Shaders.Contains(item);
        }

        public void CopyTo(Shader[] array, int arrayIndex)
        {
            m_Shaders.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Shader> GetEnumerator()
        {
            return m_Shaders.GetEnumerator();
        }

        public int IndexOf(Shader item)
        {
            return m_Shaders.IndexOf(item);
        }

        public void Insert(int index, Shader item)
        {
            m_Shaders.Insert(index, item);
        }

        public bool Remove(Shader item)
        {
            return m_Shaders.Remove(item);
        }

        public void RemoveAt(int index)
        {
            m_Shaders.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Shaders.GetEnumerator();
        }

        #endregion
    }
}
