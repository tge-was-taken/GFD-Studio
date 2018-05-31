using System.ComponentModel;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public abstract class ResourceViewModel<T> : TreeNodeViewModel<T>
        where T : GFDLibrary.Resource
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
