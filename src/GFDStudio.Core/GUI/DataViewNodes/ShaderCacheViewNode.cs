using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Shaders;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class ShaderCacheViewNode<TCache, TShader> : DataViewNode<TCache>
        where TCache : ShaderCache<TShader>, new()
        where TShader : Resource, new()
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Add |
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Expand;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Version
        {
            get => Data.Version;
            set => Set( x => x.Version, value );
        }

        [Browsable( true )]
        [DisplayName( "Shader count" )]
        public int ShaderCount => Data.Count;

        protected internal ShaderCacheViewNode( string text, TCache data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<TCache>( path => Data.Save( path ) );
            RegisterReplaceHandler<TCache>( Resource.Load<TCache> );
            RegisterAddHandler<TShader>( path => AddChildNode( DataViewNodeFactory.Create( Name( Data.Count ), Resource.Load<TShader>( path ) ) ) );

            RegisterModelUpdateHandler( () =>
            {
                var cache = new TCache { Version = Version };

                foreach ( DataViewNode node in Nodes )
                    cache.Add( (TShader)node.Data );

                return cache;
            } );
        }

        protected override void InitializeViewCore()
        {
            for ( var i = 0; i < Data.Count; i++ )
                AddChildNode( DataViewNodeFactory.Create( Name( i ), Data[i] ) );
        }

        /// <summary>
        /// Shaders have no names of their own, so they are listed by position.
        /// </summary>
        private static string Name( int index ) => $"Shader {index}";
    }

    public class ShaderCachePS3ViewNode : ShaderCacheViewNode<ShaderCachePS3, ShaderPS3>
    {
        protected internal ShaderCachePS3ViewNode( string text, ShaderCachePS3 data ) : base( text, data )
        {
        }
    }

    public class ShaderCachePSP2ViewNode : ShaderCacheViewNode<ShaderCachePSP2, ShaderPSP2>
    {
        protected internal ShaderCachePSP2ViewNode( string text, ShaderCachePSP2 data ) : base( text, data )
        {
        }
    }

    public class ShaderCachePS4ViewNode : ShaderCacheViewNode<ShaderCachePS4, ShaderPS4>
    {
        protected internal ShaderCachePS4ViewNode( string text, ShaderCachePS4 data ) : base( text, data )
        {
        }
    }

    public class ShaderCacheMetaphorDX11ViewNode : ShaderCacheViewNode<ShaderCacheMetaphorDX11, ShaderMetaphor>
    {
        protected internal ShaderCacheMetaphorDX11ViewNode( string text, ShaderCacheMetaphorDX11 data ) : base( text, data )
        {
        }
    }
}
