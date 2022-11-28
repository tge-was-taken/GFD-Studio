using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType0ViewNode : MaterialAttributeViewNode< MaterialAttributeType0 >
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [Browsable(true)]
        [TypeConverter(typeof(Vector4TypeConverter))]
        [DisplayName("Color (float)")]
        public Vector4 Color
        {
            get => GetDataProperty< Vector4 >();
            set => SetDataProperty( value );
        }

        [DisplayName( "Color (RGBA)" )]
        public System.Drawing.Color ColorRGBA
        {
            get => Data.Color.ToByte();
            set => Data.Color = value.ToFloat();
        }

        // 1C
        [ Browsable( true ) ]
        [DisplayName( "Light threshold" )]
        public float Field1C
        {
            get => GetDataProperty< float >();
            set => SetDataProperty( value );
        }

        // 20
        [Browsable( true )]
        [DisplayName( "Light factor" )]
        public float Field20
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 24
        [Browsable( true )]
        [DisplayName( "Light brightness" )]
        public float Field24
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 28
        [Browsable( true )]
        [DisplayName( "Shadow threshold" )]
        public float Field28
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 2C
        [Browsable( true )]
        [DisplayName( "Shadow factor" )]
        public float Field2C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 30
        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<MaterialAttributeType0Flags> ))]
        public MaterialAttributeType0Flags Type0Flags
        {
            get => GetDataProperty<MaterialAttributeType0Flags>();
            set => SetDataProperty( value );
        }

        public MaterialAttributeType0ViewNode( string text, MaterialAttributeType0 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Stream >( path => MaterialAttribute.Save(Data, path) );
            RegisterReplaceHandler<Stream>( Resource.Load< MaterialAttributeType0 > );
        }
    }
}