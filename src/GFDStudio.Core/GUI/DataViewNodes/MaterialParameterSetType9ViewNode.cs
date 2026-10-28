using GFDLibrary.Materials;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType9ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType9>
    {
        public MaterialParameterSetType9ViewNode( string text, MaterialParameterSetType9 data ) : base( text, data )
        {
        }

        public float P9_0 { 
            get => Data.P9_0; 
            set => Set( x => x.P9_0, value ); 
        } // 0x90
        public float P9_1 { 
            get => Data.P9_1; 
            set => Set( x => x.P9_1, value ); 
        } // 0x94
        public float P9_2 { 
            get => Data.P9_2; 
            set => Set( x => x.P9_2, value ); 
        } // 0x98
        public float P9_3 { 
            get => Data.P9_3; 
            set => Set( x => x.P9_3, value ); 
        } // 0x9c
        public Vector4 P9_4 { 
            get => Data.P9_4; 
            set => Set( x => x.P9_4, value ); 
        } // 0xa0
        public Vector4 P9_5 { 
            get => Data.P9_5; 
            set => Set( x => x.P9_5, value ); 
        } // 0xb0
        public Vector4 P9_6 { 
            get => Data.P9_6; 
            set => Set( x => x.P9_6, value ); 
        } // 0xc0
        public Vector4 P9_7 { 
            get => Data.P9_7; 
            set => Set( x => x.P9_7, value ); 
        } // 0xd0
        public Vector3 P9_8 { 
            get => Data.P9_8; 
            set => Set( x => x.P9_8, value ); 
        } // 0xe0
        public float P9_9 { 
            get => Data.P9_9; 
            set => Set( x => x.P9_9, value ); 
        } // 0xec
        public float P9_10 { 
            get => Data.P9_10; 
            set => Set( x => x.P9_10, value ); 
        } // 0xf0
        public float P9_11 { 
            get => Data.P9_11; 
            set => Set( x => x.P9_11, value ); 
        } // 0xf4
        public float P9_12 { 
            get => Data.P9_12; 
            set => Set( x => x.P9_12, value ); 
        } // 0xf8
        public float P9_13 { 
            get => Data.P9_13; 
            set => Set( x => x.P9_13, value ); 
        } // 0xfc
        public float P9_14 { 
            get => Data.P9_14; 
            set => Set( x => x.P9_14, value ); 
        } // 0x100
        public float P9_15 { 
            get => Data.P9_15; 
            set => Set( x => x.P9_15, value ); 
        } // 0x104
        public float P9_16 { 
            get => Data.P9_16; 
            set => Set( x => x.P9_16, value ); 
        } // 0x108
        public MaterialParameterSetType9.Type9Flags Flags { 
            get => Data.Flags; 
            set => Set( x => x.Flags, value ); 
        } // 0x10c

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
