using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType5ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType5>
    {
        public MaterialParameterSetType5ViewNode( string text, MaterialParameterSetType5 data ) : base( text, data )
        {
        }

        public float P5_0 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P5_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x94
        public float P5_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x98
        public float P5_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x9c
        public float P5_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P5_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa4
        public float P5_6 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa8
        public float P5_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xac
        public float P5_8 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public float P5_9 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb4
        public float P5_10 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb8
        public float P5_11 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xbc
        public float P5_12 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float P5_13 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc4
        public uint P5_14 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xc8

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
