using System.IO;
using GFDLibrary;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType7ViewNode : MaterialAttributeViewNode<MaterialAttributeType7>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public MaterialAttributeType7ViewNode( string text, MaterialAttributeType7 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType7> );
        }
    }
}