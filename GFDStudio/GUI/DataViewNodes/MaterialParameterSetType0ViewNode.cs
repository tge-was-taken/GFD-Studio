using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType0ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType0>
    {
        public MaterialParameterSetType0ViewNode( string text, MaterialParameterSetType0 data ) : base( text, data )
        {
        }

        public Vector4 P0_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P0_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P0_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa4
        public float P0_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa8
        public float P0_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xac
        public float P0_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public uint P0_6 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xb4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
