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

        public Matrix4x4 ViewMatrix
        {
            get => Data.ViewMatrix;
            set => SetDataProperty( value );
        }

        public float ClipPlaneNear
        {
            get => Data.ClipPlaneNear;
            set => SetDataProperty( value );
        }

        public float ClipPlaneFar
        {
            get => Data.ClipPlaneFar;
            set => SetDataProperty( value );
        }

        public float FieldOfView
        {
            get => Data.FieldOfView;
            set => SetDataProperty( value );
        }

        public float AspectRatio
        {
            get => Data.AspectRatio;
            set => SetDataProperty( value );
        }

        public float Field190
        {
            get => Data.Field190;
            set => SetDataProperty( value );
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