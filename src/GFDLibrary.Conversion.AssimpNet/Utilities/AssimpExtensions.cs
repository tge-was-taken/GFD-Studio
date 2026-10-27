using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Assimp;
using GFDLibrary.Graphics;

namespace GFDLibrary.Conversion.AssimpNet.Utilities
{
    public static class AssimpExtensions
    {
        // AssimpNetter hands out System.Numerics types holding assimp's own memory layout, so the
        // conversions left are the ones that change shape or convention.

        public static Vector3 ToAssimp( this Vector2 value ) => new Vector3( value.X, value.Y, 0 );

        public static IEnumerable<Vector3> ToAssimp( this IEnumerable<Vector2> values ) => values.Select( x => x.ToAssimp() );

        public static Vector3 ToAssimpAsVector3( this Vector4 value ) => new Vector3( value.X, value.Y, value.Z );

        public static Vector4 ToAssimp( this Color value )
            => new Vector4( value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f );

        public static IEnumerable<Vector4> ToAssimp( this IEnumerable<Color> values ) => values.Select( x => x.ToAssimp() );

        /// <summary>
        /// Assimp keeps the translation in the fourth column and System.Numerics in the fourth row,
        /// so moving a matrix between the two conventions is a transpose.
        /// </summary>
        public static Matrix4x4 ToAssimp( this Matrix4x4 matrix ) => Matrix4x4.Transpose( matrix );

        /// <inheritdoc cref="ToAssimp(Matrix4x4)"/>
        public static Matrix4x4 ToNumerics( this Matrix4x4 matrix ) => Matrix4x4.Transpose( matrix );

        public static void ExportColladaFile( this Scene aiScene, string path )
        {
            using ( var aiContext = new AssimpContext() )
                aiContext.ExportFile( aiScene, path, "collada", PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateSmoothNormals );
        }

        public static Color ToNumerics( this Vector4 value )
        {
            return new Color( (byte)( value.X * 255f ),
                              (byte)( value.Y * 255f ),
                              (byte)( value.Z * 255f ),
                              (byte)( value.W * 255f ) );
        }

        public static IEnumerable<Color> ToNumerics( this IEnumerable<Vector4> values ) => values.Select( x => x.ToNumerics() );

        public static Vector4 ToNumericsAsVector4( this Vector3 value ) => new Vector4( value.X, value.Y, value.Z, 1.0f );

        public static Vector2 FromAssimpAsVector2( this Vector3 value ) => new Vector2( value.X, value.Y );

        public static IEnumerable<Vector2> FromAssimpAsVector2( this IEnumerable<Vector3> values ) => values.Select( x => x.FromAssimpAsVector2() );

        public static System.Numerics.Matrix4x4 CalculateWorldTransform( this Assimp.Node node )
        {
            Matrix4x4 CalculateWorldTransformInternal( Assimp.Node currentNode )
            {
                var transform = currentNode.Transform;
                if ( currentNode.Parent != null )
                    transform *= CalculateWorldTransformInternal( currentNode.Parent );

                return transform;
            }

            return CalculateWorldTransformInternal( node ) .ToNumerics( );
        }

        /// <summary>
        /// Gets the weights assigned to each vertex. This also fixes any missing weights.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static List<(Assimp.Bone Bone, float Weight)>[] GetVertexWeights( this Assimp.Mesh mesh )
        {
            // Map out the vertex weights for each vertex
            var vertexWeights = new List<(Assimp.Bone, float)>[mesh.VertexCount];
            var missingVertexWeights = new List<int>();
            for ( int i = 0; i < mesh.VertexCount; i++ )
            {
                var weights = new List<(Assimp.Bone, float)>();
                foreach ( var bone in mesh.Bones )
                {
                    foreach ( var vertexWeight in bone.VertexWeights )
                    {
                        if ( vertexWeight.VertexID == i )
                        {
                            weights.Add( (bone, vertexWeight.Weight) );
                        }
                    }
                }

                if ( weights.Count == 0 )
                {
                    missingVertexWeights.Add( i );
                    continue;
                }

                vertexWeights[i] = weights;
            }

            // Resolve vertices without any weights by finding the closest vertex next to it that does, and taking its weights
            foreach ( var i in missingVertexWeights )
            {
                var position = mesh.Vertices[i];
                var range = 0.001f;
                List<(Assimp.Bone, float)> weights = null;

                while ( weights == null )
                {
                    for ( var j = 0; j < mesh.Vertices.Count; j++ )
                    {
                        if ( missingVertexWeights.Contains( j ) )
                            continue;

                        // ＤＥＬＴＡ
                        var otherPosition = mesh.Vertices[j];
                        var delta = position - otherPosition;
                        if ( IsWithinRange( delta, range ) )
                        {
                            weights = vertexWeights[j];
                            break;
                        }
                    }

                    range *= 2f;
                }

                vertexWeights[i] = weights;
            }

            Debug.Assert( vertexWeights.All( x => x != null ) );
            return vertexWeights;
        }

        private static bool IsWithinRange( Vector3 position, float range )
        {
            return ( position.X < 0 ? position.X >= -range : position.X <= range ) &&
                   ( position.Y < 0 ? position.Y >= -range : position.Y <= range ) &&
                   ( position.Z < 0 ? position.Z >= -range : position.Z <= range );
        }
    }
}
