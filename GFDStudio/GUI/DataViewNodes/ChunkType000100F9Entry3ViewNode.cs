using System.ComponentModel;
using GFDLibrary.Misc;

namespace GFDStudio.GUI.DataViewNodes
{
    public class ChunkType000100F9Entry3ViewNode : DataViewNode<ChunkType000100F9Entry3>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags =>
            DataViewNodeFlags.Leaf;

        [Browsable( true )]
        [DisplayName( "Gravity" )]
        public float Field00
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Maximum rotation angle" )]
        public float Field04
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Bone thickness" )]
        public float Field08
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Parent bone" )]
        public short Field0C
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        [Browsable( true )]
        [DisplayName( "Child bone" )]
        public short Field0E
        {
            get => GetDataProperty<short>();
            set => SetDataProperty( value );
        }

        public ChunkType000100F9Entry3ViewNode( string text, ChunkType000100F9Entry3 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
        }
    }
}