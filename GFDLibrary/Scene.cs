using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public sealed class Scene : Resource
    {
        public override ResourceType ResourceType => ResourceType.Scene;

        private SceneFlags mFlags;
        public SceneFlags Flags
        {
            get => mFlags;
            set
            {
                mFlags = value;
                ValidateFlags();
            }
        }

        private BonePalette mBonePalette;
        public BonePalette BonePalette
        {
            get => mBonePalette;
            set
            {
                mBonePalette = value;
                ValidateFlags();
            }
        }

        private BoundingBox? mBoundingBox;
        public BoundingBox? BoundingBox
        {
            get => mBoundingBox;
            set
            {
                mBoundingBox = value;
                ValidateFlags();
            }
        }

        private BoundingSphere? mBoundingSphere;
        public BoundingSphere? BoundingSphere
        {
            get => mBoundingSphere;
            set
            {
                mBoundingSphere = value;
                ValidateFlags();
            }
        }

        private Node mRootNode;
        public Node RootNode
        {
            get => mRootNode;
            set
            {
                mRootNode = value;
                PopulateNodeList();
            }
        }

        private List<Node> mNodeList;
        public ReadOnlyCollection<Node> Nodes => mNodeList.AsReadOnly();

        public Scene()
        {         
        }

        public Scene(uint version) : base(version)
        {
        }

        internal override void Read( ResourceReader reader )
        {
            var flags = ( SceneFlags ) reader.ReadInt32();

            if ( flags.HasFlag( SceneFlags.HasSkinning ) )
            {
                int matrixCount = reader.ReadInt32();
                BonePalette = new BonePalette( matrixCount );

                for ( int i = 0; i < BonePalette.InverseBindMatrices.Length; i++ )
                    BonePalette.InverseBindMatrices[i] = reader.ReadMatrix4x4();

                for ( int i = 0; i < BonePalette.BoneToNodeIndices.Length; i++ )
                    BonePalette.BoneToNodeIndices[i] = reader.ReadUInt16();
            }

            if ( flags.HasFlag( SceneFlags.HasBoundingBox ) )
                BoundingBox = reader.ReadBoundingBox();

            if ( flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                BoundingSphere = reader.ReadBoundingSphere();

            RootNode = Node.ReadRecursive( reader, Version );
            Flags = flags;
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( ( int ) Flags );

            if ( Flags.HasFlag( SceneFlags.HasSkinning ) )
            {
                writer.WriteInt32( BonePalette.BoneCount );

                foreach ( var t in BonePalette.InverseBindMatrices )
                    writer.WriteMatrix4x4( t );

                foreach ( var index in BonePalette.BoneToNodeIndices )
                    writer.WriteUInt16( index );
            }

            if ( Flags.HasFlag( SceneFlags.HasBoundingBox ) )
                writer.WriteBoundingBox( BoundingBox.Value );

            if ( Flags.HasFlag( SceneFlags.HasBoundingSphere ) )
                writer.WriteBoundingSphere( BoundingSphere.Value );

            Node.WriteRecursive( writer, RootNode );
        }

        public void ReplaceWith( Scene other )
        {
            // Remove geometries from this scene
            RemoveGeometryAttachments();

            BonePalette = other.BonePalette;
            BoundingBox = other.BoundingBox;
            BoundingSphere = other.BoundingSphere;
            Flags = other.Flags;

            // Replace common nodes and get the unique nodes
            var uniqueNodes = ReplaceCommonNodesAndGetUniqueNodes( other.Nodes );

            // Remove nodes that dont have attachments
            uniqueNodes.RemoveAll( x => !x.HasAttachments );

            // Fix unique nodes
            FixUniqueNodes( other, uniqueNodes );

            // Add unique nodes to root.
            foreach ( var uniqueNode in uniqueNodes )
                RootNode.AddChildNode( uniqueNode );

            // Rebuild matrix palette
            PopulateNodeList();
            RebuildBonePalette( other.Nodes.ToList() );
        }

        private List<Node> ReplaceCommonNodesAndGetUniqueNodes( IEnumerable<Node> otherNodes )
        {
            var uniqueNodes = new List<Node>();

            foreach ( var otherNode in otherNodes )
            {
                var thisNode = Nodes.SingleOrDefault( x => x.Name.Equals( otherNode.Name ) );

                if ( thisNode == null )
                {
                    // Node not present, can't merge
                    uniqueNodes.Add( otherNode );
                    continue;
                }

                // Merge attachments
                if ( otherNode.HasAttachments )
                    thisNode.Attachments.AddRange( otherNode.Attachments.Where( x => x.Type != NodeAttachmentType.Epl ) );

                // Replace properties
                foreach ( var property in otherNode.Properties )
                    thisNode.Properties[property.Key] = property.Value;
            }

            return uniqueNodes;
        }

        private void FixUniqueNodes( Scene other, List<Node> uniqueNodes )
        {
            foreach ( var uniqueNode in uniqueNodes.ToList() )
            {
                if ( uniqueNode.Parent == other.RootNode )
                    continue;

                // Find the last unique node in the hierarchy chain (going up the hierarchy)
                var lastUniqueNode = uniqueNode;
                while ( true )
                {
                    var parent = lastUniqueNode.Parent;
                    if ( parent == null || parent == other.RootNode || Nodes.SingleOrDefault( x => x.Name.Equals( parent.Name ) ) != null )
                        break;

                    lastUniqueNode = parent;
                }

                // Get unweighted geometries
                var unweightedGeometries = uniqueNode.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry )
                                                     .Select( x => x.GetValue<Geometry>() ).Where( x => x.VertexWeights == null );

                if ( unweightedGeometries.Any() )
                {
                    // If we have unweighted geometries, we have to assign vertex weights to them so that they
                    // properly animate.
                    // The node we are going to assign the weights to is the shared ancestor (between this model and the replacement one)
                    // in the hopes that it will work out.

                    // Find the bone index of this node
                    int lastUniqueNodeIndex = -1;
                    for ( int i = 0; i < other.Nodes.Count; i++ )
                    {
                        if ( other.Nodes[i].Name == lastUniqueNode.Parent.Name )
                        {
                            lastUniqueNodeIndex = i;
                            break;
                        }
                    }

                    Trace.Assert( lastUniqueNodeIndex != -1 );
                    int boneIndex = Array.IndexOf( BonePalette.BoneToNodeIndices,
                                                   lastUniqueNodeIndex );

                    if ( boneIndex == -1 )
                    {
                        Trace.Assert( BonePalette.BoneCount < 255 );

                        // Node wasn't used as a bone, so we add it
                        // TODO: This is a lazy hack. This should be done during the BonePalette fixup
                        var boneToNodeIndices = BonePalette.BoneToNodeIndices;
                        Array.Resize( ref boneToNodeIndices, BonePalette.BoneToNodeIndices.Length + 1 );
                        boneToNodeIndices[boneToNodeIndices.Length - 1] = ( ushort )lastUniqueNodeIndex;
                        BonePalette.BoneToNodeIndices = boneToNodeIndices;

                        var inverseBindMatrices = BonePalette.InverseBindMatrices;
                        Array.Resize( ref inverseBindMatrices, inverseBindMatrices.Length + 1 );
                        BonePalette.InverseBindMatrices = inverseBindMatrices;

                        boneIndex = BonePalette.BoneToNodeIndices.Length - 1;
                    }

                    // Set vertex weights
                    foreach ( var geometry in uniqueNode.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).Select( x => x.GetValue<Geometry>() ).Where( x => x.VertexWeights == null ) )
                    {
                        geometry.VertexWeights = new VertexWeight[geometry.VertexCount];
                        for ( int i = 0; i < geometry.VertexWeights.Length; i++ )
                        {
                            ref var weight = ref geometry.VertexWeights[i];
                            weight.Indices = new byte[4];
                            weight.Indices[0] = ( byte )boneIndex;
                            weight.Weights = new float[4];
                            weight.Weights[0] = 1f;
                        }
                    }
                }

                // Fix transform for the node
                var worldTransform = uniqueNode.WorldTransform;
                uniqueNode.Parent?.RemoveChildNode( uniqueNode );
                uniqueNode.LocalTransform = worldTransform;
            }
        }

        private void RebuildBonePalette( List<Node> otherNodes )
        {
            var uniqueBones = new List<Bone>();

            // Recalculate inverse bind matrices & update bone indices
            foreach ( var node in Nodes )
            {
                if ( !node.HasAttachments )
                    continue;

                Matrix4x4.Invert( node.WorldTransform, out var nodeInvWorldTransform );

                foreach ( var geometry in node.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).Select( x => x.GetValue<Geometry>() )
                                              .Where( x => x.VertexWeights != null ) )
                {
                    foreach ( var weight in geometry.VertexWeights )
                    {
                        for ( int i = 0; i < weight.Indices.Length; i++ )
                        {
                            var boneIndex = weight.Indices[i];
                            var boneWeight = weight.Weights[i];
                            if ( boneWeight == 0 )
                                continue;

                            var otherNodeIndex = BonePalette.BoneToNodeIndices[boneIndex];
                            var otherBoneNode = otherNodes[otherNodeIndex];

                            var thisBoneNode = Nodes.FirstOrDefault( x => x.Name == otherBoneNode.Name );
                            if ( thisBoneNode == null )
                            {
                                // Find parent that does exist
                                var curOtherBoneNode = otherBoneNode;
                                while ( thisBoneNode == null )
                                {
                                    thisBoneNode = Nodes.FirstOrDefault( x => x.Name == curOtherBoneNode.Parent.Name );
                                    curOtherBoneNode = curOtherBoneNode.Parent;
                                }
                            }

                            var boneTransform = thisBoneNode.WorldTransform;

                            // Attempt to fix spaghetti fingers
                            //if ( thisBoneNode.Name.Contains( "Finger" ) || thisBoneNode.Name.Contains( "Hand" ) ||
                            //     thisBoneNode.Name.Contains( "hand" ) )
                            //    boneTransform = otherBoneNode.WorldTransform;

                            var thisNodeIndex = Nodes.IndexOf( thisBoneNode );
                            Trace.Assert( thisNodeIndex != -1 );
                            var bindMatrix = boneTransform * nodeInvWorldTransform;
                            Matrix4x4.Invert( bindMatrix, out var inverseBindMatrix );
                            
                            var newBoneIndex =
                                uniqueBones.FindIndex( x => x.NodeIndex == thisNodeIndex && x.InverseBindMatrix.Equals( inverseBindMatrix ) );

                            if ( newBoneIndex == -1 )
                            {
                                // Add if unique
                                Trace.Assert( uniqueBones.Count < 255 );
                                uniqueBones.Add( new Bone( (ushort)thisNodeIndex, inverseBindMatrix ) );
                                newBoneIndex = uniqueBones.Count - 1;
                            }

                            // Update bone index
                            weight.Indices[ i ] = ( byte ) newBoneIndex;
                        }
                    }
                }
            }

            BonePalette = new BonePalette( uniqueBones );
        }

        private void RemoveGeometryAttachments()
        {
            foreach ( var node in Nodes )
            {
                if ( node.HasAttachments )
                    foreach ( var geometryAttachment in node.Attachments.Where( x => x.Type == NodeAttachmentType.Geometry ).ToList() )
                        node.Attachments.Remove( geometryAttachment );
            }
        }

        private void PopulateNodeList()
        {
            mNodeList = new List<Node>();

            void RecursivelyAddToList(Node node)
            {
                mNodeList.Add( node );
                foreach ( var childNode in node.Children )
                {
                    RecursivelyAddToList( childNode );
                }
            }

            RecursivelyAddToList( RootNode );
        }

        private void ValidateFlags()
        {
            if ( BonePalette == null )
                mFlags &= ~SceneFlags.HasSkinning;
            else
                mFlags |= SceneFlags.HasSkinning;

            if ( BoundingBox == null )
                mFlags &= ~SceneFlags.HasBoundingBox;
            else
                mFlags |= SceneFlags.HasBoundingBox;

            if ( BoundingSphere == null )
                mFlags &= ~SceneFlags.HasBoundingSphere;
            else
                mFlags |= SceneFlags.HasBoundingSphere;
        }
    }

    [Flags]
    public enum SceneFlags
    {
        HasBoundingBox    = 1 << 0,
        HasBoundingSphere = 1 << 1,
        HasSkinning       = 1 << 2,
        HasMorphs         = 1 << 3
    }
}