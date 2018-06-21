using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType4ViewNode : MaterialAttributeViewNode<MaterialAttributeType4>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        [ TypeConverter( typeof( Vector4TypeConverter ) ) ]
        public Vector4 Field0C
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        // 1C
        [Browsable( true )]
        public float Field1C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 20
        [Browsable( true )]
        public float Field20
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 24
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field24
        {
            get => GetDataProperty<Vector4>();
            set => SetDataProperty( value );
        }

        // 34
        [Browsable( true )]
        public float Field34
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 38
        [Browsable( true )]
        public float Field38
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 3C
        [Browsable( true )]
        public float Field3C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 40
        [Browsable( true )]
        public float Field40
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 44
        [Browsable( true )]
        public float Field44
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 48
        [Browsable( true )]
        public float Field48
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 4C
        [Browsable( true )]
        public float Field4C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 50
        [Browsable( true )]
        public byte Field50
        {
            get => GetDataProperty<byte>();
            set => SetDataProperty( value );
        }

        // 54
        [Browsable( true )]
        public float Field54
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 58
        [Browsable( true )]
        public float Field58
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 5C
        [Browsable( true )]
        public int Field5C
        {
            get => GetDataProperty<int>();
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