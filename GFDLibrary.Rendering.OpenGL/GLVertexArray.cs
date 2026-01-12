using System;
using System.Linq;
using System.Numerics;
using GFDLibrary.Models;
using OpenTK.Graphics.OpenGL4;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLVertexArray : IDisposable
    {
        public int Id { get; }

        public GLVertexAttributeBuffer<Vector4>[] ColorChannelBuffers { get; } = new GLVertexAttributeBuffer<Vector4>[3];
        public GLVertexAttributeBuffer<Vector3> PositionBuffer { get; }

        public GLVertexAttributeBuffer<Vector3> NormalBuffer { get; }

        public GLVertexAttributeBuffer<Vector2>[] TextureCoordinateChannelBuffers { get; } = new GLVertexAttributeBuffer<Vector2>[3];

        public GLVertexAttributeBuffer<Vector4> BoneWeightBuffer { get; }

        public GLVertexAttributeBuffer<Vector4> BoneIndexBuffer { get; }

        public GLBuffer<uint> ElementBuffer { get; }

        public PrimitiveType PrimitiveType { get; }

        public GLVertexArray( Vector3[] positions, Vector3[] normals, 
            Vector2[][] texCoordChannels,
            Graphics.Color[][] vertColorChannels, 
            uint[] indices, PrimitiveType primitiveType,
            Vector4[] boneWeights = null, Vector4[] boneIndices = null )
        {
            // vertex array
            Id = GL.GenVertexArray();
            GL.BindVertexArray( Id );

            // positions
            PositionBuffer = new GLVertexAttributeBuffer<Vector3>( positions, 0, 3, VertexAttribPointerType.Float );

            // normals
            if ( normals != null )
                NormalBuffer = new GLVertexAttributeBuffer<Vector3>( normals, 1, 3, VertexAttribPointerType.Float );


            if (texCoordChannels != null )
            {
                for ( int channelIndex = 0; channelIndex < 3; ++channelIndex)
                {
                    if ( texCoordChannels.Length > channelIndex && texCoordChannels[channelIndex] != null )
                        TextureCoordinateChannelBuffers[channelIndex] = new GLVertexAttributeBuffer<Vector2>( texCoordChannels[channelIndex], channelIndex == 0 ? 2 : 4 + channelIndex, 2, VertexAttribPointerType.Float );
                }
            }

            if (vertColorChannels != null)
            {
                for( int channelIndex = 0; channelIndex < 3; ++channelIndex)
                {
                    if ( vertColorChannels.Length > channelIndex && vertColorChannels[channelIndex] != null )
                    {
                        Vector4[] vertColorVector4 = new Vector4[vertColorChannels[channelIndex].Length];
                        for ( int vertexIndex = 0; vertexIndex < vertColorChannels[channelIndex].Length; vertexIndex++ )
                        {
                            var color = vertColorChannels[channelIndex][vertexIndex];
                            vertColorVector4[vertexIndex] = new Vector4( color.R, color.G, color.B, color.A ) / 255f;
                        }
                        ColorChannelBuffers[channelIndex] = new GLVertexAttributeBuffer<Vector4>( vertColorVector4, 7 + channelIndex, 4, VertexAttribPointerType.Float );
                    }
                }
            }

            // Bone indices go up as floats rather than integers, so that one attribute kind
            // serves every driver without a separate integer pointer call.
            if ( boneWeights != null && boneIndices != null )
            {
                BoneWeightBuffer = new GLVertexAttributeBuffer<Vector4>( boneWeights, 3, 4, VertexAttribPointerType.Float );
                BoneIndexBuffer = new GLVertexAttributeBuffer<Vector4>( boneIndices, 4, 4, VertexAttribPointerType.Float );
            }

            // element index buffer
            ElementBuffer = new GLBuffer<uint>( BufferTarget.ElementArrayBuffer, indices );
            PrimitiveType = primitiveType;
        }

        /// <summary>
        /// Replaces the positions and normals without rebuilding the array, for a mesh that is
        /// reskinned every frame.
        /// </summary>
        public void UpdateGeometry( Vector3[] positions, Vector3[] normals )
        {
            GL.BindVertexArray( Id );
            PositionBuffer.Update( BufferTarget.ArrayBuffer, positions );

            if ( normals != null )
                NormalBuffer?.Update( BufferTarget.ArrayBuffer, normals );
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
                    foreach ( var buffer in TextureCoordinateChannelBuffers )
                        buffer?.Dispose();
                    foreach ( var buffer in ColorChannelBuffers )
                        buffer?.Dispose();
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
