using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType0ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType0>
    {
        public MaterialParameterSetType0ViewNode( string text, MaterialParameterSetType0 data ) : base( text, data )
        {
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Base Color (float)" )]
        public Vector4 BaseColor {
            get => Data.BaseColor;
            set => SetDataProperty(value); 
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color BaseColorRGBA
        {
            get => Data.BaseColor.ToByte();
            set => Data.BaseColor = value.ToFloat();
        }
        [DisplayName( "Emissive Strength" )]
        public float EmissiveStrength {
            get => Data.EmissiveStrength;
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Roughness" )]
        public float Roughness {
            get => Data.Roughness;
            set => SetDataProperty(value); 
        } // 0xa4
        [DisplayName( "Metallic" )]
        public float Metallic {
            get => Data.Metallic;
            set => SetDataProperty(value); 
        } // 0xa8
        [DisplayName( "Multi Alpha" )]
        public float MultiAlpha {
            get => Data.MultiAlpha;
            set => SetDataProperty(value); 
        } // 0xac
        [DisplayName( "Bloom Intensity" )]
        public float BloomIntensity {
            get => Data.BloomIntensity;
            set => SetDataProperty(value); 
        } // 0xb0
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType0.Type0Flags> ) )]
        public MaterialParameterSetType0.Type0Flags Flags { 
            get => Data.Flags;
            set => SetDataProperty(value); 
        } // 0xb4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
