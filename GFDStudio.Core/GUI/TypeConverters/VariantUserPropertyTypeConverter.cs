using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace GFDStudio.GUI.TypeConverters
{
    public class VariantUserPropertyTypeConverter : TypeConverter
    {
        private readonly string mKey;

        public VariantUserPropertyTypeConverter( string key )
        {
            mKey = key;
        }

        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return sourceType == typeof( string );
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            return false;
        }

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            var                 input    = ( string )value;
            VariantUserProperty property = null;

            if ( int.TryParse( input, out int intValue ) )
            {
                property = new VariantUserProperty( mKey, intValue );
            }
            else if ( float.TryParse( input, out float floatValue ) )
            {
                property = new VariantUserProperty( mKey, floatValue );
            }
            else if ( bool.TryParse( input, out bool boolValue ) )
            {
                property = new VariantUserProperty( mKey, boolValue );
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
                    property = new VariantUserProperty( mKey, new Vector3( vectorFloats[0], vectorFloats[1], vectorFloats[2] ) );
                }
                else if ( vectorFloats.Length == 4 )
                {
                    property = new VariantUserProperty(  mKey, new Vector4( vectorFloats[0], vectorFloats[1], vectorFloats[2], vectorFloats[3] ) );
                }
                else
                {
                    property = new VariantUserProperty( mKey, vectorFloats.Select( x => ( byte ) x ).ToArray() );
                }
            }

            if ( property == null )
            {
                property = new VariantUserProperty( mKey, input );
            }

            return property;
        }
    }
}