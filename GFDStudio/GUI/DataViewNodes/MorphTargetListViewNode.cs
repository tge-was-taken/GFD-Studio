using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MorphTargetListViewNode : DataViewNode<MorphTargetList>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Add;

        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        public int Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        public MorphTargetListViewNode( string text, MorphTargetList data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<MorphTargetList>( path => Data.Save( path ) );
            RegisterReplaceHandler<MorphTargetList>( Resource.Load<MorphTargetList> );
            RegisterAddHandler<MorphTarget>( ( path ) =>
            {
                Data.Add( Resource.Load<MorphTarget>( path ) );
                InitializeView( true );
            } );
            RegisterModelUpdateHandler( () =>
            {
                var list = new MorphTargetList { Flags = Flags };
                foreach ( MorphTargetViewNode viewNode in Nodes )
                    list.Add( viewNode.Data );

                return list;
            } );
            RegisterCustomHandler( "Add new", () =>
            {
                Data.Add( new MorphTarget() );
                InitializeView( true );
            } );
        }

        protected override void InitializeViewCore()
        {
            for ( var i = 0; i < Data.Count; i++ )
                AddChildNode( DataViewNodeFactory.Create( $"Morph Target {i}", Data[ i ] ) );
        }
    }
}