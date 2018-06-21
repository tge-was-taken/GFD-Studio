using System.Numerics;
using GFDLibrary;

namespace GFDStudio.GUI.DataViewNodes
{
    public class CameraViewNode : DataViewNode<Camera>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Move;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public Matrix4x4 Transform
        {
            get => Data.Transform;
            set => SetDataProperty( value );
        }

        public float Field180
        {
            get => Data.Field180;
            set => SetDataProperty( value );
        }

        public float Field184
        {
            get => Data.Field184;
            set => SetDataProperty( value );
        }

        public float Field188
        {
            get => Data.Field188;
            set => SetDataProperty( value );
        }

        public float Field18C
        {
            get => Data.Field18C;
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