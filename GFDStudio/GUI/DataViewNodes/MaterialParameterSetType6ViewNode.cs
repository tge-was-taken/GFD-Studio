using GFDLibrary.Materials;
using System.Collections.Generic;
using System.Numerics;
using static GFDLibrary.Materials.MaterialParameterSetType6;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType6ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType6>
    {
        public MaterialParameterSetType6ViewNode( string text, MaterialParameterSetType6 data ) : base( text, data )
        {
        }
        public ListViewNode<MaterialParameterSetType6Field0ViewNode> P6_0 { get; set; }

        public float P6_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public float P6_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd4
        public float P6_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd8
        public float P6_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xdc
        public uint P6_5 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xe0

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }

        private List<MaterialParameterSetType6_Field0> GetType6Field0Nodes()
             => new List<MaterialParameterSetType6_Field0>() { Data.P6_0[0], Data.P6_0[1] };

        protected override void InitializeViewCore()
        {
            //(DataViewNode<List<MaterialParameterSetType6_Field0>>)DataViewNodeFactory.Create( "Field0", GetType6Field0Nodes() );
        }
    }

    public class MaterialParameterSetType6Field0ViewNode : DataViewNode<List<MaterialParameterSetType6_Field0>>
    {
        public MaterialParameterSetType6Field0ViewNode( string text, List<MaterialParameterSetType6_Field0> data ) : base( text, data )
        {
        }

        public Vector4 Field0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        }
        public float Field1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
