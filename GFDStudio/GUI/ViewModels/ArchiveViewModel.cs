using System.IO;
using GFDLibrary;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.ViewModels
{
    public class ArchiveViewModel : TreeNodeViewModel<Archive>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Add |
            TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        protected internal ArchiveViewModel( string text, Archive resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Archive>( ( path ) => Model.Save( path ) );
            RegisterReplaceHandler<Archive>( ( path ) => new Archive( path ) );
            RegisterAddHandler<Stream>( ( path ) => Nodes.Add( TreeNodeViewModelFactory.Create( path ) ) );
            RegisterModelUpdateHandler( () =>
            {
                var builder = new ArchiveBuilder();

                foreach ( TreeNodeViewModel node in Nodes )
                {
                    builder.AddFile( node.Text, ModuleExportUtilities.CreateStream( node.Model ) );
                }

                return builder.Build();
            });
        }

        protected override void InitializeViewCore()
        {
            foreach ( var entryName in Model )
            {
                var node = TreeNodeViewModelFactory.Create( entryName, Model.OpenFile( entryName ) );
                Nodes.Add( node );
            }
        }
    }
}
