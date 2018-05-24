using System.ComponentModel;
using AtlusGfdEditor.IO;
using AtlusGfdLibrary;
using AtlusGfdLibrary.Assimp;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class ModelViewModel : ResourceViewModel<Model>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace |
            TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Branch;

        [Browsable( false )]
        public TextureDictionaryViewModel TextureDictionaryViewModel { get; set; }

        [Browsable( false )]
        public MaterialDictionaryViewModel MaterialDictionaryViewModel { get; set; }

        [Browsable( false )]
        public SceneViewModel SceneViewModel { get; set; }

        [Browsable( false )]
        public ChunkType000100F9ViewModel ChunkType000100F9ViewModel { get; set; }

        protected internal ModelViewModel( string text, Model resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Model >( ( path ) => Resource.Save( Model, path ) );
            RegisterExportHandler< Assimp.Scene >( path => ModelExporter.ExportFile( Model, path ) );

            RegisterReplaceHandler<Model>( Resource.Load<Model> );
            RegisterReplaceHandler< Assimp.Scene >( path =>
            {
                var model = ModelConverterUtility.ConvertAssimpModel( path );

                if ( model == null )
                    return Model;
                else
                    return model;
            });

            RegisterModelUpdateHandler( () =>
            {
                var model = new Model( Version );
                if ( TextureDictionaryViewModel != null )
                    model.TextureDictionary = TextureDictionaryViewModel.Model;

                if ( MaterialDictionaryViewModel != null )
                    model.MaterialDictionary = MaterialDictionaryViewModel.Model;

                if ( SceneViewModel != null )
                    model.Scene = SceneViewModel.Model;

                if ( ChunkType000100F9ViewModel != null )
                    model.ChunkType000100F9 = ChunkType000100F9ViewModel.Model;

                return model;
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Model.TextureDictionary != null )
            {
                TextureDictionaryViewModel = ( TextureDictionaryViewModel )TreeNodeViewModelFactory.Create( "Textures", Model.TextureDictionary );
                Nodes.Add( TextureDictionaryViewModel );
            }

            if ( Model.MaterialDictionary != null )
            {
                MaterialDictionaryViewModel = ( MaterialDictionaryViewModel )TreeNodeViewModelFactory.Create( "Materials", Model.MaterialDictionary );
                Nodes.Add( MaterialDictionaryViewModel );
            }

            if ( Model.Scene != null )
            {
                SceneViewModel = ( SceneViewModel ) TreeNodeViewModelFactory.Create( "Scene", Model.Scene );
                Nodes.Add( SceneViewModel );
            }

            if ( Model.ChunkType000100F9 != null )
            {
                ChunkType000100F9ViewModel = ( ChunkType000100F9ViewModel )TreeNodeViewModelFactory.Create( "Chunk Type 000100F9", Model.ChunkType000100F9 );
                Nodes.Add( ChunkType000100F9ViewModel );
            }
        }
    }
}
