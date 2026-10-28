using System.ComponentModel;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Cameras;

namespace GFDStudio.GUI.DataViewNodes
{
    public class CameraViewNode : DataViewNode<Camera>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        [DisplayName( "View matrix" )]
        public Matrix4x4 ViewMatrix
        {
            get => Data.ViewMatrix;
            set => Set( x => x.ViewMatrix, value );
        }

        [DisplayName( "Clip plane near (z-min)" )]
        public float ClipPlaneNear
        {
            get => Data.ClipPlaneNear;
            set => Set( x => x.ClipPlaneNear, value );
        }

        [DisplayName( "Clip plane far (z-max)" )]
        public float ClipPlaneFar
        {
            get => Data.ClipPlaneFar;
            set => Set( x => x.ClipPlaneFar, value );
        }

        [DisplayName( "Field of view" )]
        public float FieldOfView
        {
            get => Data.FieldOfView;
            set => Set( x => x.FieldOfView, value );
        }

        [DisplayName( "Aspect ratio" )]
        public float AspectRatio
        {
            get => Data.AspectRatio;
            set => Set( x => x.AspectRatio, value );
        }

        public float Field190
        {
            get => Data.Field190;
            set => Set( x => x.Field190, value );
        }

        public CameraViewNode( string text, Camera data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Camera>( path => Data.Save( path ) );
            RegisterReplaceHandler<Camera>( Resource.Load<Camera> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}