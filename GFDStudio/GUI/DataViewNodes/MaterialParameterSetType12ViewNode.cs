using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType12ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType12>
    {
        public MaterialParameterSetType12ViewNode( string text, MaterialParameterSetType12 data ) : base( text, data )
        {
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 P12_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color P12_0_RGBA
        {
            get => Data.P12_0.ToByte();
            set => Data.P12_0 = value.ToFloat();
        }
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 P12_1 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        [DisplayName( "Edge Color (RGBA)" )]
        public System.Drawing.Color P12_1_RGBA
        {
            get => Data.P12_1.ToByte();
            set => Data.P12_1 = value.ToFloat();
        }
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 P12_2 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        [DisplayName( "Emissive Color (RGBA)" )]
        public System.Drawing.Color P12_2_RGBA
        {
            get => Data.P12_2.ToByte();
            set => Data.P12_2 = value.ToFloat();
        }
        [DisplayName( "Metallic" )]
        public float P12_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe8
        [DisplayName( "Edge Threshold" )]
        public float P12_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf4
        [DisplayName( "Edge Factor" )]
        public float P12_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf8
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType12.Type12Flags> ) )]
        public MaterialParameterSetType12.Type12Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType12.Type12Flags>(); 
            set => SetDataProperty(value); 
        } // 0x12c
        public float P12_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x108
        public Vector3 P12_8 { 
            get => GetDataProperty<Vector3>(); 
            set => SetDataProperty(value); 
        } // 0x110
        [DisplayName( "Mat Bloom Intensity" )]
        public float P12_9 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xf0
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float P12_10 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xfc
        public float P12_11 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x11c
        public float P12_12 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x120
        public float P12_13 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x124
        [DisplayName( "Mat Bloom Intensity" )]
        public float P12_14 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x10c
        [DisplayName( "Specular Color" )]
        public Vector3 P12_15 { 
            get => GetDataProperty<Vector3>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        [DisplayName( "Specular Threshold" )]
        public float P12_16 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xdc
        [DisplayName( "Specular Power" )]
        public float P12_17 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe0
        [DisplayName( "Mat Roughness" )]
        public float P12_18 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xec
        [DisplayName( "Mat Ramp Alpha" )]
        public float P12_19 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xe4
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 P12_20 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color P12_20_RGBA
        {
            get => Data.P12_20.ToByte();
            set => Data.P12_20 = value.ToFloat();
        }
        [DisplayName( "Shadow Theshold" )]
        public float P12_21 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x100
        [DisplayName( "Shadow Factor" )]
        public float P12_22 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0x104

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
