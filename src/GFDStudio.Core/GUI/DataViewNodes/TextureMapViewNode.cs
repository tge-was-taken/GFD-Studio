using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class TextureMapViewNode : DataViewNode<TextureMap>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => GetDataProperty< string >();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Flags" )]
        public int Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Minification" )]
        public TextureFilteringMethod MinificationFilter
        {
            get => Data.MinificationFilter;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Magnification" )]
        public TextureFilteringMethod MagnificationFilter
        {
            get => Data.MagnificationFilter;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode U" )]
        public TextureWrapMethod WrapModeU
        {
            get => Data.WrapModeU;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode V" )]
        public TextureWrapMethod WrapModeV
        {
            get => Data.WrapModeV;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field4C
        {
            get => Data.Field4C;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field50
        {
            get => Data.Field50;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field54
        {
            get => Data.Field54;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field58
        {
            get => Data.Field58;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field5C
        {
            get => Data.Field5C;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field60
        {
            get => Data.Field60;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field64
        {
            get => Data.Field64;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field68
        {
            get => Data.Field68;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field6C
        {
            get => Data.Field6C;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field70
        {
            get => Data.Field70;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field74
        {
            get => Data.Field74;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field78
        {
            get => Data.Field78;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field7C
        {
            get => Data.Field7C;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field80
        {
            get => Data.Field80;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => Data.Field84;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field88
        {
            get => Data.Field88;
            set => SetDataProperty( value );
        }

        public TextureMapViewNode( string text, TextureMap data ) : base( text, data )
        {
            RegisterExportHandler<Stream>( path => Data.Save(  path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<TextureMap> );
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
