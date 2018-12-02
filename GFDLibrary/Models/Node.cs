using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using GFDLibrary.Common;
using GFDLibrary.Effects;
using GFDLibrary.IO;

namespace GFDLibrary.Models
{
    public sealed class Node : Resource
    {
        public override ResourceType ResourceType => ResourceType.Node;

        private string mName;
        public string Name
        {
            get => mName;
            set => mName = value ?? throw new ArgumentNullException( nameof( value ) );
        }

        // 90
        private Vector3 mTranslation;
        public Vector3 Translation
        {
            get => mTranslation;
            set
            {
                mTranslation = value;
                mTransformDirty = true;
            }
        }

        // A0
        private Quaternion mRotation;
        public Quaternion Rotation
        {
            get => mRotation;
            set
            {
                mRotation = value;
                mTransformDirty = true;
            }
        }

        // B0
        private Vector3 mScale;
        public Vector3 Scale
        {
            get => mScale;
            set
            {
                mScale = value;
                mTransformDirty = true;
            }
        }

        private bool mTransformDirty;
        private Matrix4x4 mLocalTransform;
        public Matrix4x4 LocalTransform
        {
            get
            {
                if (mTransformDirty)
                {
                    mLocalTransform = Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale( Scale );
                    mLocalTransform.Translation = Translation;
                    mTransformDirty = false;
                }

                return mLocalTransform;
            }

            set
            {
                mLocalTransform = value;
                Matrix4x4.Decompose( mLocalTransform, out var scale, out var rotation, out var translation );
                Scale = scale;
                Rotation = rotation;
                Translation = translation;
                mTransformDirty = false;
            }
        }

        public Matrix4x4 WorldTransform
        {
            get
            {
                var transform = LocalTransform;
                if ( Parent != null )
                    transform *= Parent.WorldTransform;

                return transform;
            }
        }

        // 
        public List<NodeAttachment> Attachments { get; set; }


        public bool HasAttachments => Attachments != null && Attachments.Count > 0;

        public int AttachmentCount => HasAttachments ? Attachments.Count : 0;

        // EC
        public UserPropertyDictionary Properties { get; set; }

        public bool HasProperties => Properties != null && Properties.Count > 0;

        public int PropertyCount => HasProperties ? Properties.Count : 0;

        // E0
        public float FieldE0 { get; set; }

        private Node mParent;
        public Node Parent
        {
            get => mParent;
            set
            {
                if ( mParent != value )
                {
                    mParent = value;

                    mParent?.AddChildNode( this );
                }
            }
        }

        public bool HasParent => Parent != null;

        private readonly List<Node> mChildren;
        public ReadOnlyCollection<Node> Children => mChildren.AsReadOnly();

        public bool HasChildren => Children != null && Children.Count > 0;

        public int ChildCount => HasChildren ? Children.Count : 0;

        public Node()
        {
            Attachments = new List<NodeAttachment>();
            Properties = new UserPropertyDictionary();
            mChildren = new List<Node>();
            FieldE0 = 1.0f;
        }

        public Node( uint version ) : base(version)
        {
            Attachments = new List<NodeAttachment>();
            Properties = new UserPropertyDictionary();
            mChildren = new List<Node>();
            FieldE0 = 1.0f;
        }

        public Node( string name ) : this()
        {
            Name = name;
            Translation = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }

