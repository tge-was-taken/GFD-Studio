using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MeshViewNode : DataViewNode<Mesh>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags
            => Data.MorphTargets == null ? DataViewNodeFlags.Leaf : DataViewNodeFlags.Branch;

        public GeometryFlags Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        [DisplayName( "Vertex attribute flags" )]
        public VertexAttributeFlags VertexAttributeFlags
        {
            get => Data.VertexAttributeFlags;
            set => SetDataProperty( value );
        }

        [DisplayName( "Triangle count" )]
        public int TriangleCount => Data.TriangleCount;

        [DisplayName( "Triangle index format" )]
        public TriangleIndexFormat TriangleIndexFormat
        {
            get => Data.TriangleIndexFormat;
            set => SetDataProperty( value );
        }

        [DisplayName( "Vertex count" )]
        public int VertexCount => Data.VertexCount;

        public int Field14
        {
            get => Data.Field14;
            set => SetDataProperty( value );
        }

        [DisplayName( "Material name" )]
        public string MaterialName
        {
            get => Data.MaterialName;
            set => SetDataProperty( value );
        }
        [DisplayName( "LOD Start" )]
        public float LodStart
        {
            get => Data.LodStart;
            set => SetDataProperty( value );
        }
        [DisplayName( "LOD End" )]
        public float LodEnd
        {
            get => Data.LodEnd;
            set => SetDataProperty( value );
        }

        [Browsable(false)]
        public MorphTargetListViewNode MorphTargets { get; set; }

        public MeshViewNode( string text, Mesh data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Mesh>( path => Data.Save( path ) );
            RegisterReplaceHandler<Mesh>( Resource.Load<Mesh> );
            RegisterModelUpdateHandler( () =>
            {
                var mesh = Data;
                mesh.MorphTargets = MorphTargets.Data;
                return mesh;
            });
            RegisterCustomHandler( "Add", "New morph target list", () =>
            {
                Data.MorphTargets = new MorphTargetList { Flags = 2 };
                InitializeView( true );
            });
        }

        protected override void InitializeViewCore()
        {
            MorphTargets = ( MorphTargetListViewNode ) DataViewNodeFactory.Create( "Morph Targets", Data.MorphTargets );
            AddChildNode( MorphTargets );
        }
    }
}