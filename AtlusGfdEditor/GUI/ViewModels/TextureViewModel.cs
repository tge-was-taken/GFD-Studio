using System.ComponentModel;
using System.Drawing;
using System.IO;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;
using AtlusGfdLib.Common.Utillities;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class TextureViewModel : TreeNodeViewModel<Texture>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags => 
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => GetModelProperty<string>();
            set
            {
                SetModelProperty( value );
                Text = value;
            }
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFormat> ) )]
        public TextureFormat Format
        {
            get => GetModelProperty<TextureFormat>();
        }

        [Browsable( true )]
        [TypeConverter(typeof( Int32HexTypeConverter ) )]
        public int DataLength
        {
            get => Model.Data.Length;
        }

        [Browsable( true )]
        public byte Field1C
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field1D
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field1E
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field1F
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        protected internal TextureViewModel( string text, Texture resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Bitmap>( path => TextureDecoder.Decode( Model ).Save( path, ImageFormatHelper.GetImageFormatFromPath( path ) ) );
            RegisterExportHandler<Stream>( path => File.WriteAllBytes( path, Model.Data ) );

            RegisterReplaceHandler<Bitmap>( path => TextureEncoder.Encode( Name, Format, Field1C, Field1D, Field1E, Field1F, new Bitmap( path ) ) );
            RegisterReplaceHandler< Stream >( path => new Texture( Name, Format, File.ReadAllBytes( path ), Field1C, Field1D, Field1E, Field1F ) );

            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
