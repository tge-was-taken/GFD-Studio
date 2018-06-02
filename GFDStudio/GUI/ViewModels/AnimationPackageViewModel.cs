using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class AnimationPackageViewModel : ResourceViewModel<AnimationPackage>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags => 
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags => 
            TreeNodeViewModelFlags.Leaf;

        protected internal AnimationPackageViewModel( string text, AnimationPackage resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<AnimationPackage>( ( path ) => Resource.Save( Model, path ) );
            RegisterReplaceHandler<AnimationPackage>( Resource.Load<AnimationPackage> );
        }
    }
}