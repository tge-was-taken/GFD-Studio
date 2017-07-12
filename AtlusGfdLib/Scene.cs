using System;
using System.Collections.Generic;

namespace AtlusGfdLib
{
    public sealed class Scene : Resource
    {
        public SceneFlags Flags { get; set; }

        public MatrixMap MatrixMap { get; set; }

        public BoundingBox? BoundingBox { get; set; }

        public BoundingSphere? BoundingSphere { get; set; }

        public Node RootNode { get; set; }

        public Scene(uint version) : base(ResourceType.Scene, version)
        {
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