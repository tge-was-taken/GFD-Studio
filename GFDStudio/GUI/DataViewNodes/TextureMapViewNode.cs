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
        public int Field44
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Minification" )]
        public TextureFilteringMethod Field48
        {
            get => GetDataProperty<TextureFilteringMethod>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Magnification" )]
        public TextureFilteringMethod Field49
        {
            get => GetDataProperty<TextureFilteringMethod>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode U" )]
        public TextureWrapMethod Field4A
        {
            get => GetDataProperty<TextureWrapMethod>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode V" )]
        public TextureWrapMethod Field4B
        {
            get => GetDataProperty<TextureWrapMethod>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field4C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field50
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field54
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field58
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field5C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field60
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field64
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field68
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field6C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field70
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field74
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field78
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field7C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field80
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field88
        {
            get => GetDataProperty<float>();
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
