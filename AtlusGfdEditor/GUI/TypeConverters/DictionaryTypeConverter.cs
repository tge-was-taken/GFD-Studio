using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.TypeConverters
{
    public class UserPropertyDictionaryTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes )
        {
            var dictionary = value as UserPropertyCollection;
            if ( dictionary == null )
                return new PropertyDescriptorCollection( new PropertyDescriptor[0] );

            var properties = new PropertyDescriptor[ dictionary.Count ];

            int nextIndex = -1;
            foreach ( KeyValuePair<string, UserProperty> e in dictionary )
            {
                properties[ ++nextIndex ] = ( new UserPropertyDictionaryPropertyDescriptor( dictionary, e.Key ) );
            }

            return new PropertyDescriptorCollection( properties );
        }

        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return true;
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            return true;
        }

        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override bool IsValid( ITypeDescriptorContext context, object value )
        {
            return true;
        }
    }

    public class UserPropertyDictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly UserPropertyCollection mDictionary;
        private readonly string mKey;

        internal UserPropertyDictionaryPropertyDescriptor( UserPropertyCollection d, string key )
            : base( key, null )
        {
            mDictionary = d;
            mKey = key;
        }

        public override Type PropertyType => mDictionary[mKey].GetType();

        public override void SetValue( object component, object value )
        {
            UserProperty property = null;
            var input = ( string ) value;

            if ( int.TryParse( input, out int intValue ) )
            {
                property = new UserIntProperty( mKey, intValue );
            }
            else if ( float.TryParse( input, out float floatValue ) )
            {
                property = new UserFloatProperty( mKey, floatValue );
            }
            else if ( bool.TryParse( input, out bool boolValue ) )
            {
                property = new UserBoolProperty( mKey, boolValue );
            }
            else if ( input.Contains( "," ) )
            {
                var vectorContents = input.Trim( '[', ']' )
                                          .Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

                var vectorFloats = new float[vectorContents.Length];
                for ( int i = 0; i < vectorContents.Length; i++ )
                {
                    if ( !float.TryParse( vectorContents[i], out var vectorFloat ) )
                        break;

                    vectorFloats[ i ] = vectorFloat;
                }

                if ( vectorFloats.Length == 3 )
                {
                    property = new UserVector3Property( mKey, new Vector3( vectorFloats[ 0 ], vectorFloats[ 1 ], vectorFloats[ 2 ] ) );
                }
                else if ( vectorFloats.Length == 4 )
                {
                    property = new UserVector4Property(
                        mKey, new Vector4( vectorFloats[ 0 ], vectorFloats[ 1 ], vectorFloats[ 2 ], vectorFloats[ 3 ] ) );
                }
                else
                {
                    property = new UserByteArrayProperty( mKey, vectorFloats.Cast< byte >().ToArray() );
                }
            }

            if ( property == null )
            {
                property = new UserStringProperty( mKey, input );
            }

            mDictionary[mKey] = property;
        }

        public override object GetValue( object component )
        {
            return mDictionary[mKey].GetValue();
        }

        public override bool IsReadOnly => false;

        public override Type ComponentType => null;

        public override bool CanResetValue( object component )
        {
            return false;
        }

        public override void ResetValue( object component )
        {
        }

        public override bool ShouldSerializeValue( object component )
        {
            return false;
        }
    }
}
