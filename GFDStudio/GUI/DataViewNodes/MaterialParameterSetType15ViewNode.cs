using GFDLibrary.Materials;
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

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
