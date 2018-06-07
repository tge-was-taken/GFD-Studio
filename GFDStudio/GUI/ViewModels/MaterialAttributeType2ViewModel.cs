using System.ComponentModel;
using System.IO;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class MaterialAttributeType2ViewModel : MaterialAttributeViewModel<MaterialAttributeType2>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        public int Field0C
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        // 10
        [Browsable( true )]
        public int Field10
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType2ViewModel( string text, MaterialAttributeType2 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType2> );
        }
    }
}