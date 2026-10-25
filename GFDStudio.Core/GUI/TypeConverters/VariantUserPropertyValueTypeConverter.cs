using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace GFDStudio.GUI.TypeConverters
{
    public class VariantUserPropertyValueTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return sourceType == typeof( string );
        }

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            var input = ( string )value;

            if ( int.TryParse( input, out int intValue ) )
            {
                return intValue;
            }
            else if ( float.TryParse( input, out float floatValue ) )
            {
                return floatValue;
            }
            else if ( bool.TryParse( input, out bool boolValue ) )
            {
                return boolValue;
            }
            else if ( input.Contains( "," ) )
            {
                var vectorContents = input.Trim( '[', ']' )
                                          .Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

                var vectorFloats = new float[vectorContents.Length];
                for ( int i = 0; i < vectorContents.Length; i++ )
                {
                    if ( !float.TryParse( vectorContents[i], out var vectorFloat ) )
                        break;

                    vectorFloats[i] = vectorFloat;
                }

                if ( vectorFloats.Length == 3 )
                {
                    return new Vector3( vectorFloats[0], vectorFloats[1], vectorFloats[2] );
                }
                else if ( vectorFloats.Length == 4 )
                {
                    return new Vector4( vectorFloats[0], vectorFloats[1], vectorFloats[2], vectorFloats[3] );
                }
                else
                {
                    return vectorFloats.Select( x => ( byte ) x ).ToArray();
                }
            }

            return input;
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            return destinationType == typeof( string );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            return value.ToString();
        }
    }
}