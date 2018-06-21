using System.IO;

namespace GFDStudio.GUI.DataViewNodes
{
    public class StreamViewNode : DataViewNode<Stream>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move |
            DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected internal StreamViewNode( string text, Stream data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( ( path ) =>
            {
                using ( var fileStream = File.Create( path ) )
                {
                    Data.Position = 0;
                    Data.CopyTo( fileStream );
                }
            } );

            RegisterReplaceHandler<Stream>( File.OpenRead );
        }
    }
}
