using System.ComponentModel;
using AtlusGfdEditor.GUI.TypeConverters;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public abstract class ResourceViewModel<T> : TreeNodeViewModel<T>
        where T : AtlusGfdLibrary.Resource
    {
        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Version
        {
            get => GetModelProperty< uint >();
            set => SetModelProperty( value );
        }

        protected internal ResourceViewModel( string text, T resource ) : base( text, resource )
        {
        }
    }
}
