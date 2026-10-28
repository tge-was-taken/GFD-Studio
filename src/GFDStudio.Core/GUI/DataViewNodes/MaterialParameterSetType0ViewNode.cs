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
            set => Set( x => x.BaseColor, value ); 
        } // 0x90
        [DisplayName( "Emissive Strength" )]
        public float EmissiveStrength {
            get => Data.EmissiveStrength;
            set => Set( x => x.EmissiveStrength, value ); 
        } // 0xa0
        [DisplayName( "Roughness" )]
        public float Roughness {
            get => Data.Roughness;
            set => Set( x => x.Roughness, value ); 
        } // 0xa4
        [DisplayName( "Metallic" )]
        public float Metallic {
            get => Data.Metallic;
            set => Set( x => x.Metallic, value ); 
        } // 0xa8
        [DisplayName( "Multi Alpha" )]
        public float MultiAlpha {
            get => Data.MultiAlpha;
            set => Set( x => x.MultiAlpha, value ); 
        } // 0xac
        [DisplayName( "Bloom Intensity" )]
        public float BloomIntensity {
            get => Data.BloomIntensity;
            set => Set( x => x.BloomIntensity, value ); 
        } // 0xb0
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType0.Type0Flags> ) )]
        public MaterialParameterSetType0.Type0Flags Flags { 
            get => Data.Flags;
            set => Set( x => x.Flags, value ); 
        } // 0xb4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
