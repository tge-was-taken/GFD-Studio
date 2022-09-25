﻿using System;
using GFDLibrary.Cameras;
using GFDLibrary.Effects;
using GFDLibrary.IO;
using GFDLibrary.Lights;

namespace GFDLibrary.Models
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
                case ResourceType.Model:
                    return new NodeModelAttachment( ( Model ) resource );
                case ResourceType.Node:
                    return new NodeNodeAttachment( ( Node ) resource );
                case ResourceType.Mesh:
                    return new NodeMeshAttachment( ( Mesh )resource );
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

        public static bool IsOfCompatibleType( Resource resource )
        {
            switch ( resource.ResourceType )
            {
                case ResourceType.Model:
                case ResourceType.Node:
                case ResourceType.Mesh:
                case ResourceType.Camera:
                case ResourceType.Light:
                case ResourceType.Epl:
                case ResourceType.EplLeaf:
                case ResourceType.Morph:
                    return true;
                default:
                    return false;
            }
        }

        internal static NodeAttachment Read( ResourceReader reader, uint version )
        {
            NodeAttachmentType type = ( NodeAttachmentType )reader.ReadInt32();

            switch ( type )
            {
                case NodeAttachmentType.Invalid:
                case NodeAttachmentType.Model:
                    throw new NotSupportedException();

                //case NodeAttachmentType.Mesh:
                //  return new NodeMeshAttachment( ReadMesh( version ) );
                case NodeAttachmentType.Node:
                    return new NodeNodeAttachment( reader.ReadResource<Node>(version) );
                case NodeAttachmentType.Mesh:
                    return new NodeMeshAttachment( reader.ReadResource<Mesh>( version ) );
                case NodeAttachmentType.Camera:
                    return new NodeCameraAttachment( reader.ReadResource<Camera>( version ) );
                case NodeAttachmentType.Light:
                    return new NodeLightAttachment( reader.ReadResource<Light>( version ) );
                case NodeAttachmentType.Epl:
                    return new NodeEplAttachment( reader.ReadResource<Epl>( version ) );
                case NodeAttachmentType.EplLeaf:
                    return new NodeEplLeafAttachment( reader.ReadResource<EplLeaf>( version ) );
                case NodeAttachmentType.Morph:
                    return new NodeMorphAttachment( reader.ReadResource<Morph>( version ) );
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

    public sealed class NodeModelAttachment : NodeAttachment
    {
        public Model Model { get; set; }

        public NodeModelAttachment() : base( NodeAttachmentType.Model ) { }

        public NodeModelAttachment( Model model ) : base( NodeAttachmentType.Model ) => Model = model;

        public override Resource GetValue()
        {
            return Model;
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

    public sealed class NodeMeshAttachment : NodeAttachment
    {
        public Mesh Mesh { get; set; }

        public NodeMeshAttachment() : base( NodeAttachmentType.Mesh ) { }

        public NodeMeshAttachment( Mesh mesh ) : base( NodeAttachmentType.Mesh ) => Mesh = mesh;

        public override Resource GetValue()
        {
            return Mesh;
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
        Model = 1,
        Unknown = 2,
        Node = 3,
        Mesh = 4,
        Camera = 5,
        Light = 6,
        Epl = 7,
        EplLeaf = 8,
        Morph = 9,
    }
}