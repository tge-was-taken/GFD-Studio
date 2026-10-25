using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType4ViewNode : MaterialAttributeViewNode<MaterialAttributeType4>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Light Color (float)" )]
        public Vector4 LightColor
        {
            get => Data.LightColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Light Color (RGBA)" )]
        public System.Drawing.Color LightColorRGBA
        {
            get => Data.LightColor.ToByte();
            set => Data.LightColor = value.ToFloat();
        }

        // 1C
        [Browsable( true )]
        [DisplayName( "Light threshold" )]
        public float LightThreshold
        {
            get => Data.LightThreshold;
            set => SetDataProperty( value );
        }

        // 20
        [Browsable( true )]
        [DisplayName( "Light Factor" )]
        public float LightFactor
        {
            get => Data.LightFactor;
            set => SetDataProperty( value );
        }

        // 24
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor
        {
            get => Data.ShadowColor;
            set => SetDataProperty( value );
        }

        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color ShadowRGBA
        {
            get => Data.ShadowColor.ToByte();
            set => Data.ShadowColor = value.ToFloat();
        }

        // 34
        [Browsable( true )]
        [DisplayName( "Shadow threshold" )]
        public float ShadowThreshold
        {
            get => Data.ShadowThreshold;
            set => SetDataProperty( value );
        }

        // 38
        [Browsable( true )]
        [DisplayName( "Shadow factor" )]
        public float ShadowFactor
        {
            get => Data.ShadowFactor;
            set => SetDataProperty( value );
        }

        // 3C
        [Browsable( true )]
        [DisplayName( "Night Map Speed" )]
        public float NightMapSpeed
        {
            get => Data.NightMapSpeed;
            set => SetDataProperty( value );
        }

        // 40
        [Browsable( true )]
        [DisplayName( "Night Map Power" )]
        public float NightMapPower
        {
            get => Data.NightMapPower;
            set => SetDataProperty( value );
        }

        // 44
        [Browsable( true )]
        [DisplayName( "Night Map Scale" )]
        public float NightMapScale
        {
            get => Data.NightMapScale;
            set => SetDataProperty( value );
        }

        // 48
        [Browsable( true )]
        [DisplayName( "Night Map Height" )]
        public float NightMapHeight
        {
            get => Data.NightMapHeight;
            set => SetDataProperty( value );
        }

        // 4C
        [Browsable( true )]
        [DisplayName( "Night Map Alpha" )]
        public float NightMapAlpha
        {
            get => Data.NightMapAlpha;
            set => SetDataProperty( value );
        }

        // 50
        [Browsable( true )]
        [DisplayName( "Night Map Direction" )]
        public byte NightMapDirection
        {
            get => Data.NightMapDirection;
            set => SetDataProperty( value );
        }

        // 54
        [Browsable( true )]
        [DisplayName( "Dark Gradient Height" )]
        public float DarkGradientHeight
        {
            get => Data.DarkGradientHeight;
            set => SetDataProperty( value );
        }

        // 58
        [Browsable( true )]
        [DisplayName( "Dark Gradient Alpha" )]
        public float DarkGradientAlpha
        {
            get => Data.DarkGradientAlpha;
            set => SetDataProperty( value );
        }

        // 5C
        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType1Flags> ) )]
        [DisplayName( "Flags (2)" )]
        public MaterialAttributeType1Flags Field5C
        {
            get => Data.Flags2;
            set => SetDataProperty( value );
        }

        public MaterialAttributeType4ViewNode( string text, MaterialAttributeType4 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType4> );
        }
    }
}