using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Assimp;

namespace GFDLibrary.Conversion.AssimpNet.Utilities
{
    /// <summary>
    /// Provides various useful utilities for working with Assimp .
    /// </summary>
    public static class AssimpHelper
    {
        /// <summary>
        /// Creates a default, empty scene with a root node.
        /// </summary>
        /// <returns></returns>
        public static Scene CreateDefaultScene()
        {
            var aiScene = new Scene { RootNode = new Assimp.Node( "RootNode" ) };
            return aiScene;
        }

        /// <summary>
        /// Imports an Assimp scene with default settings.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Scene ImportScene( string path )
        {
            using ( var aiContext = new AssimpContext() )
            {
                aiContext.SetConfig( new Assimp.Configs.MeshVertexLimitConfig( 1500 ) ); // estimate
                aiContext.SetConfig( new Assimp.Configs.MeshTriangleLimitConfig( 3000 ) ); // estimate
                aiContext.SetConfig( new Assimp.Configs.VertexCacheSizeConfig( 63 ) ); // PS3/RSX vertex cache size
                aiContext.SetConfig( new Assimp.Configs.FBXPreservePivotsConfig( false ) );

                // Apply ALL the optimizations
                var postProcessSteps = Assimp.PostProcessSteps.ImproveCacheLocality | Assimp.PostProcessSteps.FindInvalidData | Assimp.PostProcessSteps.FlipUVs | Assimp.PostProcessSteps.JoinIdenticalVertices |
                                       Assimp.PostProcessSteps.LimitBoneWeights | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.GenerateSmoothNormals | Assimp.PostProcessSteps.OptimizeMeshes | Assimp.PostProcessSteps.CalculateTangentSpace;

                var aiScene = aiContext.ImportFile( path, postProcessSteps );
                return aiScene;
            }
        }

        /// <summary>
        /// Determines the best target scene node for the geometry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aiMesh"></param>
        /// <param name="aiNode"></param>
        /// <param name="nodeSearchFunc"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T DetermineBestTargetNode<T>( Assimp.Mesh aiMesh, Assimp.Node aiNode, Func<string, T> nodeSearchFunc, T fallback )
        {
            if ( aiMesh.BoneCount > 1 )
            {
                // Select node to which the geometry is weighted most
                var boneWeightCoverage = CalculateBoneWeightCoverage( aiMesh );
                var maxCoverage = boneWeightCoverage.Max( x => x.Coverage );
                var bestTargetBone = boneWeightCoverage.First( x => x.Coverage == maxCoverage ).Bone;
                return nodeSearchFunc( bestTargetBone.Name );
            }
            else if ( aiMesh.BoneCount == 1 )
            {
                // Use our only bone as the target node
                return nodeSearchFunc( aiMesh.Bones[0].Name );
            }
            else
            {
                // Try to find a parent of the geometry's ainode that exists within the existing hierarchy
                var aiNodeParent = aiNode.Parent;
                while ( aiNodeParent != null )
                {
                    var nodeParent = nodeSearchFunc( aiNodeParent.Name );
                    if ( nodeParent != null )
                        return nodeParent;

                    aiNodeParent = aiNodeParent.Parent;
                }

                // Return fallback
                return fallback;
            }
        }

        public static Assimp.Node GetHierarchyRootNode( Assimp.Node aiSceneRootNode )
        {
            // Pretty naiive for now.
            return aiSceneRootNode.Children.Single( x => !x.HasMeshes );
        }

        public static List<Assimp.Node> GetMeshNodes( Assimp.Node aiSceneRootNode )
        {
            var meshNodes = new List<Assimp.Node>();

            void FindMeshNodesRecursively( Assimp.Node aiParentNode )
            {
                foreach ( var aiNode in aiParentNode.Children )
                {
                    if ( aiNode.HasMeshes )
                        meshNodes.Add( aiNode );
                    else
                        FindMeshNodesRecursively( aiNode );
                }
            }

            FindMeshNodesRecursively( aiSceneRootNode );

            return meshNodes;
        }

        /// <summary>
        /// Calculate the weight coverage (in percent) of each bone in the geometry.
        /// </summary>
        /// <param name="aiMesh"></param>
        /// <returns></returns>
        public static List<(float Coverage, Assimp.Bone Bone)> CalculateBoneWeightCoverage( Assimp.Mesh aiMesh )
        {
            var boneScores = new List<(float Coverage, Assimp.Bone Bone)>();

            foreach ( var bone in aiMesh.Bones )
            {
                float weightTotal = 0;
                foreach ( var vertexWeight in bone.VertexWeights )
                    weightTotal += vertexWeight.Weight;

                float weightCoverage =  weightTotal / aiMesh.VertexCount ;
                boneScores.Add( (weightCoverage, bone) );
            }

            return boneScores;
        }

