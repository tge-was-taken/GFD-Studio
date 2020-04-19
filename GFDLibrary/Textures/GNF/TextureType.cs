namespace GFDLibrary.Textures.GNF
{
    public enum TextureType
    {
        /// <summary>One-dimensional texture.</summary>
        Type1D = 0x00000008,
        /// <summary>Two-dimensional texture.</summary>
        Type2D = 0x00000009,
        /// <summary>Three-dimensional volume texture.</summary>
        Type3D = 0x0000000A,
        /// <summary>Cubic environment map texture (six 2D textures arranged in a cube and indexed by a 3D direction vector). This TextureType is also used for cubemap arrays.</summary>
        Cubemap = 0x0000000B,
        /// <summary>Array of 1D textures.</summary>
        Type1DArray = 0x0000000C,
        /// <summary>Array of 2D textures.</summary>
        Type2DArray = 0x0000000D,
        /// <summary>Two-dimensional texture with multiple samples per pixel. Usually an alias into an MSAA render target.</summary>
        Type2DMsaa = 0x0000000E,
        /// <summary>Array of 2D MSAA textures.</summary>
        Type2DArrayMsaa = 0x0000000F 
    }
}