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
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public float P6_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd4
        public float P6_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd8
        public float P6_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xdc
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType6.Type6Flags> ) )]
        public MaterialParameterSetType6.Type6Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType6.Type6Flags>(); 
            set => SetDataProperty(value); 
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
        public Vector4 Field0
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color Field0_RGBA
        {
            get => Field0.ToByte();
            set => Field0 = value.ToFloat();
        }
        public float Field1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }
        public float Field4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
