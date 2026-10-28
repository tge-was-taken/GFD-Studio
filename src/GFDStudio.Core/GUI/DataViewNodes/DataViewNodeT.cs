using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using GFDStudio.DataManagement;

namespace GFDStudio.GUI.DataViewNodes
{
    public delegate T DataViewNodeUpdateHandler<out T>();

    public abstract class DataViewNode<T> : DataViewNode
    {
        private DataViewNodeUpdateHandler<T> mDataUpdateHandler;

        [Browsable( false )]
        public new T Data
        {
            get => ( T )base.Data;
            protected set => base.Data = value;
        }

        [Browsable( false )]
        public override Type DataType => typeof( T );

        protected DataViewNode( string text, T data ) : base( text )
        {
            Data = data;
        }

        /// <summary>
        /// Sets a property on the data and raises the change. The property is named by a lambda
        /// rather than a string, so renaming it on the library type is a build error here
        /// instead of something that only shows up when the property grid draws the node.
        /// </summary>
        protected void Set<TValue>( Expression<Func<T, TValue>> property, TValue value )
        {
            if ( property.Body is not MemberExpression member )
                throw new ArgumentException( "Expected a property access.", nameof( property ) );

            SetProperty( Data, value, member.Member.Name );
        }

        protected void RegisterModelUpdateHandler( DataViewNodeUpdateHandler<T> action )
        {
            mDataUpdateHandler = action;
        }

        protected override void UpdateData()
        {
            if ( mDataUpdateHandler == null || !HasBuiltChildNodes )
                return;

            Trace.TraceInformation( $"{nameof( DataViewNode<T> )} [{Text}]: {nameof( UpdateData )}" );

            IsUpdatingData = true;

            try
            {
                Data = mDataUpdateHandler();
                NotifyDataPropertyChanged();
            }
            finally
            {
                IsUpdatingData = false;
            }
        }
    }
}
