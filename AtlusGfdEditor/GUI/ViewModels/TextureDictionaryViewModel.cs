using System.Windows.Forms;
using AtlusGfdEditor.FormatModules;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class TextureDictionaryViewModel : ResourceViewModel<TextureDictionary>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move |
            TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        protected internal TextureDictionaryViewModel( string text, TextureDictionary resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<TextureDictionary>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<TextureDictionary>( Resource.Load<TextureDictionary> );

            RegisterModelUpdateHandler( () =>
            {
                var textureDictionary = new TextureDictionary( Version );
                foreach ( TextureViewModel textureAdapter in Nodes )
                {
                    textureDictionary[textureAdapter.Name] = textureAdapter.Model;
                }

                return textureDictionary;
            } );

            RegisterCustomHandler( "Convert to field texture archive", () =>
            {
                using ( var dialog = new SaveFileDialog() )
                {
                    dialog.Filter = "Field Texture Archive (*.bin)|*.bin";
                    dialog.AutoUpgradeEnabled = true;
                    dialog.CheckPathExists = true;
                    dialog.FileName = Text;
                    dialog.OverwritePrompt = true;
                    dialog.Title = "Select a file to export to.";
                    dialog.ValidateNames = true;
                    dialog.AddExtension = true;

                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    Replace( TextureDictionary.ConvertToFieldTextureArchive( Model, dialog.FileName ) );
                }
            } );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Model.Textures )
            {
                Nodes.Add( TreeNodeViewModelFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
