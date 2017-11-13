using Ai = Assimp;

namespace AtlusGfdLib.Assimp
{
    public static class AssimpImporter
    {
        public static Ai.Scene ImportFile( string filePath )
        {
            // Set up Assimp context
            var aiContext = new Ai.AssimpContext();
            aiContext.SetConfig( new Ai.Configs.MeshVertexLimitConfig( 1500 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.MeshTriangleLimitConfig( 3000 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.VertexCacheSizeConfig( 63 ) ); // PS3/RSX vertex cache size

            // Apply ALL the optimizations
            var aiScene = aiContext.ImportFile( filePath,
                                                Ai.PostProcessSteps.ImproveCacheLocality | Ai.PostProcessSteps.FindInvalidData | Ai.PostProcessSteps.FlipUVs | Ai.PostProcessSteps.JoinIdenticalVertices |
                                                Ai.PostProcessSteps.LimitBoneWeights | Ai.PostProcessSteps.Triangulate | Ai.PostProcessSteps.GenerateSmoothNormals | Ai.PostProcessSteps.OptimizeMeshes );

            return aiScene;
        }
    }
}
