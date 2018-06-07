using System.Numerics;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class CameraViewModel : TreeNodeViewModel<Camera>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public Matrix4x4 Transform
        {
            get => Model.Transform;
            set => SetModelProperty( value );
        }

        public float Field180
        {
            get => Model.Field180;
            set => SetModelProperty( value );
        }

        public float Field184
        {
            get => Model.Field184;
            set => SetModelProperty( value );
        }

        public float Field188
        {
            get => Model.Field188;
            set => SetModelProperty( value );
        }

        public float Field18C
        {
            get => Model.Field18C;
            set => SetModelProperty( value );
        }

        public float Field190
        {
            get => Model.Field190;
            set => SetModelProperty( value );
        }

        public CameraViewModel( string text, Camera resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Camera>( path => Model.Save( path ) );
            RegisterReplaceHandler<Camera>( Resource.Load<Camera> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}