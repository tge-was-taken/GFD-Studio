namespace GFDLibrary.Textures.GNF
{
    /// <summary>
    /// Specifies how the pixels of a surface are ordered in memory.
    /// </summary>
    public enum TileMode
    {
        // Depth modes (for depth buffers)
        /// <summary> Recommended for depth targets with one fragment per pixel.</summary>
        Depth_2DThin64 = 0x00000000,
        /// <summary> Recommended for depth targets with two or four fragments per pixel, or texture-readable.</summary>
        Depth_2DThin128 = 0x00000001,
        /// <summary> Recommended for depth targets with eight fragments per pixel.</summary>
        Depth_2DThin256 = 0x00000002,
        /// <summary> Recommended for depth targets with 512-byte tiles.</summary>
        Depth_2DThin512 = 0x00000003,
        /// <summary> Recommended for depth targets with 1024-byte tiled.</summary>
        Depth_2DThin_1K = 0x00000004,
        /// <summary> Not used; included only for completeness.</summary>
        Depth_1DThin = 0x00000005,
        /// <summary> Recommended for partially-resident depth surfaces. Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Depth_2DThinPrt_256 = 0x00000006,
        /// <summary> Not used; included only for completeness.</summary>
        Depth_2DThinPrt_1K = 0x00000007,
        // Display modes</summary>
        /// <summary> Recommended for any surface to be easily accessed on the CPU.</summary>
        Display_LinearAligned = 0x00000008,
        /// <summary> Not used; included only for completeness.</summary>
        Display_1DThin = 0x00000009,
        /// <summary> Recommended mode for displayable render targets.</summary>
        Display_2DThin = 0x0000000A,
        /// <summary> Supports aliasing multiple virtual texture pages to the same physical page.</summary>
        Display_ThinPrt = 0x0000000B,
        /// <summary> Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Display_2DThinPrt = 0x0000000C,
        // Thin modes (for non-displayable 1D/2D/3D surfaces)</summary>
        /// <summary> Recommended for read-only non-volume textures.</summary>
        Thin_1DThin = 0x0000000D,
        /// <summary> Recommended for non-displayable intermediate render targets and read/write non-volume textures.</summary>
        Thin_2DThin = 0x0000000E,
        /// <summary> Not used; included only for completeness.</summary>
        Thin_3DThin = 0x0000000F,
        /// <summary> Recommended for partially-resident textures (PRTs). Supports aliasing multiple virtual texture pages to the same physical page.</summary>
        Thin_ThinPrt = 0x00000010,
        /// <summary> Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Thin_2DThinPrt = 0x00000011,
        /// <summary> Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Thin_3DThinPrt = 0x00000012,
        // Thick modes (for 3D textures)</summary>
        /// <summary> Recommended for read-only volume textures.</summary>
        Thick_1DThick = 0x00000013,
        /// <summary> Recommended for volume textures to which pixel shaders will write.</summary>
        Thick_2DThick = 0x00000014,
        /// <summary> Not used; included only for completeness.</summary>
        Thick_3DThick = 0x00000015,
        /// <summary> Supports aliasing multiple virtual texture pages to the same physical page.</summary>
        Thick_ThickPrt = 0x00000016,
        /// <summary> Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Thick_2DThickPrt = 0x00000017,
        /// <summary> Does not support aliasing multiple virtual texture pages to the same physical page.</summary>
        Thick_3DThickPrt = 0x00000018,
        /// <summary> Recommended for volume textures to which pixel shaders will write.</summary>
        Thick_2DXThick = 0x00000019,
        /// <summary> Not used; included only for completeness.</summary>
        Thick_3DXThick = 0x0000001A,
        // Hugely inefficient linear display mode -- do not use!
        /// <summary> Unsupported; do not use!</summary>
        Display_LinearGeneral = 0x0000001F,
    }
}