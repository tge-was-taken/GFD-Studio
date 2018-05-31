using GFDLibrary.Cameras;
using GFDLibrary.Lights;

namespace GFDLibrary
{
    public abstract class NodeAttachment
    {
        public NodeAttachmentType Type { get; }

        protected NodeAttachment( NodeAttachmentType type )
        {
            Type = type;
        }

        public abstract object GetValue();

        public T GetValue<T>() => ( T )GetValue();
    }

    public sealed class NodeSceneAttachment : NodeAttachment
    {
        public Scene Scene { get; set; }

        public NodeSceneAttachment() : base( NodeAttachmentType.Scene ) { }

        public NodeSceneAttachment( Scene scene ) : base( NodeAttachmentType.Scene ) => Scene = scene;

        public override object GetValue()
        {
            return Scene;
        }
    }

    /*
    public sealed class NodeMeshAttachment : NodeAttachment
    {
        public Mesh Mesh { get; set; }

        public NodeMeshAttachment() : base( NodeAttachmentType.Mesh ) { }

        public NodeMeshAttachment( Mesh mesh ) : base( NodeAttachmentType.Mesh ) => Mesh = mesh;

        public override object GetValue()
        {
            return Mesh;
        }
    }
    */

    public sealed class NodeNodeAttachment : NodeAttachment
    {
        public Node Node { get; set; }

        public NodeNodeAttachment() : base( NodeAttachmentType.Node ) { }

        public NodeNodeAttachment( Node node ) : base( NodeAttachmentType.Node ) => Node = node;

        public override object GetValue()
        {
            return Node;
        }
    }

    public sealed class NodeGeometryAttachment : NodeAttachment
    {
        public Geometry Geometry { get; set; }

        public NodeGeometryAttachment() : base( NodeAttachmentType.Geometry ) { }

        public NodeGeometryAttachment( Geometry geometry ) : base( NodeAttachmentType.Geometry ) => Geometry = geometry;

        public override object GetValue()
        {
            return Geometry;
        }
    }

    public sealed class NodeCameraAttachment : NodeAttachment
    {
        public Camera Camera { get; set; }

        public NodeCameraAttachment() : base( NodeAttachmentType.Camera ) { }

        public NodeCameraAttachment( Camera camera ) : base( NodeAttachmentType.Camera ) => Camera = camera;

        public override object GetValue()
        {
            return Camera;
        }
    }
    public sealed class NodeLightAttachment : NodeAttachment
    {
        public Light Light { get; set; }

        public NodeLightAttachment() : base( NodeAttachmentType.Light ) { }

        public NodeLightAttachment( Light light ) : base( NodeAttachmentType.Light ) => Light = light;

        public override object GetValue()
        {
            return Light;
        }
    }

    public sealed class NodeEplAttachment : NodeAttachment
    {
        public Epl Epl { get; set; }

        public NodeEplAttachment() : base( NodeAttachmentType.Epl ) { }

        public NodeEplAttachment( Epl epl ) : base( NodeAttachmentType.Epl ) => Epl = epl;

        public override object GetValue()
        {
            return Epl;
        }
    }

    public sealed class NodeEplLeafAttachment : NodeAttachment
    {
        public EplLeaf EplLeaf { get; set; }

        public NodeEplLeafAttachment() : base( NodeAttachmentType.EplLeaf ) { }

        public NodeEplLeafAttachment( EplLeaf eplLeaf ) : base( NodeAttachmentType.EplLeaf ) => EplLeaf = eplLeaf;

        public override object GetValue()
        {
            return EplLeaf;
        }
    }

    public sealed class NodeMorphAttachment : NodeAttachment
    {
        public Morph Morph { get; set; }

        public NodeMorphAttachment() : base( NodeAttachmentType.Morph ) { }

        public NodeMorphAttachment( Morph morph ) : base( NodeAttachmentType.Morph ) => Morph = morph;

        public override object GetValue()
        {
            return Morph;
        }
    }

    public enum NodeAttachmentType
    {
        Invalid = 0,
        Scene = 1,
        Mesh = 2,
        Node = 3,
        Geometry = 4,
        Camera = 5,
        Light = 6,
        Epl = 7,
        EplLeaf = 8,
        Morph = 9,
    }
}