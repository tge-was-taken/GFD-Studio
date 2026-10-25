using System.ComponentModel;
using System.Drawing;
using System.IO;
using GFDLibrary;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDStudio.GUI.Forms;
using GFDStudio.GUI.TypeConverters;
using GFDStudio.FormatModules;

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
        [DisplayName( "Data length" )]
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
            RegisterExportHandler<GFDLibrary.Graphics.Image>( path => PngCodec.Save( TextureDecoder.Decode( Data ), path ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, Data.Data ) );

            RegisterReplaceHandler<GFDLibrary.Graphics.Image>( path => MainForm.Instance.HandleTextureToReplace( Data.Version, TextureEncoder.Encode( Name, Format, Field1C, Field1D, Field1E, Field1F, GFDLibrary.Textures.PngCodec.Load( path ) ) ) );
            RegisterReplaceHandler<DDSStream>( path => MainForm.Instance.HandleTextureToReplace( Data.Version, new Texture( Name, Format, File.ReadAllBytes( path ), Field1C, Field1D, Field1E, Field1F ) ) );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
