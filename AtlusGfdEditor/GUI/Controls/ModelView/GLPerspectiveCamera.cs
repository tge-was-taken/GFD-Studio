using OpenTK;

namespace AtlusGfdEditor.GUI.Controls.ModelView
{

    /// <summary>
    /// Represents the base class of all perspective cameras.
    /// </summary>
    public abstract class GLPerspectiveCamera : GLCamera
    {
        /// <summary>
        /// Gets or sets the field of view in degrees.
        /// </summary>
        public float FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        public float AspectRatio { get; set; }

        public GLPerspectiveCamera( Vector3 translation, float zNear, float zFar, float fieldOfView, float aspectRatio )
            : base( translation, zNear, zFar )
        {
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
        }

        public override Matrix4 CalculateProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView( MathHelper.DegreesToRadians( FieldOfView ), AspectRatio, ZNear, ZFar );
        }
    }
}
