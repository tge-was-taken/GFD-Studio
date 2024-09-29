using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType1ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType1>
    {
        public MaterialParameterSetType1ViewNode( string text, MaterialParameterSetType1 data ) : base( text, data )
        {
        }

        public Vector4 AmbientColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public Vector4 DiffuseColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public Vector4 SpecularColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        public Vector4 EmissiveColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float Reflectivity { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public float LerpBlendRate { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
