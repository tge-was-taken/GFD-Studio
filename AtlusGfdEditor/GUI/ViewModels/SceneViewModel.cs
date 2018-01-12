using System.ComponentModel;
using AtlusGfdEditor.GUI.Forms;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class SceneViewModel : ResourceViewModel<Scene>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Replace;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Branch;

        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<SceneFlags>))]
        public SceneFlags Flags
        {
            get => GetModelProperty< SceneFlags >();
            set => SetModelProperty( value );
        }

        [ Browsable( false ) ]
        public MatrixPaletteViewModel MatrixPaletteViewModel { get; set; }

        [ Browsable( true ) ]
        public BoundingBox? BoundingBox
        {
            get => GetModelProperty< BoundingBox? >();
        }

        [Browsable( true )]
        public BoundingSphere? BoundingSphere
        {
            get => GetModelProperty<BoundingSphere?>();
        }

        [ Browsable( false ) ]
        public NodeViewModel RootNodeViewModel { get; set; }

        public SceneViewModel( string text, Scene scene ) : base( text, scene )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Scene >( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler< Scene >( path => Resource.Load< Model >( path ) );
            RegisterReplaceHandler<Assimp.Scene>( path =>
            {
                using ( var dialog = new ModelConverterOptionsDialog( true ) )
                {
                    if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                        return Model;

                    SceneConverterOptions options = new SceneConverterOptions()
                    {
                        Version = dialog.Version,
                        ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                        GenerateVertexColors = dialog.GenerateVertexColors
                    };

                    return SceneConverter.ConvertFromAssimpScene( path, options );
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var scene = new Scene( Model.Version );
                scene.BoundingBox = Model.BoundingBox;
                scene.BoundingSphere = Model.BoundingSphere;
                scene.Flags = Model.Flags;

                if ( MatrixPaletteViewModel != null && Nodes.Contains(MatrixPaletteViewModel) )
                    scene.MatrixPalette = MatrixPaletteViewModel.Model;
                else
                    scene.MatrixPalette = null;

                if ( RootNodeViewModel != null && Nodes.Contains( RootNodeViewModel ) )
                    scene.RootNode = RootNodeViewModel.Model;
                else
                    scene.RootNode = null;

                return scene;
            });
        }

        protected override void InitializeViewCore()
        {
            if ( Model.MatrixPalette != null )
            {
                MatrixPaletteViewModel = ( MatrixPaletteViewModel )TreeNodeViewModelFactory.Create( "Matrix Palette", Model.MatrixPalette );
                Nodes.Add( MatrixPaletteViewModel );
            }

            RootNodeViewModel = ( NodeViewModel ) TreeNodeViewModelFactory.Create( Model.RootNode.Name, Model.RootNode );
            Nodes.Add( RootNodeViewModel );
        }
    }
}