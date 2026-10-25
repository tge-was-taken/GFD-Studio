using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// How enum values are written in the property grid.
    /// </summary>
    public static class EnumNames
    {
        /// <summary>
        /// The member's name as prose followed by its value, so the number stored in the file is
        /// visible alongside what it means.
        /// </summary>
        public static string Describe( Enum value )
        {
            var name = Describe( value.GetType(), value.ToString() );
            return $"{name}  ({ToNumber( value )})";
        }

        public static string Describe( Type enumType, string name )
        {
            var described = enumType.GetField( name )?
                                    .GetCustomAttributes( typeof( DescriptionAttribute ), false )
                                    .OfType<DescriptionAttribute>().FirstOrDefault();

            return described != null ? described.Description : Humanize( name );
        }

        public static IEnumerable<Enum> Members( Type enumType )
            => Enum.GetValues( enumType ).Cast<Enum>();

        public static long ToNumber( Enum value )
            => Convert.ToInt64( Convert.ChangeType( value, Enum.GetUnderlyingType( value.GetType() ) ) );

        /// <summary>
        /// Splits a name written in PascalCase into words, leaving runs of capitals alone so that
        /// things like UV and RGBA survive.
        /// </summary>
        public static string Humanize( string name )
        {
            if ( string.IsNullOrEmpty( name ) )
                return name;

            var text = new StringBuilder( name.Length + 8 );

            for ( var i = 0; i < name.Length; i++ )
            {
                var current = name[i];

                var startsWord = i > 0 &&
                                 ( char.IsUpper( current ) && !char.IsUpper( name[i - 1] ) ||
                                   char.IsUpper( current ) && i + 1 < name.Length && char.IsLower( name[i + 1] ) ||
                                   char.IsDigit( current ) && !char.IsDigit( name[i - 1] ) );

                if ( startsWord )
                    text.Append( ' ' );

                text.Append( current );
            }

            return text.ToString();
        }
    }
}
