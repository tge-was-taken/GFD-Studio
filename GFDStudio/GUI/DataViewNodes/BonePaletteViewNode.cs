using System.ComponentModel;
using System.Numerics;
using GFDLibrary;

namespace GFDStudio.GUI.DataViewNodes
{
    public class BonePaletteViewNode : DataViewNode<BonePalette>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        [ Browsable( true ) ]
        public Matrix4x4[] InverseBindMatrices
        {
            get => Data.InverseBindMatrices;
        }

        [ Browsable( true ) ]
        public ushort[] BoneToNodeIndices
        {
            get => Data.BoneToNodeIndices;
        }

        public BonePaletteViewNode( string text, BonePalette data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}