        public Node( string name, Vector3 position, Quaternion rotation, Vector3 scale ) : this()
        {
            Name = name;
            Translation = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Node( string name, Matrix4x4 localTransform ) : this()
        {
            Name = name;
            LocalTransform = localTransform;
        }

        public void AddChildNode( Node node )
        {
            node.Parent = this;

            if (!mChildren.Contains(node))
                mChildren.Add( node );
        }

        public void RemoveChildNode( Node node )
        {
            mChildren.Remove( node );
        }

        public bool FindNodeDepthFirst( string name, out Node node )
        {
            if ( Name == name )
            {
                node = this;
                return true;
            }

            foreach ( var childNode in Children )
            {
                if ( childNode.FindNodeDepthFirst( name, out node ) )
                    return true;
            }

            node = null;
            return false;
        }

        public bool FindNodeBreadthFirst( string name, out Node node )
        {
            if ( Name == name )
            {
                node = this;
                return true;
            }

            foreach ( var childNode in Children )
            {
                if ( childNode.Name == name )
                {
                    node = childNode;
                    return true;
                }
            }

            foreach ( var childNode in Children )
            {
                if ( childNode.FindNodeBreadthFirst( name, out node ) )
                    return true;
            }

            node = null;
            return false;
        }

        public bool FindParentNode( string name, out Node node )
        {
            if ( Parent == null )
            {
                node = null;
                return false;
            }
            else if ( Parent.Name == name )
            {
                node = Parent;
                return true;
            }
            else
            {
                return Parent.FindParentNode( name, out node );
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        internal static Node ReadRecursive( ResourceReader reader, uint version )
        {
            var node = reader.ReadResource<Node>( version );

            int childCount = reader.ReadInt32();

            var childStack = new Stack<Node>();
            for ( int i = 0; i < childCount; i++ )
            {
                var childNode = ReadRecursive( reader, version );
                childStack.Push( childNode );
            }

            while ( childStack.Count > 0 )
                node.AddChildNode( childStack.Pop() );

            return node;
        }

        internal static void WriteRecursive( ResourceWriter writer, Node node )
        {
            writer.WriteResource( node );
            writer.WriteInt32( node.ChildCount );

            for ( int i = 0; i < node.ChildCount; i++ )
            {
                WriteRecursive( writer, node.Children[( node.ChildCount - 1 ) - i] );
            }
        }

        internal override void Read( ResourceReader reader )
        {
            Name = reader.ReadStringWithHash( Version );
            Translation = reader.ReadVector3();
            Rotation = reader.ReadQuaternion();
            Scale = reader.ReadVector3();

            if ( Version <= 0x1090000 )
                reader.ReadByte();

            int attachmentCount = reader.ReadInt32();
            var skipProperties = false;
            for ( int i = 0; i < attachmentCount; i++ )
            {
                var attachment = NodeAttachment.Read( reader, Version );
                Attachments.Add( attachment );

                if ( attachment.Type == NodeAttachmentType.Epl && attachment.GetValue<Epl>().IncludesProperties )
                {
                    // If we read an EPL attachment that includes node property data, we can't continue.
                    // We must also remember to skip reading the properties, as they are contained in the EPL data.
                    skipProperties = true;
                    break;
                }
            }

            // Don't read properties if we read an EPL that contains the data
            if ( Version > 0x1060000 && !skipProperties )
            {
                var hasProperties = reader.ReadBoolean();
                if ( hasProperties )
                    Properties = reader.ReadResource<UserPropertyDictionary>( Version );
            }

            if ( Version > 0x1104230 )
                FieldE0 = reader.ReadSingle();
        }

        internal override void Write( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, Name );
            writer.WriteVector3( Translation );
            writer.WriteQuaternion( Rotation );
            writer.WriteVector3( Scale );

            if ( Version <= 0x1090000 )
                writer.WriteByte( 0 );

            writer.WriteInt32( AttachmentCount );
            foreach ( var attachment in Attachments )
                attachment.Write( writer );

            // Dont read properties if we have an EPL attachment that contains our property data
            if ( Version > 0x1060000 && !Attachments.Any( x => x.Type == NodeAttachmentType.Epl && x.GetValue<Epl>().IncludesProperties ) )
            {
                writer.WriteBoolean( HasProperties );
                if ( HasProperties )
                    writer.WriteResource( Properties );
            }

            if ( Version > 0x1104230 )
                writer.WriteSingle( FieldE0 );
        }
    }
}