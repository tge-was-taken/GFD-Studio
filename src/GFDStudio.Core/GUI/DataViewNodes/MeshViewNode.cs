using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Common;
using GFDLibrary.Conversion;
using GFDLibrary.Conversion.AssimpNet;
using GFDLibrary.Conversion.AssimpNet.Utilities;
using GFDLibrary.Models;
using GFDStudio.FormatModules;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MeshViewNode : ResourceViewNode<Mesh>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Branch;

        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<ModelFlags>))]
        public ModelFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [ Browsable( false ) ]
        public BoneListViewNode Bones { get; set; }

        [ Browsable( true ) ]
        [ DisplayName( "Bounding box" )]
        public BoundingBox? BoundingBox
        {
            get => Data.BoundingBox;
        }

        [Browsable( true )]
        [DisplayName( "Bounding sphere" )]
        public BoundingSphere? BoundingSphere
        {
            get => Data.BoundingSphere;
        }

        [ Browsable( false ) ]
        public NodeViewNode RootNodeViewNode { get; set; }

        public MeshViewNode( string text, Mesh mesh ) : base( text, mesh )
        {
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            RegisterReplaceHandler<AssimpScene>( path =>
            {
                var scene = AssimpHelper.ImportScene( path );
                var originalModel = Parent?.Data as ModelPack;

                var options = UserDialogs.Current.AskModelConversionOptions( scene, originalModel );
                if ( options == null )
                    return Data;

                var mesh = AssimpNetModelConverter.ConvertFromAssimpScene( scene, options );
                if ( mesh != null )
                    Data.ReplaceWith( mesh );

                return Data;
            } );

            RegisterModelUpdateHandler( () =>
            {
                var scene = new Mesh( Data.Version );
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