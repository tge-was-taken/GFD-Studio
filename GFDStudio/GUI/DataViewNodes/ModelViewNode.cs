using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.IO.Assimp;
using GFDStudio.IO;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ModelViewNode : ResourceViewNode<Model>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace |
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        [Browsable( false )]
        public TextureDictionaryViewNode Textures { get; set; }

        [Browsable( false )]
        public MaterialDictionaryViewNode Materials { get; set; }

        [Browsable( false )]
        public SceneViewNode Scene { get; set; }

        [Browsable( false )]
        public ChunkType000100F9ViewNode ChunkType000100F9 { get; set; }

        [Browsable( false )]
        public ChunkType000100F8ViewNode ChunkType000100F8 { get; set; }

        [Browsable( false )]
        public AnimationPackViewNode AnimationPack { get; set; }

        protected internal ModelViewNode( string text, Model data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Model >( ( path ) => Data.Save(  path ) );
            RegisterExportHandler< Assimp.Scene >( path => ModelExporter.ExportFile( Data, path ) );

            RegisterReplaceHandler<Model>( path =>
            {
                var model = Resource.Load<Model>( path );
                if ( model != null )
                    Data.ReplaceWith( model );

                return Data;
            });
            RegisterReplaceHandler< Assimp.Scene >( path =>
            {
                var model = ModelConverterUtility.ConvertAssimpModel( path );
                if ( model != null )
                    Data.ReplaceWith( model );

                return Data;
            });

            RegisterModelUpdateHandler( () =>
            {
                var model = new Model( Version );
                if ( Textures != null && Nodes.Contains( Textures ) )
                    model.Textures = Textures.Data;

                if ( Materials != null && Nodes.Contains( Materials ) )
                    model.Materials = Materials.Data;

                if ( Scene != null && Nodes.Contains( Scene ) )
                    model.Scene = Scene.Data;

                if ( ChunkType000100F9 != null && Nodes.Contains( ChunkType000100F9 ) )
                    model.ChunkType000100F9 = ChunkType000100F9.Data;

                if ( ChunkType000100F8 != null && Nodes.Contains( ChunkType000100F8 ) )
                    model.ChunkType000100F8 = ChunkType000100F8.Data;

                if ( AnimationPack != null && Nodes.Contains( AnimationPack ) )
                    model.AnimationPack = AnimationPack.Data;

                return model;
            } );
            RegisterCustomHandler( "Add New Animation Pack", () =>
            {
                Data.AnimationPack = new AnimationPack( Data.Version );
                InitializeView( true );
            } );
            RegisterCustomHandler( "Add New Chunk Type 000100F9", () =>
            {
                Data.ChunkType000100F9 = new ChunkType000100F9( Data.Version );
                InitializeView( true );
            } );
            RegisterCustomHandler( "Add New Chunk Type 000100F8", () =>
            {
                Data.ChunkType000100F8 = new ChunkType000100F8( Data.Version );
                InitializeView( true );
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Data.Textures != null )
            {
                Textures = ( TextureDictionaryViewNode )DataViewNodeFactory.Create( "Textures", Data.Textures );
                Nodes.Add( Textures );
            }

            if ( Data.Materials != null )
            {
                Materials = ( MaterialDictionaryViewNode )DataViewNodeFactory.Create( "Materials", Data.Materials );
                Nodes.Add( Materials );
            }

            if ( Data.Scene != null )
            {
                Scene = ( SceneViewNode ) DataViewNodeFactory.Create( "Scene", Data.Scene );
                Nodes.Add( Scene );
            }

            if ( Data.ChunkType000100F9 != null )
            {
                ChunkType000100F9 = ( ChunkType000100F9ViewNode )DataViewNodeFactory.Create( "Chunk Type 000100F9", Data.ChunkType000100F9 );
                Nodes.Add( ChunkType000100F9 );
            }

            if ( Data.ChunkType000100F8 != null )
            {
                ChunkType000100F8 = ( ChunkType000100F8ViewNode )DataViewNodeFactory.Create( "Chunk Type 000100F8", Data.ChunkType000100F8 );
                Nodes.Add( ChunkType000100F8 );
            }

            if ( Data.AnimationPack != null )
            {
                AnimationPack = ( AnimationPackViewNode )DataViewNodeFactory.Create( "Animations", Data.AnimationPack );
                Nodes.Add( AnimationPack );
            }
        }
    }
}
