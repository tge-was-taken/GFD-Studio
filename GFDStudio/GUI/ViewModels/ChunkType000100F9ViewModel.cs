using System.ComponentModel;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class ChunkType000100F9ViewModel : ResourceViewModel<ChunkType000100F9>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags =>
            TreeNodeViewModelFlags.Branch;

        [Browsable( true )]
        public int Field140
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field13C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field138
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field134
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field130
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( false )]
        public ListViewModel<ChunkType000100F9Entry1> Entry1ListViewModel { get; set; }

        [Browsable( false )]
        public ListViewModel<ChunkType000100F9Entry2> Entry2ListViewModel { get; set; }

        [Browsable( false )]
        public ListViewModel<ChunkType000100F9Entry3> Entry3ListViewModel { get; set; }

        public ChunkType000100F9ViewModel( string text, ChunkType000100F9 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<ChunkType000100F9>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<ChunkType000100F9>( Resource.Load<ChunkType000100F9> );
            RegisterModelUpdateHandler( () =>
            {
                var resource = new ChunkType000100F9( Version )
                {
                    Field140 = Field140,
                    Field13C = Field13C,
                    Field138 = Field138,
                    Field134 = Field134,
                    Field130 = Field130,
                    Entry1List = Entry1ListViewModel.Model,
                    Entry2List = Entry2ListViewModel.Model,
                    Entry3List = Entry3ListViewModel.Model
                };

                return resource;
            } );
        }

        protected override void InitializeViewCore()
        {
            Entry1ListViewModel =
                ( ListViewModel<ChunkType000100F9Entry1> )TreeNodeViewModelFactory.Create(
                    "Entry Type 1 List", Model.Entry1List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry1>( ( item, index ) => item.NodeName == null ? index.ToString() : item.NodeName ) } );

            Nodes.Add( Entry1ListViewModel );

            Entry2ListViewModel =
                ( ListViewModel<ChunkType000100F9Entry2> )TreeNodeViewModelFactory.Create(
                    "Entry Type 2 List", Model.Entry2List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry2>( ( item, index ) => item.NodeName == null ? index.ToString() : item.NodeName ) } );

            Nodes.Add( Entry2ListViewModel );

            Entry3ListViewModel =
                ( ListViewModel<ChunkType000100F9Entry3> )TreeNodeViewModelFactory.Create(
                    "Entry Type 3 List", Model.Entry3List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry3>( ( item, index ) => index.ToString() ) } );

            Nodes.Add( Entry3ListViewModel );
        }
    }
}