using System;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.TypeConverters
{
    public class QuaternionTypeConverter : TypeConverter
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
            if ( destinationType == typeof( string ) && value is Quaternion vector )
            {
                return $"[{vector.X}, {vector.Y}, {vector.Z}, {vector.W}]";
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
                var w = float.Parse( floatValueStrings[3] );
                return new Quaternion( x, y, z, w );
            }
            else
            {
                return base.ConvertFrom( context, culture, value );
            }
        }
    }
}