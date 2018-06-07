using System;
using GFDLibrary.IO;

namespace GFDLibrary
{
    public abstract class NodeAttachment
    {
        public NodeAttachmentType Type { get; }

        protected NodeAttachment( NodeAttachmentType type )
        {
            Type = type;
        }

        public abstract Resource GetValue();

        public T GetValue<T>() where T : Resource => ( T )GetValue();

        public static NodeAttachment Create( Resource resource )
        {
            switch ( resource.ResourceType )
            {
                case ResourceType.Scene:
                    return new NodeSceneAttachment( ( Scene ) resource );
                case ResourceType.Node:
                    return new NodeNodeAttachment( ( Node ) resource );
                case ResourceType.Geometry:
                    return new NodeGeometryAttachment( ( Geometry )resource );
                case ResourceType.Camera:
                    return new NodeCameraAttachment( ( Camera )resource );
                case ResourceType.Light:
                    return new NodeLightAttachment( ( Light )resource );
                case ResourceType.Epl:
                    return new NodeEplAttachment( ( Epl )resource );
                case ResourceType.EplLeaf:
                    return new NodeEplLeafAttachment( ( EplLeaf )resource );
                case ResourceType.Morph:
                    return new NodeMorphAttachment( ( Morph )resource );
                default:
                    throw new ArgumentOutOfRangeException( nameof( resource ), "Unsupported resource type" );
            }
        }

        internal static NodeAttachment Read( ResourceReader reader, uint version, out bool skipProperties )
        {
            NodeAttachmentType type = ( NodeAttachmentType )reader.ReadInt32();

            skipProperties = false;

            switch ( type )
            {
                case NodeAttachmentType.Invalid:
                case NodeAttachmentType.Scene:
                    throw new NotSupportedException();

                //case NodeAttachmentType.Mesh:
                //  return new NodeMeshAttachment( ReadMesh( version ) );
                case NodeAttachmentType.Node:
                    return new NodeNodeAttachment( reader.Read<Node>(version) );
                case NodeAttachmentType.Geometry:
                    return new NodeGeometryAttachment( reader.Read<Geometry>( version ) );
                case NodeAttachmentType.Camera:
                    return new NodeCameraAttachment( reader.Read<Camera>( version ) );
                case NodeAttachmentType.Light:
                    return new NodeLightAttachment( reader.Read<Light>( version ) );
                case NodeAttachmentType.Epl:
                    return new NodeEplAttachment( Epl.Read(reader, version, out skipProperties) );
                //case NodeAttachmentType.EplLeaf:
                //    return new NodeEplLeafAttachment( ReadEplLeaf( version ) );
                case NodeAttachmentType.Morph:
                    return new NodeMorphAttachment( reader.Read<Morph>( version ) );
                default:
                    throw new NotImplementedException( $"Unimplemented node attachment type: {type}" );
            }
        }

        internal void Write( ResourceWriter writer )
        {
            writer.Write( ( int ) Type );
            writer.WriteResource( GetValue() );
        }
    }

    public sealed class NodeSceneAttachment : NodeAttachment
    {
        public Scene Scene { get; set; }

        public NodeSceneAttachment() : base( NodeAttachmentType.Scene ) { }

        public NodeSceneAttachment( Scene scene ) : base( NodeAttachmentType.Scene ) => Scene = scene;

        public override Resource GetValue()
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

        public override Resource GetValue()
        {
            return Node;
        }
    }

    public sealed class NodeGeometryAttachment : NodeAttachment
    {
        public Geometry Geometry { get; set; }

        public NodeGeometryAttachment() : base( NodeAttachmentType.Geometry ) { }

        public NodeGeometryAttachment( Geometry geometry ) : base( NodeAttachmentType.Geometry ) => Geometry = geometry;

        public override Resource GetValue()
        {
            return Geometry;
        }
    }

    public sealed class NodeCameraAttachment : NodeAttachment
    {
        public Camera Camera { get; set; }

        public NodeCameraAttachment() : base( NodeAttachmentType.Camera ) { }

        public NodeCameraAttachment( Camera camera ) : base( NodeAttachmentType.Camera ) => Camera = camera;

        public override Resource GetValue()
        { 
            return Camera;
        }
    }
    public sealed class NodeLightAttachment : NodeAttachment
    {
        public Light Light { get; set; }

        public NodeLightAttachment() : base( NodeAttachmentType.Light ) { }

        public NodeLightAttachment( Light light ) : base( NodeAttachmentType.Light ) => Light = light;

        public override Resource GetValue()
        {
            return Light;
        }
    }

    public sealed class NodeEplAttachment : NodeAttachment
    {
        public Epl Epl { get; set; }

        public NodeEplAttachment() : base( NodeAttachmentType.Epl ) { }

        public NodeEplAttachment( Epl epl ) : base( NodeAttachmentType.Epl ) => Epl = epl;

        public override Resource GetValue()
        {
            return Epl;
        }
    }

    public sealed class NodeEplLeafAttachment : NodeAttachment
    {
        public EplLeaf EplLeaf { get; set; }

        public NodeEplLeafAttachment() : base( NodeAttachmentType.EplLeaf ) { }

        public NodeEplLeafAttachment( EplLeaf eplLeaf ) : base( NodeAttachmentType.EplLeaf ) => EplLeaf = eplLeaf;

        public override Resource GetValue()
        {
            return EplLeaf;
        }
    }

    public sealed class NodeMorphAttachment : NodeAttachment
    {
        public Morph Morph { get; set; }

        public NodeMorphAttachment() : base( NodeAttachmentType.Morph ) { }

        public NodeMorphAttachment( Morph morph ) : base( NodeAttachmentType.Morph ) => Morph = morph;

        public override Resource GetValue()
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