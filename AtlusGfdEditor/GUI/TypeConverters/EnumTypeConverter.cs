using System;
using System.ComponentModel;

namespace AtlusGfdEditor.GUI.TypeConverters
{

    // Handles undefined enum values properly
    public class EnumTypeConverter<T> : TypeConverter 
        where T : struct
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            if ( sourceType == typeof( string ) )
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom( context, sourceType );
            }
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            if ( destinationType == typeof( string ) )
            {
                return true;
            }
            else
            {
                return base.CanConvertTo( context, destinationType );
            }
        }

        public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
        {
            if ( destinationType == typeof( string ) && value is T enumValue )
            {
                return enumValue.ToString();
            }
            else
            {
                return base.ConvertTo( context, culture, value, destinationType );
            }
        }

        public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
        {
            if ( value is string input )
            {
                Enum.TryParse< T >( input, out var enumValue );
                return enumValue;
            }
            else
            {
                return base.ConvertFrom( context, culture, value );
            }
        }
    }
}