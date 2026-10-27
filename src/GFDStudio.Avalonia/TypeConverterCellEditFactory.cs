using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Edits a property through the converter it declares. Without this the grid picks an editor
    /// from the property's type alone, so a field written as hex comes out in decimal.
    /// </summary>
    public sealed class TypeConverterCellEditFactory : AbstractCellEditFactory
    {
        // Ahead of the numeric and string factories, which would otherwise claim these first.
        public override int ImportPriority => base.ImportPriority + 2000;

        public override Control HandleNewProperty( PropertyCellContext context )
        {
            var converter = DeclaredConverter( context.Property );
            if ( converter == null )
                return null;

            var box = new TextBox { HorizontalAlignment = HorizontalAlignment.Stretch };

            box.LostFocus += ( s, e ) => Commit( context, box, converter );
            box.KeyDown += ( s, e ) =>
            {
                if ( e.Key == global::Avalonia.Input.Key.Enter )
                    Commit( context, box, converter );
            };

            return box;
        }

        public override bool HandlePropertyChanged( PropertyCellContext context )
        {
            var converter = DeclaredConverter( context.Property );
            if ( converter == null || context.CellEdit is not TextBox box )
                return false;

            box.Text = Format( converter, context.GetValue() );
            return true;
        }

        private void Commit( PropertyCellContext context, TextBox box, TypeConverter converter )
        {
            try
            {
                SetAndRaise( context, box, converter.ConvertFromString( null, CultureInfo.CurrentCulture, box.Text ?? string.Empty ) );
            }
            catch ( Exception )
            {
                // Put the value that is actually stored back, rather than leave the typo showing.
                box.Text = Format( converter, context.GetValue() );
            }
        }

        private static string Format( TypeConverter converter, object value )
        {
            try
            {
                return converter.ConvertToString( null, CultureInfo.CurrentCulture, value );
            }
            catch ( Exception )
            {
                return value?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// The converter the property names itself, or null when it leaves the choice to the grid.
        /// Enums are left alone so they keep their own editor.
        /// </summary>
        private static TypeConverter DeclaredConverter( PropertyDescriptor property )
        {
            if ( property == null || property.PropertyType.IsEnum )
                return null;

            if ( property.Attributes[typeof( TypeConverterAttribute )] is not TypeConverterAttribute attribute ||
                 string.IsNullOrEmpty( attribute.ConverterTypeName ) )
                return null;

            var converter = property.Converter;
            return converter != null &&
                   converter.CanConvertTo( typeof( string ) ) &&
                   converter.CanConvertFrom( typeof( string ) )
                ? converter
                : null;
        }
    }
}
