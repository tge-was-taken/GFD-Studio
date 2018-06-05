using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Assimp;
using GFDStudio.IO;

namespace GFDStudio.GUI.ViewModels
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

        [Browsable( false )]
        public ChunkType000100F8ViewModel ChunkType000100F8ViewModel { get; set; }

        [Browsable( false )]
        public AnimationPackageViewModel AnimationPackage { get; set; }

        protected internal ModelViewModel( string text, Model resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Model >( ( path ) => Resource.Save( Model, path ) );
            RegisterExportHandler< Assimp.Scene >( path => ModelExporter.ExportFile( Model, path ) );

            RegisterReplaceHandler<Model>( path =>
            {
                var model = Resource.Load<Model>( path );
                if ( model != null )
                    Model.ReplaceWith( model );

                return Model;
            });
            RegisterReplaceHandler< Assimp.Scene >( path =>
            {
                var model = ModelConverterUtility.ConvertAssimpModel( path );
                if ( model != null )
                    Model.ReplaceWith( model );

                return Model;
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

                if ( ChunkType000100F8ViewModel != null )
                    model.ChunkType000100F8 = ChunkType000100F8ViewModel.Model;

                if ( AnimationPackage != null )
                    model.AnimationPackage = AnimationPackage.Model;

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

            if ( Model.ChunkType000100F8 != null )
            {
                ChunkType000100F8ViewModel = ( ChunkType000100F8ViewModel )TreeNodeViewModelFactory.Create( "Chunk Type 000100F8", Model.ChunkType000100F8 );
                Nodes.Add( ChunkType000100F8ViewModel );
            }

            if ( Model.AnimationPackage != null )
            {
                AnimationPackage = ( AnimationPackageViewModel )TreeNodeViewModelFactory.Create( "Animations", Model.AnimationPackage );
                Nodes.Add( AnimationPackage );
            }
        }
    }
}
