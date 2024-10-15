using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using GFDLibrary.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vector3 = System.Numerics.Vector3;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLMesh : IDisposable
    {
        public Mesh Mesh { get; }

        public GLVertexArray VertexArray { get; }

        public GLBaseMaterial Material { get; }

        public bool IsVisible { get; }

        public GLMesh( GLVertexArray vertexArray, GLBaseMaterial material, bool isVisible )
        {
            Mesh = null;
            VertexArray = vertexArray;
            Material = material;
            IsVisible = isVisible;
        }

        public GLMesh( Mesh mesh, Matrix4x4 modelMatrix, List<Bone> bones, List<GLNode> nodes, Dictionary<string, GLBaseMaterial> materials )
        {
            Mesh = mesh;

            var vertices = mesh.Vertices;
            var normals = mesh.Normals;

            if ( mesh.VertexWeights != null )
            {
                vertices = new Vector3[mesh.VertexCount];
                normals = null;

                if ( mesh.Normals != null )
                    normals = new Vector3[mesh.VertexCount];

                Matrix4x4.Invert( modelMatrix, out var modelMatrixInv );

                for ( int i = 0; i < mesh.VertexCount; i++ )
                {
                    var position = mesh.Vertices[i];
                    var normal = mesh.Normals?[i] ?? Vector3.Zero;

                    var newPosition = Vector3.Zero;
                    var newNormal = Vector3.Zero;
                    for ( int j = 0; j < mesh.VertexWeights[i].Weights.Length; j++ )
                    {
                        var weight = mesh.VertexWeights[i].Weights[j];
                        if ( weight == 0 )
                            continue;

                        var boneIndex = mesh.VertexWeights[i].Indices[j];
                        TransformVertex( bones, nodes, position, normal, ref newPosition, ref newNormal, weight, boneIndex );
                    }

                    vertices[i] = Vector3.Transform( newPosition, modelMatrixInv );

                    if ( normals != null )
                        normals[i] =
                            Vector3.Normalize( Vector3.TransformNormal( newNormal, modelMatrixInv ) );
                }
            }

            var indices = new uint[mesh.Triangles.Length * 3];
            for ( int i = 0; i < mesh.Triangles.Length; i++ )
            {
                indices[( i * 3 ) + 0] = mesh.Triangles[i].A;
                indices[( i * 3 ) + 1] = mesh.Triangles[i].B;
                indices[( i * 3 ) + 2] = mesh.Triangles[i].C;
            }

            VertexArray = new GLVertexArray( vertices, normals, mesh.TexCoordsChannel0, mesh.TexCoordsChannel1, mesh.TexCoordsChannel2, mesh.ColorChannel0, indices, PrimitiveType.Triangles );

            // material
            if ( mesh.MaterialName != null && materials != null )
            {
                if ( materials.TryGetValue( mesh.MaterialName, out var material ) )
                {
                    Material = material;
                }
                else
                {
                    Trace.TraceError( $"Mesh referenced material \"{mesh.MaterialName}\" which does not exist in the model" );
                    Material = new GLP5Material();
                }
            }
            else
            {
                Material = new GLP5Material();
            }

            IsVisible = true;
        }

        private static void TransformVertex( List<Bone> bones, List<GLNode> nodes, Vector3 position, Vector3 normal, ref Vector3 newPosition, ref Vector3 newNormal, float weight, ushort boneIndex )
        {
            var bone = bones[boneIndex];
            var boneNode = nodes[bone.NodeIndex];
            var bindMatrix = boneNode.WorldTransform;
            newPosition += Vector3.Transform( Vector3.Transform( position, bone.InverseBindMatrix ),
                                                              bindMatrix * weight );

            newNormal += Vector3.TransformNormal( Vector3.TransformNormal( normal, bone.InverseBindMatrix ),
                                                                 bindMatrix * weight );
        }

        public void Draw( Matrix4 modelMatrix, GLShaderProgram shaderProgram )
        {
            //if ( Material.METAPHOR_DistortionMaterialTest )
            //    return;
            shaderProgram.SetUniform( "uModel", modelMatrix);
            Material.Bind( shaderProgram );
            shaderProgram.Check();
            VertexArray.Draw();
            Material.Unbind( shaderProgram );
        }

        #region IDisposable Support
        private bool mDisposed; // To detect redundant calls

        protected virtual void Dispose( bool disposing )
        {
            if ( !mDisposed )
            {
                if ( disposing )
                {
                    VertexArray.Dispose();
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