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
        [DisplayName( "Capsule height" )]
        public float CapsuleHeight
        {
            get => Data.CapsuleHeight;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Capsule radius" )]
        public float CapsuleRadius
        {
            get => Data.CapsuleRadius;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Matrix" )]
        public Matrix4x4 Matrix
        {
            get => Data.Matrix;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Capsule Type" )]
        public short CapsuleType
        {
            get => Data.CapsuleType;
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

        public ChunkType000100F9Entry2ViewNode( string text, ChunkType000100F9Entry2 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => NodeName = Name = Text;
        }
    }
}