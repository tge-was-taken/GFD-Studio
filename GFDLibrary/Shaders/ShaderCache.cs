using System.Collections;
using System.Collections.Generic;
using System.IO;
using GFDLibrary.IO;

namespace GFDLibrary.Shaders
{
    public abstract class ShaderCache<TShader> : Resource, IList<TShader> where TShader : Resource, new() 
    {
        private readonly List<TShader> mShaders;

        public uint CacheVersion { get; set; }

        protected ShaderCache() : this( ResourceVersion.Persona5 )
        {
            CacheVersion = ResourceVersion.Persona5ShaderCache;
        }

        protected ShaderCache( uint version )
            : base( version )
        {
            mShaders = new List<TShader>();
            CacheVersion = ResourceVersion.Persona5ShaderCache;
        }

        protected ShaderCache( uint version, uint cacheVersion )
            : base( version )
        {
            mShaders = new List<TShader>();
            CacheVersion = cacheVersion;
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            var header = reader.ReadFileHeader();
            if ( header.Identifier != ResourceFileIdentifier.ShaderCache )
                throw new InvalidDataException( "Expected shader cache identifier" );

            CacheVersion = header.Version;

            while ( !reader.EndOfStream )
            {
                var shader = reader.ReadResource<TShader>( header.Version );
                Add( shader );
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteFileHeader( ResourceFileIdentifier.ShaderCache, CacheVersion, ResourceType );
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

        public ShaderCachePS3( uint version, uint cacheVersion ) : base( version, cacheVersion )
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

        public ShaderCachePSP2( uint version, uint cacheVersion ) : base( version, cacheVersion )
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

        public ShaderCachePS4( uint version, uint cacheVersion ) : base( version, cacheVersion )
        {
        }
    }
}
