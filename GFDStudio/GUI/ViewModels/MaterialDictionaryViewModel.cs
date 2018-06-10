using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using Ookii.Dialogs;

namespace GFDStudio.GUI.ViewModels
{
    public class MaterialDictionaryViewModel : ResourceViewModel<MaterialDictionary>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move |
            TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Add;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        public MaterialDictionaryViewModel( string text, MaterialDictionary resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<MaterialDictionary>( path => Model.Save(  path ) );
            RegisterReplaceHandler<MaterialDictionary>( Resource.Load<MaterialDictionary> );
            RegisterAddHandler< Material >( path => Model.Add( Resource.Load< Material >( path ) ) );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var dialog = new VistaFolderBrowserDialog() )
                {
                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( MaterialViewModel viewModel in Nodes )
                        viewModel.Model.Save( Path.Combine( dialog.SelectedPath, viewModel.Text + ".gmt" ) );
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var materialDictionary = new MaterialDictionary( Version );
                foreach ( MaterialViewModel adapter in Nodes )
                    materialDictionary[adapter.Name] = adapter.Model;

                return materialDictionary;
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Model.Materials )
            {
                Nodes.Add( TreeNodeViewModelFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
