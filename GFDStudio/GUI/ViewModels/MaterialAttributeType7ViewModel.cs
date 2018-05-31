using System.IO;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class MaterialAttributeType7ViewModel : MaterialAttributeViewModel<MaterialAttributeType7>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public MaterialAttributeType7ViewModel( string text, MaterialAttributeType7 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType7> );
        }
    }
}