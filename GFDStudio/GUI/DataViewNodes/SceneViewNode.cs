using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDStudio.GUI.Forms;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class SceneViewNode : ResourceViewNode<Scene>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Branch;

        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<SceneFlags>))]
        public SceneFlags Flags
        {
            get => GetDataProperty< SceneFlags >();
            set => SetDataProperty( value );
        }

        [ Browsable( false ) ]
        public BonePaletteViewNode BonePaletteViewNode { get; set; }

        [ Browsable( true ) ]
        public BoundingBox? BoundingBox
        {
            get => GetDataProperty< BoundingBox? >();
        }

        [Browsable( true )]
        public BoundingSphere? BoundingSphere
        {
            get => GetDataProperty<BoundingSphere?>();
        }

        [ Browsable( false ) ]
        public NodeViewNode RootNodeViewNode { get; set; }

        public SceneViewNode( string text, Scene scene ) : base( text, scene )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Scene >( path => Data.Save(  path ) );
            RegisterReplaceHandler<Scene>( Resource.Load< Scene > );
            RegisterReplaceHandler<Assimp.Scene>( path =>
            {
                using ( var dialog = new ModelConverterOptionsDialog( true ) )
                {
                    if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                        return Data;

                    SceneConverterOptions options = new SceneConverterOptions()
                    {
                        Version = dialog.Version,
                        ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                        GenerateVertexColors = dialog.GenerateVertexColors
                    };

                    var scene = SceneConverter.ConvertFromAssimpScene( path, options );
                    if ( scene != null )
                        Data.ReplaceWith( scene );

                    return Data;
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var scene = new Scene( Data.Version );
                scene.BoundingBox = Data.BoundingBox;
                scene.BoundingSphere = Data.BoundingSphere;
                scene.Flags = Data.Flags;

                if ( BonePaletteViewNode != null && Nodes.Contains(BonePaletteViewNode) )
                    scene.BonePalette = BonePaletteViewNode.Data;
                else
                    scene.BonePalette = null;

                if ( RootNodeViewNode != null && Nodes.Contains( RootNodeViewNode ) )
                    scene.RootNode = RootNodeViewNode.Data;
                else
                    scene.RootNode = null;

                return scene;
            });
        }

        protected override void InitializeViewCore()
        {
            if ( Data.BonePalette != null )
            {
                BonePaletteViewNode = ( BonePaletteViewNode )DataViewNodeFactory.Create( "Matrix Palette", Data.BonePalette );
                Nodes.Add( BonePaletteViewNode );
            }

            RootNodeViewNode = ( NodeViewNode ) DataViewNodeFactory.Create( Data.RootNode.Name, Data.RootNode );
            Nodes.Add( RootNodeViewNode );
        }
    }
}