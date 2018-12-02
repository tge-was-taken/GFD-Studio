using System;
using System.ComponentModel;
using System.Diagnostics;

namespace GFDStudio.GUI.DataViewNodes
{
    public delegate T DataViewNodeUpdateHandler<out T>();

    public abstract class DataViewNode<T> : DataViewNode
    {
        private bool mIsUpdatingData;
        private DataViewNodeUpdateHandler<T> mDataUpdateHandler;

        [Browsable( false )]
        public new T Data
        {
            get
            {
                if ( IsInitialized && !mIsUpdatingData && HasPendingChanges )
                    UpdateData();

                return ( T )base.Data;
            }
            protected set
            {
                base.Data = value;
            }
        }

        [Browsable( false )]
        public override Type DataType => typeof( T );

        protected DataViewNode( string text, T data ) : base( text )
        {
            Data = data;
        }

        protected void RegisterModelUpdateHandler( DataViewNodeUpdateHandler<T> action )
        {
            mDataUpdateHandler = action;
        }

        protected override void UpdateData()
        {
            if ( mDataUpdateHandler == null || !IsViewInitialized )
                return;

            Trace.TraceInformation( $"{nameof( DataViewNode<T> )} [{Text}]: {nameof( UpdateData )}" );

            // enter update state
            mIsUpdatingData = true;

            // update data
            Data = mDataUpdateHandler();
            NotifyDataPropertyChanged();

            // exit update state
            mIsUpdatingData = false;
        }
    }
}
