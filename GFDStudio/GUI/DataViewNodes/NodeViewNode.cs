using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Common;
using GFDLibrary.Models;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class NodeViewNode : DataViewNode<Node>
    {
        private VariantUserPropertyList mProperties;

        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Branch;

        [ Browsable( true ) ]
        public new string Name
        {
            get => GetDataProperty< string >();
            set
            {
                SetDataProperty( value );
                base.Name = Text = value;
            }
        }

        [ Browsable( true ) ]
        [ TypeConverter( typeof( Vector3TypeConverter ) ) ]
        public Vector3 Translation
        {
            get => GetDataProperty< Vector3 >();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( QuaternionTypeConverter ) )]
        public Quaternion Rotation
        {
            get => GetDataProperty<Quaternion>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Scale
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        }

        [Browsable(false)]
        public AttachmentListViewNode AttachmentListViewNode { get; set; }

        [ Browsable( true ) ]
        [ TypeConverter( typeof( ExpandableObjectConverter ) ) ]
        public VariantUserPropertyList Properties
        {
            get => mProperties;
            set
            {
                mProperties = value;
                Data.Properties = new UserPropertyDictionary( Properties.Select( x => x.ToTypedUserProperty() ) );
            }
        }

        [ Browsable( true ) ]
        public float FieldE0
        {
            get => GetDataProperty< float >();
            set => SetDataProperty( value );
        }

        [Browsable(false)]
        public ListViewNode<Node> Children { get; set; }

        public NodeViewNode( string text, Node data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( o, s ) => Text = Name = s.Label;

            RegisterExportHandler<Node>( path => Data.Save( path ) );
            RegisterReplaceHandler<Node>( Resource.Load<Node> );
            RegisterModelUpdateHandler( () =>
            {
                var node = new Node();
                node.Name = Name;
                node.Translation = Translation;
                node.Rotation = Rotation;
                node.Scale = Scale;
                node.Properties = new UserPropertyDictionary( Properties.Select( x => x.ToTypedUserProperty() ) );
                node.FieldE0 = FieldE0;
                node.Attachments = Nodes.Contains( AttachmentListViewNode ) ? AttachmentListViewNode.Data.Select( NodeAttachment.Create ).ToList() : null;

                if ( Nodes.Contains( Children ) )
                {
                    foreach ( var childNode in Children.Data )
                        node.AddChildNode( childNode );
                }

                return node;
            });
            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            Properties = new VariantUserPropertyList( Data.Properties, () => Properties = mProperties );

            AttachmentListViewNode = ( AttachmentListViewNode )DataViewNodeFactory.Create(
                "Attachments",
                Data.Attachments == null ? new List<Resource>() : Data.Attachments.Select( x => x.GetValue() ).ToList(),
                new object[] { new ListItemNameProvider<Resource>( ( value, index ) => value.ResourceType.ToString() ) } );

            AddChildNode( AttachmentListViewNode );


            var children = Data.Children.ToList();
            Children = ( NodeListViewNode )DataViewNodeFactory.Create(
                "Children",
                children,
                new object[] { new ListItemNameProvider<Node>( ( value, index ) => value.Name ) } );

            AddChildNode( Children );
        }
    }

    public class AttachmentListViewNode : ListViewNode<Resource>
    {
        public AttachmentListViewNode( string text, List<Resource> data, ListItemNameProvider<Resource> nameProvider ) : base( text, data, nameProvider )
        {
        }

        public AttachmentListViewNode( string text, List<Resource> data, IList<string> itemNames ) : base( text, data, itemNames )
        {
        }

        protected override void InitializeCore()
        {
            RegisterAddHandler<Resource>( file =>
            {
                var resource = Resource.Load( file );
                if ( NodeAttachment.IsOfCompatibleType( resource ) )
                {
                    AddChildNode( DataViewNodeFactory.Create( resource.ResourceType.ToString(), resource ) );
                }
                else
                {
                    MessageBox.Show( "This resource type is not supported as an attachment", "Error", MessageBoxButtons.OK );
                }
            } );

            base.InitializeCore();
        }
    }

    public class NodeListViewNode : ListViewNode<Node>
    {
        public NodeListViewNode( string text, List<Node> data, ListItemNameProvider<Node> nameProvider ) : base( text, data, nameProvider )
        {
        }

        public NodeListViewNode( string text, List<Node> data, IList<string> itemNames ) : base( text, data, itemNames )
        {
        }

        protected override void InitializeCore()
        {
            RegisterAddHandler<Node>( file =>
            {
                var node = Resource.Load<Node>( file );
                AddChildNode( DataViewNodeFactory.Create( node.Name, node ) );
            } );
            RegisterCustomHandler( "Add new", () => { AddChildNode( new NodeViewNode( "New node", new Node( "New node" ) ) ); } );
            base.InitializeCore();
        }
    }
}