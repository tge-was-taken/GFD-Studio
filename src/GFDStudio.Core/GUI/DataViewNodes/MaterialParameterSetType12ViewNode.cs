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
            set => Set( x => x.BaseColor, value ); 
        } // 0x90
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 EdgeColor {
            get => Data.EdgeColor;
            set => Set( x => x.EdgeColor, value ); 
        } // 0xb0
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor {
            get => Data.EmissiveColor;
            set => Set( x => x.EmissiveColor, value ); 
        } // 0xc0
        [DisplayName( "Metallic" )]
        public float Metallic {
            get => Data.Metallic;
            set => Set( x => x.Metallic, value ); 
        } // 0xe8
        [DisplayName( "Edge Threshold" )]
        public float EdgeThreshold {
            get => Data.EdgeThreshold;
            set => Set( x => x.EdgeThreshold, value ); 
        } // 0xf4
        [DisplayName( "Edge Factor" )]
        public float EdgeFactor {
            get => Data.EdgeFactor;
            set => Set( x => x.EdgeFactor, value ); 
        } // 0xf8
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType12.Type12Flags> ) )]
        public MaterialParameterSetType12.Type12Flags Flags {
            get => Data.Flags;
            set => Set( x => x.Flags, value ); 
        } // 0x12c
        public float P12_7 {
            get => Data.P12_7;
            set => Set( x => x.P12_7, value ); 
        } // 0x108
        public Vector3 P12_8 {
            get => Data.P12_8;
            set => Set( x => x.P12_8, value ); 
        } // 0x110
        [DisplayName( "Mat Bloom Intensity" )]
        public float P12_9 {
            get => Data.MatBloomIntensity;
            set => Set( x => x.MatBloomIntensity, value ); 
        } // 0xf0
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float EdgeRemoveYAxisFactor {
            get => Data.EdgeRemoveYAxisFactor;
            set => Set( x => x.EdgeRemoveYAxisFactor, value ); 
        } // 0xfc
        public float P12_11 {
            get => Data.P12_11;
            set => Set( x => x.P12_11, value ); 
        } // 0x11c
        public float P12_12 {
            get => Data.P12_12; 
            set => Set( x => x.P12_12, value ); 
        } // 0x120
        public float P12_13 {
            get => Data.P12_13;
            set => Set( x => x.P12_13, value ); 
        } // 0x124
        [DisplayName( "Mat Bloom Intensity" )]
        public float MatBloomIntensity2 {
            get => Data.MatBloomIntensity2;
            set => Set( x => x.MatBloomIntensity2, value ); 
        } // 0x10c
        [DisplayName( "Specular Color" )]
        public Vector3 SpecularColor {
            get => Data.SpecularColor;
            set => Set( x => x.SpecularColor, value ); 
        } // 0xd0
        [DisplayName( "Specular Threshold" )]
        public float SpecularThreshold {
            get => Data.SpecularThreshold;
            set => Set( x => x.SpecularThreshold, value ); 
        } // 0xdc
        [DisplayName( "Specular Power" )]
        public float SpecularPower {
            get => Data.SpecularPower;
            set => Set( x => x.SpecularPower, value ); 
        } // 0xe0
        [DisplayName( "Mat Roughness" )]
        public float MatRoughness {
            get => Data.MatRoughness;
            set => Set( x => x.MatRoughness, value ); 
        } // 0xec
        [DisplayName( "Mat Ramp Alpha" )]
        public float MatRampAlpha {
            get => Data.P12_19;
            set => Set( x => x.P12_19, value ); 
        } // 0xe4
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor {
            get => Data.ShadowColor;
            set => Set( x => x.ShadowColor, value ); 
        } // 0xa0
        [DisplayName( "Shadow Theshold" )]
        public float ShadowThreshold {
            get => Data.ShadowThreshold; 
            set => Set( x => x.ShadowThreshold, value ); 
        } // 0x100
        [DisplayName( "Shadow Factor" )]
        public float ShadowFactor {
            get => Data.ShadowFactor; 
            set => Set( x => x.ShadowFactor, value ); 
        } // 0x104

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
