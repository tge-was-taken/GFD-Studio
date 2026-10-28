using GFDLibrary.Misc;
using System.ComponentModel;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F9Entry3ViewNode : DataViewNode<ChunkType000100F9Entry3>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Leaf;

        [Browsable( true )]
        [DisplayName( "Length (Squared)" )]
        public float LengthSq
        {
            get => Data.LengthSq;
            set => Set( x => x.LengthSq, value );
        }

        [Browsable( true )]
        [DisplayName( "Angular Limit" )]
        public float AngularLimit
        {
            get => Data.AngularLimit;
            set => Set( x => x.AngularLimit, value );
        }

        [Browsable( true )]
        [DisplayName( "Chain thickness" )]
        public float ChainThickness
        {
            get => Data.ChainThickness;
            set => Set( x => x.ChainThickness, value );
        }

        [Browsable( true )]
        [DisplayName( "Parent bone" )]
        public short ParentBoneIndex
        {
            get => Data.ParentBoneIndex;
            set => Set( x => x.ParentBoneIndex, value );
        }

        [Browsable( true )]
        [DisplayName( "Child bone" )]
        public short ChildBoneIndex
        {
            get => Data.ChildBoneIndex;
            set => Set( x => x.ChildBoneIndex, value );
        }

        public ChunkType000100F9Entry3ViewNode( string text, ChunkType000100F9Entry3 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}