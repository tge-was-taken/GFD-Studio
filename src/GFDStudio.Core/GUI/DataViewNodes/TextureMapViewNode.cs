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
        public float TransformM11
        {
            get => Data.TransformM11;
            set => Set( x => x.TransformM11, value );
        }

        [Browsable( true )]
        public float TransformM12
        {
            get => Data.TransformM12;
            set => Set( x => x.TransformM12, value );
        }

        [Browsable( true )]
        public float TransformM13
        {
            get => Data.TransformM13;
            set => Set( x => x.TransformM13, value );
        }

        [Browsable( true )]
        public float TransformM14
        {
            get => Data.TransformM14;
            set => Set( x => x.TransformM14, value );
        }

        [Browsable( true )]
        public float TransformM21
        {
            get => Data.TransformM21;
            set => Set( x => x.TransformM21, value );
        }

        [Browsable( true )]
        public float TransformM22
        {
            get => Data.TransformM22;
            set => Set( x => x.TransformM22, value );
        }

        [Browsable( true )]
        public float TransformM23
        {
            get => Data.TransformM23;
            set => Set( x => x.TransformM23, value );
        }

        [Browsable( true )]
        public float TransformM24
        {
            get => Data.TransformM24;
            set => Set( x => x.TransformM24, value );
        }

        [Browsable( true )]
        public float TransformM31
        {
            get => Data.TransformM31;
            set => Set( x => x.TransformM31, value );
        }

        [Browsable( true )]
        public float TransformM32
        {
            get => Data.TransformM32;
            set => Set( x => x.TransformM32, value );
        }

        [Browsable( true )]
        public float TransformM33
        {
            get => Data.TransformM33;
            set => Set( x => x.TransformM33, value );
        }

        [Browsable( true )]
        public float TransformM34
        {
            get => Data.TransformM34;
            set => Set( x => x.TransformM34, value );
        }

        [Browsable( true )]
        public float TransformM41
        {
            get => Data.TransformM41;
            set => Set( x => x.TransformM41, value );
        }

        [Browsable( true )]
        public float TransformM42
        {
            get => Data.TransformM42;
            set => Set( x => x.TransformM42, value );
        }

        [Browsable( true )]
        public float TransformM43
        {
            get => Data.TransformM43;
            set => Set( x => x.TransformM43, value );
        }

        [Browsable( true )]
        public float TransformM44
        {
            get => Data.TransformM44;
            set => Set( x => x.TransformM44, value );
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
