using System;
using System.IO;
using System.Windows.Forms;
using AtlusGfdEditor.Modules;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Adapters
{
    public class ArchiveAdapter : TreeNodeAdapter<Archive>
    {
        public override MenuFlags ContextMenuFlags =>
            MenuFlags.Export | MenuFlags.Replace | MenuFlags.Add |
            MenuFlags.Move | MenuFlags.Rename | MenuFlags.Delete;

        public override Flags NodeFlags => Flags.Branch;

        protected internal ArchiveAdapter( string text, Archive resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportAction<Archive>( ( path ) => Resource.Save( path ) );
            RegisterReplaceAction<Archive>( ( path ) => new Archive( path ) );
            RegisterAddAction<Stream>( ( path ) => Nodes.Add( TreeNodeAdapterFactory.Create( path ) ) );
            RegisterRebuildAction( () =>
            {
                ArchiveBuilder builder = new ArchiveBuilder();

                foreach ( TreeNodeAdapter node in Nodes )
                {
                    builder.AddFile( node.Text, ModuleExportUtillities.CreateStream( node.Resource ) );
                }

                return builder.Build();
            });
        }

        protected override void InitializeViewCore()
        {
            foreach ( var entryName in Resource )
            {
                var node = TreeNodeAdapterFactory.Create( entryName, Resource.OpenFile( entryName ) );
                Nodes.Add( node );
            }
        }
    }
}
