using System.Collections;
using System.Collections.Generic;

namespace GFDLibrary.Shaders
{
    public abstract class ShaderCacheBase<TShader> : Resource, IList<TShader>
    {
        private List<TShader> mShaders;

        protected ShaderCacheBase( ResourceType type, uint version )
            : base( type, version)
        {
            mShaders = new List<TShader>();
        }

        #region IList implementation

        public TShader this[int index]
        {
            get
            {
                return mShaders[index];
            }
        }

        TShader IList<TShader>.this[int index]
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

        public void Add(TShader item)
        {
            mShaders.Add(item);
        }

        public void Clear()
        {
            mShaders.Clear();
        }

        public bool Contains(TShader item)
        {
            return mShaders.Contains(item);
        }

        public void CopyTo(TShader[] array, int arrayIndex)
        {
            mShaders.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TShader> GetEnumerator()
        {
            return mShaders.GetEnumerator();
        }

        public int IndexOf(TShader item)
        {
            return mShaders.IndexOf(item);
        }

        public void Insert(int index, TShader item)
        {
            mShaders.Insert(index, item);
        }

        public bool Remove(TShader item)
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

    public sealed class ShaderCachePS3 : ShaderCacheBase<ShaderPS3>
    {
        public ShaderCachePS3( uint version ) : base( ResourceType.ShaderCachePS3, version )
        {
        }
    }

    public sealed class ShaderCachePSP2 : ShaderCacheBase<ShaderPSP2>
    {
        public ShaderCachePSP2( uint version ) : base( ResourceType.ShaderCachePSP2, version )
        {
        }
    }
}
