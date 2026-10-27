using System;
using System.ComponentModel;

namespace GFDStudio.GUI.TypeConverters
{
    public class VariantUserPropertyListPropertyDescriptor : PropertyDescriptor
    {
        private readonly VariantUserPropertyList mList;
        private readonly int                     mIndex;

        internal VariantUserPropertyListPropertyDescriptor( VariantUserPropertyList list, int index )
            : base( list[index].Name, null )
        {
            mList  = list;
            mIndex = index;
        }

        public override Type PropertyType => typeof( VariantUserProperty );

        public override void SetValue( object component, object value )
        {
            mList[mIndex] = ( VariantUserProperty )value;
        }

        public override object GetValue( object component )
        {
            return mList[mIndex].Value;
        }

        public override bool IsReadOnly => false;

        public override Type ComponentType => mList.GetType();

        public override bool CanResetValue( object component )
        {
            return true;
        }

        public override void ResetValue( object component )
        {
        }

        public override bool ShouldSerializeValue( object component )
        {
            return true;
        }

        public override TypeConverter Converter => new VariantUserPropertyTypeConverter( mList[ mIndex ].Name );

        public override AttributeCollection Attributes => new AttributeCollection( null );
    }
}