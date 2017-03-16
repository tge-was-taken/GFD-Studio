using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdEditor.GfdLib
{
    public sealed class GfdShaderCache : GfdResource, IList<GfdShader>
    {
        private List<GfdShader> m_Shaders;

        internal GfdShaderCache(uint version)
            : base(GfdResourceType.ShaderCache, version)
        {
            m_Shaders = new List<GfdShader>();
        }

        #region IList implementation

        public GfdShader this[int index]
        {
            get
            {
                return m_Shaders[index];
            }
        }

        GfdShader IList<GfdShader>.this[int index]
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

        public void Add(GfdShader item)
        {
            m_Shaders.Add(item);
        }

        public void Clear()
        {
            m_Shaders.Clear();
        }

        public bool Contains(GfdShader item)
        {
            return m_Shaders.Contains(item);
        }

        public void CopyTo(GfdShader[] array, int arrayIndex)
        {
            m_Shaders.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GfdShader> GetEnumerator()
        {
            return m_Shaders.GetEnumerator();
        }

        public int IndexOf(GfdShader item)
        {
            return m_Shaders.IndexOf(item);
        }

        public void Insert(int index, GfdShader item)
        {
            m_Shaders.Insert(index, item);
        }

        public bool Remove(GfdShader item)
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
