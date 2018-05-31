using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public class MaterialAttributeType4ViewModel : MaterialAttributeViewModel<MaterialAttributeType4>
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
        [Browsable( true )]
        public float Field3C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 40
        [Browsable( true )]
        public float Field40
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 44
        [Browsable( true )]
        public float Field44
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 48
        [Browsable( true )]
        public float Field48
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 4C
        [Browsable( true )]
        public float Field4C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 50
        [Browsable( true )]
        public byte Field50
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        // 54
        [Browsable( true )]
        public float Field54
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 58
        [Browsable( true )]
        public float Field58
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 5C
        [Browsable( true )]
        public int Field5C
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType4ViewModel( string text, MaterialAttributeType4 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType4> );
        }
    }
}