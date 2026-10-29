using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using GFDLibrary.Models;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace GFDLibrary.Rendering.OpenGL
{
    public class GLMesh : IDisposable
    {
        public Geometry Geometry { get; }

        public GLVertexArray VertexArray { get; }

        public GLBaseMaterial Material { get; }

        public bool IsVisible { get; }

        /// <summary>
        /// Whether the vertices follow the bones, and so have to be worked out again whenever
        /// the pose changes.
        /// </summary>
        public bool IsSkinned { get; }

        /// <summary>
        /// The model bones this geometry actually uses. The vertices index into this rather than
        /// the model's whole list, which keeps the palette small enough for a uniform array.
        /// </summary>
        public int[] PaletteBones { get; private set; }

        public GLMesh( GLVertexArray vertexArray, GLBaseMaterial material, bool isVisible )
        {
            Geometry = null;
            VertexArray = vertexArray;
            Material = material;
            IsVisible = isVisible;
        }

        public GLMesh( Geometry geometry, Matrix4x4 modelMatrix, List<Bone> bones, List<GLNode> nodes, Dictionary<string, GLBaseMaterial> materials,
                       uint version = ResourceVersion.Persona5 )
        {
            Geometry = geometry;
            IsSkinned = geometry.VertexWeights != null;

            Vector4[] boneWeights = null;
            Vector4[] boneIndices = null;

            if ( IsSkinned )
                BuildSkinningData( geometry, out boneWeights, out boneIndices );

            var indices = new uint[geometry.Triangles.Length * 3];
            for ( int i = 0; i < geometry.Triangles.Length; i++ )
            {
                indices[( i * 3 ) + 0] = geometry.Triangles[i].A;
                indices[( i * 3 ) + 1] = geometry.Triangles[i].B;
                indices[( i * 3 ) + 2] = geometry.Triangles[i].C;
            }

            VertexArray = new GLVertexArray( geometry.Vertices, geometry.Normals,
                new[] { geometry.TexCoordsChannel0, geometry.TexCoordsChannel1, geometry.TexCoordsChannel2 }, 
                new[] { geometry.ColorChannel0, geometry.ColorChannel1, geometry.ColorChannel2 },
                indices, PrimitiveType.Triangles, version, boneWeights, boneIndices );

            // material
            if ( geometry.MaterialName != null && materials != null )
            {
                if ( materials.TryGetValue( geometry.MaterialName, out var material ) )
                {
                    Material = material;
                }
                else
                {
                    Trace.TraceError( $"Geometry referenced material \"{geometry.MaterialName}\" which does not exist in the model" );
                    Material = new GLP5Material();
                }
            }
            else
            {
                Material = new GLP5Material();
            }

            IsVisible = true;
        }

        /// <summary>
        /// Collects the bones this geometry uses and rewrites each vertex's indices to point into
        /// that short list, so the palette uploaded per frame stays small.
        /// </summary>
        private void BuildSkinningData( Geometry geometry, out Vector4[] weights, out Vector4[] indices )
        {
            var slotByBone = new Dictionary<ushort, int>();
            var palette = new List<int>();

            weights = new Vector4[geometry.VertexCount];
            indices = new Vector4[geometry.VertexCount];

            for ( var i = 0; i < geometry.VertexCount; i++ )
            {
                var vertex = geometry.VertexWeights[i];
                var w = new float[4];
                var s = new float[4];

                for ( var j = 0; j < 4 && j < vertex.Weights.Length; j++ )
                {
                    if ( vertex.Weights[j] == 0 )
                        continue;

                    var bone = vertex.Indices[j];
                    if ( !slotByBone.TryGetValue( bone, out var slot ) )
                    {
                        slot = palette.Count;
                        slotByBone.Add( bone, slot );
                        palette.Add( bone );
                    }

                    w[j] = vertex.Weights[j];
                    s[j] = slot;
                }

                weights[i] = new Vector4( w[0], w[1], w[2], w[3] );
                indices[i] = new Vector4( s[0], s[1], s[2], s[3] );
            }

            PaletteBones = palette.ToArray();
        }

        public void Draw( Matrix4 modelMatrix, GLShaderProgram shaderProgram )
        {
            // A skinned geometry is placed by its bones, so its node transform must not be applied
            // on top. The palette already puts it in world space.
            shaderProgram.SetUniform( "uModel", IsSkinned ? Matrix4.Identity : modelMatrix );
            shaderProgram.SetUniform( "uIsSkinned", IsSkinned );
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