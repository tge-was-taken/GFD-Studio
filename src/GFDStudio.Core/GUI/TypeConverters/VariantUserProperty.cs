using System.ComponentModel;
using System.Numerics;
using GFDLibrary.Common;

namespace GFDStudio.GUI.TypeConverters
{
    public class VariantUserProperty
    {
        public string Name { get; set; }

        [TypeConverter(typeof( VariantUserPropertyValueTypeConverter ) )]
        public object Value { get; set; }

        public VariantUserProperty()
        {
            Name  = "New user property";
            Value = string.Empty;
        }

        public VariantUserProperty( UserProperty userProperty )
        {
            Name  = userProperty.Name;
            Value = userProperty.GetValue();
        }

        public VariantUserProperty( string name, object value )
        {
            Name = name;
            Value = value;
        }

        public UserProperty ToTypedUserProperty()
        {
            switch ( Value )
            {
                case byte typedValue:
                    return new UserIntProperty( Name, typedValue );

                case sbyte typedValue:
                    return new UserIntProperty( Name, typedValue );

                case short typedValue:
                    return new UserIntProperty( Name, typedValue );

                case ushort typedValue:
                    return new UserIntProperty( Name, typedValue );

                case int typedValue:
                    return new UserIntProperty( Name, typedValue );

                case uint typedValue:
                    return new UserIntProperty( Name, ( int )typedValue );

                case float typedValue:
                    return new UserFloatProperty( Name, typedValue );

                case double typedValue:
                    return new UserFloatProperty( Name, ( float )typedValue );

                case bool typedValue:
                    return new UserBoolProperty( Name, typedValue );

                case string typedValue:
                    return new UserStringProperty( Name, typedValue );

                case ByteVector3 typedValue:
                    return new UserByteVector3Property( Name, typedValue );

                case ByteVector4 typedValue:
                    return new UserByteVector4Property( Name, typedValue );

                case Vector3 typedValue:
                    return new UserVector3Property( Name, typedValue );

                case Vector4 typedValue:
                    return new UserVector4Property( Name, typedValue );

                case byte[] typedValue:
                    return new UserByteArrayProperty( Name, typedValue );

                default:
                    return new UserStringProperty( Name, string.Empty );
            }
        }
    }
}