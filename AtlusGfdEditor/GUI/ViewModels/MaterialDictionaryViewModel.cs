using AtlusGfdLibrary;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialDictionaryViewModel : ResourceViewModel<MaterialDictionary>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move |
            TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        public MaterialDictionaryViewModel( string text, MaterialDictionary resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<MaterialDictionary>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<MaterialDictionary>( Resource.Load<MaterialDictionary> );

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
