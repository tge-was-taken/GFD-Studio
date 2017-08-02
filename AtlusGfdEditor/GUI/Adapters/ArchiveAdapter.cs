using System;
using System.Windows.Forms;
using AtlusGfdEditor.Modules;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class ArchiveAdapter : TreeNodeAdapter<Archive>
    {
        protected internal ArchiveAdapter( string text, Archive resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            ContextMenuOptions = ContextMenuOptions.Export | ContextMenuOptions.Replace | ContextMenuOptions.Add | 
                                 ContextMenuOptions.Move | ContextMenuOptions.Rename | ContextMenuOptions.Delete;

            RegisterExportAction<Archive>( ( path ) => Resource.Save( path ) );
            RegisterReplaceAction<Archive>( ( path ) => Resource = new Archive( path ) );
            RegisterAddAction<Archive>( ( path ) => Nodes.Add( TreeNodeAdapterFactory.Create( path ) ) );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var entryName in Resource )
            {
                var node = TreeNodeAdapterFactory.Create( entryName, Resource.OpenFile( entryName ) );
                Nodes.Add( node );
            }
        }

        protected override Archive RebuildCore()
        {
            ArchiveBuilder builder = new ArchiveBuilder();

            foreach ( TreeNodeAdapter node in Nodes )
            {
                builder.AddFile( node.Text, ModuleExportUtillities.CreateStream( node.Resource ) );
            }

            return builder.Build();
        }
    }
}
