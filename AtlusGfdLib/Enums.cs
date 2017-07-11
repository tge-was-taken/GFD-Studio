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

    public enum TextureFormat : ushort
    {
        Invalid = 0,
        DDS = 1,
    }
}
