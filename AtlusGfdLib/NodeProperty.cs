using System.Numerics;
using System.Text;

namespace AtlusGfdLib
{
    public abstract class NodeProperty
    {
        public PropertyValueType ValueType { get; }

        public string Name { get; set; }

        protected NodeProperty( PropertyValueType valueType, string name )
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

    public sealed class NodeIntProperty : NodeProperty
    {
        public int Value { get; set; }

        public NodeIntProperty( string name ) : base(PropertyValueType.Int, name) { }

        public NodeIntProperty( string name, int value ) : base(PropertyValueType.Int, name) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class NodeFloatProperty : NodeProperty
    {
        public float Value { get; set; }

        public NodeFloatProperty( string name ) : base( PropertyValueType.Float, name ) { }

        public NodeFloatProperty( string name, float value ) : base( PropertyValueType.Float, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class NodeBoolProperty : NodeProperty
    {
        public bool Value { get; set; }

        public NodeBoolProperty( string name ) : base( PropertyValueType.Bool, name ) { }

        public NodeBoolProperty( string name, bool value ) : base( PropertyValueType.Bool, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value.ToString();
        }
    }

    public sealed class NodeStringProperty : NodeProperty
    {
        public string Value { get; set; }

        public NodeStringProperty( string name ) : base( PropertyValueType.String, name ) { }

        public NodeStringProperty( string name, string value ) : base( PropertyValueType.String, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return Value;
        }
    }

    public sealed class NodeByteVector3Property : NodeProperty
    {
        public ByteVector3 Value { get; set; }

        public NodeByteVector3Property( string name ) : base( PropertyValueType.ByteVector3, name ) { }

        public NodeByteVector3Property( string name, ByteVector3 value ) : base( PropertyValueType.ByteVector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}]";
        }
    }

    public sealed class NodeByteVector4Property : NodeProperty
    {
        public ByteVector4 Value { get; set; }

        public NodeByteVector4Property( string name ) : base( PropertyValueType.ByteVector4, name ) { }

        public NodeByteVector4Property( string name, ByteVector4 value ) : base( PropertyValueType.ByteVector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}, {Value.W}]";
        }
    }

    public sealed class NodeVector3Property : NodeProperty
    {
        public Vector3 Value { get; set; }

        public NodeVector3Property( string name ) : base( PropertyValueType.Vector3, name ) { }

        public NodeVector3Property( string name, Vector3 value ) : base( PropertyValueType.Vector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}]";
        }
    }

    public sealed class NodeVector4Property : NodeProperty
    {
        public Vector4 Value { get; set; }

        public NodeVector4Property( string name ) : base( PropertyValueType.Vector4, name ) { }

        public NodeVector4Property( string name, Vector4 value ) : base( PropertyValueType.Vector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }

        protected override string ValueToUserPropertyString()
        {
            return $"[{Value.X}, {Value.Y}, {Value.Z}, {Value.W}]";
        }
    }

    public sealed class NodeByteArrayProperty : NodeProperty
    {
        public byte[] Value { get; set; }

        public NodeByteArrayProperty( string name ) : base( PropertyValueType.ByteArray, name ) { }

        public NodeByteArrayProperty( string name, byte[] value ) : base( PropertyValueType.ByteArray, name ) => Value = value;

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

    public enum PropertyValueType
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