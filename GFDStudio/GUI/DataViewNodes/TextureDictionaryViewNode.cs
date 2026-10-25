using System.Drawing;
using System.IO;
using GFDLibrary;
using GFDLibrary.Textures;
using GFDStudio.FormatModules;

namespace GFDStudio.GUI.DataViewNodes
{
    public class TextureDictionaryViewNode : ResourceViewNode<TextureDictionary>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move |
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Add;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected internal TextureDictionaryViewNode( string text, TextureDictionary data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();

            RegisterAddHandler<GFDLibrary.Graphics.Image>( path => Data.Add( TextureEncoder.Encode( Path.GetFileNameWithoutExtension( path ) + ".dds",
                                                                                 TextureFormat.DDS, GFDLibrary.Textures.PngCodec.Load( path ) ) ) );

            RegisterAddHandler<Stream>( path => Data.Add( new Texture( Path.GetFileNameWithoutExtension( path ) + ".dds",
                                                                       TextureFormat.DDS, File.ReadAllBytes( path ) ) ) );

            RegisterCustomHandler( "Add", "New texture", () =>
            {
                Data.Add(Texture.CreateDefaultTexture( "New texture.dds" ) );
                InitializeView( true );
            });

            RegisterModelUpdateHandler( () =>
            {
                var textureDictionary = new TextureDictionary( Version );
                foreach ( TextureViewNode textureAdapter in Nodes )
                {
                    textureDictionary[textureAdapter.Name] = textureAdapter.Data;
                }

                return textureDictionary;
            } );

            RegisterCustomHandler( "Convert to", "Field texture archive (PS3)", () => { ConvertToFieldTextureArchive( false ); } );
            RegisterCustomHandler( "Convert to", "Field texture archive (PS4)", () => { ConvertToFieldTextureArchive( true ); } );

            RegisterCustomHandler( "Export", "All", () =>
            {
                var directory = DataViewNodeDialogs.Current.SelectFolder( "Select a folder to export to." );
                if ( directory == null )
                    return;

                foreach ( TextureViewNode viewModel in Nodes )
                    File.WriteAllBytes( Path.Combine( directory, viewModel.Text ), viewModel.Data.Data );
            } );
        }

        private void ConvertToFieldTextureArchive( bool usePs4Format )
        {
            var result = DataViewNodeDialogs.Current.SaveFile( new FileDialogRequest
            {
                Title = "Select a file to export to.",
                Filter = "Field Texture Archive (*.bin)|*.bin",
                FileName = Text,
            } );

            if ( result == null )
                return;

            Replace( TextureDictionary.ConvertToFieldTextureArchive( Data, result.FileName, usePs4Format ) );
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Data.Textures )
            {
                var node = DataViewNodeFactory.Create( texture.Name, texture );
                AddChildNode( node );
            }
        }
    }
}
