using System.ComponentModel;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class ChunkType000100F9Entry3ViewModel : TreeNodeViewModel<ChunkType000100F9Entry3>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags =>
            TreeNodeViewModelFlags.Leaf;

        [Browsable( true )]
        public float Field00
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field04
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field08
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field0C
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field0E
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        public ChunkType000100F9Entry3ViewModel( string text, ChunkType000100F9Entry3 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}