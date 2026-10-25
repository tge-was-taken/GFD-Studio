using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace GFDStudio.GUI.TypeConverters
{
    public class Vector4TypeConverter : TypeConverter
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
            if ( destinationType == typeof( string ) && value is Vector4 vector )
            {
                return $"[{vector.X.ToString( CultureInfo.InvariantCulture )}, {vector.Y.ToString( CultureInfo.InvariantCulture )}, {vector.Z.ToString( CultureInfo.InvariantCulture )}, {vector.W.ToString( CultureInfo.InvariantCulture )}]";
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

                var x = float.Parse( floatValueStrings[0], CultureInfo.InvariantCulture );
                var y = float.Parse( floatValueStrings[1], CultureInfo.InvariantCulture );
                var z = float.Parse( floatValueStrings[2], CultureInfo.InvariantCulture );
                var w = float.Parse( floatValueStrings[3], CultureInfo.InvariantCulture );
                return new Vector4( x, y, z, w );
            }
            else
            {
                return base.ConvertFrom( context, culture, value );
            }
        }
    }

    public class Vector4ColorTypeConverter : TypeConverter
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
            if ( destinationType == typeof( string ) && value is Vector4 vector )
            {
                return $"[{vector.X.ToString( CultureInfo.InvariantCulture )}, {vector.Y.ToString( CultureInfo.InvariantCulture )}, {vector.Z.ToString( CultureInfo.InvariantCulture )}, {vector.W.ToString( CultureInfo.InvariantCulture )}]";
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
                var values = input.Trim( '[', ']' )
                                             .Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );


                var x = float.Parse( values[0], CultureInfo.InvariantCulture );
                var y = float.Parse( values[1], CultureInfo.InvariantCulture );
                var z = float.Parse( values[2], CultureInfo.InvariantCulture );
                var w = float.Parse( values[3], CultureInfo.InvariantCulture );
                return new Vector4( x, y, z, w );
            }
            else
            {
                return base.ConvertFrom( context, culture, value );
            }
        }
    }
}
