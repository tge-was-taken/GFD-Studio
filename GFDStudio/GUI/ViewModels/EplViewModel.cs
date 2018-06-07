using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class EplViewModel : TreeNodeViewModel<Epl>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public EplViewModel( string text, Epl resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Epl>( path => Model.Save( path ) );
            RegisterReplaceHandler<Epl>( Resource.Load<Epl> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}