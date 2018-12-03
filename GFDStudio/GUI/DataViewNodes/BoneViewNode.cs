using System.Numerics;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class BoneViewNode : DataViewNode<Bone>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        public ushort NodeIndex
        {
            get => GetDataProperty<ushort>();
            set => SetDataProperty( value );
        }

        public Matrix4x4 InverseBindMatrix
        {
            get => GetDataProperty<Matrix4x4>();
            set => SetDataProperty( value );
        }

        public BoneViewNode( string text, Bone data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}