using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.IO.Assimp;
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
        public TextureDictionaryViewModel Textures { get; set; }

        [Browsable( false )]
        public MaterialDictionaryViewModel Materials { get; set; }

        [Browsable( false )]
        public SceneViewModel Scene { get; set; }

        [Browsable( false )]
        public ChunkType000100F9ViewModel ChunkType000100F9 { get; set; }

        [Browsable( false )]
        public ChunkType000100F8ViewModel ChunkType000100F8 { get; set; }

        [Browsable( false )]
        public AnimationPackViewModel AnimationPack { get; set; }

        protected internal ModelViewModel( string text, Model resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Model >( ( path ) => Model.Save(  path ) );
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
                if ( Textures != null && Nodes.Contains( Textures ) )
                    model.Textures = Textures.Model;

                if ( Materials != null && Nodes.Contains( Materials ) )
                    model.Materials = Materials.Model;

                if ( Scene != null && Nodes.Contains( Scene ) )
                    model.Scene = Scene.Model;

                if ( ChunkType000100F9 != null && Nodes.Contains( ChunkType000100F9 ) )
                    model.ChunkType000100F9 = ChunkType000100F9.Model;

                if ( ChunkType000100F8 != null && Nodes.Contains( ChunkType000100F8 ) )
                    model.ChunkType000100F8 = ChunkType000100F8.Model;

                if ( AnimationPack != null && Nodes.Contains( AnimationPack ) )
                    model.AnimationPack = AnimationPack.Model;

                return model;
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Model.Textures != null )
            {
                Textures = ( TextureDictionaryViewModel )TreeNodeViewModelFactory.Create( "Textures", Model.Textures );
                Nodes.Add( Textures );
            }

            if ( Model.Materials != null )
            {
                Materials = ( MaterialDictionaryViewModel )TreeNodeViewModelFactory.Create( "Materials", Model.Materials );
                Nodes.Add( Materials );
            }

            if ( Model.Scene != null )
            {
                Scene = ( SceneViewModel ) TreeNodeViewModelFactory.Create( "Scene", Model.Scene );
                Nodes.Add( Scene );
            }

            if ( Model.ChunkType000100F9 != null )
            {
                ChunkType000100F9 = ( ChunkType000100F9ViewModel )TreeNodeViewModelFactory.Create( "Chunk Type 000100F9", Model.ChunkType000100F9 );
                Nodes.Add( ChunkType000100F9 );
            }

            if ( Model.ChunkType000100F8 != null )
            {
                ChunkType000100F8 = ( ChunkType000100F8ViewModel )TreeNodeViewModelFactory.Create( "Chunk Type 000100F8", Model.ChunkType000100F8 );
                Nodes.Add( ChunkType000100F8 );
            }

            if ( Model.AnimationPack != null )
            {
                AnimationPack = ( AnimationPackViewModel )TreeNodeViewModelFactory.Create( "Animations", Model.AnimationPack );
                Nodes.Add( AnimationPack );
            }
        }
    }
}
