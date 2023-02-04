using System.ComponentModel;
using GFDLibrary.Misc;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F9Entry1ViewNode : DataViewNode<ChunkType000100F9Entry1>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Leaf;

        [Browsable( true )]
        [DisplayName( "Motion scale" )]
        public float Field34
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }


        [Browsable( true )]
        public float Field38
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [ Browsable( true ) ]
        public float Field3C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Spring rate" )]
        public float Field40
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Node name" )]
        public string NodeName
        {
            get => GetDataProperty<string>();
            set
            {
                SetDataProperty( value );
                Text = value;
            }
        }

        [Browsable( true )]
        public float Field04
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }


        [Browsable( true )]
        public float Field08
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field10
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        public ChunkType000100F9Entry1ViewNode( string text, ChunkType000100F9Entry1 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => NodeName = Name = Text;
        }
    }
}