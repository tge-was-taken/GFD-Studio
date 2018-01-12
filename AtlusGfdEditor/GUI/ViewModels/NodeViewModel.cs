using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class NodeViewModel : TreeNodeViewModel<Node>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename;

        public override TreeNodeViewModelFlags NodeFlags 
            => Model.HasChildren ? TreeNodeViewModelFlags.Branch : TreeNodeViewModelFlags.Leaf;

        [ Browsable( true ) ]
        public new string Name
        {
            get => GetModelProperty< string >();
            set
            {
                SetModelProperty( value );
                base.Name = Text = value;
            }
        }

        [ Browsable( true ) ]
        [ TypeConverter( typeof( Vector3TypeConverter ) ) ]
        public Vector3 Translation
        {
            get => GetModelProperty< Vector3 >();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( QuaternionTypeConverter ) )]
        public Quaternion Rotation
        {
            get => GetModelProperty<Quaternion>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Scale
        {
            get => GetModelProperty<Vector3>();
            set => SetModelProperty( value );
        }

        [Browsable(false)]
        public ListViewModel<NodeAttachment> AttachmentListViewModel { get; set; }

        [ Browsable( true ) ]
        [ TypeConverter( typeof( UserPropertyDictionaryTypeConverter ) ) ]
        public UserPropertyCollection Properties
        {
            get => GetModelProperty<UserPropertyCollection>();
            set => SetModelProperty( value );
        }

        [ Browsable( true ) ]
        public float FieldE0
        {
            get => GetModelProperty< float >();
            set => SetModelProperty( value );
        }

        [Browsable(false)]
        public ListViewModel<Node> ChildrenListViewModel { get; set; }

        public NodeViewModel( string text, Node resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            /*
            AttachmentListViewModel = ( ListViewModel<NodeAttachment> )TreeNodeViewModelFactory.Create(
                "Attachments",
                Model.Attachments == null ? new List<NodeAttachment>() : Model.Attachments,
                new object[] { new ListItemNameProvider<NodeAttachment>( ( value, index ) => value.Type.ToString() ) } );

            Nodes.Add( AttachmentListViewModel );
            */

            var children = Model.Children.ToList();
            ChildrenListViewModel = ( ListViewModel<Node> )TreeNodeViewModelFactory.Create(
                "Children",
                children,
                new object[] { new ListItemNameProvider<Node>( ( value, index ) => value.Name ) } );

            Nodes.Add( ChildrenListViewModel );
        }
    }
}