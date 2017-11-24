using System.ComponentModel;
using System.IO;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialAttributeType6ViewModel : MaterialAttributeViewModel<MaterialAttributeType6>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        [ Browsable( true ) ]
        public int Field0C
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public int Field10
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public int Field14
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType6ViewModel( string text, MaterialAttributeType6 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType6> );
        }
    }
}