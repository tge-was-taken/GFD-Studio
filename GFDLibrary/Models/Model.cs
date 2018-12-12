using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using GFDLibrary.Common;
using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class Model : Resource
    {
        public override ResourceType ResourceType => ResourceType.Model;

        private ModelFlags mFlags;
        public ModelFlags Flags
        {
            get => mFlags;
            set
            {
                mFlags = value;
                ValidateFlags();
            }
        }

        private List<Bone> mBones;
        public List<Bone> Bones
        {
            get => mBones;
            set
            {
                mBones = value;
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

        public Node RootNode { get; set; }

        public IEnumerable<Node> Nodes
        {
            get
            {
                IEnumerable<Node> RecursivelyAddToList( Node node )
                {
                    yield return node;
                    foreach ( var childNode in node.Children )
                    {
                        foreach ( var childChildNode in RecursivelyAddToList( childNode ) )
                            yield return childChildNode;
                    }
                }

                return RecursivelyAddToList( RootNode );
            }
        }

        public Model()
        {         
        }

        public Model(uint version) : base(version)
        {
        }

        internal override void Read( ResourceReader reader )
        {
            var flags = ( ModelFlags ) reader.ReadInt32();

            if ( flags.HasFlag( ModelFlags.HasSkinning ) )
            {
                int boneCount = reader.ReadInt32();

                var inverseBindMatrices = new Matrix4x4[boneCount];
                var boneToNodeIndices = new ushort[boneCount];
                for ( int i = 0; i < boneCount; i++ )
                    inverseBindMatrices[i] = reader.ReadMatrix4x4();

                for ( int i = 0; i < boneCount; i++ )
                    boneToNodeIndices[i] = reader.ReadUInt16();

                Bones = new List<Bone>( boneCount );
                for ( int i = 0; i < boneCount; i++ )
                    Bones.Add( new Bone( boneToNodeIndices[ i ], inverseBindMatrices[ i ] ) );
            }

            if ( flags.HasFlag( ModelFlags.HasBoundingBox ) )
                BoundingBox = reader.ReadBoundingBox();

            if ( flags.HasFlag( ModelFlags.HasBoundingSphere ) )
                BoundingSphere = reader.ReadBoundingSphere();

            RootNode = Node.ReadRecursive( reader, Version );
            Flags = flags;
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteInt32( ( int ) Flags );

            if ( Flags.HasFlag( ModelFlags.HasSkinning ) )
            {
                writer.WriteInt32( Bones.Count );

                foreach ( var bone in Bones )
                    writer.WriteMatrix4x4( bone.InverseBindMatrix );

                foreach ( var bone in Bones )
                    writer.WriteUInt16( bone.NodeIndex );
            }

            if ( Flags.HasFlag( ModelFlags.HasBoundingBox ) )
                writer.WriteBoundingBox( BoundingBox.Value );

            if ( Flags.HasFlag( ModelFlags.HasBoundingSphere ) )
                writer.WriteBoundingSphere( BoundingSphere.Value );

            Node.WriteRecursive( writer, RootNode );
        }

        public Node GetNode( int nodeIndex )
        {
            var i = 0;
            return Nodes.FirstOrDefault( node => i++ == nodeIndex );
        }

        public void ReplaceWith( Model other )
        {
            // Remove geometries from this scene
            RemoveGeometryAttachments();

            Bones = other.Bones;
            BoundingBox = other.BoundingBox;
            BoundingSphere = other.BoundingSphere;
            Flags = other.Flags;

            var otherNodes = other.Nodes.ToList();

            // Replace common nodes and get the unique nodes
            var uniqueNodes = ReplaceCommonNodesAndGetUniqueNodes( otherNodes );

            // Remove nodes that dont have attachments
            uniqueNodes.RemoveAll( x => !x.HasAttachments );

            // Fix unique nodes
            FixUniqueNodes( other.RootNode, otherNodes, uniqueNodes );

            // Add unique nodes to root.
            foreach ( var uniqueNode in uniqueNodes )
                RootNode.AddChildNode( uniqueNode );

            // Rebuild matrix palette
            RebuildBonePalette( otherNodes );
        }

        private List<Node> ReplaceCommonNodesAndGetUniqueNodes( IEnumerable<Node> otherNodes )
        {
            var uniqueNodes = new List<Node>();

            foreach ( var otherNode in otherNodes )
            {
                if ( otherNode.Name == "RootNode" || ( otherNode.Parent == null || otherNode.Parent == otherNodes.First() ) && otherNode.Name.EndsWith( "_root" ) )
                {
                    continue;
                }

                if ( !Nodes.Any( x => x.Name == "Bip01 雜ｳ霍｡" ) )
                {
                    // Hacks to fix enemy/persona models
                    if ( otherNode.Name == "Bip01 閼頑､・" )
                        otherNode.Name = "Bip01 Spine";
                    else if ( otherNode.Parent != null && otherNode.Parent.Name == "Bip01 Spine" && otherNode.Name == "Bip01 閼頑､・" )
                        otherNode.Name = "Bip01 Spine1";
                    else if ( otherNode.Name == "Bip01 鬥・" )
                        otherNode.Name = "Bip01 Neck";
                    else if ( otherNode.Name == "Bip01 雜ｳ霍｡" )
                        otherNode.Name = "Bip01 Footsteps";
                }

                var thisNode = Nodes.SingleOrDefault( x => x.Name.Equals( otherNode.Name ) );

                if ( thisNode == null )
                {
                    // Node not present, can't merge
                    uniqueNodes.Add( otherNode );
                    continue;
                }

                // Merge attachments
                if ( otherNode.HasAttachments )
                {
                    Matrix4x4.Invert( thisNode.WorldTransform, out var thisNodeWorldTransformInv );
                    var offsetMatrix = otherNode.WorldTransform * thisNodeWorldTransformInv;

                    foreach ( var attachment in otherNode.Attachments )
                    {
                        switch ( attachment.Type )
                        {
                            case NodeAttachmentType.Mesh:
                                {
                                    var mesh = attachment.GetValue<Mesh>();

                                    for ( int i = 0; i < mesh.Vertices.Length; i++ )
                                    {
                                        var position = mesh.Vertices[ i ];
                                        var newPosition = mesh.Vertices[ i ] = Vector3.Transform( position, offsetMatrix );

                                        if ( mesh.MorphTargets != null )
                                        {
                                            foreach ( var morphTarget in mesh.MorphTargets )
                                            {
                                                Trace.Assert( morphTarget.VertexCount == mesh.VertexCount );
                                                morphTarget.Vertices[ i ] = Vector3.Transform( ( position + morphTarget.Vertices[ i ] ), offsetMatrix ) - newPosition;
                                            }
                                        }
                                    }

                                    if ( mesh.Normals != null )
                                    {
                                        for ( int i = 0; i < mesh.Normals.Length; i++ )
                                            mesh.Normals[i] = Vector3.TransformNormal( mesh.Normals[i], offsetMatrix );
                                    }
                                }
                                break;

                            case NodeAttachmentType.Epl:
                                continue;

                            case NodeAttachmentType.Light:
                                if ( thisNode.Attachments.Any( x => x.Type == NodeAttachmentType.Light ) )
                                {
                                    // Don't replace lights, likely not what we want to do
                                    continue;
                                }
                                break;
                        }

                        thisNode.Attachments.Add( attachment );
                    }
                }

                // Replace properties
                foreach ( var property in otherNode.Properties )
                    thisNode.Properties[property.Key] = property.Value;
            }

            return uniqueNodes;
        }

        private void FixUniqueNodes( Node otherRootNode, List<Node> otherNodes, List<Node> uniqueNodes )
        {
            foreach ( var uniqueNode in uniqueNodes.ToList() )
            {
                if ( uniqueNode.Parent == otherRootNode )
                    continue;

                // Find the last unique node in the hierarchy chain (going up the hierarchy)
                var lastUniqueNode = uniqueNode;
                while ( true )
                {
                    var parent = lastUniqueNode.Parent;
                    if ( parent == null || parent == otherRootNode || Nodes.SingleOrDefault( x => x.Name.Equals( parent.Name ) ) != null )
                        break;

                    lastUniqueNode = parent;
                }

                // Get unweighted geometries
                var unweightedGeometries = uniqueNode.Attachments.Where( x => x.Type == NodeAttachmentType.Mesh )
                                                     .Select( x => x.GetValue<Mesh>() ).Where( x => x.VertexWeights == null ).ToList();

                if ( unweightedGeometries.Any() )
                {
                    // If we have unweighted geometries, we have to assign vertex weights to them so that they
                    // properly animate.
                    // The node we are going to assign the weights to is the shared ancestor (between this model and the replacement one)
                    // in the hopes that it will work out.

                    // Find the bone index of this node
                    int lastUniqueNodeIndex = -1;
                    for ( int i = 0; i < otherNodes.Count; i++ )
                    {
                        if ( otherNodes[i].Name == lastUniqueNode.Parent.Name )
                        {
                            lastUniqueNodeIndex = i;
                            break;
                        }
                    }

                    Trace.Assert( lastUniqueNodeIndex != -1 );

                    if ( Bones == null )
                    {
                        Bones = new List<Bone>();
                    }

                    int boneIndex = Bones.FindIndex( x => x.NodeIndex == lastUniqueNodeIndex );

                    if ( boneIndex == -1 )
                    {
                        Trace.Assert( Bones.Count < 255 );

                        // Node wasn't used as a bone, so we add it
                        // TODO: This is a lazy hack. This should be done during the Bones fixup
                        boneIndex = Bones.Count;
                        Bones.Add( new Bone( ( ushort ) lastUniqueNodeIndex, Matrix4x4.Identity ) );
                    }

                    // Set vertex weights
                    foreach ( var geometry in unweightedGeometries )
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

                //// Fix morphs
                //var morphs = uniqueNode.Attachments.Where( x => x.Type == NodeAttachmentType.Morph ).Select( x => x.GetValue<Morph>() );
                //foreach ( var morph in morphs )
                //{
                //    // All unique nodes get assigned to the root node
                //    morph.NodeName = "RootNode";
                //}

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
            var nodes = Nodes.ToList();
            foreach ( var node in nodes )
            {
                if ( !node.HasAttachments )
                    continue;

                Matrix4x4.Invert( node.WorldTransform, out var nodeInvWorldTransform );

                foreach ( var geometry in node.Attachments.Where( x => x.Type == NodeAttachmentType.Mesh ).Select( x => x.GetValue<Mesh>() )
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

                            var otherNodeIndex = Bones[boneIndex].NodeIndex;
                            var otherBoneNode = otherNodes[otherNodeIndex];

                            var thisBoneNode = nodes.FirstOrDefault( x => x.Name == otherBoneNode.Name );
                            if ( thisBoneNode == null )
                            {
                                // Find parent that does exist
                                var curOtherBoneNode = otherBoneNode.Parent;
                                while ( thisBoneNode == null && curOtherBoneNode != null )
                                {
                                    thisBoneNode = nodes.FirstOrDefault( x => x.Name == curOtherBoneNode.Name );
                                    curOtherBoneNode = curOtherBoneNode.Parent;
                                }

                                if ( thisBoneNode == null )
                                    thisBoneNode = RootNode;
                            }

                            var boneTransform = thisBoneNode.WorldTransform;

                            // Attempt to fix spaghetti fingers
                            //if ( thisBoneNode.Name.Contains( "Finger" ) || thisBoneNode.Name.Contains( "Hand" ) ||
                            //     thisBoneNode.Name.Contains( "hand" ) )
                            //    boneTransform = otherBoneNode.WorldTransform;

                            var thisNodeIndex = nodes.IndexOf( thisBoneNode );
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

            Bones = uniqueBones;
        }

        private void RemoveGeometryAttachments()
        {
            foreach ( var node in Nodes )
            {
                if ( node.HasAttachments )
                    foreach ( var geometryAttachment in node.Attachments.Where( x => x.Type == NodeAttachmentType.Mesh ).ToList() )
                        node.Attachments.Remove( geometryAttachment );
            }
        }

        private void ValidateFlags()
        {
            if ( Bones == null || Bones.Count == 0 )
                mFlags &= ~ModelFlags.HasSkinning;
            else
                mFlags |= ModelFlags.HasSkinning;

            if ( BoundingBox == null )
                mFlags &= ~ModelFlags.HasBoundingBox;
            else
                mFlags |= ModelFlags.HasBoundingBox;

            if ( BoundingSphere == null )
                mFlags &= ~ModelFlags.HasBoundingSphere;
            else
                mFlags |= ModelFlags.HasBoundingSphere;
        }
    }

    [Flags]
    public enum ModelFlags
    {
        HasBoundingBox    = 1 << 0,
        HasBoundingSphere = 1 << 1,
        HasSkinning       = 1 << 2,
        HasMorphs         = 1 << 3
    }
}