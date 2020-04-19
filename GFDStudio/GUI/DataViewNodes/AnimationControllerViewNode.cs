using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Animations;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationControllerViewNode : DataViewNode<AnimationController>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        [DisplayName( "Target kind" )]
        public TargetKind TargetKind
        {
            get => GetDataProperty<TargetKind>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Target ID" )]
        public int TargetId
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Target name" )]
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
            TextChanged += ( s, o ) => TargetName = Name = Text;
            RegisterExportHandler<AnimationController>( Data.Save );
            RegisterReplaceHandler<AnimationController>( Resource.Load<AnimationController> );
        }
    }
}