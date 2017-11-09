using OpenTK;

namespace AtlusGfdEditor.GUI.Controls.ModelView
{
    /// <summary>
    /// Represents a perspective target camera.
    /// </summary>
    public class GLPerspectiveTargetCamera : GLPerspectiveCamera
    {
        /// <summary>
        /// Gets or sets the target vector of the camera.
        /// </summary>
        public Vector3 Target { get; set; }

        public GLPerspectiveTargetCamera( Vector3 translation, float zNear, float zFar, float fieldOfView, float aspectRatio, Vector3 target ) 
            : base( translation, zNear, zFar, fieldOfView, aspectRatio )
        {
            Target = target;
        }

        public override Matrix4 CalculateViewMatrix()
        {
            var eye = Translation;
            var target = Target;
            var up = Translation * Vector3.UnitY;

            var view = Matrix4.LookAt(
                eye,
                target,
                up );

            return view;
        }
    }
}
