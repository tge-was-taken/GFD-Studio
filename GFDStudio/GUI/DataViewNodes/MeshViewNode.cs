using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MeshViewNode : DataViewNode<Mesh>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public GeometryFlags Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        public VertexAttributeFlags VertexAttributeFlags
        {
            get => Data.VertexAttributeFlags;
            set => SetDataProperty( value );
        }

        public int TriangleCount => Data.TriangleCount;

        public TriangleIndexType TriangleIndexType
        {
            get => Data.TriangleIndexType;
            set => SetDataProperty( value );
        }

        public int VertexCount => Data.VertexCount;

        public int Field14
        {
            get => Data.Field14;
            set => SetDataProperty( value );
        }

        public string MaterialName
        {
            get => Data.MaterialName;
            set => SetDataProperty( value );
        }
        public float FieldD4
        {
            get => Data.FieldD4;
            set => SetDataProperty( value );
        }

        public float FieldD8
        {
            get => Data.FieldD8;
            set => SetDataProperty( value );
        }

        public MeshViewNode( string text, Mesh data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Mesh>( path => Data.Save( path ) );
            RegisterReplaceHandler<Mesh>( Resource.Load<Mesh> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}