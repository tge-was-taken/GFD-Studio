using System.Collections;
using System.Collections.Generic;
using GFDLibrary.IO;

namespace GFDLibrary.Shaders
{
    public abstract class ShaderCache<TShader> : Resource, IList<TShader> where TShader : Resource, new() 
    {
        private readonly List<TShader> mShaders;

        /// <summary>
        /// The version in the file header, which is the only version a shader cache carries.
        /// </summary>
        public uint CacheVersion
        {
            get => Version;
            set => Version = value;
        }

        protected ShaderCache() : this( ResourceVersion.Persona5ShaderCache )
        {
        }

        protected ShaderCache( uint version )
            : base( version )
        {
            mShaders = new List<TShader>();
        }

        protected override void ReadCore( ResourceReader reader )
        {
            // The caller has already taken the file header off the stream, so what is left is
            // shaders until the end of the file. A cache carries no count of its own.
            while ( !reader.EndOfStream )
                Add( reader.ReadResource<TShader>( Version ) );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            foreach ( var shader in this )
                writer.WriteResource( shader );
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

    public sealed class ShaderCachePS3 : ShaderCache<ShaderPS3>
    {
        public override ResourceType ResourceType => ResourceType.ShaderCachePS3;

        public ShaderCachePS3()
        {           
        }

        public ShaderCachePS3( uint version ) : base( version )
        {
        }

    }

    public sealed class ShaderCachePSP2 : ShaderCache<ShaderPSP2>
    {
        public override ResourceType ResourceType => ResourceType.ShaderCachePSP2;

        public ShaderCachePSP2()
        {          
        }

        public ShaderCachePSP2( uint version ) : base( version )
        {
        }

    }

    public class ShaderCachePS4 : ShaderCache<ShaderPS4>
    {
        public override ResourceType ResourceType => ResourceType.ShaderCachePS4;

        public ShaderCachePS4()
        {      
        }

        public ShaderCachePS4( uint version ) : base( version )
        {
        }

    }

    public class ShaderCacheMetaphorDX11 : ShaderCache<ShaderMetaphor>
    {
        public override ResourceType ResourceType => ResourceType.ShaderCacheMetaphorDX11;

        public ShaderCacheMetaphorDX11()
        {
        }

        public ShaderCacheMetaphorDX11( uint version ) : base( version )
        {
        }

    }
}
