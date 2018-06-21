using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class NodeViewNode : DataViewNode<Node>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Rename | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags 
            => ( Data.HasChildren || Data.HasAttachments ) ? DataViewNodeFlags.Branch : DataViewNodeFlags.Leaf;

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
        [ TypeConverter( typeof( UserPropertyDictionaryTypeConverter ) ) ]
        public UserPropertyCollection Properties
        {
            get => GetDataProperty<UserPropertyCollection>();
            set => SetDataProperty( value );
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
                node.Properties = Properties;
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
            AttachmentListViewNode = ( AttachmentListViewNode )DataViewNodeFactory.Create(
                "Attachments",
                Data.Attachments == null ? new List<Resource>() : Data.Attachments.Select( x => x.GetValue() ).ToList(),
                new object[] { new ListItemNameProvider<Resource>( ( value, index ) => value.ResourceType.ToString() ) } );

            Nodes.Add( AttachmentListViewNode );


            var children = Data.Children.ToList();
            Children = ( ListViewNode<Node> )DataViewNodeFactory.Create(
                "Children",
                children,
                new object[] { new ListItemNameProvider<Node>( ( value, index ) => value.Name ) } );

            Nodes.Add( Children );
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
                    Data.Add( resource );
                }
                else
                {
                    MessageBox.Show( "This resource type is not supported as an attachment", "Error", MessageBoxButtons.OK );
                }
            } );

            base.InitializeCore();
        }
    }
}