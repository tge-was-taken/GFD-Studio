using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class SkinnedBoneMap
    {
        public int MatrixCount => BoneInverseBindMatrices.Length;

        public Matrix4x4[] BoneInverseBindMatrices { get; set; }

        public ushort[] BoneToNodeIndices { get; set; }

        public SkinnedBoneMap( int matrixCount )
        {
            BoneInverseBindMatrices = new Matrix4x4[matrixCount];
            BoneToNodeIndices = new ushort[matrixCount];
        }

        public static SkinnedBoneMap Create( List<int> skinnedNodeIndices, List<Matrix4x4> worldMatrices, out Dictionary<int, int> nodeToBoneMap )
        {
            var map = new SkinnedBoneMap( skinnedNodeIndices.Count );
            nodeToBoneMap = new Dictionary<int, int>();

            for ( int i = 0; i < skinnedNodeIndices.Count; i++ )
            {
                Matrix4x4 matrix;
                if ( !Matrix4x4.Invert( worldMatrices[i], out matrix ) )
                    matrix = Matrix4x4.Identity;

                map.BoneInverseBindMatrices[i] = matrix;
                map.BoneToNodeIndices[i] = (ushort)skinnedNodeIndices[i];
                nodeToBoneMap[skinnedNodeIndices[i]] = i;
            }

            return map;
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