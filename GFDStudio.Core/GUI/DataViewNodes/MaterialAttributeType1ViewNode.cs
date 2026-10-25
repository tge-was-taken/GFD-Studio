using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType1ViewNode : MaterialAttributeViewNode<MaterialAttributeType1>
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
        public float LightTreshold
        {
            get => Data.LightTreshold;
            set => SetDataProperty( value );
        }

        // 20
        [Browsable( true )]
        [DisplayName( "Light factor" )]
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
        public System.Drawing.Color ShadowColorRGBA
        {
            get => Data.ShadowColor.ToByte();
            set
            {
                Data.ShadowColor = value.ToFloat();
                NotifyPropertyChanged( nameof( ShadowColorRGBA ) );
            }
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
        [ Browsable( true ) ]
        public MaterialAttributeType1Flags Type1Flags
        {
            get => GetDataProperty< MaterialAttributeType1Flags >();
            set => SetDataProperty( value );
        }

        public MaterialAttributeType1ViewNode( string text, MaterialAttributeType1 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType1> );
        }
    }
}