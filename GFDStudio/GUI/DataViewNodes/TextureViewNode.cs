using System.ComponentModel;
using System.Drawing;
using System.IO;
using GFDLibrary;
using GFDLibrary.IO.Utilities;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class TextureViewNode : DataViewNode<Texture>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => 
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => GetDataProperty<string>();
            set
            {
                SetDataProperty( value );
                Text = value;
            }
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFormat> ) )]
        public TextureFormat Format
        {
            get => GetDataProperty<TextureFormat>();
        }

        [Browsable( true )]
        [TypeConverter(typeof( Int32HexTypeConverter ) )]
        public int DataLength
        {
            get => Data.Data.Length;
        }

        [Browsable( true )]
        public byte Field1C
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field1D
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field1E
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public byte Field1F
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        protected internal TextureViewNode( string text, Texture data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Bitmap>( path => TextureDecoder.Decode( Data ).Save( path, ImageFormatHelper.GetImageFormatFromPath( path ) ) );
            RegisterExportHandler<Stream>( path => File.WriteAllBytes( path, Data.Data ) );

            RegisterReplaceHandler<Bitmap>( path => TextureEncoder.Encode( Name, Format, Field1C, Field1D, Field1E, Field1F, new Bitmap( path ) ) );
            RegisterReplaceHandler< Stream >( path => new Texture( Name, Format, File.ReadAllBytes( path ), Field1C, Field1D, Field1E, Field1F ) );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
