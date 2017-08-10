using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public abstract class ResourceAdapter<T> : TreeNodeAdapter<T>
        where T : AtlusGfdLib.Resource
    {
        [Browsable( true )]
        [TypeConverter( typeof( UInt32HexTypeConverter ) )]
        public uint Version
        {
            get => Resource.Version;
            set => Resource.Version = value;
        }

        protected internal ResourceAdapter( string text, T resource ) : base( text, resource )
        {
        }
    }
}
