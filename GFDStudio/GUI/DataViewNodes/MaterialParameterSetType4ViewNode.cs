using GFDLibrary.Materials;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType4ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType4>
    {
        public MaterialParameterSetType4ViewNode( string text, MaterialParameterSetType4 data ) : base( text, data )
        {
        }

        public Vector4 P4_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public Vector4 P4_1 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P4_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public float P4_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb4
        public float P4_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb8
        public uint P4_5 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xcc
        public float P4_6 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xbc
        public float P4_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float P4_8 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
