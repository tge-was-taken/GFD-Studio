using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Animations;

namespace GFDStudio.GUI.DataViewNodes
{
    public class AnimationJointViewNode : DataViewNode<AnimationJoint>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags => DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        [DisplayName( "Target kind" )]
        public TargetKind TargetKind
        {
            get => Data.TargetKind;
            set => Set( x => x.TargetKind, value );
        }

        [DisplayName( "Target ID" )]
        public int TargetId
        {
            get => Data.TargetId;
            set => Set( x => x.TargetId, value );
        }

        [DisplayName( "Target name" )]
        public string TargetName
        {
            get => Data.TargetName;
            set => Set( x => x.TargetName, value );
        }

        public AnimationJointViewNode( string text, AnimationJoint data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => TargetName = Name = Text;
            RegisterExportHandler<AnimationJoint>( Data.Save );
            RegisterReplaceHandler<AnimationJoint>( Resource.Load<AnimationJoint> );
        }
    }
}