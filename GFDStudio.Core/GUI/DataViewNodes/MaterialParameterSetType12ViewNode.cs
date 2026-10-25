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
            get => Data.BaseColor;
            set => SetDataProperty(value); 
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color BaseColorRGBA
        {
            get => Data.BaseColor.ToByte();
            set => Data.BaseColor = value.ToFloat();
        }
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 EdgeColor {
            get => Data.EdgeColor;
            set => SetDataProperty(value); 
        } // 0xb0
        [DisplayName( "Edge Color (RGBA)" )]
        public System.Drawing.Color EdgeColorRGBA
        {
            get => Data.EdgeColor.ToByte();
            set => Data.EdgeColor = value.ToFloat();
        }
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor {
            get => Data.EmissiveColor;
            set => SetDataProperty(value); 
        } // 0xc0
        [DisplayName( "Emissive Color (RGBA)" )]
        public System.Drawing.Color EmissiveColorRGBA
        {
            get => Data.EmissiveColor.ToByte();
            set => Data.EmissiveColor = value.ToFloat();
        }
        [DisplayName( "Metallic" )]
        public float Metallic {
            get => Data.Metallic;
            set => SetDataProperty(value); 
        } // 0xe8
        [DisplayName( "Edge Threshold" )]
        public float EdgeThreshold {
            get => Data.EdgeThreshold;
            set => SetDataProperty(value); 
        } // 0xf4
        [DisplayName( "Edge Factor" )]
        public float EdgeFactor {
            get => Data.EdgeFactor;
            set => SetDataProperty(value); 
        } // 0xf8
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType12.Type12Flags> ) )]
        public MaterialParameterSetType12.Type12Flags Flags {
            get => Data.Flags;
            set => SetDataProperty(value); 
        } // 0x12c
        public float P12_7 {
            get => Data.P12_7;
            set => SetDataProperty(value); 
        } // 0x108
        public Vector3 P12_8 {
            get => Data.P12_8;
            set => SetDataProperty(value); 
        } // 0x110
        [DisplayName( "Mat Bloom Intensity" )]
        public float P12_9 {
            get => Data.MatBloomIntensity;
            set => SetDataProperty(value); 
        } // 0xf0
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float EdgeRemoveYAxisFactor {
            get => Data.EdgeRemoveYAxisFactor;
            set => SetDataProperty(value); 
        } // 0xfc
        public float P12_11 {
            get => Data.P12_11;
            set => SetDataProperty(value); 
        } // 0x11c
        public float P12_12 {
            get => Data.P12_12; 
            set => SetDataProperty(value); 
        } // 0x120
        public float P12_13 {
            get => Data.P12_13;
            set => SetDataProperty(value); 
        } // 0x124
        [DisplayName( "Mat Bloom Intensity" )]
        public float MatBloomIntensity2 {
            get => Data.MatBloomIntensity2;
            set => SetDataProperty(value); 
        } // 0x10c
        [DisplayName( "Specular Color" )]
        public Vector3 SpecularColor {
            get => Data.SpecularColor;
            set => SetDataProperty(value); 
        } // 0xd0
        [DisplayName( "Specular Threshold" )]
        public float SpecularThreshold {
            get => Data.SpecularThreshold;
            set => SetDataProperty(value); 
        } // 0xdc
        [DisplayName( "Specular Power" )]
        public float SpecularPower {
            get => Data.SpecularPower;
            set => SetDataProperty(value); 
        } // 0xe0
        [DisplayName( "Mat Roughness" )]
        public float MatRoughness {
            get => Data.MatRoughness;
            set => SetDataProperty(value); 
        } // 0xec
        [DisplayName( "Mat Ramp Alpha" )]
        public float MatRampAlpha {
            get => Data.P12_19;
            set => SetDataProperty(value); 
        } // 0xe4
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor {
            get => Data.ShadowColor;
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color ShadowColorRGBA
        {
            get => Data.ShadowColor.ToByte();
            set => Data.ShadowColor = value.ToFloat();
        }
        [DisplayName( "Shadow Theshold" )]
        public float ShadowThreshold {
            get => Data.ShadowThreshold; 
            set => SetDataProperty(value); 
        } // 0x100
        [DisplayName( "Shadow Factor" )]
        public float ShadowFactor {
            get => Data.ShadowFactor; 
            set => SetDataProperty(value); 
        } // 0x104

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
