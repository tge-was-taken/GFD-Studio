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
        public BoneListViewNode Bones { get; set; }

        [ Browsable( true ) ]
        [ DisplayName( "Bounding box" )]
        public BoundingBox? BoundingBox
        {
            get => GetDataProperty< BoundingBox? >();
        }

        [Browsable( true )]
        [DisplayName( "Bounding sphere" )]
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
            base.InitializeCore();
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
                        GenerateVertexColors = dialog.GenerateVertexColors,
                        MinimalVertexAttributes = dialog.MinimalVertexAttributes,
                        AutoAddGFDHelperIDs = dialog.AutoAddGFDHelperIDs
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

                if ( Bones != null && Nodes.Contains(Bones) )
                    scene.Bones = Bones.Data;
                else
                    scene.Bones = null;

                if ( RootNodeViewNode != null && Nodes.Contains( RootNodeViewNode ) )
                    scene.RootNode = RootNodeViewNode.Data;
                else
                    scene.RootNode = null;

                return scene;
            });
        }

        protected override void InitializeViewCore()
        {
            if ( Data.Bones != null )
            {
                Bones = ( BoneListViewNode )DataViewNodeFactory.Create( "Bones", Data.Bones );
                AddChildNode( Bones );
            }

            RootNodeViewNode = ( NodeViewNode ) DataViewNodeFactory.Create( Data.RootNode.Name, Data.RootNode );
            AddChildNode( RootNodeViewNode );
        }
    }
}