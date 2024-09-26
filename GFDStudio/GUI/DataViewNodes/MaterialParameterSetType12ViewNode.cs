using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType12ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType12>
    {
        public MaterialParameterSetType12ViewNode( string text, MaterialParameterSetType12 data ) : base( text, data )
        {
        }

        public Vector4 P12_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public Vector4 P12_1 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public Vector4 P12_2 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float P12_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe8
        public float P12_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf4
        public float P12_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf8
        public uint P12_6 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0x12c
        public float P12_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x108
        public Vector3 P12_8 { 
            get => GetDataProperty<Vector3>(); 
            set => SetDataProperty(value); 
        } // 0x110
        public float P12_9 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf0
        public float P12_10 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xfc
        public float P12_11 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x11c
        public float P12_12 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x120
        public float P12_13 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x124
        public float P12_14 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x10c
        public Vector3 P12_15 { 
            get => GetDataProperty<Vector3>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public float P12_16 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xdc
        public float P12_17 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe0
        public float P12_18 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xec
        public float P12_19 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe4
        public Vector4 P12_20 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P12_21 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x100
        public float P12_22 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x104

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
