using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType3ViewNode : MaterialAttributeViewNode<MaterialAttributeType3>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
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
        public float Field1C
        {
            get => Data.Field1C;
            set => Set( x => x.Field1C, value );
        }

        // 20
        [Browsable( true )]
        public float Field20
        {
            get => Data.Field20;
            set => Set( x => x.Field20, value );
        }

        // 24
        [Browsable( true )]
        public float Field24
        {
            get => Data.Field24;
            set => Set( x => x.Field24, value );
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
        public float Field30
        {
            get => Data.Field30;
            set => Set( x => x.Field30, value );
        }

        // 34
        [Browsable( true )]
        public float Field34
        {
            get => Data.Field34;
            set => Set( x => x.Field34, value );
        }

        // 38
        [Browsable( true )]
        public float Field38
        {
            get => Data.Field38;
            set => Set( x => x.Field38, value );
        }

        // 3C
        [Browsable( true )]
        public int Field3C
        {
            get => Data.Field3C;
            set => Set( x => x.Field3C, value );
        }


        public MaterialAttributeType3ViewNode( string text, MaterialAttributeType3 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType3> );
        }
    }
}