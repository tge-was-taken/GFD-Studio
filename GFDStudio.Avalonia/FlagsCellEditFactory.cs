using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Shows a flags enum as the list of set members with a button to change them. The built in
    /// editor spells out every member as a checkbox, which fills the grid.
    /// </summary>
    public sealed class FlagsCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority + 2200;

        public override Control HandleNewProperty( PropertyCellContext context )
        {
            var type = EnumCellEditFactory.UnderlyingEnum( context.Property?.PropertyType );
            if ( type == null || !EnumCellEditFactory.IsFlags( type ) )
                return null;

            var text = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = global::Avalonia.Media.TextTrimming.CharacterEllipsis,
            };

            var edit = new Button { Content = "...", MinWidth = 32, Padding = new global::Avalonia.Thickness( 6, 0 ) };
            edit.Click += async ( s, e ) =>
            {
                var chosen = await FlagsWindow.Ask( edit, context.Property.DisplayName, type, context.GetValue() as Enum );
                if ( chosen != null )
                    SetAndRaise( context, edit, chosen );
            };

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions( "*,Auto" ), ColumnSpacing = 6 };
            Grid.SetColumn( text, 0 );
            Grid.SetColumn( edit, 1 );
            row.Children.Add( text );
            row.Children.Add( edit );

            return row;
        }

        public override bool HandlePropertyChanged( PropertyCellContext context )
        {
            var type = EnumCellEditFactory.UnderlyingEnum( context.Property?.PropertyType );
            if ( type == null || !EnumCellEditFactory.IsFlags( type ) ||
                 context.CellEdit is not Grid row || row.Children.FirstOrDefault() is not TextBlock text )
                return false;

            text.Text = Summarize( type, context.GetValue() as Enum );
            text.SetValue( ToolTip.TipProperty, text.Text );
            return true;
        }

        private static string Summarize( Type type, Enum value )
        {
            if ( value == null )
                return "(none)";

            var set = EnumNames.Members( type )
                               .Where( x => EnumNames.ToNumber( x ) != 0 && value.HasFlag( x ) )
                               .Select( x => EnumNames.Describe( type, x.ToString() ) )
                               .ToList();

            return set.Count == 0 ? "(none)" : string.Join( ", ", set );
        }
    }
}
