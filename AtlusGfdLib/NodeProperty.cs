using System;
using System.Numerics;

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
    }

    public class NodeIntProperty : NodeProperty
    {
        public int Value { get; set; }

        public NodeIntProperty( string name ) : base(PropertyValueType.Int, name) { }

        public NodeIntProperty( string name, int value ) : base(PropertyValueType.Int, name) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeFloatProperty : NodeProperty
    {
        public float Value { get; set; }

        public NodeFloatProperty( string name ) : base( PropertyValueType.Float, name ) { }

        public NodeFloatProperty( string name, float value ) : base( PropertyValueType.Float, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeBoolProperty : NodeProperty
    {
        public bool Value { get; set; }

        public NodeBoolProperty( string name ) : base( PropertyValueType.Bool, name ) { }

        public NodeBoolProperty( string name, bool value ) : base( PropertyValueType.Bool, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeStringProperty : NodeProperty
    {
        public string Value { get; set; }

        public NodeStringProperty( string name ) : base( PropertyValueType.String, name ) { }

        public NodeStringProperty( string name, string value ) : base( PropertyValueType.String, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeByteVector3Property : NodeProperty
    {
        public ByteVector3 Value { get; set; }

        public NodeByteVector3Property( string name ) : base( PropertyValueType.ByteVector3, name ) { }

        public NodeByteVector3Property( string name, ByteVector3 value ) : base( PropertyValueType.ByteVector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeByteVector4Property : NodeProperty
    {
        public ByteVector4 Value { get; set; }

        public NodeByteVector4Property( string name ) : base( PropertyValueType.ByteVector4, name ) { }

        public NodeByteVector4Property( string name, ByteVector4 value ) : base( PropertyValueType.ByteVector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeVector3Property : NodeProperty
    {
        public Vector3 Value { get; set; }

        public NodeVector3Property( string name ) : base( PropertyValueType.Vector3, name ) { }

        public NodeVector3Property( string name, Vector3 value ) : base( PropertyValueType.Vector3, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeVector4Property : NodeProperty
    {
        public Vector4 Value { get; set; }

        public NodeVector4Property( string name ) : base( PropertyValueType.Vector4, name ) { }

        public NodeVector4Property( string name, Vector4 value ) : base( PropertyValueType.Vector4, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
        }
    }

    public class NodeArrayProperty : NodeProperty
    {
        public byte[] Value { get; set; }

        public NodeArrayProperty( string name ) : base( PropertyValueType.Array, name ) { }

        public NodeArrayProperty( string name, byte[] value ) : base( PropertyValueType.Array, name ) => Value = value;

        public override object GetValue()
        {
            return Value;
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
        Array        = 9,
    }
}