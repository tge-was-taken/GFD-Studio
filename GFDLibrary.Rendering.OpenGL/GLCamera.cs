using OpenTK;

namespace GFDLibrary.Rendering.OpenGL
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

        protected GLCamera( Vector3 translation, float zNear, float zFar )
        {
            Translation = translation;
            ZNear = zNear;
            ZFar = zFar;
        }

        public abstract Matrix4 Projection { get; }

        public abstract Matrix4 View { get; }

        public Matrix4 ViewProjection => View * Projection;

        public void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "view",       View);
            shaderProgram.SetUniform( "projection", Projection);
        }
    }
}
