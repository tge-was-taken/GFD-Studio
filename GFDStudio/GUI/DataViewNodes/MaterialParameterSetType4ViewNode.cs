using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;
namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType4ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType4>
    {
        public MaterialParameterSetType4ViewNode( string text, MaterialParameterSetType4 data ) : base( text, data )
        {
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 BaseColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color P4_0_RGBA
        {
            get => Data.BaseColor.ToByte();

            set => Data.BaseColor = value.ToFloat();
        }
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Emissive Color (RGBA)" )]
        public System.Drawing.Color P4_1_RGBA
        {
            get => Data.EmissiveColor.ToByte();
            set => Data.EmissiveColor = value.ToFloat();
        }
        [DisplayName( "Distortion Power" )]
        public float DistortionPower { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        [DisplayName( "Distortion Threshold" )]
        public float DistortionThreshold { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb4
        public float P4_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb8
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType4.Type4Flags> ) )]
        public MaterialParameterSetType4.Type4Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType4.Type4Flags>(); 
            set => SetDataProperty(value); 
        } // 0xcc
        [DisplayName( "Mat Bloom Intensity" )]
        public float MatBloomIntensity { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xbc
        [DisplayName( "Fitting Tile" )]
        public float P4_7 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        [DisplayName( "Multi Fitting Tile" )]
        public float P4_8 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xc4

        

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
