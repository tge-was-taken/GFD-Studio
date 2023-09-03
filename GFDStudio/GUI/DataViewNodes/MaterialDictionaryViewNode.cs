using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDLibrary.Models.Conversion;
using GFDStudio.GUI.Forms;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialDictionaryViewNode : ResourceViewNode<MaterialDictionary>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move |
            DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Add | DataViewNodeMenuFlags.Convert;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        public MaterialDictionaryViewNode( string text, MaterialDictionary data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            RegisterAddHandler<Material>( path => Data.Add( Resource.Load<Material>( path ) ) );
            RegisterCustomHandler( "Add", "New material", () =>
            {
                Data.Add( new Material( "New material" ) );
                InitializeView( true );
            } );
            //RegisterCustomHandler("Convert to", "Material preset (All)", () => { ConvertAllToMaterialPreset(); });
            RegisterCustomHandler( "Export", "All", () =>
            {
                var dialog = new VistaFolderBrowserDialog();
                {
                    if ( dialog.ShowDialog() != true )
                        return;

                    foreach ( MaterialViewNode viewModel in Nodes )
                        viewModel.Data.Save( Path.Combine( dialog.SelectedPath, viewModel.Text + ".gmt" ) );
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var materialDictionary = new MaterialDictionary( Version );
                foreach ( MaterialViewNode adapter in Nodes )
                    materialDictionary[adapter.Name] = adapter.Data;

                return materialDictionary;
            } );
        }

        private void ConvertAllToMaterialPreset()
        {
            using (var dialog = new ModelConverterOptionsDialog(false))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                ModelPackConverterOptions options = new ModelPackConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version
                };

                foreach (var material in Data.Materials )
                {
                    ReplaceProcessing( material.GetType(), options.MaterialPreset );
                }
                
            }
        }

        protected override void InitializeViewCore()
        {
            foreach ( var texture in Data.Materials )
            {
                AddChildNode( DataViewNodeFactory.Create( texture.Name, texture ) );
            }
        }
    }
}
