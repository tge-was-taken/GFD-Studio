using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Materials;
using Ookii.Dialogs;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialDictionaryViewNode : ResourceViewNode<MaterialDictionary>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move |
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Add;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        public MaterialDictionaryViewNode( string text, MaterialDictionary data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<MaterialDictionary>( path => Data.Save(  path ) );
            RegisterReplaceHandler<MaterialDictionary>( Resource.Load<MaterialDictionary> );
            RegisterAddHandler< Material >( path => Data.Add( Resource.Load< Material >( path ) ) );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var dialog = new VistaFolderBrowserDialog() )
                {
                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( MaterialViewNode viewModel in Nodes )
                        viewModel.Data.Save( Path.Combine( dialog.SelectedPath, viewModel.Text + ".gmt" ) );
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var materialDictionary = new MaterialDictionary( Version );
                foreach ( MaterialViewNode adapter in Nodes )
                    materialDictionary[adapter.Name] = adapter.Data;

                return materialDictionary;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Data.Materials )
            {
                AddChildNode( DataViewNodeFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
