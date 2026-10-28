using GFDLibrary;
using GFDLibrary.Materials;
using GFDStudio.GUI.TypeConverters;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace GFDStudio.GUI.DataViewNodes
{
    public class MaterialAttributeType1ViewNode : MaterialAttributeViewNode<MaterialAttributeType1>
    {
        public override DataViewNodeMenuFlags ContextMenuFlags =>
            DataViewNodeMenuFlags.Export | DataViewNodeMenuFlags.Replace | DataViewNodeMenuFlags.Delete;

        public override DataViewNodeFlags NodeFlags
            => DataViewNodeFlags.Leaf;

        // 0C
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Light Color (float)" )]
        public Vector4 LightColor
        {
            get => Data.LightColor;
            set => Set( x => x.LightColor, value );
        }

        // 1C
        [Browsable( true )]
        [DisplayName( "Light threshold" )]
        public float LightThreshold
        {
            get => Data.LightThreshold;
            set => Set( x => x.LightThreshold, value );
        }

        // 20
        [Browsable( true )]
        [DisplayName( "Light factor" )]
        public float LightFactor
        {
            get => Data.LightFactor;
            set => Set( x => x.LightFactor, value );
        }

        // 24
        [Browsable( true )]
        [TypeConverter( typeof( Vector4TypeConverter ) )]
        [DisplayName( "Shadow Color (float)" )]
        public Vector4 ShadowColor
        {
            get => Data.ShadowColor;
            set => Set( x => x.ShadowColor, value );
        }

        // 34
        [Browsable( true )]
        [DisplayName( "Shadow threshold" )]
        public float ShadowThreshold
        {
            get => Data.ShadowThreshold;
            set => Set( x => x.ShadowThreshold, value );
        }

        // 38
        [Browsable( true )]
        [DisplayName( "Shadow factor" )]
        public float ShadowFactor
        {
            get => Data.ShadowFactor;
            set => Set( x => x.ShadowFactor, value );
        }

        // 3C
        [ Browsable( true ) ]
        public MaterialAttributeType1Flags Type1Flags
        {
            get => Data.Type1Flags;
            set => Set( x => x.Type1Flags, value );
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