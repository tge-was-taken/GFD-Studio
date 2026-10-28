using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType8ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType8>
    {
        public MaterialParameterSetType8ViewNode( string text, MaterialParameterSetType8 data ) : base( text, data )
        {
        }

        public float P8_0 { 
            get => Data.P8_0; 
            set => Set( x => x.P8_0, value ); 
        } // 0x90
        public float P8_1 { 
            get => Data.P8_1; 
            set => Set( x => x.P8_1, value ); 
        } // 0x94
        public float P8_2 { 
            get => Data.P8_2; 
            set => Set( x => x.P8_2, value ); 
        } // 0x98
        public float P8_3 { 
            get => Data.P8_3; 
            set => Set( x => x.P8_3, value ); 
        } // 0x9c
        public Vector4 P8_4 { 
            get => Data.P8_4; 
            set => Set( x => x.P8_4, value ); 
        } // 0xa0
        public float P8_5 { 
            get => Data.P8_5; 
            set => Set( x => x.P8_5, value ); 
        } // 0xb0
        public float P8_6 { 
            get => Data.P8_6; 
            set => Set( x => x.P8_6, value ); 
        } // 0xb4
        public float P8_7 { 
            get => Data.P8_7; 
            set => Set( x => x.P8_7, value ); 
        } // 0xb8
        public float P8_8 { 
            get => Data.P8_8; 
            set => Set( x => x.P8_8, value ); 
        } // 0xbc
        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
