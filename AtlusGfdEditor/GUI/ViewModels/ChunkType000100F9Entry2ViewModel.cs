using System.ComponentModel;
using System.Numerics;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class ChunkType000100F9Entry2ViewModel : TreeNodeViewModel<ChunkType000100F9Entry2>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags =>
            TreeNodeViewModelFlags.Leaf;


        [Browsable( true )]
        public float Field88
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public Matrix4x4 Field8C
        {
            get => GetModelProperty<Matrix4x4>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public short Field94
        {
            get => GetModelProperty<short>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public string NodeName
        {
            get => GetModelProperty<string>();
            set
            {
                SetModelProperty( value );
                Text = value;
            }
        }

        public ChunkType000100F9Entry2ViewModel( string text, ChunkType000100F9Entry2 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => NodeName = Name = Text;
        }
    }
}