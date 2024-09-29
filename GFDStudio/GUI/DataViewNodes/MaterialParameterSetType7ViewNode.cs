using GFDLibrary.Materials;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType7ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType7>
    {
        public MaterialParameterSetType7ViewNode( string text, MaterialParameterSetType7 data ) : base( text, data )
        {
        }

        public float P7_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf0
        public uint P7_2 { 
            get => GetDataProperty<uint>(); 
            set => SetDataProperty(value); 
        } // 0xf4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
