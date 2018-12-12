using System;
using System.Linq;
using System.Numerics;
using GFDLibrary.Models;
using OpenTK.Graphics.OpenGL;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLVertexArray : IDisposable
    {
        public int Id { get; }

        public GLVertexAttributeBuffer<Vector3> PositionBuffer { get; }

        public GLVertexAttributeBuffer<Vector3> NormalBuffer { get; }

        public GLVertexAttributeBuffer<Vector2> TextureCoordinateChannel0Buffer { get; }

        public GLBuffer<uint> ElementBuffer { get; }

        public PrimitiveType PrimitiveType { get; }

        public GLVertexArray( Vector3[] positions, Vector3[] normals, Vector2[] texCoords, uint[] indices, PrimitiveType primitiveType )
        {
            // vertex array
            Id = GL.GenVertexArray();
            GL.BindVertexArray( Id );

            // positions
            PositionBuffer = new GLVertexAttributeBuffer<Vector3>( positions, 0, 3, VertexAttribPointerType.Float );

            // normals
            if ( normals != null )
                NormalBuffer = new GLVertexAttributeBuffer<Vector3>( normals, 1, 3, VertexAttribPointerType.Float );

            if ( texCoords != null )
            {
                // texture coordinate channel 0
                TextureCoordinateChannel0Buffer = new GLVertexAttributeBuffer<Vector2>( texCoords, 2, 2, VertexAttribPointerType.Float );
            }

            // element index buffer
            ElementBuffer = new GLBuffer<uint>( BufferTarget.ElementArrayBuffer, indices );
            PrimitiveType = primitiveType;
        }

        public static void UnbindAll() => GL.BindVertexArray( 0 );

        public void Bind()
        {
            GL.BindVertexArray( Id );
        }

        public void Draw()
        {
            Bind();
            GL.DrawElements( PrimitiveType, ElementBuffer.Count, DrawElementsType.UnsignedInt, 0 );
        }

        #region IDisposable Support
        private bool mDisposed = false; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    PositionBuffer.Dispose();
                    NormalBuffer?.Dispose();
                    TextureCoordinateChannel0Buffer?.Dispose();
                    ElementBuffer.Dispose();
                    GL.DeleteVertexArray( Id );
                }

                mDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose( true );
        }
        #endregion
    }
}