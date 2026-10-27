using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Edits a colour held as four floats. The swatch covers the usual case and the text keeps
    /// the exact values, which matter because these fields are not always within nought to one:
    /// a specular alpha, for instance, carries a power.
    /// </summary>
    public sealed class Vector4ColorCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority + 2300;

        public override Control HandleNewProperty( PropertyCellContext context )
        {
            if ( !IsColor( context.Property ) )
                return null;

            var picker = new PreviewableColorPicker { Width = 64, VerticalAlignment = VerticalAlignment.Center };
            var text = new TextBox { HorizontalAlignment = HorizontalAlignment.Stretch };

            picker.ColorChanged += ( s, e ) =>
            {
                var current = Current( context );
                var chosen = picker.Color;

                // The picker only reaches nought to one, so a value beyond that is left as it was.
                var value = new Vector4(
                    chosen.R / 255f, chosen.G / 255f, chosen.B / 255f,
                    current.W > 1f ? current.W : chosen.A / 255f );

                text.Text = Format( context.Property, value );
                SetAndRaise( context, picker, value );
            };

            text.LostFocus += ( s, e ) => Commit( context, text, picker );
            text.KeyDown += ( s, e ) =>
            {
                if ( e.Key == global::Avalonia.Input.Key.Enter )
                    Commit( context, text, picker );
            };

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions( "Auto,*" ), ColumnSpacing = 6 };
            Grid.SetColumn( picker, 0 );
            Grid.SetColumn( text, 1 );
            row.Children.Add( picker );
            row.Children.Add( text );

            return row;
        }

        public override bool HandlePropertyChanged( PropertyCellContext context )
        {
            if ( !IsColor( context.Property ) || context.CellEdit is not Grid row )
                return false;

            var value = Current( context );

            foreach ( var child in row.Children )
            {
                switch ( child )
                {
                    case PreviewableColorPicker picker:
                        picker.Color = ToAvalonia( value );
                        break;

                    case TextBox text:
                        text.Text = Format( context.Property, value );
                        break;
                }
            }

            return true;
        }

        private void Commit( PropertyCellContext context, TextBox text, PreviewableColorPicker picker )
        {
            try
            {
                var value = context.Property.Converter.ConvertFromString( null, CultureInfo.CurrentCulture, text.Text ?? string.Empty );
                picker.Color = ToAvalonia( (Vector4)value );
                SetAndRaise( context, text, value );
            }
            catch ( Exception )
            {
                text.Text = Format( context.Property, Current( context ) );
            }
        }

        private static Vector4 Current( PropertyCellContext context )
            => context.GetValue() is Vector4 value ? value : default;

        private static string Format( PropertyDescriptor property, Vector4 value )
        {
            try
            {
                return property.Converter.ConvertToString( null, CultureInfo.CurrentCulture, value );
            }
            catch ( Exception )
            {
                return value.ToString();
            }
        }

        private static global::Avalonia.Media.Color ToAvalonia( Vector4 value )
            => global::Avalonia.Media.Color.FromArgb( Channel( value.W ), Channel( value.X ), Channel( value.Y ), Channel( value.Z ) );

        private static byte Channel( float value )
            => (byte)Math.Clamp( (int)MathF.Round( value * 255f ), 0, 255 );

        /// <summary>
        /// A four float property counts as a colour when it is named like one.
        /// </summary>
        private static bool IsColor( PropertyDescriptor property )
            => property != null &&
               property.PropertyType == typeof( Vector4 ) &&
               ( property.Name.Contains( "Color", StringComparison.OrdinalIgnoreCase ) ||
                 property.DisplayName.Contains( "color", StringComparison.OrdinalIgnoreCase ) );
    }
}
