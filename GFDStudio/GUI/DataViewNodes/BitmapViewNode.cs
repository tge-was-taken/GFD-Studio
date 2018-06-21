using System.Drawing;

namespace GFDStudio.GUI.DataViewNodes
{
    public class BitmapViewNode : DataViewNode<Bitmap>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected internal BitmapViewNode( string text, Bitmap data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Bitmap>( ( path ) => Data.Save( path ) );
            RegisterReplaceHandler<Bitmap>( ( path ) => new Bitmap( path ) );
        }
    }
}
