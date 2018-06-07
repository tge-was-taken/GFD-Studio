using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public class UserPropertyCollection : Resource, IDictionary<string, UserProperty>
    {
        private readonly Dictionary<string, UserProperty> mDictionary;

        public override ResourceType ResourceType => ResourceType.UserPropertyCollection;

        public UserPropertyCollection()
        {
            mDictionary = new Dictionary< string, UserProperty >();
        }

        public UserPropertyCollection( uint version ) : base(version)
        {
            mDictionary = new Dictionary<string, UserProperty>();
        }

        public void Add( UserProperty property )
        {
            mDictionary[property.Name] = property;
        }

        public void Remove( UserProperty property )
        {
            Remove( property.Name );
        }

        internal override void Read( ResourceReader reader )
        {
            int propertyCount = reader.ReadInt32();

            for ( int i = 0; i < propertyCount; i++ )
            {
                var type = ( UserPropertyValueType )reader.ReadInt32();
                var name = reader.ReadStringWithHash( Version );
                var size = reader.ReadInt32();

                UserProperty property;
                switch ( type )
                {
                    case UserPropertyValueType.Int:
                        property = new UserIntProperty( name, reader.ReadInt32() );
                        break;
                    case UserPropertyValueType.Float:
                        property = new UserFloatProperty( name, reader.ReadSingle() );
                        break;
                    case UserPropertyValueType.Bool:
                        property = new UserBoolProperty( name, ( ( BinaryReader ) reader ).ReadBoolean() );
                        break;
                    case UserPropertyValueType.String:
                        property = new UserStringProperty( name, reader.ReadString( size - 1 ) );
                        break;
                    case UserPropertyValueType.ByteVector3:
                        property = new UserByteVector3Property( name, reader.ReadByteVector3() );
                        break;
                    case UserPropertyValueType.ByteVector4:
                        property = new UserByteVector4Property( name, reader.ReadByteVector4() );
                        break;
                    case UserPropertyValueType.Vector3:
                        property = new UserVector3Property( name, reader.ReadVector3() );
                        break;
                    case UserPropertyValueType.Vector4:
                        property = new UserVector4Property( name, reader.ReadVector4() );
                        break;
                    case UserPropertyValueType.ByteArray:
                        property = new UserByteArrayProperty( name, reader.ReadBytes( size ) );
                        break;
                    default:
                        throw new InvalidDataException( $"Unknown node property type: {type}" );
                }

                Add( property );
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( Count );

            foreach ( var property in Values )
            {
                writer.WriteInt32( ( int )property.ValueType );
                writer.WriteStringWithHash( Version, property.Name );

                switch ( property.ValueType )
                {
                    case UserPropertyValueType.Int:
                        writer.WriteInt32( 4 );
                        writer.WriteInt32( property.GetValue<int>() );
                        break;
                    case UserPropertyValueType.Float:
                        writer.WriteInt32( 4 );
                        writer.WriteSingle( property.GetValue<float>() );
                        break;
                    case UserPropertyValueType.Bool:
                        writer.WriteInt32( 1 );
                        writer.WriteBoolean( property.GetValue<bool>() );
                        break;
                    case UserPropertyValueType.String:
                        {
                            var value = property.GetValue<string>();
                            writer.WriteInt32( value.Length + 1 );
                            writer.WriteString( value, value.Length );
                        }
                        break;
                    case UserPropertyValueType.ByteVector3:
                        writer.WriteInt32( 3 );
                        writer.WriteByteVector3( property.GetValue<ByteVector3>() );
                        break;
                    case UserPropertyValueType.ByteVector4:
                        writer.WriteInt32( 4 );
                        writer.WriteByteVector4( property.GetValue<ByteVector4>() );
                        break;
                    case UserPropertyValueType.Vector3:
                        writer.WriteInt32( 12 );
                        writer.WriteVector3( property.GetValue<Vector3>() );
                        break;
                    case UserPropertyValueType.Vector4:
                        writer.WriteInt32( 16 );
                        writer.WriteVector4( property.GetValue<Vector4>() );
                        break;
                    case UserPropertyValueType.ByteArray:
                        {
                            var value = property.GetValue<byte[]>();
                            writer.WriteInt32( value.Length );
                            writer.WriteBytes( value );
                        }
                        break;
                    default:
                        throw new InvalidOperationException( $"Unknown node property type: {property.ValueType}" );
                }
            }
        }

        #region IDictionary implementation

        public UserProperty this[string key] { get => mDictionary[key]; set => mDictionary[key] = value; }

        public ICollection<string> Keys => ( ( IDictionary<string, UserProperty> )mDictionary ).Keys;

        public ICollection<UserProperty> Values => ( ( IDictionary<string, UserProperty> )mDictionary ).Values;

        public int Count => mDictionary.Count;

        public bool IsReadOnly => ( ( IDictionary<string, UserProperty> )mDictionary ).IsReadOnly;

        public void Add( string key, UserProperty value )
        {
            mDictionary.Add( key, value );
        }

        public void Add( KeyValuePair<string, UserProperty> item )
        {
            ( ( IDictionary<string, UserProperty> )mDictionary ).Add( item );
        }

        public void Clear()
        {
            mDictionary.Clear();
        }

        public bool Contains( KeyValuePair<string, UserProperty> item )
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).Contains( item );
        }

        public bool ContainsKey( string key )
        {
            return mDictionary.ContainsKey( key );
        }

        public void CopyTo( KeyValuePair<string, UserProperty>[] array, int arrayIndex )
        {
            ( ( IDictionary<string, UserProperty> )mDictionary ).CopyTo( array, arrayIndex );
        }

        public IEnumerator<KeyValuePair<string, UserProperty>> GetEnumerator()
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).GetEnumerator();
        }

        public bool Remove( string key )
        {
            return mDictionary.Remove( key );
        }

        public bool Remove( KeyValuePair<string, UserProperty> item )
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).Remove( item );
        }

        public bool TryGetValue( string key, out UserProperty value )
        {
            return mDictionary.TryGetValue( key, out value );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IDictionary<string, UserProperty> )mDictionary ).GetEnumerator();
        }

        #endregion
    }
}
