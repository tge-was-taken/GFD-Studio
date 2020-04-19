using OpenTK;

namespace GFDLibrary.Rendering.OpenGL
{
    /// <summary>
    /// Represents a perspective target camera whose target is fixed to be relative to its own position. 
    /// </summary>
    public class GLPerspectiveFreeCamera : GLPerspectiveTargetCamera
    {
        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        public Quaternion Rotation { get; set; }

        public GLPerspectiveFreeCamera( Vector3 translation, float zNear, float zFar, float fieldOfView, float aspectRatio, Quaternion rotation )
            : base( translation, zNear, zFar, fieldOfView, aspectRatio, Vector3.Zero )
        {
            Rotation = rotation;
        }

        public override Matrix4 View
        {
            get
            {
                // 'eye' position
                var eye = Translation;

                // forward vector of rotation
                var forward = Rotation * Vector3.UnitZ;

                // up vector of rotation
                var up = Rotation * Vector3.UnitY;

                // target position is one unit ahead of the camera
                // the distance is multiplied by the forward vector of the rotation to account
                // for the changed direction caused by the rotation
                var target = eye + ( forward * -1 );

                var view = Matrix4.LookAt(
                    eye,
                    target,
                    up );

                return view;
            }
        }
    }
}
