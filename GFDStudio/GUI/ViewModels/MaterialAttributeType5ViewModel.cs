using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public class MaterialAttributeType5ViewModel : MaterialAttributeViewModel<MaterialAttributeType5>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        [Browsable( true )]
        public int Field0C
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public int Field10
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field14
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field18
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field1C
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field2C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field30
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field34
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field38
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field3C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field48
        {
            get => GetModelProperty<Vector4>();
            set => SetModelProperty( value );
        }

        public MaterialAttributeType5ViewModel( string text, MaterialAttributeType5 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType5> );
        }
    }
}