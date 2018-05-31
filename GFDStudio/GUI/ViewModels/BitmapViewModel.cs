using System.Drawing;

namespace GFDStudio.GUI.ViewModels
{
    public class BitmapViewModel : TreeNodeViewModel<Bitmap>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Leaf;

        protected internal BitmapViewModel( string text, Bitmap resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Bitmap>( ( path ) => Model.Save( path ) );
            RegisterReplaceHandler<Bitmap>( ( path ) => new Bitmap( path ) );
        }
    }
}
