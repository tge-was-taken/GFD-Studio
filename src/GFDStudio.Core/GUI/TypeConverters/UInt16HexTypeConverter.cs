using System;
using System.ComponentModel;

namespace GFDStudio.GUI.TypeConverters
{
    public class UInt16HexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            return destinationType == typeof( string ) || base.CanConvertTo( context, destinationType );
        }

        public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
        {
            if ( destinationType == typeof( string ) && value is ushort )
                return $"0x{value:X4}";

            return base.ConvertTo( context, culture, value, destinationType );
        }

        public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
        {
            if ( value is string input )
            {
                if ( input.StartsWith( "0x", StringComparison.OrdinalIgnoreCase ) )
                    input = input.Substring( 2 );

                return ushort.Parse( input, System.Globalization.NumberStyles.HexNumber, culture );
            }

            return base.ConvertFrom( context, culture, value );
        }
    }
}
