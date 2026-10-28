using GFDLibrary.Materials;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType15ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType15>
    {
        public MaterialParameterSetType15ViewNode( string text, MaterialParameterSetType15 data ) : base( text, data )
        {
        }

        public uint P15_LayerCount { 
            get => Data.P15_LayerCount; 
            set => Set( x => x.P15_LayerCount, value ); 
        } // 0x2d0
        public float P15_TriPlanarScale { 
            get => Data.P15_TriPlanarScale; 
            set => Set( x => x.P15_TriPlanarScale, value ); 
        } // 0x2d4
        public MaterialParameterSetType15.Type15Flags Flags { 
            get => Data.Flags; 
            set => Set( x => x.Flags, value ); 
        } // 0x2d8

        [Browsable( false )]
        public List<DataViewNode> Layers { get; set; }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected override void InitializeCore()
        {
            Layers.Clear();
            for ( int i = 0; i < Data.P15_0.Length; i++ )
                Layers.Add( DataViewNodeFactory.Create( $"Layer {i}", Data.P15_0[i] ) );
            foreach ( var layer in Layers )
                AddChildNode( layer );
        }
    }

    public class MaterialParameterSetType15Field0ViewNode : DataViewNode<MaterialParameterSetType15.MaterialParameterSetType15Layer>
    {
        public MaterialParameterSetType15Field0ViewNode( string text, MaterialParameterSetType15.MaterialParameterSetType15Layer data ) : base( text, data ) { }
        public float Field0
        {
            get => Data.Field0;
            set => Set( x => x.Field0, value );
        }
        public float Field1
        {
            get => Data.Field1;
            set => Set( x => x.Field1, value );
        }
        public float Field2
        {
            get => Data.Field2;
            set => Set( x => x.Field2, value );
        }
        public float Field3
        {
            get => Data.Field3;
            set => Set( x => x.Field3, value );
        }
        public float Field4
        {
            get => Data.Field4;
            set => Set( x => x.Field4, value );
        }
        public float Field5
        {
            get => Data.Field5;
            set => Set( x => x.Field5, value );
        }
        public Vector3 Field6
        {
            get => Data.Field6;
            set => Set( x => x.Field6, value );
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
