using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLVertexAttributeBuffer<T> : GLBuffer<T> where T : struct
    {
        public GLVertexAttributeBuffer( T[] data, int attributeIndex, int attributeSize, VertexAttribPointerType type ) : base( BufferTarget.ArrayBuffer, data )
        {
            // configure vertex attribute
            GL.VertexAttribPointer( attributeIndex, attributeSize, type, false, 0, 0 );

            // enable vertex attribute
            GL.EnableVertexAttribArray( attributeIndex );
        }
    }
}