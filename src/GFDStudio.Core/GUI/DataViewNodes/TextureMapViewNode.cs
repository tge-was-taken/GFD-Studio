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
            get => Data.Name;
            set => Set( x => x.Name, value );
        }

        [Browsable( true )]
        [DisplayName( "Flags" )]
        public int Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Minification" )]
        public TextureFilteringMethod MinificationFilter
        {
            get => Data.MinificationFilter;
            set => Set( x => x.MinificationFilter, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureFilteringMethod> ) )]
        [DisplayName( "Texture Filtering - Magnification" )]
        public TextureFilteringMethod MagnificationFilter
        {
            get => Data.MagnificationFilter;
            set => Set( x => x.MagnificationFilter, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode U" )]
        public TextureWrapMethod WrapModeU
        {
            get => Data.WrapModeU;
            set => Set( x => x.WrapModeU, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<TextureWrapMethod> ) )]
        [DisplayName( "Wrap Mode V" )]
        public TextureWrapMethod WrapModeV
        {
            get => Data.WrapModeV;
            set => Set( x => x.WrapModeV, value );
        }

        [Browsable( true )]
        public float Field4C
        {
            get => Data.Field4C;
            set => Set( x => x.Field4C, value );
        }

        [Browsable( true )]
        public float Field50
        {
            get => Data.Field50;
            set => Set( x => x.Field50, value );
        }

        [Browsable( true )]
        public float Field54
        {
            get => Data.Field54;
            set => Set( x => x.Field54, value );
        }

        [Browsable( true )]
        public float Field58
        {
            get => Data.Field58;
            set => Set( x => x.Field58, value );
        }

        [Browsable( true )]
        public float Field5C
        {
            get => Data.Field5C;
            set => Set( x => x.Field5C, value );
        }

        [Browsable( true )]
        public float Field60
        {
            get => Data.Field60;
            set => Set( x => x.Field60, value );
        }

        [Browsable( true )]
        public float Field64
        {
            get => Data.Field64;
            set => Set( x => x.Field64, value );
        }

        [Browsable( true )]
        public float Field68
        {
            get => Data.Field68;
            set => Set( x => x.Field68, value );
        }

        [Browsable( true )]
        public float Field6C
        {
            get => Data.Field6C;
            set => Set( x => x.Field6C, value );
        }

        [Browsable( true )]
        public float Field70
        {
            get => Data.Field70;
            set => Set( x => x.Field70, value );
        }

        [Browsable( true )]
        public float Field74
        {
            get => Data.Field74;
            set => Set( x => x.Field74, value );
        }

        [Browsable( true )]
        public float Field78
        {
            get => Data.Field78;
            set => Set( x => x.Field78, value );
        }

        [Browsable( true )]
        public float Field7C
        {
            get => Data.Field7C;
            set => Set( x => x.Field7C, value );
        }

        [Browsable( true )]
        public float Field80
        {
            get => Data.Field80;
            set => Set( x => x.Field80, value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => Data.Field84;
            set => Set( x => x.Field84, value );
        }

        [Browsable( true )]
        public float Field88
        {
            get => Data.Field88;
            set => Set( x => x.Field88, value );
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
