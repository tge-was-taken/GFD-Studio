using GFDLibrary.Animations;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationControllerViewNode : DataViewNode<AnimationController>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => 0;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        public TargetKind TargetKind
        {
            get => GetDataProperty<TargetKind>();
            set => SetDataProperty( value );
        }

        public int TargetId
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        public string TargetName
        {
            get => GetDataProperty<string>();
            set => SetDataProperty( value );
        }

        public AnimationControllerViewNode( string text, AnimationController data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}