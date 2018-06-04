using System.ComponentModel;
using GFDLibrary;
using GFDStudio.GUI.Forms;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
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
        public BonePaletteViewModel BonePaletteViewModel { get; set; }

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

                    var scene = SceneConverter.ConvertFromAssimpScene( path, options );
                    if ( scene != null )
                        Model.ReplaceWith( scene );

                    return Model;
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var scene = new Scene( Model.Version );
                scene.BoundingBox = Model.BoundingBox;
                scene.BoundingSphere = Model.BoundingSphere;
                scene.Flags = Model.Flags;

                if ( BonePaletteViewModel != null && Nodes.Contains(BonePaletteViewModel) )
                    scene.BonePalette = BonePaletteViewModel.Model;
                else
                    scene.BonePalette = null;

                if ( RootNodeViewModel != null && Nodes.Contains( RootNodeViewModel ) )
                    scene.RootNode = RootNodeViewModel.Model;
                else
                    scene.RootNode = null;

                return scene;
            });
        }

        protected override void InitializeViewCore()
        {
            if ( Model.BonePalette != null )
            {
                BonePaletteViewModel = ( BonePaletteViewModel )TreeNodeViewModelFactory.Create( "Matrix Palette", Model.BonePalette );
                Nodes.Add( BonePaletteViewModel );
            }

            RootNodeViewModel = ( NodeViewModel ) TreeNodeViewModelFactory.Create( Model.RootNode.Name, Model.RootNode );
            Nodes.Add( RootNodeViewModel );
        }
    }
}