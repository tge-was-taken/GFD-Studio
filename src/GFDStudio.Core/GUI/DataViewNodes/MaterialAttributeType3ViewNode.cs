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
        public float Field24
        {
            get => GetDataProperty<float>();
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
        public float Field30
        {
            get => GetDataProperty<float>();
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
        public int Field3C
        {
            get => GetDataProperty<int>();
            set => SetDataProperty( value );
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