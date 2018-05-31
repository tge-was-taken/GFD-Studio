using System.IO;

namespace GFDStudio.GUI.ViewModels
{
    public class StreamViewModel : TreeNodeViewModel<Stream>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move |
            TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Leaf;

        protected internal StreamViewModel( string text, Stream resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( ( path ) =>
            {
                using ( var fileStream = File.Create( path ) )
                {
                    Model.Position = 0;
                    Model.CopyTo( fileStream );
                }
            } );

            RegisterReplaceHandler<Stream>( File.OpenRead );
        }
    }
}
