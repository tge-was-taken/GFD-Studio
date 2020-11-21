using System;
using System.IO;
using System.Windows.Forms;
using GFDLibrary.Common;
using GFDLibrary.Textures;
using GFDLibrary.Textures.DDS;
using GFDLibrary.Textures.GNF;
using GFDStudio.FormatModules;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;

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
            RegisterAddHandler<Stream>( ( path ) => AddChildNode( DataViewNodeFactory.Create( path ) ) );
            RegisterModelUpdateHandler( () =>
            {
                var builder = new ArchiveBuilder();

                foreach ( DataViewNode node in Nodes )
                {
                    builder.AddFile( node.Text, ModuleExportUtilities.CreateStream( node.Data ) );
                }

                return builder.Build();
            });
            RegisterCustomHandler("Export", "All", () =>
            {
                var dialog = new VistaFolderBrowserDialog();
                {
                    if ( dialog.ShowDialog() != true )
                        return;

                    foreach ( DataViewNode node in Nodes )
                    {
                        // Hack for field texture archives: prefer DDS output format
                        Type type = null;
                        if ( node.DataType == typeof( FieldTexturePS3 ) || node.DataType == typeof( GNFTexture ) )
                            type = typeof( DDSStream );

                        node.Export( Path.Combine( dialog.SelectedPath, node.Text ), type );
                    }
                }
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var entryName in Data )
            {
                var node = DataViewNodeFactory.Create( entryName, Data.OpenFile( entryName ) );
                AddChildNode( node );
            }
        }
    }
}
