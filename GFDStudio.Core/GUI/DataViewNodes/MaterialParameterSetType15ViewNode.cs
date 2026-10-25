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
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0x2d0
        public float P15_TriPlanarScale { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x2d4
        public uint P15_LerpBlendRate { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
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
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public float Field1
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public float Field2
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public float Field3
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public float Field4
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public float Field5
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }
        public Vector3 Field6
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
