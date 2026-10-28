using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public abstract class MaterialParameterSetType2ViewNodeBase<T> : MaterialParameterSetViewNodeBase<T> where T : MaterialParameterSetType2Base
    {
        
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 BaseColor
        {
            get => Data.BaseColor;
            set => Set( x => x.BaseColor, value );
        } // 0x90
        
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor
        {
            get => Data.ShadowColor;
            set => Set( x => x.ShadowColor, value );
        } // 0xa0

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 EdgeColor
        {
            get => Data.EdgeColor;
            set => Set( x => x.EdgeColor, value );
        } // 0xb0

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor
        {
            get => Data.EmissiveColor;
            set => Set( x => x.EmissiveColor, value );
        } // 0xc0

        [TypeConverter( typeof( Vector3TypeConverter ) )]
        [DisplayName( "Specular Color" )]
        public Vector3 SpecularColor
        {
            get => Data.SpecularColor;
            set => Set( x => x.SpecularColor, value );
        } // 0xd0
        [DisplayName( "Specular Power" )]
        public float SpecularPower
        {
            get => Data.SpecularPower;
            set => Set( x => x.SpecularPower, value );
        } // 0xe0
        [DisplayName( "Metallic" )]
        public float Metallic
        {
            get => Data.Metallic;
            set => Set( x => x.Metallic, value );
        } // 0xe4
        
        [DisplayName( "Edge Threshold" )]
        public float EdgeThreshold
        {
            get => Data.EdgeThreshold;
            set => Set( x => x.EdgeThreshold, value );
        } // 0xf0
        
        [DisplayName( "Edge Factor" )]
        public float EdgeFactor
        {
            get => Data.EdgeFactor;
            set => Set( x => x.EdgeFactor, value );
        } // 0xf4
        
        [DisplayName( "Shadow Threshold" )]
        public float ShadowThreshold
        {
            get => Data.ShadowThreshold;
            set => Set( x => x.ShadowThreshold, value );
        } // 0xfc
        
        [DisplayName( "Shadow Factor" )]
        public float ShadowFactor
        {
            get => Data.ShadowFactor;
            set => Set( x => x.ShadowFactor, value );
        } // 0x100
        public MaterialParameterSetType2Base.Type2Flags Flags
        {
            get => Data.Flags;
            set => Set( x => x.Flags, value );
        } // 0x130
        public float P2_12
        {
            get => Data.P2_12;
            set => Set( x => x.P2_12, value );
        } // 0x104
        public Vector3 P2_13
        {
            get => Data.P2_13;
            set => Set( x => x.P2_13, value );
        } // 0x10c
        
        [DisplayName( "Mat Bloom Intensity" )]
        public float MatBloomIntensity
        {
            get => Data.MatBloomIntensity;
            set => Set( x => x.MatBloomIntensity, value );
        } // 0xec
        [DisplayName( "Specular Threshold" )]
        public float SpecularThreshold
        {
            get => Data.SpecularThreshold;
            set => Set( x => x.SpecularThreshold, value );
        } // 0xdc
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float EdgeRemoveYAxisFactor
        {
            get => Data.EdgeRemoveYAxisFactor;
            set => Set( x => x.EdgeRemoveYAxisFactor, value );
        } // 0xf8
        public float P2_17
        {
            get => Data.P2_17;
            set => Set( x => x.P2_17, value );
        } // 0x118
        public float P2_18
        {
            get => Data.P2_18;
            set => Set( x => x.P2_18, value );
        } // 0x11c
        public float P2_19
        {
            get => Data.P2_19;
            set => Set( x => x.P2_19, value );
        } // 0x120
        public float P2_20
        {
            get => Data.P2_20;
            set => Set( x => x.P2_20, value );
        } // 0x108
        [DisplayName( "Mat Roughness" )]
        public float MatRoughness
        {
            get => Data.MatRoughness;
            set => Set( x => x.MatRoughness, value );
        } // 0xe8
        [DisplayName( "Fitting Tile" )]
        public float FittingTile
        {
            get => Data.FittingTile;
            set => Set( x => x.FittingTile, value );
        } // 0x128
        [DisplayName( "Multi Fitting Tile" )]
        public float MultiFittingTile
        {
            get => Data.MultiFittingTile;
            set => Set( x => x.MultiFittingTile, value );
        } // 0x12c
        public MaterialParameterSetType2ViewNodeBase( string text, T data ) : base( text, data ) {}

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;
        protected override void InitializeCore() { }
    }

    public class MaterialParameterSetType2ViewNode : MaterialParameterSetType2ViewNodeBase<MaterialParameterSetType2>
    {
        public MaterialParameterSetType2ViewNode( string text, MaterialParameterSetType2 data ) : base( text, data ) { }
    }
    public class MaterialParameterSetType3ViewNode : MaterialParameterSetType2ViewNodeBase<MaterialParameterSetType3>
    {
        public MaterialParameterSetType3ViewNode( string text, MaterialParameterSetType3 data ) : base( text, data ) { }
    }
    public class MaterialParameterSetType13ViewNode : MaterialParameterSetType2ViewNodeBase<MaterialParameterSetType13>
    {
        public MaterialParameterSetType13ViewNode( string text, MaterialParameterSetType13 data ) : base( text, data ) { }
    }
}
