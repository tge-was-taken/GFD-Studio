using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class ChunkType000100F8ViewModel : ResourceViewModel<ChunkType000100F8>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Move;

        public override TreeNodeViewModelFlags NodeFlags =>
            TreeNodeViewModelFlags.Leaf;

        protected internal ChunkType000100F8ViewModel( string text, ChunkType000100F8 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<ChunkType000100F8>( ( path ) => Model.Save( path ) );
            RegisterReplaceHandler<ChunkType000100F8>( Resource.Load<ChunkType000100F8> );
        }
    }
}