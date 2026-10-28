using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType10ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType10>
    {
        public MaterialParameterSetType10ViewNode( string text, MaterialParameterSetType10 data ) : base( text, data )
        {
        }

        public Vector4 P10_0 { 
            get => Data.P10_0; 
            set => Set( x => x.P10_0, value ); 
        } // 0x90
        public float P10_1 { 
            get => Data.P10_1; 
            set => Set( x => x.P10_1, value ); 
        } // 0xa0
        public float P10_2 { 
            get => Data.P10_2; 
            set => Set( x => x.P10_2, value ); 
        } // 0xa4
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType10.Type10Flags> ) )]
        public MaterialParameterSetType10.Type10Flags Flags { 
            get => Data.Flags; 
            set => Set( x => x.Flags, value ); 
        } // 0xa8

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
