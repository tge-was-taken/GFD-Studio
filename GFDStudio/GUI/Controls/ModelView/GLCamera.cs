using OpenTK;

namespace GFDStudio.GUI.Controls.ModelView
{
    public abstract class GLCamera
    {
        /// <summary>
        /// Gets or sets the translation (eye) vector of the camera.
        /// </summary>
        public Vector3 Translation { get; set; }

        /// <summary>
        /// Gets or sets the near edge of the projection volume.
        /// </summary>
        public float ZNear { get; set; }

        /// <summary>
        /// Gets or sets the far edge of the projection volume.
        /// </summary>
        public float ZFar { get; set; }

        public GLCamera( Vector3 translation, float zNear, float zFar )
        {
            Translation = translation;
            ZNear = zNear;
            ZFar = zFar;
        }

        public abstract Matrix4 CalculateProjectionMatrix();

        public abstract Matrix4 CalculateViewMatrix();

        public Matrix4 CalculateViewProjectionMatrix()
        {
            var projection = CalculateProjectionMatrix();
            var view = CalculateViewMatrix();

            return view * projection;
        }
    }
}
