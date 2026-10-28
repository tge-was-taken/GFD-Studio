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
            set => Set( x => x.CapsuleHeight, value );
        }

        [Browsable( true )]
        [DisplayName( "Capsule radius" )]
        public float CapsuleRadius
        {
            get => Data.CapsuleRadius;
            set => Set( x => x.CapsuleRadius, value );
        }

        [Browsable( true )]
        [DisplayName( "Matrix" )]
        public Matrix4x4 Matrix
        {
            get => Data.Matrix;
            set => Set( x => x.Matrix, value );
        }

        [Browsable( true )]
        [DisplayName( "Capsule Type" )]
        public short CapsuleType
        {
            get => Data.CapsuleType;
            set => Set( x => x.CapsuleType, value );
        }

        [Browsable( true )]
        [DisplayName( "Node name" )]
        public string NodeName
        {
            get => Data.NodeName;
            set
            {
                Set( x => x.NodeName, value );
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