        /// <summary>
        /// Splits the geometry into submeshes that use <paramref name="maxBoneCount"/> or less bones.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="geometry"></param>
        /// <param name="maxBoneCount"></param>
        /// <returns></returns>
        public static List<Assimp.Mesh> SplitMeshByBoneCount( Assimp.Mesh geometry, int maxBoneCount )
        {
            if ( geometry.BoneCount <= maxBoneCount )
                return new List<Assimp.Mesh> { geometry };

            var vertexWeights = geometry.GetVertexWeights();
            var subMeshes = new List<Assimp.Mesh>();
            var remainingFaces = geometry.Faces.ToList();

            while ( remainingFaces.Count > 0 )
            {
                var usedBones = new HashSet<Assimp.Bone>();
                var faces = new List<Face>();

                // Get faces that fit inside the new geometry
                foreach ( var face in remainingFaces )
                {
                    var faceUsedBones = face.Indices.SelectMany( y => vertexWeights[y].Select( z => z.Item1 ) ).ToList();
                    var faceUniqueUsedBoneCount = faceUsedBones.Count( x => !usedBones.Contains( x ) );
                    if (  usedBones.Count + faceUniqueUsedBoneCount  > maxBoneCount )
                    {
                        // Skip
                        continue;
                    }

                    // It does fit ＼(^o^)／
                    faces.Add( face );
                    foreach ( var node in faceUsedBones )
                        usedBones.Add( node );

                    Debug.Assert( usedBones.Count <= maxBoneCount );
                }

                if ( faces.Count == 0 )
                {
                    if ( remainingFaces.All( x => x.Indices.SelectMany( y => vertexWeights[y].Select( z => z.Item1 ) ).Count() > maxBoneCount ) )
                    {
                        // Need to reduce weights per face
                        Debug.Assert( false ); // would need averaging..
                    }
                }

                // Remove the faces we claimed from the pool
                foreach ( var face in faces )
                    remainingFaces.Remove( face );

                // Build submesh
                var subMesh = new Assimp.Mesh()
                {
                    MaterialIndex = geometry.MaterialIndex,
                    //MorphMethod = geometry.MorphMethod,
                    Name = geometry.Name + $"_submesh{subMeshes.Count}",
                    PrimitiveType = geometry.PrimitiveType,
                };
                var vertexCache = new List<Vertex>();
                foreach ( var face in faces )
                {
                    var newFace = new Face();

                    foreach ( var index in face.Indices )
                    {
                        var cacheIndex = FindVertexCacheIndex( geometry, index, vertexWeights, vertexCache, out var vertex );
                        if ( cacheIndex == -1 )
                        {
                            // This vertex is new
#if DEBUG
                            for ( int i = 0; i < vertex.Weights.Count; i++ )
                                Debug.Assert( usedBones.Contains( vertex.Weights[i].Item1 ) );
#endif
                            cacheIndex = vertexCache.Count;
                            vertexCache.Add( vertex );
                        }

                        newFace.Indices.Add( cacheIndex );
                    }

                    subMesh.Faces.Add( newFace );
                }

                PopulateSubMeshVertexData( geometry, subMesh, vertexCache );

#if DEBUG
                CheckIfAllVerticesHaveWeights( subMesh );
#endif

                subMeshes.Add( subMesh );
            }

            return subMeshes;
        }

        /// <summary>
        /// Splits the geometry into submeshes that use <paramref name="maxVertexCount"/> or less vertices.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="maxVertexCount"></param>
        /// <returns></returns>
        public static List<Assimp.Mesh> SplitMeshByVertexCount( Assimp.Mesh geometry, int maxVertexCount )
        {
            if ( geometry.VertexCount <= maxVertexCount )
                return new List<Assimp.Mesh> { geometry };

            var vertexWeights = geometry.HasBones ? geometry.GetVertexWeights() : null;
            var remainingFaces = geometry.Faces.ToList();
            var subMeshes = new List<Assimp.Mesh>();

            while ( remainingFaces.Count > 0 )
            {
                // Build submesh
                var subMesh = new Assimp.Mesh()
                {
                    MaterialIndex = geometry.MaterialIndex,
                    //MorphMethod = geometry.MorphMethod,
                    Name = geometry.Name + $"_submesh{subMeshes.Count}",
                    PrimitiveType = geometry.PrimitiveType,
                };

                var processedFaces = new List<Face>();
                var vertexCache = new List<Vertex>();

                // Get faces that fit inside the new geometry
                foreach ( var face in remainingFaces )
                {
                    var newVertices = new List<Vertex>();
                    var newFace = new Face();
                    foreach ( var i in face.Indices )
                    {
                        var cacheIndex = FindVertexCacheIndex( geometry, i, vertexWeights, vertexCache, out var vertex );
                        if ( cacheIndex == -1 )
                        {
                            // This vertex is new
                            cacheIndex = vertexCache.Count + newVertices.Count;
                            newVertices.Add( vertex );
                        }

                        newFace.Indices.Add( cacheIndex );
                    }

                    if ( vertexCache.Count + newVertices.Count > maxVertexCount )
                    {
                        // Doesn't fit
                        continue;
                    }

                    // It does fit ＼(^o^)／
                    subMesh.Faces.Add( newFace );
                    processedFaces.Add( face );
                    foreach ( var vertex in newVertices )
                        vertexCache.Add( vertex );

                    Debug.Assert( vertexCache.Count <= maxVertexCount );

                    if ( vertexCache.Count == maxVertexCount )
                    {
                        // We're done for sure
                        break;
                    }
                }

                // Remove the faces we processed from the remaining list
                foreach ( var face in processedFaces )
                    remainingFaces.Remove( face );

                PopulateSubMeshVertexData( geometry, subMesh, vertexCache );

#if DEBUG
                CheckIfAllVerticesHaveWeights( subMesh );
#endif

                subMeshes.Add( subMesh );
            }

            return subMeshes;
        }

