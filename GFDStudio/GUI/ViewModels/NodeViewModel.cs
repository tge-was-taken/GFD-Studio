using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public class NodeViewModel : TreeNodeViewModel<Node>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags 
            => ( Model.HasChildren || Model.HasAttachments ) ? TreeNodeViewModelFlags.Branch : TreeNodeViewModelFlags.Leaf;

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
        public ListViewModel<Resource> AttachmentListViewModel { get; set; }

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
            RegisterExportHandler<Node>( path => Model.Save( path ) );
            RegisterReplaceHandler<Node>( Resource.Load<Node> );
            RegisterCustomHandler( "Add Attachment", () =>
            {
                using ( var dialog = new OpenFileDialog() )
                {
                    dialog.Filter = ModuleFilterGenerator.GenerateFilterForAllSupportedImportFormats();
                    dialog.AutoUpgradeEnabled = true;
                    dialog.CheckPathExists = true;
                    dialog.FileName = Text;
                    dialog.Title = "Select a file to open.";
                    dialog.ValidateNames = true;
                    dialog.AddExtension = true;

                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;

                    try
                    {
                        var resource = Resource.Load( dialog.FileName );
                        AttachmentListViewModel.Nodes.Add( TreeNodeViewModelFactory.Create( resource.ResourceType.ToString(), resource ) );
                        AttachmentListViewModel.HasPendingChanges = true;
                    }
                    catch ( Exception e )
                    {
                    }
                }
            } );
            RegisterModelUpdateHandler( () =>
            {
                var node = new Node();
                node.Name = Name;
                node.Translation = Translation;
                node.Rotation = Rotation;
                node.Scale = Scale;
                node.Properties = Properties;
                node.FieldE0 = FieldE0;
                node.Attachments = Nodes.Contains( AttachmentListViewModel ) ? AttachmentListViewModel.Model.Select( NodeAttachment.Create ).ToList() : null;

                if ( Nodes.Contains( ChildrenListViewModel ) )
                {
                    foreach ( var childNode in ChildrenListViewModel.Model )
                        node.AddChildNode( childNode );
                }

                return node;
            });
            TextChanged += ( s, o ) => Name = Text;
        }

        protected override void InitializeViewCore()
        {
            AttachmentListViewModel = ( ListViewModel<Resource> )TreeNodeViewModelFactory.Create(
                "Attachments",
                Model.Attachments == null ? new List<Resource>() : Model.Attachments.Select( x => x.GetValue() ).ToList(),
                new object[] { new ListItemNameProvider<Resource>( ( value, index ) => value.ResourceType.ToString() ) } );

            Nodes.Add( AttachmentListViewModel );


            var children = Model.Children.ToList();
            ChildrenListViewModel = ( ListViewModel<Node> )TreeNodeViewModelFactory.Create(
                "Children",
                children,
                new object[] { new ListItemNameProvider<Node>( ( value, index ) => value.Name ) } );

            Nodes.Add( ChildrenListViewModel );
        }
    }
}