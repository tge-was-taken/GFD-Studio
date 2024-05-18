using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Misc;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F9ViewNode : ResourceViewNode<ChunkType000100F9>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Branch;

        [Browsable( true )]
        [DisplayName( "Iterations" )]
        public int Field140
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Deceleration" )]
        public float Field13C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Max Steps" )]
        public float Field138
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Transfers tolerance" )]
        public float Field134
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Velocity tolerance" )]
        public float Field130
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( false )]
        public ListViewNode<ChunkType000100F9Entry1> Entry1ListViewNode { get; set; }

        [Browsable( false )]
        public ListViewNode<ChunkType000100F9Entry2> Entry2ListViewNode { get; set; }

        [Browsable( false )]
        public ListViewNode<ChunkType000100F9Entry3> Entry3ListViewNode { get; set; }

        public ChunkType000100F9ViewNode( string text, ChunkType000100F9 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            RegisterModelUpdateHandler( () =>
            {
                var resource = new ChunkType000100F9( Version )
                {
                    Field140 = Field140,
                    Field13C = Field13C,
                    Field138 = Field138,
                    Field134 = Field134,
                    Field130 = Field130,
                    Entry1List = Entry1ListViewNode.Data,
                    Entry2List = Entry2ListViewNode.Data,
                    Entry3List = Entry3ListViewNode.Data
                };

                return resource;
            } );
        }

        protected override void InitializeViewCore()
        {
            Entry1ListViewNode =
                ( ListViewNode<ChunkType000100F9Entry1> )DataViewNodeFactory.Create(
                    "Entry Type 1 List", Data.Entry1List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry1>( ( item, index ) => item.NodeName ?? index.ToString() ) } );

            AddChildNode( Entry1ListViewNode );

            Entry2ListViewNode =
                ( ListViewNode<ChunkType000100F9Entry2> )DataViewNodeFactory.Create(
                    "Entry Type 2 List", Data.Entry2List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry2>( ( item, index ) => item.NodeName ?? index.ToString() ) } );

            AddChildNode( Entry2ListViewNode );

            Entry3ListViewNode =
                ( ListViewNode<ChunkType000100F9Entry3> )DataViewNodeFactory.Create(
                    "Entry Type 3 List", Data.Entry3List,
                    new object[] { new ListItemNameProvider<ChunkType000100F9Entry3>( ( item, index ) => index.ToString() ) } );

            AddChildNode( Entry3ListViewNode );
        }
    }
}