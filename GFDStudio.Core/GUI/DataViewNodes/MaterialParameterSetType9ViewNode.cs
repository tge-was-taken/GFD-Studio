using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType9ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType9>
    {
        public MaterialParameterSetType9ViewNode( string text, MaterialParameterSetType9 data ) : base( text, data )
        {
        }

        public float P9_0 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P9_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x94
        public float P9_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x98
        public float P9_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x9c
        public Vector4 P9_4 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public Vector4 P9_5 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public Vector4 P9_6 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public Vector4 P9_7 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public Vector3 P9_8 { 
            get => GetDataProperty<Vector3>(); 
            set => SetDataProperty(value); 
        } // 0xe0
        public float P9_9 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xec
        public float P9_10 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf0
        public float P9_11 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf4
        public float P9_12 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf8
        public float P9_13 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xfc
        public float P9_14 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x100
        public float P9_15 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x104
        public float P9_16 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x108
        public uint P9_17 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0x10c

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
