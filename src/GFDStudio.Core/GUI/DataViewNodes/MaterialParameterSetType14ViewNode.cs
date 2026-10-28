using GFDLibrary.Materials;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType14ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType14>
    {
        public MaterialParameterSetType14ViewNode( string text, MaterialParameterSetType14 data ) : base( text, data )
        {
        }

        public Vector4 P14_0 { 
            get => Data.P14_0; 
            set => Set( x => x.P14_0, value ); 
        } // 0x90
        public uint P14_1 { 
            get => Data.P14_1; 
            set => Set( x => x.P14_1, value ); 
        } // 0xa0

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
