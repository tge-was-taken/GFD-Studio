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
        public Vector4 P0_0 { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        [DisplayName( "Base Color (RGBA)" )]
        public System.Drawing.Color P0_0_RGBA
        {
            get => Data.P0_0.ToByte();
            set => Data.P0_0 = value.ToFloat();
        }
        [DisplayName( "Emissive Strength" )]
        public float P0_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Roughness" )]
        public float P0_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa4
        [DisplayName( "Metallic" )]
        public float P0_3 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa8
        [DisplayName( "Multi Alpha" )]
        public float P0_4 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xac
        [DisplayName( "Bloom Intensity" )]
        public float P0_5 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType0.Type0Flags> ) )]
        public MaterialParameterSetType0.Type0Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType0.Type0Flags>(); 
            set => SetDataProperty(value); 
        } // 0xb4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
