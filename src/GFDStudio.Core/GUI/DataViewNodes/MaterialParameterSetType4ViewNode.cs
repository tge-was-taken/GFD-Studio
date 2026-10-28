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
            get => Data.BaseColor;
            set => Set( x => x.BaseColor, value ); 
        } // 0x90
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Emissive Color (float)" )]
        public Vector4 EmissiveColor { 
            get => Data.EmissiveColor; 
            set => Set( x => x.EmissiveColor, value ); 
        } // 0xa0
        [DisplayName( "Distortion Power" )]
        public float DistortionPower {
            get => Data.DistortionPower;
            set => Set( x => x.DistortionPower, value ); 
        } // 0xb0
        [DisplayName( "Distortion Threshold" )]
        public float DistortionThreshold {
            get => Data.DistortionThreshold;
            set => Set( x => x.DistortionThreshold, value ); 
        } // 0xb4
        public float P4_4 {
            get => Data.P4_4; 
            set => Set( x => x.P4_4, value ); 
        } // 0xb8
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType4.Type4Flags> ) )]
        public MaterialParameterSetType4.Type4Flags Flags {
            get => Data.Flags;
            set => Set( x => x.Flags, value ); 
        } // 0xcc
        [DisplayName( "Mat Bloom Intensity" )]
        public float MatBloomIntensity {
            get => Data.MatBloomIntensity;
            set => Set( x => x.MatBloomIntensity, value ); 
        } // 0xbc
        [DisplayName( "Fitting Tile" )]
        public float FittingTile {
            get => Data.FittingTile; 
            set => Set( x => x.FittingTile, value ); 
        } // 0xc0
        [DisplayName( "Multi Fitting Tile" )]
        public float MultiFittingTile {
            get => Data.MultiFittingTile;
            set => Set( x => x.MultiFittingTile, value ); 
        } // 0xc4

        

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
