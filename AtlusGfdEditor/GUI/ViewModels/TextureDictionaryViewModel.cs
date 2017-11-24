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
