using System.ComponentModel;
using GFDLibrary;
using GFDLibrary.Models;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MorphViewNode : DataViewNode<Morph>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public int TargetCount => Data.TargetCount;

        [DisplayName( "Target ints" )]
        public int[] TargetInts
        {
            get => Data.TargetInts;
            set => SetDataProperty( value );
        }

        [DisplayName( "Node name" )]
        public string NodeName
        {
            get => Data.NodeName;
            set => SetDataProperty( value );
        }

        public MorphViewNode( string text, Morph data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Morph>( path => Data.Save( path ) );
            RegisterReplaceHandler<Morph>( Resource.Load<Morph> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}