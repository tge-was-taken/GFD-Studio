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
            get => Data.Field00;
            set => Set( x => x.Field00, value );
        }

        // 0C
        [Browsable( true )]
        public float Field0C
        {
            get => Data.Field0C;
            set => Set( x => x.Field0C, value );
        }

        // 10
        [Browsable( true )]
        public float Field10
        {
            get => Data.Field10;
            set => Set( x => x.Field10, value );
        }

        // 14
        [Browsable( true )]
        public float Field14
        {
            get => Data.Field14;
            set => Set( x => x.Field14, value );
        }

        // 18
        [Browsable( true )]
        public float Field18
        {
            get => Data.Field18;
            set => Set( x => x.Field18, value );
        }

        // 1C
        [Browsable( true )]
        [TypeConverter( typeof( Vector3TypeConverter ) )]
        public Vector3 Field1C
        {
            get => Data.Field1C;
            set => Set( x => x.Field1C, value );
        }

        // 28
        [Browsable( true )]
        public float Field28
        {
            get => Data.Field28;
            set => Set( x => x.Field28, value );
        }

        // 2C
        [Browsable( true )]
        public float Field2C
        {
            get => Data.Field2C;
            set => Set( x => x.Field2C, value );
        }


        // 30
        [Browsable( true )]
        public int Field30
        {
            get => Data.Field30;
            set => Set( x => x.Field30, value );
        }

        // 34
        [Browsable( true )]
        public int Field34
        {
            get => Data.Field34;
            set => Set( x => x.Field34, value );
        }

        // 38
        [Browsable( true )]
        public int Field38
        {
            get => Data.Field38;
            set => Set( x => x.Field38, value );
        }

        // 3C
        [Browsable( true )]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType8Flags> ) )]
        public MaterialAttributeType8Flags Type8Flags
        {
            get => Data.Type8Flags;
            set => Set( x => x.Type8Flags, value );
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