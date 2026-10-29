using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Shaders;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class ShaderViewNode<TShader> : DataViewNode<TShader>
        where TShader : Shader, new()
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move |
            DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<ShaderType> ) )]
        public ShaderType ShaderType
        {
            get => Data.ShaderType;
            set => Set( x => x.ShaderType, value );
        }

        [Browsable( true )]
        [DisplayName( "Data length" )]
        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int DataLength => Data.DataLength;

        [Browsable( true )]
        [TypeConverter( typeof( UInt16HexTypeConverter ) )]
        public ushort Field06
        {
            get => Data.Field06;
            set => Set( x => x.Field06, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint MatFlags0
        {
            get => Data.MatFlags0;
            set => Set( x => x.MatFlags0, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint MatFlags1
        {
            get => Data.MatFlags1;
            set => Set( x => x.MatFlags1, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint MatFlags2
        {
            get => Data.MatFlags2;
            set => Set( x => x.MatFlags2, value );
        }

        [Browsable( true )]
        public uint Texcoord0
        {
            get => Data.Texcoord0;
            set => Set( x => x.Texcoord0, value );
        }

        [Browsable( true )]
        public uint Texcoord1
        {
            get => Data.Texcoord1;
            set => Set( x => x.Texcoord1, value );
        }

        protected internal ShaderViewNode( string text, TShader data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<TShader>( path => Data.Save( path ) );
            RegisterReplaceHandler<TShader>( Resource.Load<TShader> );

            // Swapping the compiled shader on its own is the usual edit, so it is offered
            // without the flags around it.
            RegisterExportHandler<ShaderBinary>( path => File.WriteAllBytes( path, Data.Data ) );
            RegisterReplaceHandler<ShaderBinary>( path =>
            {
                Data.Data = File.ReadAllBytes( path );
                return Data;
            } );
        }
    }

    public class ShaderPS3ViewNode : ShaderViewNode<ShaderPS3>
    {
        protected internal ShaderPS3ViewNode( string text, ShaderPS3 data ) : base( text, data )
        {
        }
    }

    public class ShaderPSP2ViewNode : ShaderViewNode<ShaderPSP2>
    {
        protected internal ShaderPSP2ViewNode( string text, ShaderPSP2 data ) : base( text, data )
        {
        }
    }

    public class ShaderPS4ViewNode : ShaderViewNode<ShaderPS4>
    {
        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint ShaderType2
        {
            get => Data.ShaderType2;
            set => Set( x => x.ShaderType2, value );
        }

        protected internal ShaderPS4ViewNode( string text, ShaderPS4 data ) : base( text, data )
        {
        }
    }

    public class ShaderMetaphorViewNode : ShaderViewNode<ShaderMetaphor>
    {
        [Browsable( true )]
        [TypeConverter( typeof( UInt16HexTypeConverter ) )]
        public ushort Field06_2
        {
            get => Data.Field06_2;
            set => Set( x => x.Field06_2, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Field1C
        {
            get => Data.Field1C;
            set => Set( x => x.Field1C, value );
        }

        protected internal ShaderMetaphorViewNode( string text, ShaderMetaphor data ) : base( text, data )
        {
        }
    }
}
