using System.ComponentModel;
using System.IO;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.ViewModels
{
    public class MaterialAttributeType3ViewModel : MaterialAttributeViewModel<MaterialAttributeType3>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags =>
            TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        public float Field0C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 10
        [Browsable( true )]
        public float Field10
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 14
        [Browsable( true )]
        public float Field14
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        // 18
        [Browsable( true )]
        public float Field18
        {
            get => GetModelProperty<float>();
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
        [Browsable( true )]
        public float Field30
        {
            get => GetModelProperty<float>();
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
        public int Field3C
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }


        public MaterialAttributeType3ViewModel( string text, MaterialAttributeType3 resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType3> );
        }
    }
}