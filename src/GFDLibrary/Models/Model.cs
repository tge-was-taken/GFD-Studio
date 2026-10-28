using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using GFDLibrary.Cameras;
using GFDLibrary.Common;
using GFDLibrary.IO;
using GFDLibrary.Lights;

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

        public byte Field100_10 { get; set; }

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

        public IEnumerable<Mesh> Meshes
            => Nodes.SelectMany( n => n.Meshes );

        public Model()
        {         
        }

        public Model(uint version) : base(version)
        {
        }

        protected override void ReadCore( ResourceReader reader )
        {
            var flags = ( ModelFlags ) reader.ReadInt32();

            if ( flags.HasFlag( ModelFlags.HasSkinning ) )
            {
                int boneCount = reader.ReadInt32();

                var inverseBindMatrices = new Matrix4x4[boneCount];
                var boneToNodeIndices = new ushort[reader.ValidateCount( boneCount, 2 )];
                for ( int i = 0; i < boneCount; i++ )
                    inverseBindMatrices[i] = reader.ReadMatrix4x4();

                for ( int i = 0; i < boneCount; i++ )
                    boneToNodeIndices[i] = reader.ReadUInt16();

                Bones = new List<Bone>( boneCount );
                for ( int i = 0; i < boneCount; i++ )
                    Bones.Add( new Bone( boneToNodeIndices[ i ], inverseBindMatrices[ i ] ) );
                if ( Version >= 0x2040001 )
                    Field100_10 = reader.ReadByte();
            }

            if ( flags.HasFlag( ModelFlags.HasBoundingBox ) )
                BoundingBox = reader.ReadBoundingBox();

            if ( flags.HasFlag( ModelFlags.HasBoundingSphere ) )
                BoundingSphere = reader.ReadBoundingSphere();

            RootNode = Node.ReadRecursive( reader, Version );
            Flags = flags;
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteInt32( ( int ) Flags );

            if ( Flags.HasFlag( ModelFlags.HasSkinning ) )
            {
                writer.WriteInt32( Bones.Count );

                foreach ( var bone in Bones )
                    writer.WriteMatrix4x4( bone.InverseBindMatrix );

                foreach ( var bone in Bones )
                    writer.WriteUInt16( bone.NodeIndex );

                if ( Version >= 0x2040001 )
                    writer.WriteByte( Field100_10 );
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
                                                if ( morphTarget.VertexCount != mesh.VertexCount )
                                                    throw new InvalidDataException( "A morph target has a different vertex count to its mesh." );
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

                    if ( lastUniqueNodeIndex == -1 )
                        throw new InvalidDataException( "No node carries the bone the model refers to." );

                    if ( Bones == null )
                    {
                        Bones = new List<Bone>();
                    }

                    int boneIndex = Bones.FindIndex( x => x.NodeIndex == lastUniqueNodeIndex );

                    if ( boneIndex == -1 )
                    {
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
                            weight.Indices = new ushort[4];
                            weight.Indices[0] = (ushort)boneIndex;
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
                            if ( thisNodeIndex == -1 )
                                throw new InvalidDataException( "A bone names a node that is not in the model." );
                            var bindMatrix = boneTransform * nodeInvWorldTransform;
                            Matrix4x4.Invert( bindMatrix, out var inverseBindMatrix );
                            
                            var newBoneIndex =
                                uniqueBones.FindIndex( x => x.NodeIndex == thisNodeIndex && x.InverseBindMatrix.Equals( inverseBindMatrix ) );

                            if ( newBoneIndex == -1 )
                            {
                                // Add if unique
                                uniqueBones.Add( new Bone( (ushort)thisNodeIndex, inverseBindMatrix ) );
                                newBoneIndex = uniqueBones.Count - 1;
                            }

                            // Update bone index
                            weight.Indices[ i ] = (ushort)newBoneIndex;
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

        public void MergeWith(Model other)
        {
            if (Version != other.Version)
                throw new InvalidOperationException(
                    $"Cannot merge models with different versions: {Version:X} vs {other.Version:X}");

            var baseNodes = Nodes.ToList();
            var baseNodeNames = new HashSet<string>(baseNodes.Select(n => n.Name));
            var otherNodes = other.Nodes.ToList();
            var processedOtherNodes = new HashSet<Node>();
            var clonedMeshes = new List<Mesh>();

            // handle nodes that share a name with a base model.
            // These must be processed before cloning so their meshes/attachments
            // are transferred to the existing model's nodes with matching names
            foreach (var otherNode in otherNodes)
            {
                if (otherNode.Name == "RootNode" ||
                    (otherNode.Parent != null && otherNode.Parent == otherNodes[0] && otherNode.Name.EndsWith("_root")))
                    continue;

                if (!baseNodeNames.Contains(otherNode.Name))
                    continue;

                if (RootNode.FindNodeBreadthFirst(otherNode.Name, out var existingNode))
                {
                    foreach (var attachment in otherNode.Attachments)
                    {
                        if (attachment.Type == NodeAttachmentType.Mesh)
                        {
                            var clonedMesh = DeepCloneMesh(attachment.GetValue<Mesh>(), Version);
                            clonedMeshes.Add(clonedMesh);
                            existingNode.Attachments.Add(new NodeMeshAttachment(clonedMesh));
                        }
                        else if (attachment.Type != NodeAttachmentType.Epl &&
                                 attachment.Type != NodeAttachmentType.EplLeaf)
                        {
                            existingNode.Attachments.Add(DeepCloneAttachment(attachment, Version));
                        }
                    }

                    if (otherNode.HasProperties)
                    {
                        foreach (var kvp in otherNode.Properties)
                            existingNode.Properties[kvp.Key] = kvp.Value;
                    }
                }

                processedOtherNodes.Add(otherNode);
            }

            // Second pass: clone subtrees for nodes that are new,
            // deleting any children already handled in the first pass.
            foreach (var otherNode in otherNodes)
            {
                if (otherNode.Name == "RootNode" ||
                    (otherNode.Parent != null && otherNode.Parent == otherNodes[0] && otherNode.Name.EndsWith("_root")))
                    continue;

                if (processedOtherNodes.Contains(otherNode))
                    continue;

                var subtreeRoot = FindNewSubtreeRoot(otherNode, other, baseNodeNames);

                if (!processedOtherNodes.Contains(subtreeRoot))
                {
                    var clonedSubtree = DeepCloneNode(subtreeRoot, processedOtherNodes);
                    CollectMeshes(clonedSubtree, clonedMeshes);

                    Node baseParent = null;
                    if (subtreeRoot.Parent != null &&
                        subtreeRoot.Parent.Name != "RootNode" &&
                        baseNodeNames.Contains(subtreeRoot.Parent.Name))
                    {
                        RootNode.FindNodeBreadthFirst(subtreeRoot.Parent.Name, out baseParent);
                    }

                    (baseParent ?? RootNode).AddChildNode(clonedSubtree);
                    MarkSubtreeProcessed(subtreeRoot, processedOtherNodes, processedOtherNodes);
                }
            }

            // After the node tree has been merged, capture the new order.
            var postMergeNodes = Nodes.ToList();

            // Build a name-based old-index -> new-index reindex for base bones.
            // merging models shifts bone indices and they must be updated
            // Must run even when other model has no bones (face models can have 0 "new" bones when merged into body).
            if (Bones != null && Bones.Count > 0)
            {
                var nameToNewIndex = new Dictionary<string, int>();
                for (int n = 0; n < postMergeNodes.Count; n++)
                    nameToNewIndex[postMergeNodes[n].Name] = n;

                var oldIndexToName = new Dictionary<int, string>();
                for (int n = 0; n < baseNodes.Count; n++)
                    oldIndexToName[n] = baseNodes[n].Name;

                for (int b = 0; b < Bones.Count; b++)
                {
                    var oldIdx = Bones[b].NodeIndex;
                    if (oldIndexToName.TryGetValue(oldIdx, out var name) &&
                        nameToNewIndex.TryGetValue(name, out var newIdx))
                    {
                        Bones[b].NodeIndex = (ushort)newIdx;
                    }
                }
            }

            // No other bones to merge — just validate and return
            if (other.Bones == null || other.Bones.Count == 0)
            {
                ValidateFlags();
                return;
            }

            if (Bones == null)
                Bones = new List<Bone>();

            // Build bone remap table using post-merge node positions
            var baseBoneNodeNameToIndex = new Dictionary<string, int>();
            for (int i = 0; i < Bones.Count; i++)
            {
                var name = postMergeNodes[Bones[i].NodeIndex].Name;
                if (!baseBoneNodeNameToIndex.ContainsKey(name))
                    baseBoneNodeNameToIndex[name] = i;
            }

            var newBones = new List<Bone>();
            var remapTable = new int[other.Bones.Count];

            for (int i = 0; i < other.Bones.Count; i++)
            {
                var otherBone = other.Bones[i];
                var otherNode = otherNodes[otherBone.NodeIndex];

                if (baseBoneNodeNameToIndex.TryGetValue(otherNode.Name, out var baseIdx))
                {
                    remapTable[i] = baseIdx;
                }
                else
                {
                    var mergedNode = postMergeNodes.FirstOrDefault(n => n.Name == otherNode.Name);
                    if (mergedNode == null)
                    {
                        remapTable[i] = 0;
                        continue;
                    }

                    var mergedNodeIndex = (ushort)postMergeNodes.IndexOf(mergedNode);
                    newBones.Add(new Bone(mergedNodeIndex, otherBone.InverseBindMatrix));
                    remapTable[i] = Bones.Count + newBones.Count - 1;
                }
            }

            // Remap vertex weights in all cloned meshes
            foreach (var mesh in clonedMeshes)
            {
                if (mesh.VertexWeights == null)
                    continue;

                for (int v = 0; v < mesh.VertexWeights.Length; v++)
                {
                    for (int w = 0; w < mesh.VertexWeights[v].Indices.Length; w++)
                    {
                        if (mesh.VertexWeights[v].Weights[w] != 0)
                            mesh.VertexWeights[v].Indices[w] = (ushort)remapTable[mesh.VertexWeights[v].Indices[w]];
                    }
                }
            }

            // Append new bones
            Bones.AddRange(newBones);

            ValidateFlags();
        }

        internal static Mesh DeepCloneMesh(Mesh source, uint version)
        {
            var clone = new Mesh(version);
            clone.Vertices = source.Vertices?.ToArray();
            clone.Normals = source.Normals?.ToArray();
            clone.Tangents = source.Tangents?.ToArray();
            clone.Binormals = source.Binormals?.ToArray();
            clone.ColorChannel0 = source.ColorChannel0?.ToArray();
            clone.ColorChannel1 = source.ColorChannel1?.ToArray();
            clone.ColorChannel2 = source.ColorChannel2?.ToArray();
            clone.TexCoordsChannel0 = source.TexCoordsChannel0?.ToArray();
            clone.TexCoordsChannel1 = source.TexCoordsChannel1?.ToArray();
            clone.TexCoordsChannel2 = source.TexCoordsChannel2?.ToArray();
            clone.Triangles = source.Triangles?.ToArray();
            clone.TriangleIndexFormat = source.TriangleIndexFormat;
            clone.Field14 = source.Field14;
            clone.Unk_StrideType = source.Unk_StrideType;
            clone.Unk_VertexWeight = source.Unk_VertexWeight;
            clone.LodStart = source.LodStart;
            clone.LodEnd = source.LodEnd;

            if (source.VertexWeights != null)
            {
                clone.VertexWeights = new VertexWeight[source.VertexWeights.Length];
                for (int i = 0; i < source.VertexWeights.Length; i++)
                {
                    clone.VertexWeights[i] = new VertexWeight(
                        source.VertexWeights[i].Weights?.ToArray(),
                        source.VertexWeights[i].Indices?.ToArray());
                }
            }

            if (source.MorphTargets != null)
            {
                clone.MorphTargets = new MorphTargetList(version) { Flags = source.MorphTargets.Flags };
                foreach (var morphTarget in source.MorphTargets)
                {
                    var clonedTarget = new MorphTarget(version) { Flags = morphTarget.Flags };
                    clonedTarget.Vertices.AddRange(morphTarget.Vertices);
                    clone.MorphTargets.Add(clonedTarget);
                }
            }

            clone.MaterialName = source.MaterialName;
            clone.BoundingBox = source.BoundingBox;
            clone.BoundingSphere = source.BoundingSphere;
            clone.Flags = source.Flags;
            clone.VertexAttributeFlags = source.VertexAttributeFlags;
            return clone;
        }

        internal static NodeAttachment DeepCloneAttachment(NodeAttachment source, uint version)
        {
            switch (source.Type)
            {
                case NodeAttachmentType.Mesh:
                    return new NodeMeshAttachment(DeepCloneMesh(source.GetValue<Mesh>(), version));
                case NodeAttachmentType.Node:
                    return new NodeNodeAttachment(DeepCloneNode(source.GetValue<Node>()));
                case NodeAttachmentType.Camera:
                    {
                        var src = source.GetValue<Camera>();
                        var cam = new Camera(version)
                        {
                            ViewMatrix = src.ViewMatrix,
                            ClipPlaneNear = src.ClipPlaneNear,
                            ClipPlaneFar = src.ClipPlaneFar,
                            FieldOfView = src.FieldOfView,
                            AspectRatio = src.AspectRatio,
                            Field190 = src.Field190,
                            Field198 = src.Field198,
                            Field19C = src.Field19C,
                            Field1A0 = src.Field1A0
                        };
                        return new NodeCameraAttachment(cam);
                    }
                case NodeAttachmentType.Light:
                    {
                        var src = source.GetValue<Light>();
                        var light = new Light(version)
                        {
                            Flags = src.Flags,
                            Type = src.Type,
                            AmbientColor = src.AmbientColor,
                            DiffuseColor = src.DiffuseColor,
                            SpecularColor = src.SpecularColor,
                            Field20 = src.Field20,
                            Field04 = src.Field04,
                            Field08 = src.Field08,
                            Field10 = src.Field10,
                            AttenuationStart = src.AttenuationStart,
                            AttenuationEnd = src.AttenuationEnd,
                            Field60 = src.Field60,
                            Field64 = src.Field64,
                            Field68 = src.Field68,
                            AngleInnerCone = src.AngleInnerCone,
                            AngleOuterCone = src.AngleOuterCone,
                            Field98 = src.Field98,
                            Field9C = src.Field9C
                        };
                        return new NodeLightAttachment(light);
                    }
                case NodeAttachmentType.Morph:
                    {
                        var src = source.GetValue<Morph>();
                        var morph = new Morph(version)
                        {
                            NodeName = src.NodeName,
                            TargetInts = src.TargetInts?.ToArray()
                        };
                        return new NodeMorphAttachment(morph);
                    }
                default:
                    return null;
            }
        }

        internal static Node DeepCloneNode(Node source, HashSet<Node> skipSourceNodes = null)
        {
            var clone = new Node(source.Name)
            {
                Translation = source.Translation,
                Rotation = source.Rotation,
                Scale = source.Scale,
                FieldE0 = source.FieldE0
            };

            foreach (var attachment in source.Attachments)
            {
                var clonedAttachment = DeepCloneAttachment(attachment, source.Version);
                if (clonedAttachment != null)
                    clone.Attachments.Add(clonedAttachment);
            }

            if (source.HasProperties)
            {
                foreach (var kvp in source.Properties)
                    clone.Properties[kvp.Key] = kvp.Value;
            }

            foreach (var child in source.Children)
            {
                if (skipSourceNodes != null && skipSourceNodes.Contains(child))
                    continue;
                clone.AddChildNode(DeepCloneNode(child, skipSourceNodes));
            }

            return clone;
        }

        private static void CollectMeshes(Node node, List<Mesh> meshes)
        {
            foreach (var attachment in node.Attachments)
            {
                if (attachment.Type == NodeAttachmentType.Mesh)
                    meshes.Add(attachment.GetValue<Mesh>());
            }
            foreach (var child in node.Children)
                CollectMeshes(child, meshes);
        }

        private static Node FindNewSubtreeRoot(Node node, Model otherModel, HashSet<string> baseNodeNames)
        {
            var current = node;
            while (current.Parent != null &&
                   current.Parent.Name != "RootNode" &&
                   !baseNodeNames.Contains(current.Parent.Name))
            {
                current = current.Parent;
            }
            return current;
        }

        private static void MarkSubtreeProcessed(Node root, HashSet<Node> processed, HashSet<Node> skipNodes = null)
        {
            if (skipNodes != null && skipNodes.Contains(root))
                return;
            processed.Add(root);
            foreach (var child in root.Children)
                MarkSubtreeProcessed(child, processed, skipNodes);
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