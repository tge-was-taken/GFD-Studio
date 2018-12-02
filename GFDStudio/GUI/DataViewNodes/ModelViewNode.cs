using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Common;
using GFDLibrary.Models;
using GFDLibrary.Models.Conversion;
using GFDStudio.GUI.Forms;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ModelViewNode : ResourceViewNode<Model>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Branch;

        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<ModelFlags>))]
        public ModelFlags Flags
        {
            get => GetDataProperty< ModelFlags >();
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

        public ModelViewNode( string text, Model model ) : base( text, model )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Model >( path => Data.Save(  path ) );
            RegisterReplaceHandler<Model>( Resource.Load< Model > );
            RegisterReplaceHandler<Assimp.Scene>( path =>
            {
                using ( var dialog = new ModelConverterOptionsDialog( true ) )
                {
                    if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                        return Data;

                    ModelConverterOptions options = new ModelConverterOptions()
                    {
                        Version = dialog.Version,
                        ConvertSkinToZUp = dialog.ConvertSkinToZUp,
                        GenerateVertexColors = dialog.GenerateVertexColors
                    };

                    var scene = ModelConverter.ConvertFromAssimpScene( path, options );
                    if ( scene != null )
                        Data.ReplaceWith( scene );

                    return Data;
                }
            } );

            RegisterModelUpdateHandler( () =>
            {
                var scene = new Model( Data.Version );
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
                BonePaletteViewNode = ( BonePaletteViewNode )DataViewNodeFactory.Create( "Bone Palette", Data.BonePalette );
                AddChildNode( BonePaletteViewNode );
            }

            RootNodeViewNode = ( NodeViewNode ) DataViewNodeFactory.Create( Data.RootNode.Name, Data.RootNode );
            AddChildNode( RootNodeViewNode );
        }
    }
}