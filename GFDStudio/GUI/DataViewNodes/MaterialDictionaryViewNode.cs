using System.IO;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDLibrary.Materials;
using GFDStudio.GUI.Forms;

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
            RegisterCustomHandler("Convert to", "Material preset (All)", () => { ConvertAllToMaterialPreset(); });
            RegisterCustomHandler( "Export", "All", () =>
            {
                var directory = UserDialogs.Current.SelectFolder( "Select a folder to export to." );
                if ( directory == null )
                    return;

                foreach ( MaterialViewNode viewModel in Nodes )
                    viewModel.Data.Save( Path.Combine( directory, viewModel.Text + ".gmt" ) );
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
            var options = UserDialogs.Current.AskModelConversionOptions( null, null );
            if ( options == null )
                return;

            Replace( MaterialDictionary.ConvertAllToMaterialPreset( Data, options ) );
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
