using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using GFDLibrary.Common;
using GFDStudio.GUI.Forms;

namespace GFDStudio.GUI.TypeConverters
{
    public class VariantUserPropertyList : CollectionBase, ICustomTypeDescriptor, IEnumerable<VariantUserProperty>
    {
        private Action mModifiedCallback;

        public VariantUserPropertyList( UserPropertyDictionary dictionary, Action modified )
        {
            if ( dictionary != null )
            {
                foreach ( var userProperty in dictionary.Values )
                {
                    List.Add( new VariantUserProperty( userProperty ) );
                }
            }

            mModifiedCallback = modified;
        }

        public VariantUserProperty this[ int index ]
        {
            get => ( VariantUserProperty )List[ index ];
            set => List[index] = value;
        }

        protected override void OnClearComplete()
        {
            if ( mModifiedCallback != null )
            {
                mModifiedCallback();
                MainForm.Instance.UpdateSelection();
            }

            base.OnClearComplete();
        }

        protected override void OnInsertComplete( int index, object value )
        {
            if ( mModifiedCallback != null )
            {
                mModifiedCallback();
                MainForm.Instance.UpdateSelection();
            }

            base.OnInsertComplete( index, value );
        }

        protected override void OnRemoveComplete( int index, object value )
        {
            if ( mModifiedCallback != null )
            {
                mModifiedCallback();
                MainForm.Instance.UpdateSelection();
            }
            base.OnRemoveComplete( index, value );
        }

        protected override void OnSetComplete( int index, object oldValue, object newValue )
        {
            if ( mModifiedCallback != null )
            {
                mModifiedCallback();
                MainForm.Instance.UpdateSelection();
            }
            base.OnSetComplete( index, oldValue, newValue );
        }

        public PropertyDescriptorCollection GetProperties( Attribute[] attributes )
            => GetProperties();

        public PropertyDescriptorCollection GetProperties()
        {
            var properties = new PropertyDescriptor[Count];

            for ( var i = 0; i < Count; i++ )
            {
                properties[ i ] = new VariantUserPropertyListPropertyDescriptor( this, i );
            }

            return new PropertyDescriptorCollection( properties );
        }

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes( this, true );

        public string GetClassName() => TypeDescriptor.GetClassName( this, true );

        public string GetComponentName() => TypeDescriptor.GetComponentName( this, true );

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter( this, true );

        public EventDescriptor GetDefaultEvent()
            => TypeDescriptor.GetDefaultEvent( this, true );

        public PropertyDescriptor GetDefaultProperty()
            => TypeDescriptor.GetDefaultProperty( this, true );

        public object GetEditor( Type editorBaseType )
            => TypeDescriptor.GetEditor( this, editorBaseType, true );

        public EventDescriptorCollection GetEvents()
            => TypeDescriptor.GetEvents( this, true );

        public EventDescriptorCollection GetEvents( Attribute[] attributes )
            => TypeDescriptor.GetEvents( this, attributes, true );

        public object GetPropertyOwner( PropertyDescriptor pd )
            => this;

        IEnumerator<VariantUserProperty> IEnumerable<VariantUserProperty>.GetEnumerator()
        {
            foreach ( VariantUserProperty userProperty in List )
                yield return userProperty;
        }
    }
}
