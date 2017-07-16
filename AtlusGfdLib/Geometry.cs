using System;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Geometry
    {
        public GeometryFlags Flags { get; set; }

        public VertexAttributeFlags VertexAttributeFlags { get; set; }

        public int TriangleCount => Triangles != null ? Triangles.Length : 0;

        public TriangleIndexType TriangleIndexType { get; set; }

        public int VertexCount => Vertices.Length;

        public int Field14 { get; set; }

        // Vertex attributes
        private Vector3[] mVertices;
        public Vector3[] Vertices
        {
            get => mVertices;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Position;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Position;

                mVertices = value;
            }
        }

        private Vector3[] mNormals;
        public Vector3[] Normals
        {
            get => mNormals;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Normal;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Normal;

                mNormals = value;
            }
        }

        private Vector3[] mTangents;
        public Vector3[] Tangents
        {
            get => mTangents;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Tangent;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Tangent;

                mTangents = value;
            }
        }

        private Vector3[] mBinormals;
        public Vector3[] Binormals
        {
            get => mBinormals;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Binormal;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Binormal;

                mBinormals = value;
            }
        }

        private uint[] mColorChannel0;
        public uint[] ColorChannel0
        {
            get => mColorChannel0;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Color0;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Color0;

                mColorChannel0 = value;
            }
        }

        private Vector2[] mTexCoordsChannel0;
        public Vector2[] TexCoordsChannel0
        {
            get => mTexCoordsChannel0;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.TexCoord0;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.TexCoord0;

                mTexCoordsChannel0 = value;
            }
        }

        private Vector2[] mTexCoordsChannel1;
        public Vector2[] TexCoordsChannel1
        {
            get => mTexCoordsChannel1;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.TexCoord1;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.TexCoord1;

                mTexCoordsChannel1 = value;
            }
        }

        private Vector2[] mTexCoordsChannel2;
        public Vector2[] TexCoordsChannel2
        {
            get => mTexCoordsChannel2;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.TexCoord2;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.TexCoord2;

                mTexCoordsChannel2 = value;
            }
        }

        private uint[] mColorChannel1;
        public uint[] ColorChannel1
        {
            get => mColorChannel1;
            set
            {
                if ( value != null )
                    VertexAttributeFlags |= VertexAttributeFlags.Color1;
                else
                    VertexAttributeFlags &= ~VertexAttributeFlags.Color1;

                mColorChannel1 = value;
            }
        }

        private VertexWeight[] mVertexWeights;
        public VertexWeight[] VertexWeights
        {
            get => mVertexWeights;
            set
            {
                if ( value != null )
                    Flags |= GeometryFlags.HasVertexWeights;
                else
                    Flags &= ~GeometryFlags.HasVertexWeights;

                mVertexWeights = value;
            }
        }

        private Triangle[] mTriangles;
        public Triangle[] Triangles
        {
            get => mTriangles;
            set
            {
                if ( value != null )
                    Flags |= GeometryFlags.HasTriangles;
                else
                    Flags &= ~GeometryFlags.HasTriangles;

                mTriangles = value;
            }
        }

        private MorphTargetList mMorphTargets;
        public MorphTargetList MorphTargets
        {
            get => mMorphTargets;
            set
            {
                if ( value != null )
                    Flags |= GeometryFlags.HasMorphTargets;
                else
                    Flags &= ~GeometryFlags.HasMorphTargets;

                mMorphTargets = value;
            }
        }

        private string mMaterialName;
        public string MaterialName
        {
            get => mMaterialName;
            set
            {
                if ( !string.IsNullOrEmpty( value ) )
                    Flags |= GeometryFlags.HasMaterial;
                else
                    Flags &= ~GeometryFlags.HasMaterial;

                mMaterialName = value;
            }
        }

        private BoundingBox? mBoundingBox;
        public BoundingBox? BoundingBox
        {
            get => mBoundingBox;
            set
            {
                if ( value != null )
                    Flags |= GeometryFlags.HasBoundingBox;
                else
                    Flags &= ~GeometryFlags.HasBoundingBox;

                mBoundingBox = value;
            }
        }

        private BoundingSphere? mBoundingSphere;
        public BoundingSphere? BoundingSphere
        {
            get => mBoundingSphere;
            set
            {
                if ( value != null )
                    Flags |= GeometryFlags.HasBoundingSphere;
                else
                    Flags &= ~GeometryFlags.HasBoundingSphere;

                mBoundingSphere = value;
            }
        }

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
        HasMorphTargets   = 1 << 6,
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