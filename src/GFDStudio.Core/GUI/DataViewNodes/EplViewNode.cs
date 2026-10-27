using GFDLibrary;
using GFDLibrary.Effects;

namespace GFDStudio.GUI.DataViewNodes
{
    public class EplViewNode : DataViewNode<Epl>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public EplViewNode( string text, Epl data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Epl>( path => Data.Save( path ) );
            RegisterReplaceHandler<Epl>( Resource.Load<Epl> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}