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
            get => Data.AmbientColor; 
            set => Set( x => x.AmbientColor, value ); 
        } // 0x90
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Diffuse color (float)" )]
        public Vector4 DiffuseColor { 
            get => Data.DiffuseColor; 
            set => Set( x => x.DiffuseColor, value ); 
        } // 0xa0
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Specular color (float)" )]
        public Vector4 SpecularColor { 
            get => Data.SpecularColor; 
            set => Set( x => x.SpecularColor, value ); 
        } // 0xb0
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive color (float)" )]
        public Vector4 EmissiveColor { 
            get => Data.EmissiveColor; 
            set => Set( x => x.EmissiveColor, value ); 
        } // 0xc0
        public float Reflectivity { 
            get => Data.Reflectivity; 
            set => Set( x => x.Reflectivity, value ); 
        } // 0xd0
        public float LerpBlendRate { 
            get => Data.LerpBlendRate; 
            set => Set( x => x.LerpBlendRate, value ); 
        } // 0xd4

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
