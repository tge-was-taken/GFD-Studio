using System.ComponentModel;
using System.Numerics;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class MatrixPaletteViewModel : TreeNodeViewModel<MatrixPalette>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        [ Browsable( true ) ]
        public Matrix4x4[] InverseBindMatrices
        {
            get => Model.InverseBindMatrices;
        }

        [ Browsable( true ) ]
        public ushort[] BoneToNodeIndices
        {
            get => Model.BoneToNodeIndices;
        }

        public MatrixPaletteViewModel( string text, MatrixPalette resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}