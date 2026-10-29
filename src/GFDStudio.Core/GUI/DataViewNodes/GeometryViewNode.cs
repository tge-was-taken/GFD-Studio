using System.ComponentModel;
using GFDLibrary;
using System.Numerics;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class GeometryViewNode : DataViewNode<Geometry>
    {
        private void NotifyGeometryChanged()
        {
            HasPendingChanges = true;
            InitializeView( true );
        }

        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace;

        public override DataViewNodeFlags NodeFlags
            => Data.MorphTargets == null ? DataViewNodeFlags.Leaf : DataViewNodeFlags.Branch;

        public GeometryFlags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        }

        [DisplayName( "Vertex attribute flags" )]
        public VertexAttributeFlags VertexAttributeFlags
        {
            get => Data.VertexAttributeFlags;
            set => Set( x => x.VertexAttributeFlags, value );
        }

        [DisplayName( "Triangle count" )]
        public int TriangleCount => Data.TriangleCount;

        [DisplayName( "Triangle index format" )]
        public TriangleIndexFormat TriangleIndexFormat
        {
            get => Data.TriangleIndexFormat;
            set => Set( x => x.TriangleIndexFormat, value );
        }

        [DisplayName( "Vertex count" )]
        public int VertexCount => Data.VertexCount;

        public int Field14
        {
            get => Data.Field14;
            set => Set( x => x.Field14, value );
        }

        [DisplayName( "Material name" )]
        public string MaterialName
        {
            get => Data.MaterialName;
            set => Set( x => x.MaterialName, value );
        }
        [DisplayName( "LOD Start" )]
        public float LodStart
        {
            get => Data.LodStart;
            set => Set( x => x.LodStart, value );
        }
        [DisplayName( "LOD End" )]
        public float LodEnd
        {
            get => Data.LodEnd;
            set => Set( x => x.LodEnd, value );
        }

        [Browsable(false)]
        public MorphTargetListViewNode MorphTargets { get; set; }

        public GeometryViewNode( string text, Geometry data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterCustomHandler( "Regenerate", "Normals", () =>
            {
                GeometryMath.RegenerateNormals( Data );
                NotifyGeometryChanged();
            } );
            RegisterCustomHandler( "Regenerate", "Tangents and binormals", () =>
            {
                GeometryMath.RegenerateTangentSpace( Data );
                NotifyGeometryChanged();
            } );
            RegisterCustomHandler( "Regenerate", "Set binormals to a direction", () =>
            {
                var direction = UserDialogs.Current.AskDirection(
                    "Set binormals", "Every binormal in this geometry is set to this direction.", Vector3.UnitZ );

                if ( direction == null )
                    return;

                GeometryMath.SetBinormals( Data, direction.Value );
                NotifyGeometryChanged();
            } );

            RegisterExportHandler<Geometry>( path => Data.Save( path ) );
            RegisterReplaceHandler<Geometry>( Resource.Load<Geometry> );
            RegisterModelUpdateHandler( () =>
            {
                var geometry = Data;
                geometry.MorphTargets = MorphTargets.Data;
                return geometry;
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