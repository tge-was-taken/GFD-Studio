using System.ComponentModel;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Lights;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class LightViewNode : DataViewNode<Light>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags
            => DataViewNodeMenuFlags.Delete | DataViewNodeMenuFlags.Move | DataViewNodeMenuFlags.Export;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        public LightFlags Flags
        {
            get => Data.Flags;
            set => SetDataProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field30
        {
            get => Data.Field30;
            set => SetDataProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field40
        {
            get => Data.Field40;
            set => SetDataProperty( value );
        }

        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field50
        {
            get => Data.Field50;
            set => SetDataProperty( value );
        }

        public LightType Type
        {
            get => Data.Type;
            set => SetDataProperty( value );
        }

        public float Field20
        {
            get => Data.Field20;
            set => SetDataProperty( value );
        }

        public float Field04
        {
            get => Data.Field04;
            set => SetDataProperty( value );
        }

        public float Field08
        {
            get => Data.Field08;
            set => SetDataProperty( value );
        }

        public float Field10
        {
            get => Data.Field10;
            set => SetDataProperty( value );
        }

        public float Field6C
        {
            get => Data.Field6C;
            set => SetDataProperty( value );
        }

        public float Field70
        {
            get => Data.Field70;
            set => SetDataProperty( value );
        }

        public float Field60
        {
            get => Data.Field60;
            set => SetDataProperty( value );
        }

        public float Field64
        {
            get => Data.Field64;
            set => SetDataProperty( value );
        }

        public float Field68
        {
            get => Data.Field68;
            set => SetDataProperty( value );
        }

        public float Field74
        {
            get => Data.Field74;
            set => SetDataProperty( value );
        }

        public float Field78
        {
            get => Data.Field78;
            set => SetDataProperty( value );
        }

        public LightViewNode( string text, Light data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Light>( path => Data.Save( path ) );
            RegisterReplaceHandler<Light>( Resource.Load<Light> );
        }

        protected override void InitializeViewCore()
        {
        }
    }
}