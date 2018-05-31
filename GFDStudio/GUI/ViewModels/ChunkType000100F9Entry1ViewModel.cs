using System.ComponentModel;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class ChunkType000100F9Entry1ViewModel : TreeNodeViewModel<ChunkType000100F9Entry1>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags =>
            TreeNodeViewModelFlags.Leaf;

        [Browsable( true )]
        public float Field34
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }


        [Browsable( true )]
        public float Field38
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [ Browsable( true ) ]
        public float Field3C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => GetModelProperty<float>();
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
        public float Field10
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        public ChunkType000100F9Entry1ViewModel( string text, ChunkType000100F9Entry1 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => NodeName = Name = Text;
        }
    }
}