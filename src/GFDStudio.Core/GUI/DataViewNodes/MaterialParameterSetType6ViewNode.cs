using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType6ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType6>
    {
        public MaterialParameterSetType6ViewNode( string text, MaterialParameterSetType6 data ) : base( text, data )
        {
            Layers = new();
        }
        public ListViewNode<MaterialParameterSetType6Field0ViewNode> P6_0 { get; set; }

        public float P6_1 { 
            get => Data.P6_1; 
            set => Set( x => x.P6_1, value ); 
        } // 0xd0
        public float P6_2 { 
            get => Data.P6_2; 
            set => Set( x => x.P6_2, value ); 
        } // 0xd4
        public float P6_3 { 
            get => Data.P6_3; 
            set => Set( x => x.P6_3, value ); 
        } // 0xd8
        public float P6_4 { 
            get => Data.P6_4; 
            set => Set( x => x.P6_4, value ); 
        } // 0xdc
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType6.Type6Flags> ) )]
        public MaterialParameterSetType6.Type6Flags Flags { 
            get => Data.Flags; 
            set => Set( x => x.Flags, value ); 
        } // 0xe0

        [Browsable( false )]
        public List<DataViewNode> Layers { get; set; }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Branch;

        protected override void InitializeCore()
        {

        }

        protected override void InitializeViewCore()
        {
            Layers.Clear();
            for ( int i = 0; i < Data.P6_0.Length; i++)
                Layers.Add( DataViewNodeFactory.Create( $"Layer {i}", Data.P6_0[i] ) );
            foreach (var layer in Layers)
                AddChildNode( layer );
        }
    }

    public class MaterialParameterSetType6Field0ViewNode : DataViewNode<MaterialParameterSetType6.MaterialParameterSetType6_Field0>
    {
        public MaterialParameterSetType6Field0ViewNode( string text, MaterialParameterSetType6.MaterialParameterSetType6_Field0 data ) : base( text, data ) {}

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 BaseColor
        {
            get => Data.BaseColor;
            set => Set( x => x.BaseColor, value );
        } // 0x90
        public float Field1 {
            get => Data.Field1;
            set => Set( x => x.Field1, value ); 
        }
        public float Field2 {
            get => Data.Field2;
            set => Set( x => x.Field2, value ); 
        }
        public float Field3 {
            get => Data.Field3;
            set => Set( x => x.Field3, value ); 
        }
        public float Field4 {
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
