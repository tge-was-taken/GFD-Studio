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
            get => GetDataProperty<Vector4>(); 
            set => SetDataProperty(value); 
        } // 0x90
        public float P10_1 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa0
        public float P10_2 { 
            get => GetDataProperty<float>(); 
            set => SetDataProperty(value); 
        } // 0xa4
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType10.Type10Flags> ) )]
        public MaterialParameterSetType10.Type10Flags Flags { 
            get => GetDataProperty<MaterialParameterSetType10.Type10Flags>(); 
            set => SetDataProperty(value); 
        } // 0xa8

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
