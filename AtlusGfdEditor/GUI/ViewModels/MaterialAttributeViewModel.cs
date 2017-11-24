using System.ComponentModel;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
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
        public MaterialAttributeType Type
        {
            get => GetModelProperty<MaterialAttributeType>();
        }

        protected MaterialAttributeViewModel( string text, T resource ) : base( text, resource )
        {
        }
    }
}