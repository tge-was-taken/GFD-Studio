using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Textures;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;

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
            RegisterExportHandler<TextureDictionary>( path => Data.Save(  path ) );
            RegisterReplaceHandler<TextureDictionary>( Resource.Load<TextureDictionary> );

            RegisterAddHandler<Bitmap>( path => Data.Add( TextureEncoder.Encode( Path.GetFileNameWithoutExtension( path ) + ".dds",
                                                                                 TextureFormat.DDS, new Bitmap( path ) ) ) );

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
                var dialog = new VistaFolderBrowserDialog();
                {
                    if ( dialog.ShowDialog() != true )
                        return;

                    foreach ( TextureViewNode viewModel in Nodes )
                        File.WriteAllBytes( Path.Combine( dialog.SelectedPath, viewModel.Text ), viewModel.Data.Data );
                }
            } );
        }

        private void ConvertToFieldTextureArchive( bool usePs4Format )
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

                Replace( TextureDictionary.ConvertToFieldTextureArchive( Data, dialog.FileName, usePs4Format ) );
            }
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
