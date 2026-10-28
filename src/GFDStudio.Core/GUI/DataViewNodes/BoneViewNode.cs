using System.ComponentModel;
using System.Numerics;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class BoneViewNode : DataViewNode<Bone>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        [DisplayName( "Node index" )]
        public ushort NodeIndex
        {
            get => Data.NodeIndex;
            set => Set( x => x.NodeIndex, value );
        }

        [DisplayName( "Inverse bind matrix" )]
        public Matrix4x4 InverseBindMatrix
        {
            get => Data.InverseBindMatrix;
            set => Set( x => x.InverseBindMatrix, value );
        }

        public BoneViewNode( string text, Bone data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}