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
        [DisplayName( "Sphere Radius" )]
        public float SphereRadius
        {
            get => Data.SphereRadius;
            set => Set( x => x.SphereRadius, value );
        }


        [Browsable( true )]
        [DisplayName( "Mass" )]
        public float Mass
        {
            get => Data.Mass;
            set => Set( x => x.Mass, value );
        }

        [ Browsable( true ) ]
        [DisplayName( "Restoring force" )]
        public float RestoringForce
        {
            get => Data.RestoringForce;
            set => Set( x => x.RestoringForce, value );
        }

        [Browsable( true )]
        [DisplayName( "Wind rate" )]
        public float WindRate
        {
            get => Data.WindRate;
            set => Set( x => x.WindRate, value );
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

        [Browsable( true )]
        public float Field04
        {
            get => Data.Field04;
            set => Set( x => x.Field04, value );
        }


        [Browsable( true )]
        public float Field08
        {
            get => Data.Field08;
            set => Set( x => x.Field08, value );
        }

        [Browsable( true )]
        public float Field10
        {
            get => Data.Field10;
            set => Set( x => x.Field10, value );
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