using System.ComponentModel;
using System.IO;
using System.Numerics;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialAttributeType0ViewModel : MaterialAttributeViewModel< MaterialAttributeType0 >
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        // 0C
        [Browsable(true)]
        [TypeConverter(typeof(Vector4TypeConverter))]
        public Vector4 Field0C
        {
            get => GetModelProperty< Vector4 >();
            set => SetModelProperty( value );
        }

        // 1C
        [ Browsable( true ) ]
        public float Field1C
        {
            get => GetModelProperty< float >();
            set => SetModelProperty( value );
        }

        // 20
        [Browsable( true )]
        public float Field20
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 24
        [Browsable( true )]
        public float Field24
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 28
        [Browsable( true )]
        public float Field28
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 2C
        [Browsable( true )]
        public float Field2C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 30
        [ Browsable( true ) ]
        [TypeConverter(typeof(EnumTypeConverter<MaterialAttributeType0Flags> ))]
        public MaterialAttributeType0Flags Type0Flags
        {
            get => GetModelProperty<MaterialAttributeType0Flags>();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType0ViewModel( string text, MaterialAttributeType0 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler< Stream >( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load< MaterialAttributeType0 > );
        }
    }
}