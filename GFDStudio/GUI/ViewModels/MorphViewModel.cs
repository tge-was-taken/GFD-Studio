using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class MorphViewModel : TreeNodeViewModel<Morph>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public int TargetCount => Model.TargetCount;

        public int[] TargetInts
        {
            get => Model.TargetInts;
            set => SetModelProperty( value );
        }

        public string MaterialName
        {
            get => Model.MaterialName;
            set => SetModelProperty( value );
        }

        public MorphViewModel( string text, Morph resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Morph>( path => Model.Save( path ) );
            RegisterReplaceHandler<Morph>( Resource.Load<Morph> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}