using System;
using System.Collections.Generic;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Scene : Resource
    {
        public InverseBindPoseMatrixMap InverseBindPoseMatrixMap { get; set; }

        public BoundingBox? BoundingBox { get; set; }

        public BoundingSphere? BoundingSphere { get; set; }

        public Node RootNode { get; set; }

        internal Scene(uint version)
            : base(ResourceType.Scene, version)
        {
        }
    }

    public sealed class InverseBindPoseMatrixMap
    {
        public Matrix4x4[] InverseBindPoseMatrices { get; set; }

        public short[] RemapIndices { get; set; }

        public InverseBindPoseMatrixMap( int matrixCount )
        {
            InverseBindPoseMatrices = new Matrix4x4[matrixCount];
            RemapIndices = new short[matrixCount];
        }
    }


    [Flags]
    public enum SceneFlags
    {
        HasBoundingBox    = 1 << 0,
        HasBoundingSphere = 1 << 1,
        HasVertexWeights  = 1 << 2,
        HasMorphs         = 1 << 3
    }
}