        private static int FindVertexCacheIndex( Assimp.Mesh geometry, int i, List<(Assimp.Bone, float)>[] vertexWeights, List<Vertex> vertexCache, out Vertex vertex )
        {
            var position = geometry.HasVertices ? geometry.Vertices[i] : new Vector3();
            var normal = geometry.HasNormals ? geometry.Normals[i] : new Vector3();
            var tangent = geometry.HasTangentBasis ? geometry.Tangents[i] : new Vector3();
            var texCoord = geometry.HasTextureCoords( 0 ) ? geometry.TextureCoordinateChannels[0][i] : new Vector3();
            var texCoord2 = geometry.HasTextureCoords( 1 ) ? geometry.TextureCoordinateChannels[1][i] : new Vector3();
            var color = geometry.HasVertexColors( 0 ) ? geometry.VertexColorChannels[0][i] : new Vector4();
            var weights = geometry.HasBones ? vertexWeights[i] : new List<(Assimp.Bone, float)>();
            var cacheIndex = vertexCache.FindIndex( y => y.Position == position && y.Normal == normal && y.Tangent == tangent && y.TexCoord == texCoord &&
                                                         y.TexCoord2 == texCoord2 && y.Color == color &&
                                                         y.Weights.SequenceEqual( weights ) );

            vertex = new Vertex( position, normal, tangent, texCoord, texCoord2, color, weights );
            return cacheIndex;
        }

        private static void PopulateSubMeshVertexData( Assimp.Mesh geometry, Assimp.Mesh subMesh, List<Vertex> vertexCache )
        {
            var vertexIndex = 0;
            foreach ( var vertex in vertexCache )
            {
                // Split the vertex data from the cache
                if ( geometry.HasVertices )
                    subMesh.Vertices.Add( vertex.Position );

                if ( geometry.HasNormals )
                    subMesh.Normals.Add( vertex.Normal );

                if ( geometry.HasTangentBasis )
                    subMesh.Tangents.Add( vertex.Tangent );

                if ( geometry.HasTextureCoords( 0 ) )
                    subMesh.TextureCoordinateChannels[0].Add( vertex.TexCoord );

                if ( geometry.HasTextureCoords( 1 ) )
                    subMesh.TextureCoordinateChannels[1].Add( vertex.TexCoord2 );

                if ( geometry.HasVertexColors( 0 ) )
                    subMesh.VertexColorChannels[0].Add( vertex.Color );

                if ( geometry.HasBones )
                {
                    foreach ( var boneWeight in vertex.Weights )
                    {
                        var subMeshBone = subMesh.Bones.FirstOrDefault( x => x.Name == boneWeight.Bone.Name );
                        if ( subMeshBone == null )
                        {
                            subMeshBone = new Assimp.Bone
                            {
                                Name = boneWeight.Bone.Name,
                                OffsetMatrix = boneWeight.Bone.OffsetMatrix
                            };
                            subMesh.Bones.Add( subMeshBone );
                        }

                        subMeshBone.VertexWeights.Add( new Assimp.VertexWeight( vertexIndex, boneWeight.Weight ) );
                    }
                }

                ++vertexIndex;
            }
        }

        private static void CheckIfAllVerticesHaveWeights( Assimp.Mesh subMesh )
        {
            if ( subMesh.HasBones )
            {
                var vertexIndices = Enumerable.Range( 0, subMesh.VertexCount ).ToList();
                foreach ( var bone in subMesh.Bones )
                {
                    foreach ( var vertexWeight in bone.VertexWeights )
                        vertexIndices.Remove( vertexWeight.VertexID );
                }

                Debug.Assert( vertexIndices.Count == 0 );
            }
        }

        private struct Vertex
        {
            public readonly Vector3 Position;
            public readonly Vector3 Normal;
            public readonly Vector3 Tangent;
            public readonly Vector3 TexCoord;
            public readonly Vector3 TexCoord2;
            public readonly Vector4 Color;
            public readonly List<(Assimp.Bone Bone, float Weight)> Weights;

            public Vertex( Vector3 position, Vector3 normal, Vector3 tangent, Vector3 texCoord, Vector3 texCoord2, Vector4 color, List<(Assimp.Bone, float)> weights )
            {
                Position = position;
                Normal = normal;
                Tangent = tangent;
                TexCoord = texCoord;
                TexCoord2 = texCoord2;
                Color = color;
                Weights = weights;
            }
        }
    }
}
