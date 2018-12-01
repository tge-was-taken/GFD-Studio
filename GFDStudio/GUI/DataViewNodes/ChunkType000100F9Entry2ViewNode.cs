using System.ComponentModel;
using System.Numerics;
using GFDLibrary.Misc;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F9Entry2ViewNode : DataViewNode<ChunkType000100F9Entry2>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Leaf;


        [Browsable( true )]
        public float Field88
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public Matrix4x4 Field8C
        {
            get => GetDataProperty<Matrix4x4>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public short Field94
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        public string NodeName
        {
            get => GetDataProperty<string>();
            set
            {
                SetDataProperty( value );
                Text = value;
            }
        }

        public ChunkType000100F9Entry2ViewNode( string text, ChunkType000100F9Entry2 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => NodeName = Name = Text;
        }
    }
}