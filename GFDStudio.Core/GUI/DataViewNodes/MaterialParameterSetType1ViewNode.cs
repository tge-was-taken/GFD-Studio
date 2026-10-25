using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType1ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType1>
    {
        public MaterialParameterSetType1ViewNode( string text, MaterialParameterSetType1 data ) : base( text, data )
        {
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Ambient color (float)" )]
        public Vector4 AmbientColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 DiffuseColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Specular color (float)" )]
        public Vector4 SpecularColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xb0
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive color (float)" )]
        public Vector4 EmissiveColor { 
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0xc0
        public float Reflectivity { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd0
        public float LerpBlendRate { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xd4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
