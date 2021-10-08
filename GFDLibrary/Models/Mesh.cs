using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using GFDLibrary.Common;
using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class Mesh : Resource
    {
        public override ResourceType ResourceType => ResourceType.Mesh;

        public GeometryFlags Flags { get; set; }

        public VertexAttributeFlags VertexAttributeFlags { get; set; }

        public int TriangleCount => Triangles?.Length ?? 0;

        public TriangleIndexFormat TriangleIndexFormat { get; set; }

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

        public Vector2[][] TexCoordChannels => new[] { mTexCoordsChannel0, mTexCoordsChannel1, mTexCoordsChannel2 };
        public uint[][] ColorChannels => new[] { mColorChannel0, mColorChannel1 };

        public Mesh()
        {
            TriangleIndexFormat = TriangleIndexFormat.UInt16;
        }

        public Mesh(uint version):base(version)
        {
            
        }

        /// <summary>
        /// Transforms the vertices using the bone weights while utilizing the inverse bind matrices from the bone palette.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodes"></param>
        /// <param name="usedBones"></param>
        /// <returns></returns>
        public (Vector3[] Vertices, Vector3[] Normals) Transform( Node parentNode, IList<Node> nodes, List<Bone> usedBones,
            bool includeWeights = true, bool includeParentTransform = false, Matrix4x4? offsetTransform = null )
        {
            var vertices = new Vector3[VertexCount];
            Vector3[] normals = null;

            if ( Normals != null )
                normals = new Vector3[VertexCount];

            var parentNodeWorldTransform = parentNode.WorldTransform;
            Matrix4x4.Invert(parentNodeWorldTransform, out var parentNodeWorldTransformInv );

            for ( int i = 0; i < VertexCount; i++ )
            {
                var position = Vertices[i];
                var normal = Normals?[i] ?? Vector3.Zero;
                var newPosition = Vector3.Zero;
                var newNormal = Vector3.Zero;

                if (includeWeights)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        var weight = VertexWeights[i].Weights[j];
                        if (weight == 0)
                            continue;

                        var boneIndex = VertexWeights[i].Indices[j];
                        var boneNodeIndex = usedBones[boneIndex].NodeIndex;
                        var boneNode = nodes[boneNodeIndex];
                        var inverseBindMatrix = usedBones[boneIndex].InverseBindMatrix;
                        var bindMatrix = boneNode.WorldTransform;
                        newPosition += Vector3.Transform(Vector3.Transform(position, inverseBindMatrix), bindMatrix * weight);
                        newNormal += Vector3.TransformNormal(Vector3.TransformNormal(normal, inverseBindMatrix), bindMatrix * weight);
                    }

                    if (!includeParentTransform)
                    {
                        newPosition = Vector3.Transform(newPosition, parentNodeWorldTransformInv);

                        if (normals != null)
                            newNormal =
                                Vector3.Normalize(Vector3.TransformNormal(newNormal, parentNodeWorldTransformInv));
                    }
                }
                else
                {
                    newPosition = position;
                    newNormal = normal;

                    if (includeParentTransform)
                    {
                        newPosition = Vector3.Transform(newPosition, parentNodeWorldTransform );

                        if (normals != null)
                            newNormal =
                                Vector3.Normalize(Vector3.TransformNormal(newNormal, parentNodeWorldTransform));

                    }
                }

                if (offsetTransform != null)
                {
                    newPosition = Vector3.Transform(newPosition, offsetTransform.Value);

                    if (normals != null)
                        newNormal =
                            Vector3.Normalize(Vector3.TransformNormal(newNormal, offsetTransform.Value));
                }

                vertices[i] = newPosition;
                if (normals != null)
                    normals[i] = newNormal;
            }

            return (vertices, normals);
        }

        internal override void Read( ResourceReader reader, long endPosition = -1 )
        {
            var flags = ( GeometryFlags )reader.ReadInt32();
            Flags = flags;
            VertexAttributeFlags = ( VertexAttributeFlags )reader.ReadInt32();

            int triangleCount = 0;

            if ( Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                triangleCount = reader.ReadInt32();
                TriangleIndexFormat = ( TriangleIndexFormat )reader.ReadInt16();
                Triangles = new Triangle[triangleCount];
            }

            int vertexCount = reader.ReadInt32();

            if ( Version > 0x1103020 )
            {
                Field14 = reader.ReadInt32();
            }

            AllocateBuffers( vertexCount );

            for ( int i = 0; i < vertexCount; i++ )
            {
                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                    Vertices[i] = reader.ReadVector3();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
                    Normals[i] = reader.ReadVector3();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
                    Tangents[i] = reader.ReadVector3();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
                    Binormals[i] = reader.ReadVector3();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
                    ColorChannel0[i] = reader.ReadUInt32();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
                    TexCoordsChannel0[i] = reader.ReadVector2();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                    TexCoordsChannel1[i] = reader.ReadVector2();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                    TexCoordsChannel2[i] = reader.ReadVector2();

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                    ColorChannel1[i] = reader.ReadUInt32();

                if ( Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                {
                    const int MAX_NUM_WEIGHTS = 4;
                    VertexWeights[i].Weights = new float[MAX_NUM_WEIGHTS];

                    for ( int j = 0; j < VertexWeights[i].Weights.Length; j++ )
                    {
                        VertexWeights[i].Weights[j] = reader.ReadSingle();
                    }

                    VertexWeights[i].Indices = new byte[MAX_NUM_WEIGHTS];
                    uint indices = reader.ReadUInt32();

                    for ( int j = 0; j < VertexWeights[i].Indices.Length; j++ )
                    {
                        int shift = j * 8;
                        VertexWeights[i].Indices[j] = ( byte )( ( indices & ( 0xFF << shift ) ) >> shift );
                    }
                }
            }

            if ( Flags.HasFlag( GeometryFlags.HasMorphTargets ) )
            {
                MorphTargets = reader.ReadResource<MorphTargetList>( Version );
            }

            if ( Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                for ( int i = 0; i < Triangles.Length; i++ )
                {
                    switch ( TriangleIndexFormat )
                    {
                        case TriangleIndexFormat.UInt16:
                            Triangles[i].A = reader.ReadUInt16();
                            Triangles[i].B = reader.ReadUInt16();
                            Triangles[i].C = reader.ReadUInt16();
                            break;
                        case TriangleIndexFormat.UInt32:
                            Triangles[i].A = reader.ReadUInt32();
                            Triangles[i].B = reader.ReadUInt32();
                            Triangles[i].C = reader.ReadUInt32();
                            break;
                        default:
                            throw new Exception( $"Unsupported triangle index type: {TriangleIndexFormat}" );
                    }
                }
            }

            if ( Flags.HasFlag( GeometryFlags.HasMaterial ) )
            {
                MaterialName = reader.ReadStringWithHash( Version );
            }

            if ( Flags.HasFlag( GeometryFlags.HasBoundingBox ) )
            {
                BoundingBox = reader.ReadBoundingBox();
            }

            if ( Flags.HasFlag( GeometryFlags.HasBoundingSphere ) )
            {
                BoundingSphere = reader.ReadBoundingSphere();
            }

            if ( Flags.HasFlag( GeometryFlags.Flag1000 ) )
            {
                FieldD4 = reader.ReadSingle();
                FieldD8 = reader.ReadSingle();
            }

            Trace.Assert( Flags == flags, "Mesh flags don't match flags in file" );
        }

        private void AllocateBuffers( int vertexCount )
        {
            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
            {
                Vertices = new Vector3[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
            {
                Normals = new Vector3[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
            {
                Tangents = new Vector3[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
            {
                Binormals = new Vector3[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
            {
                ColorChannel0 = new uint[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
            {
                TexCoordsChannel0 = new Vector2[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
            {
                TexCoordsChannel1 = new Vector2[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
            {
                TexCoordsChannel2 = new Vector2[vertexCount];
            }

            if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
            {
                ColorChannel1 = new uint[vertexCount];
            }

            if ( Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
            {
                VertexWeights = new VertexWeight[vertexCount];
            }
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( ( int )Flags );
            writer.WriteInt32( ( int )VertexAttributeFlags );

            if ( Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                writer.WriteInt32( TriangleCount );
                writer.WriteInt16( ( short )TriangleIndexFormat );
            }

            writer.WriteInt32( VertexCount );

            if ( Version > 0x1103020 )
            {
                writer.WriteInt32( Field14 );
            }

            for ( int i = 0; i < VertexCount; i++ )
            {
                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Position ) )
                    writer.WriteVector3( Vertices[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Normal ) )
                    writer.WriteVector3( Normals[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Tangent ) )
                    writer.WriteVector3( Tangents[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Binormal ) )
                    writer.WriteVector3( Binormals[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color0 ) )
                    writer.WriteUInt32( ColorChannel0[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord0 ) )
                    writer.WriteVector2( TexCoordsChannel0[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord1 ) )
                    writer.WriteVector2( TexCoordsChannel1[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.TexCoord2 ) )
                    writer.WriteVector2( TexCoordsChannel2[i] );

                if ( VertexAttributeFlags.HasFlag( VertexAttributeFlags.Color1 ) )
                    writer.WriteUInt32( ColorChannel1[i] );

                if ( Flags.HasFlag( GeometryFlags.HasVertexWeights ) )
                {
                    var vertexWeight = VertexWeights[i];
                    for ( int j = 0; j < 4; j++ )
                        writer.WriteSingle( vertexWeight.Weights[j] );

                    int indices = vertexWeight.Indices[0] << 00 | ( vertexWeight.Indices[1] << 08 ) |
                                  vertexWeight.Indices[2] << 16 | ( vertexWeight.Indices[3] << 24 );

                    writer.WriteInt32( indices );
                }
            }

            if ( Flags.HasFlag( GeometryFlags.HasMorphTargets ) )
            {
                writer.WriteResource( MorphTargets );
            }

            if ( Flags.HasFlag( GeometryFlags.HasTriangles ) )
            {
                for ( int i = 0; i < Triangles.Length; i++ )
                {
                    switch ( TriangleIndexFormat )
                    {
                        case TriangleIndexFormat.UInt16:
                            writer.WriteUInt16( ( ushort )Triangles[i].A );
                            writer.WriteUInt16( ( ushort )Triangles[i].B );
                            writer.WriteUInt16( ( ushort )Triangles[i].C );
                            break;
                        case TriangleIndexFormat.UInt32:
                            writer.WriteUInt32( Triangles[i].A );
                            writer.WriteUInt32( Triangles[i].B );
                            writer.WriteUInt32( Triangles[i].C );
                            break;
                        default:
                            throw new InvalidDataException( $"Unsupported triangle index type: {TriangleIndexFormat}" );
                    }
                }
            }

            if ( Flags.HasFlag( GeometryFlags.HasMaterial ) )
            {
                writer.WriteStringWithHash( Version, MaterialName );
            }

            if ( Flags.HasFlag( GeometryFlags.HasBoundingBox ) )
            {
                writer.WriteBoundingBox( BoundingBox.Value );
            }

            if ( Flags.HasFlag( GeometryFlags.HasBoundingSphere ) )
            {
                writer.WriteBoundingSphere( BoundingSphere.Value );
            }

            if ( Flags.HasFlag( GeometryFlags.Flag1000 ) )
            {
                writer.WriteSingle( FieldD4 );
                writer.WriteSingle( FieldD8 );
            }
        }
    }

    public enum TriangleIndexFormat
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
        Flag80000000 = 1u << 31, // 20 bytes, after HasBoundingBox
    }
}