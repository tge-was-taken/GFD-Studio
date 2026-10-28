using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType6ViewNode : MaterialAttributeViewNode<MaterialAttributeType6>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        [ Browsable( true ) ]
        public int Field0C
        {
            get => Data.Field0C;
            set => Set( x => x.Field0C, value );
        }

        [Browsable( true )]
        public int Field10
        {
            get => Data.Field10;
            set => Set( x => x.Field10, value );
        }

        [Browsable( true )]
        public int Field14
        {
            get => Data.Field14;
            set => Set( x => x.Field14, value );
        }

        public MaterialAttributeType6ViewNode( string text, MaterialAttributeType6 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType6> );
        }
    }
}