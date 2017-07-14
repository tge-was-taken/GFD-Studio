using System;
using System.Collections.Generic;

namespace AtlusGfdLib
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

        private SkinnedBoneMap mMatrixMap;
        public SkinnedBoneMap MatrixMap
        {
            get => mMatrixMap;
            set
            {
                mMatrixMap = value;
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
        public IReadOnlyList<Node> Nodes => mNodeList;

        public Scene(uint version) : base(ResourceType.Scene, version)
        {
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
            if ( MatrixMap == null )
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