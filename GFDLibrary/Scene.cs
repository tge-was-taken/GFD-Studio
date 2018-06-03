using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace GFDLibrary
{
    public sealed class Scene : Resource
    {
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

        private MatrixPalette mMatrixPalette;
        public MatrixPalette MatrixPalette
        {
            get => mMatrixPalette;
            set
            {
                mMatrixPalette = value;
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

        public Scene(uint version) : base(ResourceType.Scene, version)
        {
        }

        /// <summary>
        /// Helper method that enumerates over all geometry attachments in the scene.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Geometry> EnumerateGeometries()
        {
            foreach ( var node in Nodes )
            {
                if ( !node.HasAttachments )
                    continue;

                foreach ( var attachment in node.Attachments )
                {
                    if ( attachment.Type != NodeAttachmentType.Geometry )
                        continue;

                    yield return attachment.GetValue<Geometry>();
                }
            }
        }

        public void ReplaceWith( Scene other )
        {
            // Remove geometries from this scene
            ClearGeometries();

            MatrixPalette = other.MatrixPalette;
            BoundingBox = other.BoundingBox;
            BoundingSphere = other.BoundingSphere;
            Flags = other.Flags;

            var uniqueNodes = new List<Node>();
            foreach ( var otherNode in other.Nodes )
            {
                var thisNode = Nodes.SingleOrDefault( x => x.Name.Equals( otherNode.Name ) );

                if ( thisNode == null )
                {
                    // Node not present, can't merge
                    uniqueNodes.Add( otherNode );
                    continue;
                }

                // Merge attachments
                if (otherNode.HasAttachments)
                    thisNode.Attachments.AddRange( otherNode.Attachments );

                // Replace properties
                foreach ( var property in otherNode.Properties )
                    thisNode.Properties[property.Key] = property.Value;
            }

            // Condense unique nodes
            foreach ( var uniqueNode in uniqueNodes.ToList() )
            {
                if ( uniqueNode.Parent != other.RootNode )
                {
                    // Find top of hierarchy
                    var parent = uniqueNode.Parent;
                    while ( parent.Parent != other.RootNode )
                    {
                        var thisNode = Nodes.SingleOrDefault( x => x.Name.Equals( parent.Name ) );
                        Trace.Assert( thisNode == null );
                        parent = parent.Parent;
                    }

                    // Remove children from unique nodes list, we only need to the topmost parent.
                    foreach ( var child in parent.Children )
                        uniqueNodes.Remove( child );
                }
            }

            // Add unique nodes to root.
            foreach ( var uniqueNode in uniqueNodes )
                RootNode.AddChildNode( uniqueNode );

            FixupMatrixPalette( MatrixPalette, other.Nodes.ToList() );

            PopulateNodeList();
        }

        private void FixupMatrixPalette( MatrixPalette matrixPalette, List<Node> otherNodes )
        {
            for ( int i = 0; i < matrixPalette.BoneToNodeIndices.Length; i++ )
            {
                // Remap node indices to the correct ones
                var otherNodeIndex = matrixPalette.BoneToNodeIndices[ i ];
                var otherNode = otherNodes[ otherNodeIndex ];
                var thisNode = Nodes.FirstOrDefault( x => x.Name == otherNode.Name );
                Trace.Assert( thisNode != null );
                var thisNodeIndex = Nodes.IndexOf( thisNode );
                matrixPalette.BoneToNodeIndices[i] = (byte)thisNodeIndex;
            }
        }

        public void ClearGeometries()
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
            if ( MatrixPalette == null )
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