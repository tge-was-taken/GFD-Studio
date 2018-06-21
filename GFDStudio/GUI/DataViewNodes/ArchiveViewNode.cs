using System.IO;
using GFDLibrary;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ArchiveViewNode : DataViewNode<Archive>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Add |
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected internal ArchiveViewNode( string text, Archive data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Archive>( ( path ) => Data.Save( path ) );
            RegisterReplaceHandler<Archive>( ( path ) => new Archive( path ) );
            RegisterAddHandler<Stream>( ( path ) => Nodes.Add( DataViewNodeFactory.Create( path ) ) );
            RegisterModelUpdateHandler( () =>
            {
                var builder = new ArchiveBuilder();

                foreach ( DataViewNode node in Nodes )
                {
                    builder.AddFile( node.Text, ModuleExportUtilities.CreateStream( node.Data ) );
                }

                return builder.Build();
            });
        }

        protected override void InitializeViewCore()
        {
            foreach ( var entryName in Data )
            {
                var node = DataViewNodeFactory.Create( entryName, Data.OpenFile( entryName ) );
                Nodes.Add( node );
            }
        }
    }
}
