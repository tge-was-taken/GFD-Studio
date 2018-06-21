using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType1ViewNode : MaterialAttributeViewNode<MaterialAttributeType1>
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
        [ Browsable( true ) ]
        public MaterialAttributeType1Flags Type1Flags
        {
            get => GetDataProperty< MaterialAttributeType1Flags >();
            set => SetDataProperty( value );
        }

        public MaterialAttributeType1ViewNode( string text, MaterialAttributeType1 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType1> );
        }
    }
}