using System;
using System.Numerics;

namespace AtlusGfdLib
{
    public class Geometry
    {
        public GeometryFlags Flags { get; set; }

        public VertexAttributeFlags VertexAttributeFlags { get; set; }

        public int TriangleCount => Triangles.Length;

        public TriangleIndexType TriangleIndexType { get; set; }

        public Triangle[] Triangles { get; set; }

        public int VertexCount => Vertices.Length;

        public int Field14 { get; set; }

        // Vertex attributes
        public Vector3[] Vertices { get; set; }

        public Vector3[] Normals { get; set; }

        public Vector3[] Tangents { get; set; }

        public Vector3[] Binormals { get; set; }

        public uint[] ColorChannel0 { get; set; }

        public Vector2[] TexCoordsChannel0 { get; set; }

        public Vector2[] TexCoordsChannel1 { get; set; }

        public Vector2[] TexCoordsChannel2 { get; set; }

        public uint[] ColorChannel1 { get; set; }

        public VertexWeight[] VertexWeights { get; set; }

        public string MaterialName { get; set; }

        public BoundingBox? BoundingBox { get; set; }

        public BoundingSphere? BoundingSphere { get; set; }

        public float FieldD4 { get; set; }

        public float FieldD8 { get; set; }
    }

    public enum TriangleIndexType
    {
        None = 0,
        UInt16 = 1,
        UInt32 = 2
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
        Flag100           = 1 << 8,
        Flag200           = 1 << 9,
        Flag400           = 1 << 10,
        Flag800           = 1 << 11,
        Flag1000          = 1 << 12, // 2 floats
        Flag2000          = 1 << 13,
        Flag4000          = 1 << 14, // render flag
        Flag8000          = 1 << 15,
        Flag10000         = 1 << 16,
        Flag20000         = 1 << 17,
        Flag40000         = 1 << 18,
        Flag80000         = 1 << 19,
        Flag100000        = 1 << 20,
        Flag200000        = 1 << 21,
        Flag400000        = 1 << 22,
        Flag800000        = 1 << 23,
        Flag1000000       = 1 << 24,
        Flag2000000       = 1 << 25,
        Flag4000000       = 1 << 26,
        Flag8000000       = 1 << 27,
        Flag10000000      = 1 << 28,
        Flag20000000      = 1 << 29,
        Flag40000000      = 1 << 30, // r7 |= 8
        Flag80000000      = 1u << 31,
    }

    [Flags]
    public enum VertexAttributeFlags : uint
    {
        Position     = 1 << 1,
        Flag4        = 1 << 2,
        Flag8        = 1 << 3,
        Normal       = 1 << 4, // might be normals. maybe normal should be position
        Color0       = 1 << 6,
        Flag40       = 1 << 7,
        TexCoord0    = 1 << 8,
        TexCoord1    = 1 << 9,
        TexCoord2    = 1 << 10,
        TexCoord3    = 1 << 11,
        TexCoord4    = 1 << 12,
        TexCoord5    = 1 << 13,
        TexCoord6    = 1 << 14,
        TexCoord7    = 1 << 15,
        Flag10000    = 1 << 16,
        Flag20000    = 1 << 17,
        Flag40000    = 1 << 18,
        Flag80000    = 1 << 19,
        Flag100000   = 1 << 20,
        Flag200000   = 1 << 21,
        Flag400000   = 1 << 22,
        Flag800000   = 1 << 23,
        Flag1000000  = 1 << 24,
        Flag2000000  = 1 << 25,
        Flag4000000  = 1 << 26,
        Flag8000000  = 1 << 27,
        Tangent      = 1 << 28,
        Binormal     = 1 << 29, // 12 bytes, after tangent -- binormal?
        Color1       = 1 << 30, // 4 bytes, after tex coord 2
        Flag80000000 = 1u << 31, // 20 bytes, after Flag40000000
    }
}