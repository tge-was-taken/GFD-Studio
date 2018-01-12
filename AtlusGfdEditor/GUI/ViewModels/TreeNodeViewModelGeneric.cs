using System;
using System.ComponentModel;
using System.Diagnostics;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public delegate T TreeNodeViewModelModelUpdateHandler<out T>();

    public abstract class TreeNodeViewModel<T> : TreeNodeViewModel
    {
        private bool mIsUpdatingModel;
        private TreeNodeViewModelModelUpdateHandler<T> mModelUpdateHandler;

        [Browsable( false )]
        public new T Model
        {
            get
            {
                if ( IsInitialized && !mIsUpdatingModel && HasPendingChanges )
                    UpdateModel();

                return ( T )base.Model;
            }
            protected set
            {
                base.Model = value;
            }
        }

        [Browsable( false )]
        public override Type ModelType => typeof( T );

        protected TreeNodeViewModel( string text, T resource ) : base( text )
        {
            Model = resource;
        }

        protected void RegisterModelUpdateHandler( TreeNodeViewModelModelUpdateHandler<T> action )
        {
            mModelUpdateHandler = action;
        }

        protected override void UpdateModel()
        {
            if ( mModelUpdateHandler == null )
                return;

            Trace.TraceInformation( $"{nameof( TreeNodeViewModel<T> )} [{Text}]: {nameof( UpdateModel )}" );

            // enter update state
            mIsUpdatingModel = true;

            // update model
            Model = mModelUpdateHandler();
            NotifyModelPropertyChanged();

            // exit update state
            mIsUpdatingModel = false;
        }
    }
}
