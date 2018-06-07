using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class GeometryViewModel : TreeNodeViewModel<Geometry>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public GeometryFlags Flags
        {
            get => Model.Flags;
            set => SetModelProperty( value );
        }

        public VertexAttributeFlags VertexAttributeFlags
        {
            get => Model.VertexAttributeFlags;
            set => SetModelProperty( value );
        }

        public int TriangleCount => Model.TriangleCount;

        public TriangleIndexType TriangleIndexType
        {
            get => Model.TriangleIndexType;
            set => SetModelProperty( value );
        }

        public int VertexCount => Model.VertexCount;

        public int Field14
        {
            get => Model.Field14;
            set => SetModelProperty( value );
        }

        public string MaterialName
        {
            get => Model.MaterialName;
            set => SetModelProperty( value );
        }
        public float FieldD4
        {
            get => Model.FieldD4;
            set => SetModelProperty( value );
        }

        public float FieldD8
        {
            get => Model.FieldD8;
            set => SetModelProperty( value );
        }

        public GeometryViewModel( string text, Geometry resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Geometry>( path => Model.Save( path ) );
            RegisterReplaceHandler<Geometry>( Resource.Load<Geometry> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}