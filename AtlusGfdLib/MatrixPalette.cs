using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class MatrixPalette
    {
        public int MatrixCount => InverseBindMatrices.Length;

        public Matrix4x4[] InverseBindMatrices { get; set; }

        public ushort[] BoneToNodeIndices { get; set; }

        public MatrixPalette( int matrixCount )
        {
            InverseBindMatrices = new Matrix4x4[matrixCount];
            BoneToNodeIndices = new ushort[matrixCount];
        }

        public static Matrix4x4 YToZUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );

        public static Matrix4x4 ZToYUpMatrix = new Matrix4x4( 1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1 );

        public static MatrixPalette Create( List<int> skinnedNodeIndices, List<Matrix4x4> skinnedNodeWorldTransformMatrices, out Dictionary<int, int> nodeToBoneMap )
        {
            var map = new MatrixPalette( skinnedNodeIndices.Count );
            nodeToBoneMap = new Dictionary<int, int>();

            for ( int i = 0; i < skinnedNodeIndices.Count; i++ )
            {
                var worldTransformMatrix = skinnedNodeWorldTransformMatrices[i];
                var inverseBindMatrix = CreateInverseBindMatrix( worldTransformMatrix );

                map.InverseBindMatrices[i] = inverseBindMatrix;
                map.BoneToNodeIndices[i] = (ushort)skinnedNodeIndices[i];
                nodeToBoneMap[skinnedNodeIndices[i]] = i;
            }

            return map;
        }

        private static Matrix4x4 CreateInverseBindMatrix( Matrix4x4 worldTransformMatrix )
        {
            var bindMatrixZUp = worldTransformMatrix * YToZUpMatrix;

            Matrix4x4.Invert( bindMatrixZUp, out var inverseBindMatrix );

            return inverseBindMatrix;
        }

        private static Matrix4x4 CreateWorldTransformMatrix( Matrix4x4 inverseBindMatrix )
        {
            Matrix4x4.Invert( inverseBindMatrix, out var bindMatrix );

            var bindMatrixYUp = bindMatrix * ZToYUpMatrix;

            return bindMatrixYUp;
        }

        private static IReadOnlyList<int> GetUniqueUsedBoneIndicesForAllGeometry( IReadOnlyList<Node> nodes )
        {
            var uniqueUsedBoneIndices = new List<int>();

            for ( int i = 0; i < nodes.Count; i++ )
            {
                var node = nodes[i];

                if ( !node.HasAttachments )
                    continue;

                foreach ( var geometry in node.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).Select(x => x.GetValue<Geometry>()) )
                {
                    if ( !( geometry.Flags.HasFlag( GeometryFlags.HasVertexWeights ) ) )
                    {
                        if ( !uniqueUsedBoneIndices.Contains( i ) )
                            uniqueUsedBoneIndices.Add( i );
                    }
                    else
                    {
                        foreach ( var vertexWeight in geometry.VertexWeights )
                        {
                            for ( int j = 0; j < 4; j++ )
                            {
                                if ( vertexWeight.Weights[j] == 0.0f )
                                    continue;

                                if ( !uniqueUsedBoneIndices.Contains( vertexWeight.Indices[j] ) )
                                    uniqueUsedBoneIndices.Add( vertexWeight.Indices[j] );
                            }
                        }
                    }
                }
            }

            return uniqueUsedBoneIndices;
        }
    }
}