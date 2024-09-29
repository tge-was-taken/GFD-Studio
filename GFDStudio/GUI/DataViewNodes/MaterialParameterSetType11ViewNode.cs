using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType11ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType11>
    {
        public MaterialParameterSetType11ViewNode( string text, MaterialParameterSetType11 data ) : base( text, data )
        {
        }

        public Vector4 P11_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P11_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
