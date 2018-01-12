using System.Numerics;
using System.Text;

namespace AtlusGfdLib
{
    public abstract class UserProperty
    {
        public UserPropertyValueType ValueType { get; }

        public string Name { get; set; }

        protected UserProperty( UserPropertyValueType valueType, string name )
        {
            ValueType = valueType;
            Name = name;
        }

        public abstract object GetValue();

        public T GetValue<T>() => ( T )GetValue();

        public string ToUserPropertyString()
        {
            return $"{Name} = {ValueToUserPropertyString()}";
        }

        protected abstract string ValueToUserPropertyString();
    }

    public sealed class UserIntProperty : UserProperty
    {
        public int Value { get; set; }

        public UserIntProperty( string name ) : base(UserPropertyValueType.Int, name) { }

        public UserIntProperty( string name, int value ) : base(UserPropertyValueType.Int, name) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class UserFloatProperty : UserProperty
    {
        public float Value { get; set; }

        public UserFloatProperty( string name ) : base( UserPropertyValueType.Float, name ) { }

        public UserFloatProperty( string name, float value ) : base( UserPropertyValueType.Float, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class UserBoolProperty : UserProperty
    {
        public bool Value { get; set; }

        public UserBoolProperty( string name ) : base( UserPropertyValueType.Bool, name ) { }

        public UserBoolProperty( string name, bool value ) : base( UserPropertyValueType.Bool, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class UserStringProperty : UserProperty
    {
        public string Value { get; set; }

        public UserStringProperty( string name ) : base( UserPropertyValueType.String, name ) { }

        public UserStringProperty( string name, string value ) : base( UserPropertyValueType.String, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value;
        }
    }

    public sealed class UserByteVector3Property : UserProperty
    {
        public ByteVector3 Value { get; set; }

        public UserByteVector3Property( string name ) : base( UserPropertyValueType.ByteVector3, name ) { }

        public UserByteVector3Property( string name, ByteVector3 value ) : base( UserPropertyValueType.ByteVector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}]";
        }
    }

    public sealed class UserByteVector4Property : UserProperty
    {
        public ByteVector4 Value { get; set; }

        public UserByteVector4Property( string name ) : base( UserPropertyValueType.ByteVector4, name ) { }

        public UserByteVector4Property( string name, ByteVector4 value ) : base( UserPropertyValueType.ByteVector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}, {Value.W}]";
        }
    }

    public sealed class UserVector3Property : UserProperty
    {
        public Vector3 Value { get; set; }

        public UserVector3Property( string name ) : base( UserPropertyValueType.Vector3, name ) { }

        public UserVector3Property( string name, Vector3 value ) : base( UserPropertyValueType.Vector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}]";
        }
    }

    public sealed class UserVector4Property : UserProperty
    {
        public Vector4 Value { get; set; }

        public UserVector4Property( string name ) : base( UserPropertyValueType.Vector4, name ) { }

        public UserVector4Property( string name, Vector4 value ) : base( UserPropertyValueType.Vector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}, {Value.W}]";
        }
    }

    public sealed class UserByteArrayProperty : UserProperty
    {
        public byte[] Value { get; set; }

        public UserByteArrayProperty( string name ) : base( UserPropertyValueType.ByteArray, name ) { }

        public UserByteArrayProperty( string name, byte[] value ) : base( UserPropertyValueType.ByteArray, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            var builder = new StringBuilder();
            builder.Append( "[" );

            if ( Value.Length > 0 )
            {
                builder.Append( Value[0].ToString() );
            }

            for ( int i = 1; i < Value.Length; i++ )
            {
                builder.Append( $" {Value[i]}" );
            }

            builder.Append( "]" );

            return builder.ToString();
        }
    }

    public enum UserPropertyValueType
    {
        Invalid      = 0,
        Int          = 1,
        Float        = 2,
        Bool         = 3,
        String       = 4,
        ByteVector3  = 5,
        ByteVector4  = 6,
        Vector3      = 7,
        Vector4      = 8,
        ByteArray    = 9,
    }
}