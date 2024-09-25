using GFDLibrary.Materials;

namespace GFDStudio.GUI.DataViewNodes
{
    // Soon
    public class MaterialParameterSetType0ViewNode : DataViewNode<MaterialParameterSetType0>
    {
        public MaterialParameterSetType0ViewNode( string text, MaterialParameterSetType0 data ) : base( text, data )
        {
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {
            
        }
    }
}
