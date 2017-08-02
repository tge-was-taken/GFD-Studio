using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public abstract class TreeNodeAdapter<T> : TreeNodeAdapter
    {
        public new T Resource
        {
            get
            {
                //if ( IsInitialized )
                //    Rebuild();

                return (T)base.Resource;
            }
            protected set
            {
                base.Resource = value;
            }
        }

        protected TreeNodeAdapter( string text, T resource ) : base( text )
        {
            Resource = resource;
        }

        protected abstract T RebuildCore();

        private void Rebuild()
        {
            Resource = RebuildCore();
        }
    }
}
