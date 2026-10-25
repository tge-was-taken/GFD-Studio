using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialParameterSetType5ViewNode : MaterialParameterSetViewNodeBase<MaterialParameterSetType5>
    {
        public MaterialParameterSetType5ViewNode( string text, MaterialParameterSetType5 data ) : base( text, data )
        {
        }

        public float P5_0 {
            get => Data.P5_0;
            set => SetDataProperty(value); 
        } // 0x90
        public float P5_1 {
            get => Data.P5_1;
            set => SetDataProperty(value); 
        } // 0x94
        [DisplayName( "TC Scale" )]
        public float TCScale {
            get => Data.TCScale;
            set => SetDataProperty(value); 
        } // 0x98
        public float P5_3 {
            get => Data.P5_3; 
            set => SetDataProperty(value); 
        } // 0x9c
        [DisplayName( "Ocean Depth Scale" )]
        public float OceanDepthScale {
            get => Data.OceanDepthScale;
            set => SetDataProperty(value); 
        } // 0xa0
        [DisplayName( "Disturbance Camera Scale" )]
        public float DisturbanceCameraScale {
            get => Data.DisturbanceCameraScale;
            set => SetDataProperty(value); 
        } // 0xa4
        [DisplayName( "Disturbance Depth Scale" )]
        public float DisturbanceDepthScale {
            get => Data.DisturbanceDepthScale;
            set => SetDataProperty(value); 
        } // 0xa8
        [DisplayName( "Scattering Camera Scale" )]
        public float ScatteringCameraScale {
            get => Data.ScatteringCameraScale;
            set => SetDataProperty(value); 
        } // 0xac
        [DisplayName( "Disturbance Tolerance" )]
        public float DisturbanceTolerance {
            get => Data.DisturbanceTolerance;
            set => SetDataProperty(value); 
        } // 0xb0
        [DisplayName( "Foam Distance" )]
        public float FoamDistance {
            get => Data.FoamDistance; 
            set => SetDataProperty(value); 
        } // 0xb4
        [DisplayName( "Caustics Tolerance" )]
        public float CausticsTolerance {
            get => Data.CausticsTolerance; 
            set => SetDataProperty(value); 
        } // 0xb8
        public float P5_11 {
            get => Data.P5_11;
            set => SetDataProperty(value); 
        } // 0xbc
        [DisplayName( "Texture Animation Speed" )]
        public float TextureAnimationSpeed {
            get => Data.TextureAnimationSpeed;
            set => SetDataProperty(value); 
        } // 0xc0
        public float P5_13 {
            get => Data.P5_13;
            set => SetDataProperty(value); 
        } // 0xc4
        [TypeConverter( typeof( EnumTypeConverter<MaterialParameterSetType5.Type5Flags> ) )]
        public MaterialParameterSetType5.Type5Flags Flags {
            get => Data.Flags;
            set => SetDataProperty(value); 
        } // 0xc8

        public override DataViewNodeMenuFlags ContextMenuFlags => 0;
        public override DataViewNodeFlags NodeFlags => DataViewNodeFlags.Leaf;

        protected override void InitializeCore()
        {

        }
    }
}
