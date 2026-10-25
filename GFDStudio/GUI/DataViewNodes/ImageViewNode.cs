using GFDLibrary.Graphics;
using GFDLibrary.Textures;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ImageViewNode : DataViewNode<Image>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected internal ImageViewNode( string text, Image data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Image>( path => PngCodec.Save( Data, path ) );
            RegisterReplaceHandler<Image>( PngCodec.Load );
        }
    }
}
