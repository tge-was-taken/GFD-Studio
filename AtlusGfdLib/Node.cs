using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Node
    {
        private string mName;
        public string Name
        {
            get => mName;
            set
            {
                mName = value ?? throw new ArgumentNullException( nameof( value ) );
            }
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
        public Dictionary<string, NodeProperty> Properties { get; set; }

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

                    if ( mParent != null )
                        mParent.AddChildNode( this );
                }
            }
        }

        public bool HasParent => Parent != null;

        private List<Node> mChildren;
        public ReadOnlyCollection<Node> Children => mChildren.AsReadOnly();

        public bool HasChildren => Children != null && Children.Count > 0;

        public int ChildCount => HasChildren ? Children.Count : 0;

        internal Node()
        {
            Attachments = new List<NodeAttachment>();
            Properties = new Dictionary<string, NodeProperty>();
            mChildren = new List<Node>();
        }

        public Node( string name )
        {
            Name = name;
            Translation = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.Zero;
            Attachments = new List<NodeAttachment>();
            Properties = new Dictionary<string, NodeProperty>();
            FieldE0 = 1.0f;
            mChildren = new List<Node>();
        }

        public Node( string name, Vector3 position, Quaternion rotation, Vector3 scale )
        {
            Name = name;
            Translation = position;
            Rotation = rotation;
            Scale = scale;
            Attachments = new List<NodeAttachment>();
            Properties = new Dictionary<string, NodeProperty>();
            FieldE0 = 1.0f;
            mChildren = new List<Node>();
        }

        public void AddChildNode( Node node )
        {
            node.Parent = this;

            if (!mChildren.Contains(node))
                mChildren.Add( node );
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
    }
}