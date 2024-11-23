using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType7ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType7>
    {
        public MaterialParameterSetType7ViewNode( string text, MaterialParameterSetType7 data ) : base( text, data )
        {
        }

        public float P7_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf0
        public uint P7_2 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xf4

        [Browsable( false )]
        public List<DataViewNode> Layers { get; set; }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected override void InitializeCore()
        {
            Layers.Clear();
            for ( int i = 0; i < Data.P7_0.Length; i++ )
                Layers.Add( DataViewNodeFactory.Create( $"Layer {i}", Data.P7_0[i] ) );
            foreach ( var layer in Layers )
                AddChildNode( layer );
        }
    }

    public class MaterialParameterSetType7Field0ViewNode : DataViewNode<MaterialParameterSetType7.MaterialParameterSetType7_Field0>
    {
        public MaterialParameterSetType7Field0ViewNode( string text, MaterialParameterSetType7.MaterialParameterSetType7_Field0 data ) : base( text, data ) { }
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

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
