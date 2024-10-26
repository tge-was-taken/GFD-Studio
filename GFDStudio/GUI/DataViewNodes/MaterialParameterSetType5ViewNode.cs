using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType5ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType5>
    {
        public MaterialParameterSetType5ViewNode( string text, MaterialParameterSetType5 data ) : base( text, data )
        {
        }

        public float P5_0 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P5_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x94
        [DisplayName( "TC Scale" )]
        public float P5_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x98
        public float P5_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x9c
        [DisplayName( "Ocean Depth Scale" )]
        public float P5_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Disturbance Camera Scale" )]
        public float P5_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa4
        [DisplayName( "Disturbance Depth Scale" )]
        public float P5_6 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa8
        [DisplayName( "Scattering Camera Scale" )]
        public float P5_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xac
        [DisplayName( "Disturbance Tolerance" )]
        public float P5_8 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        [DisplayName( "Foam Distance" )]
        public float P5_9 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb4
        [DisplayName( "Caustics Tolerance" )]
        public float P5_10 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb8
        public float P5_11 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xbc
        [DisplayName( "Texture Animation Speed" )]
        public float P5_12 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float P5_13 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc4
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType5.Type5Flags> ) )]
        public MaterialParameterSetType5.Type5Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType5.Type5Flags>(); 
            set => SetDataProperty(value); 
        } // 0xc8

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
