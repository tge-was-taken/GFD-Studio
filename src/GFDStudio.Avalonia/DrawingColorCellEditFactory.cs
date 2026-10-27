using System.Drawing;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Edits the view nodes' <see cref="Color"/> properties. The property grid only knows
    /// Avalonia's own colour type, and would leave these out of the list entirely.
    /// </summary>
    public sealed class DrawingColorCellEditFactory : AbstractCellEditFactory
    {
        // Ahead of the built in factories, which would otherwise claim the property first.
        public override int ImportPriority => base.ImportPriority + 1000;

        public override Control HandleNewProperty( PropertyCellContext context )
        {
            if ( context.Property.PropertyType != typeof( Color ) )
                return null;

            var picker = new PreviewableColorPicker { HorizontalAlignment = HorizontalAlignment.Stretch };
            picker.ColorChanged += ( s, e ) => SetAndRaise( context, picker, ToDrawing( picker.Color ) );

            return picker;
        }

        public override bool HandlePropertyChanged( PropertyCellContext context )
        {
            if ( context.Property.PropertyType != typeof( Color ) || context.CellEdit is not PreviewableColorPicker picker )
                return false;

            picker.Color = ToAvalonia( (Color)context.GetValue() );
            return true;
        }

        private static Color ToDrawing( global::Avalonia.Media.Color value )
            => Color.FromArgb( value.A, value.R, value.G, value.B );

        private static global::Avalonia.Media.Color ToAvalonia( Color value )
            => global::Avalonia.Media.Color.FromArgb( value.A, value.R, value.G, value.B );
    }
}
