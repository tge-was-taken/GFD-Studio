using System.ComponentModel;
using System.IO;
using System.Numerics;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType5ViewNode : MaterialAttributeViewNode<MaterialAttributeType5>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        [Browsable( true )]
        public int Field0C
        {
            get => Data.Field0C;
            set => Set( x => x.Field0C, value );
        }

        [Browsable( true )]
        public int Field10
        {
            get => Data.Field10;
            set => Set( x => x.Field10, value );
        }

        [Browsable( true )]
        public float Field14
        {
            get => Data.Field14;
            set => Set( x => x.Field14, value );
        }

        [Browsable( true )]
        public float Field18
        {
            get => Data.Field18;
            set => Set( x => x.Field18, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field1C
        {
            get => Data.Field1C;
            set => Set( x => x.Field1C, value );
        }

        [Browsable( true )]
        public float Field2C
        {
            get => Data.Field2C;
            set => Set( x => x.Field2C, value );
        }

        [Browsable( true )]
        public float Field30
        {
            get => Data.Field30;
            set => Set( x => x.Field30, value );
        }

        [Browsable( true )]
        public float Field34
        {
            get => Data.Field34;
            set => Set( x => x.Field34, value );
        }

        [Browsable( true )]
        public float Field38
        {
            get => Data.Field38;
            set => Set( x => x.Field38, value );
        }

        [Browsable( true )]
        public float Field3C
        {
            get => Data.Field3C;
            set => Set( x => x.Field3C, value );
        }

        [Browsable( true )]
        public float Field40
        {
            get => Data.Field40;
            set => Set( x => x.Field40, value );
        }

        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        public Vector4 Field48
        {
            get => Data.Field48;
            set => Set( x => x.Field48, value );
        }

        public MaterialAttributeType5ViewNode( string text, MaterialAttributeType5 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType5> );
        }
    }
}