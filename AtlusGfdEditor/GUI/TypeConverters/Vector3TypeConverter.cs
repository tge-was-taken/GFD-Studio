using System;
using System.ComponentModel;
using System.Numerics;

namespace AtlusGfdEditor.GUI.TypeConverters
{
    public class Vector3TypeConverter : TypeConverter
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
            if ( destinationType == typeof( string ) && value is Vector3 vector )
            {
                return $"[{vector.X}, {vector.Y}, {vector.Z}]";
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
                var floatValueStrings = input.Trim( '[', ']' )
                                             .Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );

                var x = float.Parse( floatValueStrings[0] );
                var y = float.Parse( floatValueStrings[1] );
                var z = float.Parse( floatValueStrings[2] );
                return new Vector3( x, y, z );
            }
            else
            {
                return base.ConvertFrom( context, culture, value );
            }
        }
    }
}