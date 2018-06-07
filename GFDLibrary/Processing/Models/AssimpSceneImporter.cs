using Ai = Assimp;

namespace GFDLibrary.IO.Assimp
{
    public static class AssimpSceneImporter
    {
        public static Ai.Scene ImportFile( string filePath)
        {
            // Set up Assimp context
            var aiContext = new Ai.AssimpContext();
            aiContext.SetConfig( new Ai.Configs.MeshVertexLimitConfig( 1500 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.MeshTriangleLimitConfig( 3000 ) ); // estimate
            aiContext.SetConfig( new Ai.Configs.VertexCacheSizeConfig( 63 ) ); // PS3/RSX vertex cache size

            // Apply ALL the optimizations
            var postProcessSteps = Ai.PostProcessSteps.ImproveCacheLocality | Ai.PostProcessSteps.FindInvalidData | Ai.PostProcessSteps.FlipUVs | Ai.PostProcessSteps.JoinIdenticalVertices |
                                   Ai.PostProcessSteps.LimitBoneWeights | Ai.PostProcessSteps.Triangulate | Ai.PostProcessSteps.GenerateSmoothNormals | Ai.PostProcessSteps.OptimizeMeshes;

            var aiScene = aiContext.ImportFile( filePath, postProcessSteps );

            return aiScene;
        }
    }
}
