using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlusGfdLib
{
    public enum AnimationControllerType
    {
        Invalid = 0,
        PRS = 1,
        Material = 2,
        Camera = 3,
        Morph = 4
    }

    [Flags]
    public enum MaterialFlags
    {
        HasProperties    = 1 << 16,
        HasDiffuseMap    = 1 << 20,
        HasNormalMap     = 1 << 21,
        HasSpecularMap   = 1 << 22,
        HasReflectionMap = 1 << 23,
        HasHighlightMap  = 1 << 24,
        HasGlowMap       = 1 << 25,
        HasNightMap      = 1 << 26,
        HasDetailMap     = 1 << 27,
        HasShadowMap     = 1 << 28,
    }

    [Flags]
    public enum MeshVertexFlags
    {
        Normal = 1 << 1,
        Color = 1 << 6,
        Texture1 = 1 << 8,
        Texture2 = 1 << 9,
        Texture3 = 1 << 10,
        Texture4 = 1 << 11,
        Texture5 = 1 << 12,
        Texture6 = 1 << 13,
        Texture7 = 1 << 14,
        Texture8 = 1 << 15,
        Tangent = 1 << 28
    }

    public enum NodeAttachmentType
    {
        Mesh = 4,
        Camera = 5,
        Sky = 6,
        Particle = 7,
        Morpher = 9,
    }

    public enum ResourceType
    {
        Invalid = 0,
        Model,
        AnimationList,
        TextureDictionary,
        MaterialDictionary,
        Scene,
        ShaderCache,
    }

    [Flags]
    public enum SceneFlags
    {
        BoundingBox = 1 << 0,
        Skinning = 1 << 2,
        Morphers = 1 << 3
    }

    public enum UserPropertyType
    {
        None = 0,
        Int = 1,
        Float = 2,
        Bool = 3,
        String = 4
    }

    public enum TextureFormat : ushort
    {
        Invalid = 0,
        DDS = 1,
    }
}
