using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// Shows a <see cref="DataViewNode"/> tree and is the view the nodes talk back to.
    /// </summary>
    public sealed class DataViewNodeTree : TreeView, IDataViewNodeView
    {
        private static readonly Dictionary<string, Bitmap> sIcons = new();

        private DataViewNode mRoot;

        public DataViewNodeTree()
        {
            ItemTemplate = new FuncTreeDataTemplate<DataViewNode>( BuildRow, ChildrenOf );

            // The node owns whether it is expanded, so that rebuilding the items keeps the shape
            // the user had opened up.
            Styles.Add( new Style( x => x.OfType<TreeViewItem>() )
            {
                Setters =
                {
                    new Setter( TreeViewItem.IsExpandedProperty,
                                new Binding( nameof( DataViewNode.IsExpanded ) ) { Mode = BindingMode.TwoWay } ),
                },
            } );
        }

        protected override Type StyleKeyOverride => typeof( TreeView );

        /// <summary>
        /// A shortcut shown in a context menu is only a hint in Avalonia, so the keys are matched
        /// against the selected node's menu here.
        /// </summary>
        protected override void OnKeyDown( KeyEventArgs e )
        {
            base.OnKeyDown( e );

            if ( e.Handled || SelectedNode?.MenuItems == null )
                return;

            var action = FindShortcut( SelectedNode.MenuItems, e.Key, e.KeyModifiers );
            if ( action == null )
                return;

            e.Handled = true;
            action();
        }

        private static Action FindShortcut( IEnumerable<DataViewNodeMenuItem> items, Key key, KeyModifiers modifiers )
        {
            foreach ( var item in items )
            {
                if ( item.Action != null && !item.Shortcut.IsNone && Matches( item.Shortcut, key, modifiers ) )
                    return item.Action;

                var nested = FindShortcut( item.Items, key, modifiers );
                if ( nested != null )
                    return nested;
            }

            return null;
        }

        private static bool Matches( DataViewNodeMenuShortcut shortcut, Key key, KeyModifiers modifiers )
            => DataViewNodeMenuAvalonia.ToKey( shortcut.Key ) == key &&
               DataViewNodeMenuAvalonia.ToModifiers( shortcut.Modifiers ) == modifiers;

        public DataViewNode SelectedNode => SelectedItem as DataViewNode;

        public event EventHandler<DataViewNode> NodeSelected;

        public void Load( DataViewNode root )
        {
            mRoot = root;
            root.AttachView( this );
            root.InitializeView();
            ItemsSource = new[] { root };
        }

        public void Clear()
        {
            mRoot = null;
            ItemsSource = Array.Empty<DataViewNode>();
        }

        //
        // IDataViewNodeView
        //

        public void Select( DataViewNode node ) => SelectedItem = node;

        public void RefreshSelection() => NodeSelected?.Invoke( this, SelectedNode );

        public void ExpandAll() => ExpandSubTree( mRoot );

        public void EnsureVisible( DataViewNode node )
        {
            for ( var parent = node?.Parent; parent != null; parent = parent.Parent )
                parent.IsExpanded = true;

            RebuildItems();
        }

        public void BeginRename( DataViewNode node ) => RenameWindow.Show( this, node );

        public void NotifyUserPropertyChanged( DataViewNode node, string propertyName )
        {
            if ( node == SelectedNode )
                RefreshSelection();
        }

        public void OnChildrenChanged( DataViewNode node )
        {
            // The collection raises its own change notification, so the bound items keep up on
            // their own. Only a node that is not in the tree yet needs the list rebuilt.
            if ( node == mRoot )
                RebuildItems();
        }

        private void RebuildItems()
        {
            if ( mRoot == null )
                return;

            ItemsSource = null;
            ItemsSource = new[] { mRoot };
        }

        private static void ExpandSubTree( DataViewNode node )
        {
            if ( node == null )
                return;

            node.InitializeView();
            node.IsExpanded = true;

            foreach ( var child in node.Nodes )
                ExpandSubTree( child );
        }

        /// <summary>
        /// Asks a node to fill itself in before its children are read, which is what makes the
        /// tree load a level at a time.
        /// </summary>
        private static IEnumerable ChildrenOf( DataViewNode node )
        {
            node.InitializeView();
            return node.Nodes;
        }

        private static Control BuildRow( DataViewNode node, INameScope scope )
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };

            var icon = LoadIcon( node.ImageKey );
            if ( icon != null )
            {
                row.Children.Add( new Image
                {
                    Source = icon,
                    Width = 16,
                    Height = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                } );
            }

            var text = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            text.Bind( TextBlock.TextProperty, new Binding( nameof( DataViewNode.Text ) ) );
            row.Children.Add( text );

            row.ContextMenu = node.MenuItems.ToContextMenu();

            return row;
        }

        private static Bitmap LoadIcon( string path )
        {
            if ( string.IsNullOrWhiteSpace( path ) )
                return null;

            if ( sIcons.TryGetValue( path, out var cached ) )
                return cached;

            Bitmap bitmap = null;
            try
            {
                if ( File.Exists( path ) )
                    bitmap = new Bitmap( path );
            }
            catch ( Exception )
            {
                // An icon that will not load is not worth failing the tree over.
            }

            sIcons[path] = bitmap;
            return bitmap;
        }

    }
}
