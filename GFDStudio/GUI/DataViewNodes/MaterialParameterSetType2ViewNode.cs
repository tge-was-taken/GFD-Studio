using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType2ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType2_3_13>
    {
        
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 P2_0
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color P2_0_RGBA
        {
            get => Data.P2_0.ToByte();
            set => Data.P2_0 = value.ToFloat();
        }
        
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 P2_1
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xa0
        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color P2_1_RGBA
        {
            get => Data.P2_1.ToByte();
            set => Data.P2_1 = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 P2_2
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xb0
        [DisplayName( "Edge Color (RGBA)" )]
        public System.Drawing.Color P2_2_RGBA
        {
            get => Data.P2_2.ToByte();
            set => Data.P2_2 = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 P2_3
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xc0
        [DisplayName( "Emissive Color (RGBA)" )]
        public System.Drawing.Color P2_3_RGBA
        {
            get => Data.P2_3.ToByte();
            set => Data.P2_3 = value.ToFloat();
        }

        [TypeConverter( typeof( Vector3TypeConverter ) )]
        [DisplayName( "Specular Color" )]
        public Vector3 P2_4
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        } // 0xd0
        [DisplayName( "Specular Power" )]
        public float P2_5
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe0
        [DisplayName( "Metallic" )]
        public float P2_6
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe4
        
        [DisplayName( "Edge Threshold" )]
        public float P2_7
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xf0
        
        [DisplayName( "Edge Factor" )]
        public float P2_8
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xf4
        
        [DisplayName( "Shadow Threshold" )]
        public float P2_9
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xfc
        
        [DisplayName( "Shadow Factor" )]
        public float P2_10
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x100
        public uint P2_11
        {
            get => GetDataProperty<uint>();
            set => SetDataProperty( value );
        } // 0x130
        public float P2_12
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x104
        public Vector3 P2_13
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        } // 0x10c
        
        [DisplayName( "Mat Bloom Intensity" )]
        public float P2_14
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xec
        [DisplayName( "Specular Threshold" )]
        public float P2_15
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xdc
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float P2_16
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xf8
        public float P2_17
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x118
        public float P2_18
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x11c
        public float P2_19
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x120
        public float P2_20
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x108
        [DisplayName( "Mat Roughness" )]
        public float P2_21
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe8
        [DisplayName( "Fitting Tile" )]
        public float P2_22
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x128
        [DisplayName( "Multi Fitting Tile" )]
        public float P2_23
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x12c
        public MaterialParameterSetType2ViewNode( string text, MaterialParameterSetType2_3_13 data ) : base( text, data )
        {
        }

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
