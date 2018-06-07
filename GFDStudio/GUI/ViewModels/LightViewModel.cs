using System.ComponentModel;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.ViewModels
{
    public class LightViewModel : TreeNodeViewModel<Light>
    {
        public override TreeNodeViewModelMenuFlags ContextMenuFlags
            => TreeNodeViewModelMenuFlags.Delete | TreeNodeViewModelMenuFlags.Move | TreeNodeViewModelMenuFlags.Export;

        public override TreeNodeViewModelFlags NodeFlags
            => TreeNodeViewModelFlags.Leaf;

        public LightFlags Flags
        {
            get => Model.Flags;
            set => SetModelProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field30
        {
            get => Model.Field30;
            set => SetModelProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field40
        {
            get => Model.Field40;
            set => SetModelProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field50
        {
            get => Model.Field50;
            set => SetModelProperty( value );
        }

        public LightType Type
        {
            get => Model.Type;
            set => SetModelProperty( value );
        }

        public float Field20
        {
            get => Model.Field20;
            set => SetModelProperty( value );
        }

        public float Field04
        {
            get => Model.Field04;
            set => SetModelProperty( value );
        }

        public float Field08
        {
            get => Model.Field08;
            set => SetModelProperty( value );
        }

        public float Field10
        {
            get => Model.Field10;
            set => SetModelProperty( value );
        }

        public float Field6C
        {
            get => Model.Field6C;
            set => SetModelProperty( value );
        }

        public float Field70
        {
            get => Model.Field70;
            set => SetModelProperty( value );
        }

        public float Field60
        {
            get => Model.Field60;
            set => SetModelProperty( value );
        }

        public float Field64
        {
            get => Model.Field64;
            set => SetModelProperty( value );
        }

        public float Field68
        {
            get => Model.Field68;
            set => SetModelProperty( value );
        }

        public float Field74
        {
            get => Model.Field74;
            set => SetModelProperty( value );
        }

        public float Field78
        {
            get => Model.Field78;
            set => SetModelProperty( value );
        }

        public LightViewModel( string text, Light resource ) : base( text, resource )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Light>( path => Model.Save( path ) );
            RegisterReplaceHandler<Light>( Resource.Load<Light> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}