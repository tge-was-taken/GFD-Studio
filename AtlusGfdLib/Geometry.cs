using System;
using System.Numerics;

namespace AtlusGfdLib
{
    public class Geometry
    {
        public GeometryFlags Flags { get; set; }

        public VertexAttributeFlags VertexAttributes { get; set; }

        public int TriangleCount => Triangles.Length;

        public short TriangleIndexBase { get; set; }

        public Triangle[] Triangles { get; set; }

        public int VertexCount => Vertices.Length;

        public int Field14 { get; set; }

        // Vertex attributes
        public Vector3[] Vertices { get; set; }

        public Vector3[] Normals { get; set; }

        public Vector3[] Tangents { get; set; }

        public Vector3[] Binormals { get; set; }

        public uint[] Colors { get; set; }

        public Vector2[][] TexCoords { get; }

        public VertexWeight[] VertexWeights { get; set; }
    }

    [Flags]
    public enum GeometryFlags : uint
    {
        HasVertexWeights  = 1 << 0,
        HasMaterial       = 1 << 1,
        HasTriangles      = 1 << 2,
        HasBoundingBox    = 1 << 3,
        HasBoundingSphere = 1 << 4,
        Flag20            = 1 << 5, // render flag
        Flag40            = 1 << 6,
        Flag80            = 1 << 7, // render flag
        Flag100           = 1 << 8, // 2 floats
        Flag4000          = 1 << 14, // render flag
        Flag40000000      = 1 << 30, // r7 |= 8
    }

    [Flags]
    public enum VertexAttributeFlags : uint
    {
        Normal       = 1 << 1,
        Flag10       = 1 << 4, // 12 bytes, after normals
        Color0       = 1 << 6,
        Color1       = 1 << 7,
        TexCoord0    = 1 << 8,
        TexCoord1    = 1 << 9,
        TexCoord2    = 1 << 10,
        TexCoord3    = 1 << 11,
        TexCoord4    = 1 << 12,
        TexCoord5    = 1 << 13,
        TexCoord6    = 1 << 14,
        TexCoord7    = 1 << 15,
        Tangent      = 1 << 28,
        Flag20000000 = 1 << 29, // 12 bytes, after tangent -- binormal?
        Flag40000000 = 1 << 30, // 4 bytes, after tex coord 2
        Flag80000000 = 1u << 31, // 20 bytes, after Flag40000000
    }
}