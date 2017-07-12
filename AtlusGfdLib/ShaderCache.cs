using System.Collections;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class ShaderCache : Resource, IList<Shader>
    {
        private List<Shader> mShaders;

        public ShaderCache(uint version)
            : base(ResourceType.ShaderCache, version)
        {
            mShaders = new List<Shader>();
        }

        #region IList implementation

        public Shader this[int index]
        {
            get
            {
                return mShaders[index];
            }
        }

        Shader IList<Shader>.this[int index]
        {
            get
            {
                return mShaders[index];
            }

            set
            {
                mShaders[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return mShaders.Count;
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
            mShaders.Add(item);
        }

        public void Clear()
        {
            mShaders.Clear();
        }

        public bool Contains(Shader item)
        {
            return mShaders.Contains(item);
        }

        public void CopyTo(Shader[] array, int arrayIndex)
        {
            mShaders.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Shader> GetEnumerator()
        {
            return mShaders.GetEnumerator();
        }

        public int IndexOf(Shader item)
        {
            return mShaders.IndexOf(item);
        }

        public void Insert(int index, Shader item)
        {
            mShaders.Insert(index, item);
        }

        public bool Remove(Shader item)
        {
            return mShaders.Remove(item);
        }

        public void RemoveAt(int index)
        {
            mShaders.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mShaders.GetEnumerator();
        }

        #endregion
    }
}
