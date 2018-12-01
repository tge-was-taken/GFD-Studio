using GFDLibrary;
using GFDLibrary.Misc;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F8ViewNode : ResourceViewNode<ChunkType000100F8>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Leaf;

        protected internal ChunkType000100F8ViewNode( string text, ChunkType000100F8 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<ChunkType000100F8>( ( path ) => Data.Save( path ) );
            RegisterReplaceHandler<ChunkType000100F8>( Resource.Load<ChunkType000100F8> );
        }
    }
}