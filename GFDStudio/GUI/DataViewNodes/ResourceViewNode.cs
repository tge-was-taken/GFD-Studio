using System.ComponentModel;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class ResourceViewNode<T> : DataViewNode<T>
        where T : GFDLibrary.Resource
    {
        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Version
        {
            get => GetDataProperty< uint >();
            set => SetDataProperty( value );
        }

        protected internal ResourceViewNode( string text, T data ) : base( text, data )
        {
        }
    }
}
