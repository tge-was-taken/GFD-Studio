using System.ComponentModel;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public abstract class MaterialAttributeViewModel<T> : TreeNodeViewModel< T > where T : MaterialAttribute
    {
        [Browsable( true )]
        [TypeConverter(typeof(EnumTypeConverter<MaterialAttributeFlags>))]
        public MaterialAttributeFlags Flags
        {
            get => GetModelProperty<MaterialAttributeFlags>();
            set => SetModelProperty( value );
        }

        [Browsable(true)]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType> ) )]
        public MaterialAttributeType AttributeType
        {
            get => GetModelProperty<MaterialAttributeType>();
        }

        protected MaterialAttributeViewModel( string text, T resource ) : base( text, resource )
        {
        }
    }
}