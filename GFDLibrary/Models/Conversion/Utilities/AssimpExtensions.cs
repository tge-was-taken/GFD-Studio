using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Assimp;
using GFDLibrary.Graphics;
using Matrix4x4 = Assimp.Matrix4x4;
using Quaternion = Assimp.Quaternion;

namespace GFDLibrary.Models.Conversion.Utilities
{
    public static class AssimpExtensions
    {
        public static Vector3D ToAssimp( this Vector3 value ) => new Vector3D( value.X, value.Y, value.Z );

        public static IEnumerable<Vector3D> ToAssimp( this IEnumerable<Vector3> values ) => values.Select( x => x.ToAssimp() );

        public static Vector3D ToAssimp( this Vector2 value ) => new Vector3D( value.X, value.Y, 0 );

        public static IEnumerable<Vector3D> ToAssimp( this IEnumerable<Vector2> values ) => values.Select( x => x.ToAssimp() );

        public static Color4D ToAssimp( this Vector4 value ) => new Color4D( value.X, value.Y, value.Z, value.W );

        public static Color3D ToAssimpAsColor3D( this Vector4 value ) => new Color3D( value.X, value.Y, value.Z );

        public static IEnumerable<Color4D> ToAssimp( this IEnumerable<Vector4> values ) => values.Select( x => x.ToAssimp() );

        public static Quaternion ToAssimp( this System.Numerics.Quaternion quaternion ) =>
            new Quaternion( quaternion.W, quaternion.X, quaternion.Y, quaternion.Z );

        public static Matrix4x4 ToAssimp( this System.Numerics.Matrix4x4 matrix )
        {
            return new Matrix4x4( matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                                         matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                                         matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                                         matrix.M14, matrix.M24, matrix.M34, matrix.M44 );
        }

        public static Color4D ToAssimp( this Color value )
        {
            return new Color4D( value.R / 255f,
                                       value.G / 255f,
                                       value.B / 255f,
                                       value.A / 255f );
        }

        public static IEnumerable<Color4D> ToAssimp( this IEnumerable<Color> values ) => values.Select( x => x.ToAssimp() );

        public static void ExportColladaFile( this Scene aiScene, string path )
        {
            using ( var aiContext = new AssimpContext() )
                aiContext.ExportFile( aiScene, path, "collada", PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateSmoothNormals );
        }

        public static Color ToNumerics( this Color4D value )
        {
            return new Color( ( byte )( value.R * 255f ),
                              ( byte )( value.G * 255f ),
                              ( byte )( value.B * 255f ),
                              ( byte )( value.A * 255f ) );
        }

        public static Vector4 ToNumerics( this Color3D value )
        {
            return new Vector4( value.R,
                                value.G,
                                value.B,
                                1.0f );
        }

        public static IEnumerable<Color> ToNumerics( this IEnumerable<Color4D> values ) => values.Select( x => x.ToNumerics() );

        public static System.Numerics.Matrix4x4 ToNumerics( this Matrix4x4 matrix )
        {
            return new System.Numerics.Matrix4x4( matrix.A1, matrix.B1, matrix.C1, matrix.D1,
                                  matrix.A2, matrix.B2, matrix.C2, matrix.D2,
                                  matrix.A3, matrix.B3, matrix.C3, matrix.D3,
                                  matrix.A4, matrix.B4, matrix.C4, matrix.D4 );
        }

        public static Vector3 ToNumerics( this Vector3D value )
        {
            return new Vector3( value.X, value.Y, value.Z );
        }

        public static IEnumerable<Vector3> ToNumerics( this IEnumerable<Vector3D> values ) => values.Select( x => x.ToNumerics() );

        public static Vector2 FromAssimpAsVector2( this Vector3D value )
        {
            return new Vector2( value.X, value.Y );
        }

        public static IEnumerable<Vector2> FromAssimpAsVector2( this IEnumerable<Vector3D> values ) => values.Select( x => x.FromAssimpAsVector2() );

        public static System.Numerics.Quaternion ToNumerics( this Quaternion value )
        {
            return new System.Numerics.Quaternion( value.X, value.Y, value.Z, value.W );
        }

        public static System.Numerics.Matrix4x4 CalculateWorldTransform( this Assimp.Node node )
        {
            Matrix4x4 CalculateWorldTransformInternal( Assimp.Node currentNode )
            {
                var transform = currentNode.Transform;
                if ( currentNode.Parent != null )
                    transform *= CalculateWorldTransformInternal( currentNode.Parent );

                return transform;
            }

            return ToNumerics( CalculateWorldTransformInternal( node ) );
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

        private static bool IsWithinRange( Vector3D position, float range )
        {
            return ( position.X < 0 ? position.X >= -range : position.X <= range ) &&
                   ( position.Y < 0 ? position.Y >= -range : position.Y <= range ) &&
                   ( position.Z < 0 ? position.Z >= -range : position.Z <= range );
        }
    }
}
