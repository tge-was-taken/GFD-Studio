namespace GFDLibrary
{
    public enum ResourceType
    {
        Invalid,
        Model,
        ShaderCachePS3,
        ShaderCachePSP2 = 4,
        ShaderCachePS4 = 6,

        // These are all used as intermediary output formats
        TextureMap = 'R' << 24 | 'I' << 16 | 'G' << 8,
        TextureDictionary,
        Texture,
        ShaderPS3,
        ShaderPSP2,
        ShaderPS4,
        Scene,
        Node,
        UserPropertyCollection,
        Morph,
        MorphTarget,
        MorphTargetList,
        MaterialDictionary,
        ChunkType000100F9,
        ChunkType000100F8,
        AnimationPack,
        MaterialAttribute,
        Material,
        Light,
        Geometry,
        Camera,
        Epl,
        EplLeaf,
        Animation,
        AnimationExtraData,
        KeyframeTrack,
        AnimationController
    }
}