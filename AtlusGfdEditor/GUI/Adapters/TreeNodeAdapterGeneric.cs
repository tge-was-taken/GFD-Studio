using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdEditor.GUI.Adapters
{
    public delegate T TreeNodeAdapterRebuildAction<T>();

    public abstract class TreeNodeAdapter<T> : TreeNodeAdapter
    {
        private bool mIsRebuilding;
        private TreeNodeAdapterRebuildAction<T> mRebuildAction;

        [Browsable( false )]
        public new T Resource
        {
            get
            {
                if ( IsInitialized && !mIsRebuilding && IsDirty )
                    Rebuild();

                return (T)base.Resource;
            }
            private set
            {
                base.Resource = value;
            }
        }

        [Browsable( false )]
        public override Type ResourceType => typeof( T );

        protected TreeNodeAdapter( string text, T resource ) : base( text )
        {
            Resource = resource;
        }

        protected void RegisterRebuildAction( TreeNodeAdapterRebuildAction<T> action )
        {
            mRebuildAction = action;
        }

        protected override void Rebuild()
        {
            if ( mRebuildAction != null )
            {
                Trace.TraceInformation( $"{nameof( TreeNodeAdapter<T> )} [{Text}]: {nameof( Rebuild )}" );

                // enter rebuild state
                mIsRebuilding = true;

                // rebuild resource
                Resource = mRebuildAction();
                NotifyResourcePropertyChanged();

                // exit rebuild state
                mIsRebuilding = false;
            }
        }
    }
}
