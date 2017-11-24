using System.ComponentModel;
using System.IO;
using System.Numerics;
using AtlusGfdEditor.GUI.TypeConverters;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialAttributeType1ViewModel : MaterialAttributeViewModel<MaterialAttributeType1>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        [ TypeConverter( typeof( Vector4TypeConverter ) ) ]
        public Vector4 Field0C
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        // 1C
        [Browsable( true )]
        public float Field1C
        {
            get => GetModelProperty<float>();
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
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field24
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        // 34
        [Browsable( true )]
        public float Field34
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 38
        [Browsable( true )]
        public float Field38
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 3C
        [ Browsable( true ) ]
        public MaterialAttributeType1Flags Type1Flags
        {
            get => GetModelProperty< MaterialAttributeType1Flags >();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType1ViewModel( string text, MaterialAttributeType1 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType1> );
        }
    }
}