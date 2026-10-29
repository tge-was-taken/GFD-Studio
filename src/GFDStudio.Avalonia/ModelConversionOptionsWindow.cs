using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDStudio.GUI;
using PropertyGridControl = Avalonia.PropertyGrid.Controls.PropertyGrid;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Asks how to convert an imported scene: the global settings, plus per material and per geometry
    /// overrides picked from a tree.
    /// </summary>
    public sealed class ModelConversionOptionsWindow : Window
    {
        private readonly ModelConverterOptionsViewModel mViewModel;
        private readonly PropertyGridControl mItemProperties = new() { IsTitleVisible = false };

        private ModelConversionOptionsWindow( Assimp.Scene scene, ModelPack originalModel )
        {
            mViewModel = ModelConverterOptionsBuilder.Build( scene, originalModel );

            Title = "Mesh conversion options";
            Width = 980;
            Height = 640;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var tree = new TreeView
            {
                ItemsSource = BuildGroups(),
                ItemTemplate = new FuncTreeDataTemplate<OptionsGroup>(
                    ( group, scope ) => new TextBlock { Text = group.Name },
                    group => group.Items ),
            };

            tree.SelectionChanged += ( s, e ) =>
            {
                if ( tree.SelectedItem is OptionsGroup { Target: not null } group )
                    mItemProperties.DataContext = group.Target;
            };

            var ok = new Button { Content = "OK", MinWidth = 80, IsDefault = true };
            ok.Click += ( s, e ) => Close( mViewModel.MapToModelConverterOptions() );

            var cancel = new Button { Content = "Cancel", MinWidth = 80, IsCancel = true };
            cancel.Click += ( s, e ) => Close( null );

            var panes = new Grid { ColumnDefinitions = new ColumnDefinitions( "250,4,*,4,*" ) };
            Add( panes, tree, 0 );
            Add( panes, new GridSplitter { ResizeDirection = GridResizeDirection.Columns }, 1 );
            Add( panes, new PropertyGridControl { DataContext = mViewModel, IsTitleVisible = false }, 2 );
            Add( panes, new GridSplitter { ResizeDirection = GridResizeDirection.Columns }, 3 );
            Add( panes, mItemProperties, 4 );

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8,
                Margin = new Thickness( 12 ),
                Children = { ok, cancel },
            };

            var root = new DockPanel();
            DockPanel.SetDock( buttons, Dock.Bottom );
            root.Children.Add( buttons );
            root.Children.Add( panes );
            Content = root;
        }

        public static Task<ModelConverterOptions> Show( Window owner, Assimp.Scene scene, ModelPack originalModel )
            => new ModelConversionOptionsWindow( scene, originalModel ).ShowDialog<ModelConverterOptions>( owner );

        private List<OptionsGroup> BuildGroups() =>
        [
            new OptionsGroup( "Materials", null, mViewModel.Materials.Select( x => new OptionsGroup( x.Name, x, [] ) ).ToList() ),
            new OptionsGroup( "Geometries", null, mViewModel.Geometries.Select( x => new OptionsGroup( x.Name, x, [] ) ).ToList() ),
        ];

        private static void Add( Grid grid, Control control, int column )
        {
            Grid.SetColumn( control, column );
            grid.Children.Add( control );
        }

        private sealed record OptionsGroup( string Name, object Target, List<OptionsGroup> Items );
    }
}
