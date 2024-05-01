using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using static GFDLibrary.Materials.MaterialAttributeType2;

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
        public Vector4 Field0C
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Light Color (RGBA)" )]
        public System.Drawing.Color LightColorRGBA
        {
            get => Data.Field0C.ToByte();
            set => Data.Field0C = value.ToFloat();
        }

        // 1C
        [Browsable( true )]
        [DisplayName( "Light threshold" )]
        public float Field1C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 20
        [Browsable( true )]
        [DisplayName( "Light Factor" )]
        public float Field20
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 24
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 Field24
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        [DisplayName( "Shadow Color (RGBA)" )]
        public System.Drawing.Color ShadowRGBA
        {
            get => Data.Field24.ToByte();
            set => Data.Field24 = value.ToFloat();
        }

        // 34
        [Browsable( true )]
        [DisplayName( "Shadow threshold" )]
        public float Field34
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 38
        [Browsable( true )]
        [DisplayName( "Shadow factor" )]
        public float Field38
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 3C
        [Browsable( true )]
        [DisplayName( "Night Map Speed" )]
        public float Field3C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 40
        [Browsable( true )]
        [DisplayName( "Night Map Power" )]
        public float Field40
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 44
        [Browsable( true )]
        [DisplayName( "Night Map Scale" )]
        public float Field44
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 48
        [Browsable( true )]
        [DisplayName( "Night Map Height" )]
        public float Field48
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 4C
        [Browsable( true )]
        [DisplayName( "Night Map Alpha" )]
        public float Field4C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 50
        [Browsable( true )]
        [DisplayName( "Night Map Direction" )]
        public byte Field50
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        // 54
        [Browsable( true )]
        [DisplayName( "Dark Gradient Height" )]
        public float Field54
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 58
        [Browsable( true )]
        [DisplayName( "Dark Gradient Alpha" )]
        public float Field58
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 5C
        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType1Flags> ) )]
        [DisplayName( "Flags" )]
        public MaterialAttributeType1Flags Field5C
        {
            get => GetDataProperty<MaterialAttributeType1Flags>();
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