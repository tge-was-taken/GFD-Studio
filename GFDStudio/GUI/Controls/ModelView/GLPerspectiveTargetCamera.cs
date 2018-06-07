using OpenTK;

namespace GFDStudio.GUI.Controls.ModelView
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

            var view = Matrix4.LookAt(
                eye,
                target,
                Vector3.UnitY );

            return view;
        }

        public void Rotate( float angleX, float angleY )
        {
            var rotX = Matrix4.CreateRotationX( angleX );
            var rotY = Matrix4.CreateRotationY( angleY );
            var transform =  Matrix4.CreateTranslation( -Target ) * rotX * rotY * Matrix4.CreateTranslation( Target );
            Translation = new Vector3( new Vector4( Translation, 1 ) * transform );
            Target = new Vector3( new Vector4( Target, 1 ) * transform );
        }
    }
}
