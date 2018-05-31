using OpenTK;

namespace GFDStudio.GUI.Controls.ModelView
{
    public class GLOrthographicCamera : GLCamera
    {
        /// <summary>
        /// Gets or sets the width of the projection volume.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the projection volume.
        /// </summary>
        public float Height { get; set; }

        public GLOrthographicCamera( Vector3 translation, float zNear, float zFar, float width, float height )
            : base( translation, zNear, zFar )
        {
            Width = width;
            Height = height;
        }

        public override Matrix4 CalculateProjectionMatrix()
        {
            return Matrix4.CreateOrthographic( Width, Height, ZNear, ZFar );
        }

        public override Matrix4 CalculateViewMatrix()
        {
            var eye = Translation;
            var up = Vector3.UnitY;

            var view = Matrix4.LookAt(
                eye,
                eye + new Vector3(0, 0, -1),
                up );

            return view;
        }
    }
}
