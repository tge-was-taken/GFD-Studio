namespace AtlusGfdLib
{
    internal enum ChunkType : uint
    {
        ChunkListEnd            = 0,
        ChunkListStart          = 1,
        ShaderCache             = 2,
        Scene                   = 0x00010003,
        MaterialDictionary      = 0x000100FB,
        TextureDictionary       = 0x000100FC,
        AnimationList           = 0x000100FD
    }
}

