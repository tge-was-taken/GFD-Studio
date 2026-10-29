using System;
using System.Numerics;

namespace GFDLibrary.Models
{
    /// <summary>
    /// Works out a geometry's normals and tangent space from its triangles, for geometries that arrive
    /// without them or whose own are wrong.
    /// </summary>
    public static class GeometryMath
    {
        /// <summary>
        /// Recalculates smooth vertex normals by accumulating the face normals around each
        /// vertex, weighted by face area, which the uncrossed edge product already gives.
        /// </summary>
        public static void RegenerateNormals( Geometry geometry )
        {
            if ( !HasGeometry( geometry ) )
                return;

            var normals = new Vector3[geometry.VertexCount];

            foreach ( var triangle in geometry.Triangles )
            {
                if ( !InRange( geometry, triangle ) )
                    continue;

                var a = geometry.Vertices[triangle.A];
                var b = geometry.Vertices[triangle.B];
                var c = geometry.Vertices[triangle.C];

                var faceNormal = Vector3.Cross( b - a, c - a );

                normals[triangle.A] += faceNormal;
                normals[triangle.B] += faceNormal;
                normals[triangle.C] += faceNormal;
            }

            geometry.Normals = Normalize( normals );
        }

        /// <summary>
        /// Recalculates the tangent and binormal from how the first texture coordinates change
        /// across each triangle, so they line up with the way a normal map is laid out.
        /// </summary>
        public static void RegenerateTangentSpace( Geometry geometry )
        {
            if ( !HasGeometry( geometry ) || geometry.TexCoordsChannel0 == null )
                return;

            if ( geometry.Normals == null )
                RegenerateNormals( geometry );

            var tangents = new Vector3[geometry.VertexCount];
            var binormals = new Vector3[geometry.VertexCount];

            foreach ( var triangle in geometry.Triangles )
            {
                if ( !InRange( geometry, triangle ) )
                    continue;

                var a = geometry.Vertices[triangle.A];
                var edge1 = geometry.Vertices[triangle.B] - a;
                var edge2 = geometry.Vertices[triangle.C] - a;

                var uvA = geometry.TexCoordsChannel0[triangle.A];
                var deltaUv1 = geometry.TexCoordsChannel0[triangle.B] - uvA;
                var deltaUv2 = geometry.TexCoordsChannel0[triangle.C] - uvA;

                var determinant = deltaUv1.X * deltaUv2.Y - deltaUv2.X * deltaUv1.Y;
                if ( MathF.Abs( determinant ) < 1e-12f )
                    continue;

                var scale = 1f / determinant;
                var tangent = ( edge1 * deltaUv2.Y - edge2 * deltaUv1.Y ) * scale;
                var binormal = ( edge2 * deltaUv1.X - edge1 * deltaUv2.X ) * scale;

                foreach ( var index in new[] { triangle.A, triangle.B, triangle.C } )
                {
                    tangents[index] += tangent;
                    binormals[index] += binormal;
                }
            }

            // Gram-Schmidt, so the tangent sits in the plane the normal defines.
            for ( var i = 0; i < geometry.VertexCount; i++ )
            {
                var normal = geometry.Normals[i];
                tangents[i] -= normal * Vector3.Dot( normal, tangents[i] );
                binormals[i] -= normal * Vector3.Dot( normal, binormals[i] );
            }

            geometry.Tangents = Normalize( tangents );
            geometry.Binormals = Normalize( binormals );
        }

        /// <summary>
        /// Points every binormal the same way. Shaders that read the binormal as a direction
        /// rather than part of a tangent basis, such as a signed distance field toon shader,
        /// want this rather than something derived from the surface.
        /// </summary>
        public static void SetBinormals( Geometry geometry, Vector3 direction )
        {
            if ( geometry.VertexCount == 0 )
                return;

            var normalised = direction.Length() < 1e-6f ? Vector3.UnitZ : Vector3.Normalize( direction );
            var binormals = new Vector3[geometry.VertexCount];

            for ( var i = 0; i < binormals.Length; i++ )
                binormals[i] = normalised;

            geometry.Binormals = binormals;
        }

        private static bool HasGeometry( Geometry geometry )
            => geometry?.Vertices != null && geometry.Triangles != null && geometry.VertexCount > 0;

        private static bool InRange( Geometry geometry, Triangle triangle )
            => triangle.A < geometry.VertexCount && triangle.B < geometry.VertexCount && triangle.C < geometry.VertexCount;

        private static Vector3[] Normalize( Vector3[] vectors )
        {
            for ( var i = 0; i < vectors.Length; i++ )
                vectors[i] = vectors[i].LengthSquared() < 1e-12f ? Vector3.Zero : Vector3.Normalize( vectors[i] );

            return vectors;
        }
    }
}
