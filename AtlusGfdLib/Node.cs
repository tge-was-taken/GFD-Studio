using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace AtlusGfdLib
{
    public sealed class Node
    {
        public string Name { get; set; }

        // 90
        public Vector3 Position { get; set; }

        // A0
        public Quaternion Rotation { get; set; }

        // B0
        public Vector3 Scale { get; set; }

        // 
        public List<NodeAttachment> Attachments { get; set; }

        // EC
        public Dictionary<string, NodeProperty> Properties { get; set; }

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

        private List<Node> mChildren;
        public ReadOnlyCollection<Node> Children => mChildren.AsReadOnly();

        internal Node()
        {
            Attachments = new List<NodeAttachment>();
            Properties = new Dictionary<string, NodeProperty>();
            mChildren = new List<Node>();
        }

        public Node( string name )
        {
            Name = name;
            Position = Vector3.Zero;
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
            Position = position;
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

        public bool FindNode( string name, out Node node )
        {
            if ( Name == name )
            {
                node = this;
                return true;
            }

            foreach ( var childNode in Children )
            {
                if ( childNode.FindNode( name, out node ) )
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