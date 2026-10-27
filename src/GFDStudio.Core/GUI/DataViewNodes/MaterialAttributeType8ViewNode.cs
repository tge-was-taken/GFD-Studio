using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType8ViewNode : MaterialAttributeViewNode<MaterialAttributeType8>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 00
        [Browsable( true )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Field00
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        }

        // 0C
        [Browsable( true )]
        public float Field0C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 10
        [Browsable( true )]
        public float Field10
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 14
        [Browsable( true )]
        public float Field14
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 18
        [Browsable( true )]
        public float Field18
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 1C
        [Browsable( true )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Field1C
        {
            get => GetDataProperty<Vector3>();
            set => SetDataProperty( value );
        }

        // 28
        [Browsable( true )]
        public float Field28
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }

        // 2C
        [Browsable( true )]
        public float Field2C
        {
            get => GetDataProperty<float>();
            set => SetDataProperty( value );
        }


        // 30
        [Browsable( true )]
        public int Field30
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        // 34
        [Browsable( true )]
        public int Field34
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        // 38
        [Browsable( true )]
        public int Field38
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
        }

        // 3C
        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType8Flags> ) )]
        public MaterialAttributeType8Flags Type8Flags
        {
            get => GetDataProperty<MaterialAttributeType8Flags>();
            set => SetDataProperty( value );
        }

        public MaterialAttributeType8ViewNode( string text, MaterialAttributeType8 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType8> );
        }
    }
}