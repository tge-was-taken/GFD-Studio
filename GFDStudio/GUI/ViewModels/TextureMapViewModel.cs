using System.ComponentModel;
using System.IO;
using GFDLibrary;

namespace GFDStudio.GUI.ViewModels
{
    public class TextureMapViewModel : TreeNodeViewModel<TextureMap>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Export | TreeNodeViewModelMenuFlags.Replace | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Rename | TreeNodeViewModelMenuFlags.Delete;

        public override TreeNodeViewModelFlags NodeFlags => TreeNodeViewModelFlags.Leaf;

        [Browsable( true )]
        public new string Name
        {
            get => GetModelProperty< string >();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public int Field44
        {
            get => GetModelProperty<int>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field48
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field49
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4A
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public byte Field4B
        {
            get => GetModelProperty<byte>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field4C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field50
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field54
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field58
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field5C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field60
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field64
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field68
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field6C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field70
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field74
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field78
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field7C
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field80
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field84
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        [Browsable( true )]
        public float Field88
        {
            get => GetModelProperty<float>();
            set => SetModelProperty( value );
        }

        public TextureMapViewModel( string text, TextureMap resource ) : base( text, resource )
        {
            RegisterExportHandler<Stream>( path => Resource.Save( Model, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<TextureMap> );
        }

        protected override void InitializeCore()
        {
            TextChanged += ( s, o ) => Name = Text;
        }
    }
}
