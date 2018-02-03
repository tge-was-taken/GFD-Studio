namespace AtlusGfdLibrary.IO.Resource
{
    internal struct ResourceFileHeader
    {
        public string Magic;
        public uint Version;
        public ResourceFileType Type;
        public int Unknown;

        public const int    SIZE              = 16;
        public const string MAGIC_FS          = "GFS0";
        public const string MAGIC_SHADERCACHE = "GSC0";
    }

    internal enum ResourceFileType
    {
        ModelResourceBundle = 1,
        ShaderCachePS3 = 2,
        ShaderCachePSP2 = 4,

        // Custom ones
        CustomGenericResourceBundle = 'R' << 24 | 'I' << 16 | 'G' << 8 | 0,
        CustomTextureDictionary,
        CustomTexture,
        CustomMaterialDictionary,
        CustomMaterial,
        CustomTextureMap,
        CustomMaterialAttribute,    
    }
}
