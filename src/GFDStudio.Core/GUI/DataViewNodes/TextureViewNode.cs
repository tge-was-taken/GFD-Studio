using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDStudio.IO;
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
            get => Data.Name;
            set
            {
                Set( x => x.Name, value );
                Text = value;
            }
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFormat> ) )]
        public TextureFormat Format
        {
            get => Data.Format;
        }

        [Browsable( true )]
        [TypeConverter(typeof( Int32HexTypeConverter ) )]
        [DisplayName( "Data length" )]
        public int DataLength
        {
            get => Data.Data.Length;
        }

        [Browsable( true )]
        public byte MinificationFilter
        {
            get => Data.MinificationFilter;
            set => Set( x => x.MinificationFilter, value );
        }

        [Browsable( true )]
        public byte MagnificationFilter
        {
            get => Data.MagnificationFilter;
            set => Set( x => x.MagnificationFilter, value );
        }

        [Browsable( true )]
        public byte WrapModeU
        {
            get => Data.WrapModeU;
            set => Set( x => x.WrapModeU, value );
        }

        [Browsable( true )]
        public byte WrapModeV
        {
            get => Data.WrapModeV;
            set => Set( x => x.WrapModeV, value );
        }

        protected internal TextureViewNode( string text, Texture data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<GFDLibrary.Graphics.Image>( path => PngCodec.Save( TextureDecoder.Decode( Data ), path ) );
            RegisterExportHandler<DDSStream>( path => File.WriteAllBytes( path, Data.Data ) );

            RegisterReplaceHandler<GFDLibrary.Graphics.Image>( path => TextureReplacement.Prepare( Data.Version, TextureEncoder.Encode( Name, Format, MinificationFilter, MagnificationFilter, WrapModeU, WrapModeV, GFDLibrary.Textures.PngCodec.Load( path ) ) ) );
            RegisterReplaceHandler<DDSStream>( path => TextureReplacement.Prepare( Data.Version, new Texture( Name, Format, File.ReadAllBytes( path ), MinificationFilter, MagnificationFilter, WrapModeU, WrapModeV ) ) );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
