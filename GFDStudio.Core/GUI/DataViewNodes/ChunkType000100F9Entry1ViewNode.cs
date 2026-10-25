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
            set => SetDataProperty( value );
        }


        [Browsable( true )]
        [DisplayName( "Mass" )]
        public float Mass
        {
            get => Data.Mass;
            set => SetDataProperty( value );
        }

        [ Browsable( true ) ]
        [DisplayName( "Restoring force" )]
        public float RestoringForce
        {
            get => Data.RestoringForce;
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Wind rate" )]
        public float WindRate
        {
            get => Data.WindRate;
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