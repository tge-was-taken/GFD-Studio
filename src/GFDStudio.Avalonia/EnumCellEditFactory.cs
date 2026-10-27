using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Lists a plain enum's members by name and value. The built in editor shows the name alone,
    /// which hides the number that is actually written to the file.
    /// </summary>
    public sealed class EnumCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority + 2100;

        public override Control HandleNewProperty( PropertyCellContext context )
        {
            var type = UnderlyingEnum( context.Property?.PropertyType );
            if ( type == null || IsFlags( type ) )
                return null;

            var box = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ItemsSource = EnumNames.Members( type ).Select( x => new Entry( x ) ).ToList(),
            };

            box.SelectionChanged += ( s, e ) =>
            {
                if ( box.SelectedItem is Entry entry )
                    SetAndRaise( context, box, entry.Value );
            };

            return box;
        }

        public override bool HandlePropertyChanged( PropertyCellContext context )
        {
            var type = UnderlyingEnum( context.Property?.PropertyType );
            if ( type == null || IsFlags( type ) || context.CellEdit is not ComboBox box )
                return false;

            var current = context.GetValue();
            box.SelectedItem = box.ItemsSource?.OfType<Entry>()
                                  .FirstOrDefault( x => Equals( x.Value, current ) );

            return true;
        }

        internal static Type UnderlyingEnum( Type type )
        {
            if ( type == null )
                return null;

            var bare = Nullable.GetUnderlyingType( type ) ?? type;
            return bare.IsEnum ? bare : null;
        }

        internal static bool IsFlags( Type enumType )
            => enumType.GetCustomAttributes( typeof( FlagsAttribute ), false ).Length > 0;

        private sealed record Entry( Enum Value )
        {
            public override string ToString() => EnumNames.Describe( Value );
        }
    }
}
