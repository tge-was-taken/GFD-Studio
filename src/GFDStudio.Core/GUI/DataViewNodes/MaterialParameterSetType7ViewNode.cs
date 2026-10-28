using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType7ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType7>
    {
        public MaterialParameterSetType7ViewNode( string text, MaterialParameterSetType7 data ) : base( text, data )
        {
        }

        public float P7_1 { 
            get => Data.P7_1; 
            set => Set( x => x.P7_1, value ); 
        } // 0xf0
        public MaterialParameterSetType7.Type7Flags Flags { 
            get => Data.Flags; 
            set => Set( x => x.Flags, value ); 
        } // 0xf4

        [Browsable( false )]
        public List<DataViewNode> Layers { get; set; }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected override void InitializeCore()
        {
            Layers.Clear();
            for ( int i = 0; i < Data.P7_0.Length; i++ )
                Layers.Add( DataViewNodeFactory.Create( $"Layer {i}", Data.P7_0[i] ) );
            foreach ( var layer in Layers )
                AddChildNode( layer );
        }
    }

    public class MaterialParameterSetType7Field0ViewNode : DataViewNode<MaterialParameterSetType7.MaterialParameterSetType7_Field0>
    {
        public MaterialParameterSetType7Field0ViewNode( string text, MaterialParameterSetType7.MaterialParameterSetType7_Field0 data ) : base( text, data ) { }
        public float Field0
        {
            get => Data.Field0;
            set => Set( x => x.Field0, value );
        }
        public float Field1
        {
            get => Data.Field1;
            set => Set( x => x.Field1, value );
        }
        public float Field2
        {
            get => Data.Field2;
            set => Set( x => x.Field2, value );
        }
        public float Field3
        {
            get => Data.Field3;
            set => Set( x => x.Field3, value );
        }
        public float Field4
        {
            get => Data.Field4;
            set => Set( x => x.Field4, value );
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
