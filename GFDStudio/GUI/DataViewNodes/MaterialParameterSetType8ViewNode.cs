using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType8ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType8>
    {
        public MaterialParameterSetType8ViewNode( string text, MaterialParameterSetType8 data ) : base( text, data )
        {
        }

        public float P8_0 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P8_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x94
        public float P8_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x98
        public float P8_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x9c
        public Vector4 P8_4 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P8_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public float P8_6 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb4
        public float P8_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb8
        public float P8_8 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xbc
        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
