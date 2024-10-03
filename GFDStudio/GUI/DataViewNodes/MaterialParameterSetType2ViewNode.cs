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
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color P2_0_RGBA
        {
            get => Data.BaseColor.ToByte();
            set => Data.BaseColor = value.ToFloat();
        }
        
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xa0
        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color P2_1_RGBA
        {
            get => Data.ShadowColor.ToByte();
            set => Data.ShadowColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Edge Color (float)" )]
        public Vector4 EdgeColor
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xb0
        [DisplayName( "Edge Color (RGBA)" )]
        public System.Drawing.Color P2_2_RGBA
        {
            get => Data.EdgeColor.ToByte();
            set => Data.EdgeColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        } // 0xc0
        [DisplayName( "Emissive Color (RGBA)" )]
        public System.Drawing.Color P2_3_RGBA
        {
            get => Data.EmissiveColor.ToByte();
            set => Data.EmissiveColor = value.ToFloat();
        }

        [TypeConverter( typeof( Vector3TypeConverter ) )]
        [DisplayName( "Specular Color" )]
        public Vector3 SpecularColor
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        } // 0xd0
        [DisplayName( "Specular Power" )]
        public float SpecularPower
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe0
        [DisplayName( "Metallic" )]
        public float Metallic
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe4
        
        [DisplayName( "Edge Threshold" )]
        public float EdgeThreshold
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xf0
        
        [DisplayName( "Edge Factor" )]
        public float EdgeFactor
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xf4
        
        [DisplayName( "Shadow Threshold" )]
        public float ShadowThreshold
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xfc
        
        [DisplayName( "Shadow Factor" )]
        public float ShadowFactor
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x100
        public MaterialParameterSetType2Base.Type2Flags Flags
        {
            get => GetDataProperty<MaterialParameterSetType2Base.Type2Flags>();
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
        public float MatBloomIntensity
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xec
        [DisplayName( "Specular Threshold" )]
        public float SpecularThreshold
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xdc
        [DisplayName( "Edge Remove Y Axis Factor" )]
        public float EdgeRemoveYAxisFactor
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
        public float MatRoughness
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0xe8
        [DisplayName( "Fitting Tile" )]
        public float FittingTile
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        } // 0x128
        [DisplayName( "Multi Fitting Tile" )]
        public float MultiFittingTile
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
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
