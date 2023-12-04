using GFDLibrary.Common;
using OpenTK;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLPerspectiveCamera : GLCamera
    {
        /// <summary>
        /// Gets or sets the field of view in degrees.
        /// </summary>
        public float FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        public float AspectRatio { get; set; }
        /// <summary>
        /// Gets or sets the position of the model.
        /// </summary>
        public Vector3 ModelTranslation { get; set; }
        /// <summary>
        /// Gets or sets the rotation of the model.
        /// </summary>
        public Vector3 ModelRotation { get; set; }
        /// <summary>
        /// Gets or sets the camera offset from the model.
        /// </summary>
        public Vector3 Offset { get; set; }
        protected GLPerspectiveCamera( float zNear, float zFar, float fieldOfView, float aspectRatio,
            Vector3 translation, Vector3 offset,
            Vector3 modelTranslation, Vector3 modelRotation )
            : base( translation, zNear, zFar )
        {
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            Offset = offset;
            ModelTranslation = modelTranslation;
            ModelRotation = modelRotation;
        }
        public GLPerspectiveCamera( float zNear, float zFar, float fieldOfView, float aspectRatio,
            BoundingSphere bs,
            Vector3 modelTranslation, Vector3 modelRotation )
            : base( Vector3.Zero, zNear, zFar )
        {
            Translation = new Vector3( 0, bs.Center.Y * ( 1 / 3 ), bs.Radius * 2f );
            Offset = new Vector3( 0, -bs.Center.Y * 0.75f, 0 );
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            ModelTranslation = modelTranslation;
            ModelRotation = modelRotation;
        }
        public override Matrix4 View
        {
            get
            {
                var view = Matrix4.LookAt(
                    Translation,
                    Translation - Vector3.UnitZ,
                    Vector3.UnitY );

                view = Matrix4.CreateTranslation( ModelTranslation ) * view;
                view = Matrix4.CreateRotationX( ModelRotation.X ) * view;
                view = Matrix4.CreateRotationY( ModelRotation.Y ) * view;
                view = Matrix4.CreateRotationZ( ModelRotation.Z ) * view;
                view = Matrix4.CreateTranslation( Offset ) * view;

                return view;
            }
        }
        public override Matrix4 Projection =>
            Matrix4.CreatePerspectiveFieldOfView( MathHelper.DegreesToRadians( FieldOfView ), AspectRatio, ZNear, ZFar );
    }
}
