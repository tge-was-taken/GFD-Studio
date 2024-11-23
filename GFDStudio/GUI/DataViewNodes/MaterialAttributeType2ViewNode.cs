using System.ComponentModel;
using System.IO;
using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using static GFDLibrary.Materials.MaterialAttributeType2;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType2ViewNode : MaterialAttributeViewNode<MaterialAttributeType2>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [ Browsable( true ) ]
        [TypeConverter( typeof( EnumTypeConverter<MaterialAttributeType2Flags> ) )]
        [DisplayName( "Flags" )]
        public new MaterialAttributeType2Flags Flags2
        {
            get => Data.Flags2;
            set => SetDataProperty( value );
        }

        // 10
        [Browsable( true )]
        [DisplayName( "Color" )]
        public int Color
        {
            get => Data.Color;
            set => SetDataProperty( value );
        }

        public MaterialAttributeType2ViewNode( string text, MaterialAttributeType2 data ) : base( text, data )
        {
        }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Stream>( path => MaterialAttribute.Save( Data, path ) );
            RegisterReplaceHandler<Stream>( Resource.Load<MaterialAttributeType2> );
        }
    }
}