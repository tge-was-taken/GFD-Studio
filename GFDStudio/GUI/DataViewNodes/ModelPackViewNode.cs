using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Misc;
using GFDLibrary.Models.Conversion;
using GFDStudio.IO;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ModelPackViewNode : ResourceViewNode<ModelPack>
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
        public ModelViewNode Model { get; set; }

        [Browsable( false )]
        public ChunkType000100F9ViewNode ChunkType000100F9 { get; set; }

        [Browsable( false )]
        public ChunkType000100F8ViewNode ChunkType000100F8 { get; set; }

        [Browsable( false )]
        public AnimationPackViewNode Animations { get; set; }

        protected internal ModelPackViewNode( string text, ModelPack data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< ModelPack >( ( path ) => Data.Save(  path ) );
            RegisterExportHandler< Assimp.Scene >( path => ModelPackExporter.ExportFile( Data, path ) );

            RegisterReplaceHandler<ModelPack>( path =>
            {
                var model = Resource.Load<ModelPack>( path );
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
                var model = new ModelPack( Version );
                if ( Textures != null && Nodes.Contains( Textures ) )
                    model.Textures = Textures.Data;

                if ( Materials != null && Nodes.Contains( Materials ) )
                    model.Materials = Materials.Data;

                if ( Model != null && Nodes.Contains( Model ) )
                    model.Model = Model.Data;

                if ( ChunkType000100F9 != null && Nodes.Contains( ChunkType000100F9 ) )
                    model.ChunkType000100F9 = ChunkType000100F9.Data;

                if ( ChunkType000100F8 != null && Nodes.Contains( ChunkType000100F8 ) )
                    model.ChunkType000100F8 = ChunkType000100F8.Data;

                if ( Animations != null && Nodes.Contains( Animations ) )
                    model.AnimationPack = Animations.Data;

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

            if ( Data.Model != null )
            {
                Model = ( ModelViewNode ) DataViewNodeFactory.Create( "Model", Data.Model );
                Nodes.Add( Model );
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
                Animations = ( AnimationPackViewNode )DataViewNodeFactory.Create( "Animations", Data.AnimationPack );
                Nodes.Add( Animations );
            }
        }
    }
}
