using System.ComponentModel;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class MaterialAttributeViewNode<T> : DataViewNode< T > where T : MaterialAttribute
    {
        [Browsable( true )]
        [TypeConverter(typeof(EnumTypeConverter<MaterialAttributeFlags>))]
        public MaterialAttributeFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [Browsable(true)]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType> ) )]
        [DisplayName( "Attribute type" )]
        public MaterialAttributeType AttributeType
        {
            get => Data.AttributeType;
        }

        protected MaterialAttributeViewNode( string text, T data ) : base( text, data )
        {
        }
    }
}