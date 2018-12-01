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
            get => GetDataProperty<MaterialAttributeFlags>();
            set => SetDataProperty( value );
        }

        [Browsable(true)]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType> ) )]
        public MaterialAttributeType AttributeType
        {
            get => GetDataProperty<MaterialAttributeType>();
        }

        protected MaterialAttributeViewNode( string text, T data ) : base( text, data )
        {
        }
    }
}